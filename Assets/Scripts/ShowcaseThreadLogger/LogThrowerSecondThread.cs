using System.Threading;
using UnityEngine;
using Random = System.Random; //  UnityRandom не потокобезопасно

namespace ShowcaseThreadLogger
{
    public class LogThrowerSecondThread : MonoBehaviour
    {
        [SerializeField] private int threadSleepMilliseconds = 500;

        private Thread _logThread;
        private bool _isThreadAlive;
        private Random _random; 

        private void Start()
        {
            _random = new Random();
            _isThreadAlive = true;
            _logThread = new Thread(SendLogs);
            _logThread.IsBackground = true;
            _logThread.Start();
        }

        private void OnDestroy()
        {
            _isThreadAlive = false;
            _logThread.Abort();
        }

        private void SendLogs()
        {
            while (_isThreadAlive)
            {
                LogThread();
                Thread.Sleep(threadSleepMilliseconds);
            }
        }

        private void LogThread()
        {
            int randomInt = _random.Next(0, 4);

            switch (randomInt)
            {
                case 0: Debug.Log("SECOND THREAD: Log"); break;
                case 1: Debug.LogWarning("SECOND THREAD: Warning"); break;
                case 2: Debug.LogError("SECOND THREAD: Error"); break;
                case 3:
                    int a = 1;
                    int b = 0;
                    int divideByZero = a/b;
                    break;
            }
        }
    }
}