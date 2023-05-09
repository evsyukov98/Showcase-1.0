using Services.ConfigsServices;
using Services.LoadingServices;
using Services.SaveServices;
using UnityEngine;
using Zenject;

namespace Bootstrap.Zenject
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private ConfigsManager configsManagerPrefab;
        [SerializeField] private LoadingManager loadingManagerPrefab;

        public override void InstallBindings()
        {
            BindStateProvider();
            BindConfigsManager();
            BindLoadingManager();
        }

        // Стейт провайдер (Сохранения) 
        private void BindStateProvider()
        {
            Container.
                Bind<IStateProvider>().             // Всем кто затребует IStateProvider  
                To<StateProvider>().                // Дать экземпляр StateProvider 
                AsSingle();                         // Синглтон
        } 
        
        /*
        private void BindConfigsManager()
        {
            // создать префаб 
            ConfigsManager configsManager = Container.InstantiatePrefabForComponent<ConfigsManager>(configsManagerPrefab); 

            Container.
                Bind<ConfigsManager>().             // Всем кто затребует ConfigsManager
                FromInstance(configsManager).       // Дать экземпляр из префаба configsManager      
                AsSingle();                         // Синглтон
        }*/
        
        // Конфиги
        private void BindConfigsManager()
        {
            Container.
                Bind<ConfigsManager>().                         // Всем кто затребует ConfigsManager
                FromComponentInNewPrefab(configsManagerPrefab). // Создать экземпляр из префаба configsManagerPrefab
                AsSingle();                                     // Синглтон
        }

        // Загрузка
        private void BindLoadingManager()
        {
            Container.
                Bind<LoadingManager>().
                FromComponentInNewPrefab(loadingManagerPrefab).
                AsSingle();
        }
    }
}