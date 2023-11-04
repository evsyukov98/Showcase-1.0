using Services.LoadingServices;
using UI.Common;
using UnityEngine;
using Zenject;

namespace UI.LoadingWindow
{
    public class LoadingWindow : WindowController
    {
        [SerializeField] private ProgressBar progressBar;

        private LoadingManager _loadingManager;
        
        private float _loadingDuration;

        private float _progressMultiplier;
        private float _passedTime;
        private float _progress;

        [Inject]
        private void Inject(LoadingManager loadingManager)
        {
            _loadingManager = loadingManager;
        }
        
        private void Start()
        {
            StartLoading(_loadingManager.GetLoadingDuration());
        }

        private void ResetParams()
        {
            _passedTime = 0;
            _progress = 0;
        }

        private void StartLoading(float loadingDuration)
        {
            _loadingDuration = loadingDuration;
            _progressMultiplier = 1 / _loadingDuration;

            ShowWindow();
        }
        
        private void Update()
        {
            if (_passedTime <= _loadingDuration)
            {
                _passedTime += Time.deltaTime;
                _progress += Time.deltaTime * _progressMultiplier;

                UpdateBar(_progress);
            }
            else
            {
                _loadingManager.AllowSceneActivation();
                ResetParams();
            }
        }
        
        private void UpdateBar(float progress)
        {
            progressBar.SetProgress(progress);
        }
    }
}
