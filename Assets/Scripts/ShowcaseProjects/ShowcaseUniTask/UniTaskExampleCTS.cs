using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ShowcaseProjects.ShowcaseUniTask
{
    public class UniTaskExampleCTS : MonoBehaviour
    {
        [SerializeField] private Button runButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Image resultImage;
        [SerializeField] private TMP_InputField url;
        
        private void Awake()
        {
            runButton.onClick.AddListener(OnRunButton);
        }

        private async void OnRunButton()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            cancelButton.onClick.AddListener(() =>
            {
                cts.Cancel();
                cancelButton.onClick.RemoveAllListeners();
            });
            
            await UniTask.Delay(1000, cancellationToken: cts.Token);
            UnityWebRequest requestWebTexture = await UnityWebRequestTexture.GetTexture(url.text).SendWebRequest().WithCancellation(cts.Token);
            
            Texture2D myTexture = DownloadHandlerTexture.GetContent(requestWebTexture);
            
            resultImage.sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);
        }
    }
}