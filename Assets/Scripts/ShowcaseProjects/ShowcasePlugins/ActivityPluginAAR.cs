using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcasePlugins
{
    public class ActivityPluginAAR : MonoBehaviour
    {
        [SerializeField] private Button activityButton;

        private AndroidJavaClass _myPlugin;
        private AndroidJavaObject _currentActivity;

        private void Awake()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) // в блоке using чтобы автоматически задиспозилось
                {
                    _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }

                _myPlugin = new AndroidJavaClass("com.showcase.activitycheckplugin.StartActivityHelper");
            }
        }
        
        private void Start()
        {
            activityButton.onClick.AddListener(OnActivityButton);
        }

        private void OnActivityButton() => StartPluginActivity();

        private void StartPluginActivity()
        {
            Debug.Log("MY LOG: AAR PLUGIN BUTTON");

            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("MY LOG: ANDROID PLATFORM DETECTED");

                if (_currentActivity == null)
                {
                    Debug.Log("MY LOG: CURRENT ACTIVITY NULL");
                }
        
                _currentActivity.Call(                                  
                    "runOnUiThread",                                
                    new AndroidJavaRunnable(OpenActivity));

                Debug.Log("MY LOG: runOnUiThread CALLED");
            }
        }

        private void OpenActivity()
        {
            Debug.Log("MY LOG: OPEN ACTIVITY CALLED");

            if (_myPlugin == null)
            {
                Debug.Log("MY LOG: MY PLUGIN NULL");
            }
    
            try
            {
                _myPlugin.CallStatic("openActivity", _currentActivity);
                Debug.Log("MY LOG: OPEN ACTIVITY COMPLETED");
            }
            catch (Exception e)
            {
                Debug.LogError("MY LOG: EXCEPTION IN OPEN ACTIVITY: " + e);
            }
        }

        private void OnDestroy()
        {
            _currentActivity?.Dispose();
            _myPlugin?.Dispose();
        }
    }
}