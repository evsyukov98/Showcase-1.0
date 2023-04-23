using System;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace ShowcaseThreadLogger
{
    public class Logger : MonoBehaviour
    {
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
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string message, string stacktrace, LogType type)
        {
            _fileWriter.Write(new LogMessage(type, message));
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.L))
            {
                UnityEditor.EditorUtility.RevealInFinder(_workDirectory);
            }
            #endif
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
