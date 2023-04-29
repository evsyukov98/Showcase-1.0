using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcaseThreadLogger
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private Button openFolder;
        
        private string _workDirectory;
        private FileWriter _fileWriter;

        private void Start()
        {
            openFolder.onClick.AddListener(OnOpenFolder);
        }

        private void OnOpenFolder()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(_workDirectory);
#endif
        }

        private void Awake()
        {
            _workDirectory = $"{Application.persistentDataPath}/Logs";

            if (!Directory.Exists(_workDirectory))
            {
                Directory.CreateDirectory(_workDirectory);
            }

            _fileWriter = new FileWriter(_workDirectory);
            // При вызове лога из другого потока OnLogMessageReceived будет запущен та томже потоке.
            // А если ошибка возникла в основном потоке то будет работать как стандартный logMessageReceived
            Application.logMessageReceivedThreaded += OnLogMessageReceived; 
        }

        private void OnLogMessageReceived(string message, string stacktrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                _fileWriter.Write(new LogMessage(type, message));
                _fileWriter.Write(new LogMessage(type, stacktrace));
            }
            else
            {
                _fileWriter.Write(new LogMessage(type, message));
            }
        }

        private void OnDestroy()
        {
            _fileWriter.Dispose();
        }
    }

    public class LogMessage
    {
        public LogType Type { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }

        public LogMessage(LogType type, string message)
        {
            Type = type;
            Message = message;
            Time = DateTime.UtcNow;
        }
    }
}
