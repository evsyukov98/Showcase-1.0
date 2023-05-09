using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ShowcaseThreadLogger
{
    public class LogThrowerMainThread : MonoBehaviour
    {
        [SerializeField] private Button enableButton;
        [SerializeField] private TMP_InputField timerSecondField;
        [SerializeField] private float timeBetweenMessagesSeconds = 1f;

        private bool _isLogsActive = false;
        private float _timer;

        private void Start()
        {
            enableButton.onClick.AddListener(OnEnableButton);
        }
        
        private void OnEnableButton()
        {
            if (float.TryParse(timerSecondField.text, out var parsedFloat))
            {
                timeBetweenMessagesSeconds = parsedFloat;
                _isLogsActive = !_isLogsActive;
            }
        }

        private void Update()
        {
            if (_isLogsActive)
            {
                _timer += Time.deltaTime;

                if (_timer > timeBetweenMessagesSeconds)
                {
                    _timer = 0;
                    LogMain();
                }
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
                case 3: Debug.Log(GetLongMessage(4000)); break;
            }
        }

        private string GetLongMessage(int count)
        {
            var str = new StringBuilder();
            str.Append("L");

            for (int i = 0; i < count; i++)
            {
                str.Append("o");
            }
            
            str.Append("ng");

            return str.ToString();
        }
    }
}
