using UnityEngine;
using Random = UnityEngine.Random;

namespace ShowcaseThreadLogger
{
    public class LogThrowerMainThread : MonoBehaviour
    {
        [SerializeField] private float timeBetweenMessagesSeconds = 1;
        
        private float _timer;


        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer > timeBetweenMessagesSeconds)
            {
                _timer = 0;
                LogMain();
            }
        }

        private void LogMain()
        {
            int randomInt = Random.Range(0, 4);

            switch (randomInt)
            {
                case 0: Debug.Log("MAIN THREAD: Log"); break;
                case 1: Debug.LogWarning("MAIN THREAD: Warning"); break;
                case 2: Debug.LogError("MAIN THREAD: Error"); break;
                case 3:
                    GameObject nullObject = null;
                    nullObject.SetActive(false);
                    break;
            }

        }
    }
}
