using System;
using Services.LoadingServices;
using UnityEngine;
using Zenject;

namespace UI.MainMenuScene
{
    public class ProjectsMenu : MonoBehaviour
    {
        [SerializeField] private MainMenuData mainMenuData;
        [Space, Header("UI elements")]
        [SerializeField] private Transform buttonParent;
        [SerializeField] private OpenProjectButton buttonPrefab;
        
        private LoadingManager _loadingManager;

        [Inject]
        private void Inject(LoadingManager loadingManager)
        {
            _loadingManager = loadingManager;
        }

        private void Start()
        {
            foreach (ShowcaseProjectData data in mainMenuData.AllProjects)
            {
                OpenProjectButton button = Instantiate(buttonPrefab, buttonParent);
                button.Init(_loadingManager, data);
            }
        }
    }
}