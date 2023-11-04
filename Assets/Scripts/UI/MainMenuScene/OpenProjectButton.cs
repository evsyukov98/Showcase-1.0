using Services.LoadingServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenuScene
{
    public class OpenProjectButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI titleName;

        private ShowcaseProjectData _data;
        private LoadingManager _loadingManager;
        
        public void Init(LoadingManager loadingManager ,ShowcaseProjectData data)
        {
            _loadingManager = loadingManager;

            _data = data;
            titleName.text = _data.TitleName;
            
            button.onClick.AddListener(OpenScene);
        } 

        private void OpenScene()
        {
            _loadingManager.Load(_data.SceneName);
        }
    }
}