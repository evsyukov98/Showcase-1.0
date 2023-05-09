using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcaseThreadLogger
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private Button deleteLoggerFile;
        [SerializeField] private Button openFolder;
        [SerializeField] private TMP_InputField workDirectoryField;
        [SerializeField] private TextMeshProUGUI lastLogText;
        
        private string _workDirectory;
        private FileWriter _fileWriter;

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

        private void Start()
        {
            openFolder.onClick.AddListener(OnOpenFolder);
            deleteLoggerFile.onClick.AddListener(OnDeleteLoggerFile);
            workDirectoryField.text = _workDirectory;
            
            openFolder.interactable = false;
#if UNITY_EDITOR
            openFolder.interactable = true;
#endif
        }

        private void OnDeleteLoggerFile()
        {
            Directory.Delete(_workDirectory);
        }

        private void OnOpenFolder()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(_workDirectory);
#endif
        }

        private void OnLogMessageReceived(string message, string stacktrace, LogType type)
        {
            lastLogText.text = message;
            
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
