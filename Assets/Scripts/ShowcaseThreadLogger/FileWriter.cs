using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;

namespace ShowcaseThreadLogger
{
    public class FileWriter
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string LogTimeFormat = "{0} [{1}]: {2}\r";

        private string _folder;
        private string _filePath;
        
        private FileAppender _appender;
        private Thread _workingThread; 
        private readonly ConcurrentQueue<LogMessage> _messages = new ConcurrentQueue<LogMessage>();     // Потокобезопасный Queue(очередь)
        private bool _isThreadAlive;

        public FileWriter(string folder)
        {
            _folder = folder;
            ManagePath();
            
            _workingThread = new Thread(StoreMessages);
            _workingThread.IsBackground = true;
            _workingThread.Priority = ThreadPriority.BelowNormal;
            _workingThread.Start();
        }

        private void ManagePath()
        {
            string currentDate = DateTime.UtcNow.ToString(DateFormat);
            _filePath = $"{_folder}/{currentDate}.log";
        }

        public void Write(LogMessage message)
        {
            _messages.Enqueue(message);
        }

        private void StoreMessages()
        {
            while (!_isThreadAlive)
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
                            message.Type, 
                            message.Message);

                        if (_appender.Append(messageToWrite))
                            _messages.TryDequeue(out message);
                        else
                            Thread.Sleep(5);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
            }
        }
        
        
    }
}