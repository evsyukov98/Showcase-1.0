using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random; //  UnityRandom не потокобезопасно

namespace ShowcaseThreadLogger
{
    public class LogThrowerSecondThread : MonoBehaviour
    {
        [SerializeField] private Button enableButton;
        [SerializeField] private TMP_InputField timerSecondField;
        [SerializeField] private float threadSleepSeconds = 0.5f;

        private Thread _logThread;
        private bool _isThreadAlive;
        private Random _random; 

        private void Start()
        {
            _random = new Random();
            enableButton.onClick.AddListener(OnEnableButton);
        }

        private void OnEnableButton()
        {
            if (float.TryParse(timerSecondField.text, out var parsedFloat))
            {
                threadSleepSeconds = parsedFloat;
                
                if (_isThreadAlive)
                    StopLogThrower();
                else
                    StartLogThrower();
            }
        }

        private void OnDestroy()
        {
            StopLogThrower();
        }

        private void StartLogThrower()
        {
            _isThreadAlive = true;
            _logThread = new Thread(SendLogs)
            {
                IsBackground = true
            };
            _logThread.Start();
        }

        private void StopLogThrower()
        {
            _isThreadAlive = false;
            _logThread.Abort();
            _logThread = null;
        }

        private void SendLogs()
        {
            while (_isThreadAlive)
            {
                LogThread();
                Thread.Sleep((int)(threadSleepSeconds * 1000));
            }
        }

        private void LogThread()
        {
            int randomInt = _random.Next(0, 5);

            switch (randomInt)
            {
                case 0: Debug.Log("SECOND THREAD: Log"); break;
                case 1: Debug.Log("SECOND THREAD: Log"); break;
                case 2: Debug.LogWarning("SECOND THREAD: Warning"); break;
                case 3: Debug.LogError("SECOND THREAD: Error"); break;
                case 4: int a = 1; int b = 0; int divideByZero = a/b; break;
            }
        }
    }
}