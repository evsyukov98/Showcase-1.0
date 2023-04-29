using System;
using System.Collections;
using Services.ConfigsServices;
using Services.SaveServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.LoadingServices
{
    public class LoadingManager : StateManager<LoadingManagerState>
    {
        private string _loadSceneName;
        private bool _isFirstLoad = true;
        
        private CommonData _commonData;
        
        private AsyncOperation _asyncOperation;
        
        public event Action LoadLevelEvent;
        public event Action LoadFinishEvent;

        [Inject]
        public void Inject(ConfigsManager configsManager)
        {
            _commonData = configsManager.GetCommonData;
        }
        
        protected override void CreateNewState()
        {
            State = new LoadingManagerState();
        }
        
        /// <summary>
        /// Получить необходимую длительность загрузки.
        /// </summary>
        public float GetLoadingDuration()
        {
            float result;

            if (State.isFirstLaunchApp)
            {
                result = _commonData.firstLoadAppTime;
            }
            else
            {
                if (_isFirstLoad)
                    result = _commonData.loadAppTime;
                else
                    result = _commonData.interstitialLoadTime;
            }
            
            return result;
        }
        
        /// <summary>
        /// Метод который разрешает завершить загрузку и перейти на загруженную сцену
        /// </summary>
        public void AllowSceneActivation()
        {
            _isFirstLoad = false;

            if(_asyncOperation != null)
            {            
                _asyncOperation.allowSceneActivation = true;
            }
        }
        
        /// <summary>
        /// Метод для загрузки сцены.
        /// </summary>
        /// <param name="sceneName">название сцены</param>
        public void Load(string sceneName)
        {
            _loadSceneName = sceneName;
            
            SceneManager.LoadScene(ConstantStatics.ConstantScenes.LOAD);
            StartCoroutine(LoadSceneAsyncRoutine());
        }
        
        /// <summary>
        /// Загрузить сцену асинхронно.
        /// Сцена не запуститься пока [ asyncOperation.allowSceneActivation = false; ]
        /// </summary>
        private IEnumerator LoadSceneAsyncRoutine()
        {
            yield return null;
            
            LoadLevelEvent?.Invoke();

            _asyncOperation = SceneManager.LoadSceneAsync(_loadSceneName);
            _asyncOperation.completed += OnSceneLoaded;
            _asyncOperation.allowSceneActivation = false;
            
            while (!_asyncOperation.isDone)
            {
                yield return null;
            }
        }
        
        /// <summary>
        /// Вызов ивента завершение загрузки
        /// </summary>
        private void OnSceneLoaded(AsyncOperation asyncOperation)
        {
            _asyncOperation.completed -= OnSceneLoaded;
            LoadFinishEvent?.Invoke();

            if (State.isFirstLaunchApp)
            {
                State.isFirstLaunchApp = false;
                Save();
            }
        }
    }

    public class LoadingManagerState : IState
    {
        public bool isFirstLaunchApp;

        public LoadingManagerState()
        {
            isFirstLaunchApp = true;
        }
    }
}
