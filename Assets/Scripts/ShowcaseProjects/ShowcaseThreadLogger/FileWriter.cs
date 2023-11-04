using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using Unity.VisualScripting;
using ThreadPriority = System.Threading.ThreadPriority;

namespace ShowcaseThreadLogger
{
    public class FileWriter : IDisposable
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string LogTimeFormat = "{0} [{1}]: {2}\r";
        private const int MAX_MESSAGE_LENGTH = 3500;

        private string _folder;
        private string _filePath;
        
        private FileAppender _appender;
        private Thread _workingThread; 
        private readonly ConcurrentQueue<LogMessage> _messages = new ConcurrentQueue<LogMessage>();     // Потокобезопасный Queue(очередь)
        private bool _isThreadAlive = true;
        
        // ManualResetEvent останавливает поток при вызове WaitOne() но только если до этого был вызван Reset();
        private readonly ManualResetEvent _mre = new ManualResetEvent(true); 

        public FileWriter(string folder)
        {
            _folder = folder;
            ManagePath();
            
            _workingThread = new Thread(StoreMessages)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            _workingThread.Start();
        }

        private void ManagePath()
        {
            string currentDate = DateTime.UtcNow.ToString(DateFormat);
            _filePath = $"{_folder}/{currentDate}.log";
        }

        public void Write(LogMessage message)
        {
            try
            {
                if (message.Message.Length > MAX_MESSAGE_LENGTH)
                {
                    string preview = message.Message.Substring(0, MAX_MESSAGE_LENGTH);
                    _messages.Enqueue(new LogMessage(message.Type, 
                        $"Message is too long {message.Message.Length}. Preview {preview}...")
                    {
                        Time = message.Time
                    });
                }
                else
                {
                    _messages.Enqueue(message);
                }
                _mre.Set();                         // игнор WaitOne (т.е. разблокировка потока)
            }
            catch (Exception)
            { }
        }

        private void StoreMessages()
        {
            while (_isThreadAlive)
            {
                while (!_messages.IsEmpty) // если есть сообщение которые нужно записать
                {
                    try
                    {
                        LogMessage message;
                        if (!_messages.TryPeek(out message)) // попытаться взять сообщение 
                        {
                            Thread.Sleep(5);
                            continue;
                        }

                        if (_appender == null || _appender.FilePath != _filePath)
                            _appender = new FileAppender(_filePath);

                        string messageToWrite = string.Format(LogTimeFormat,
                            message.Time.ToString(CultureInfo.InvariantCulture), 
                            message.Type, message.Message);

                        if (_appender.Append(messageToWrite))
                            _messages.TryDequeue(out message);
                        else
                            Thread.Sleep(5);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }

                _mre.Reset();                       
                _mre.WaitOne();   // если был вызван Reset() то остановить работу пока не будет вызван Set      
            }
        }

        public void Dispose()
        {
            _isThreadAlive = false;         // прервать цикл (Abort не гарантирует полной остановки работы потока в момент вызова)
            _workingThread?.Abort();        // остановить поток
            GC.SuppressFinalize(this);   // уведомить GC что уже все почищено и не нужно запускать сборщик мусора
        }
    }
}