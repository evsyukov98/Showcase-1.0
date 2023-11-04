using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcasePlugins
{
    public class VibrationPluginJAR : MonoBehaviour
    {
        [SerializeField] private Button vibrateButton;
        [SerializeField] private TMP_InputField duration;
        [SerializeField] private TMP_InputField strength;

        private long _durationMilliseconds = 1000;
        private int _strengthAmplitude = 100;

        private AndroidJavaClass _myPlugin; 
        private AndroidJavaObject _currentActivity;        // обьект текущего activity для того чтобы взять доступ в нашем случае к вибрации

        private void Awake()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); // нативный плагин unity для управление циклом жизни в Andorid
                _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                unityPlayer.Dispose();
            
                _myPlugin = new AndroidJavaClass("com.showcase.vibrationplugin.VibrationHelper");
            }
        }

        private void Start()
        {
            vibrateButton.onClick.AddListener(OnVibrateButton);
        }

        private void OnVibrateButton()
        {
            Debug.Log("MY LOG: JAR PLUGIN BUTTON");

            _durationMilliseconds = long.TryParse(duration.text, out var resultDuration) ? resultDuration : _durationMilliseconds;
            if (int.TryParse(strength.text, out var resultStrength))
            {
                if (resultStrength is > 0 and < 255)
                    _strengthAmplitude = resultStrength;
                else
                    _strengthAmplitude = 255;
            }

            Vibrate(_durationMilliseconds, _strengthAmplitude);
        }

        private void Vibrate(long milliseconds, int amplitude)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                _myPlugin.CallStatic("vibrate", milliseconds, _currentActivity, amplitude);
            }
            else
            {
                Debug.Log($"Necessary platform Android: " +
                          $"Duration - {_durationMilliseconds}, Strength - {_strengthAmplitude},");
            }
        }

        private void OnDestroy()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                _myPlugin?.Dispose();
                _currentActivity?.Dispose();
            }
            
            vibrateButton.onClick.RemoveAllListeners();
        }
    }
}
