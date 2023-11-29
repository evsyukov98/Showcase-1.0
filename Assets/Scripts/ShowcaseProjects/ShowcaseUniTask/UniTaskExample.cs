using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ShowcaseProjects.ShowcaseUniTask
{
    public class UniTaskExample : MonoBehaviour
    {
        [SerializeField] private int startFrameDelay = 10;
        [SerializeField] private Button defaultRun;
        [SerializeField] private TextMeshProUGUI resultText;
        
        private async void Start()
        {
            // await frame-based operation like a coroutine
            await UniTask.DelayFrame(startFrameDelay);
            resultText.text = $"Waited {startFrameDelay} frames";
            
            defaultRun.onClick.AddListener(OnButtonClick);
        }

        private async void OnButtonClick()
        {
            string result = await RunAsync();
            
            resultText.text = result.Substring(0, Math.Min(result.Length, 100));
        }

        private async UniTask<string> RunAsync()
        {
            // replacement of yield return null
            await UniTask.NextFrame();
            
            UniTask<string> task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
            UniTask<string> task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
            UniTask<string> task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));
            
            // concurrent async-wait and get results easily by tuple syntax
            var (google, bing, yahoo) = await UniTask.WhenAll(task1, task2, task3);

            return google + bing + yahoo;
        }
        
        // get async webrequest
        private async UniTask<string> GetTextAsync(UnityWebRequest unityRequest)
        {
            UnityWebRequest op = await unityRequest.SendWebRequest();
            return op.downloadHandler.text;
        }
    }
}
