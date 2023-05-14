using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcasePlugins
{
    public class VibrationPluginJar : MonoBehaviour
    {
        [SerializeField] private Button vibrateButton;
        [SerializeField] private TMP_InputField duration;
        [SerializeField] private TMP_InputField strength;

        private long _durationMilliseconds = 1000;
        private int _strengthAmplitude = 100;

        private AndroidJavaClass _vibrationHelper; // мой плагин
        private AndroidJavaObject _context;        // обьект текущего activity для того чтобы взять доступ в нашем случае к вибрации

        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Handheld.Vibrate();                 // чтоб взять автоматически разрешение (на некоторых устройствах не получается получить доступ к вибрации даже через AndroidManifest)

                _vibrationHelper = new AndroidJavaClass("com.showcase.vibrationplugin.VibrationHelper");
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); // нативный плагин unity для управление циклом жизни в Andorid
                _context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                unityPlayer.Dispose();
                
                vibrateButton.onClick.AddListener(OnVibrateButton);
            }
        }

        private void OnVibrateButton()
        {
            _durationMilliseconds = long.TryParse(duration.text, out var resultDuration) ? resultDuration : _durationMilliseconds;
            if (int.TryParse(strength.text, out var resultStrength))
            {
                if (resultStrength is < 0 or > 255)
                {
                    _strengthAmplitude = resultStrength;
                }
                
                _strengthAmplitude = 255;
            }

            Vibrate(_durationMilliseconds, _strengthAmplitude);
        }

        private void Vibrate(long milliseconds, int amplitude)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                _vibrationHelper.CallStatic("vibrate", milliseconds, _context, amplitude);
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
                _vibrationHelper?.Dispose();
                _context?.Dispose();
            }
            
            vibrateButton.onClick.RemoveAllListeners();
        }
    }
}
