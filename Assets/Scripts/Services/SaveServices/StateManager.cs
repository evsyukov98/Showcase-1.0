using System;
using UnityEngine;
using Zenject;

namespace Services.SaveServices
{
    public abstract class StateManager <TState> : MonoBehaviour where TState : class, IState
    {
        private IStateProvider _stateProvider;

        protected TState State;
        
        [Inject]
        public void Inject(IStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
            Setup();
        }

        private void Setup()
        {
            Type type = GetType();
            
            if (!_stateProvider.TryGetState(type, out State))
            {
                CreateNewState();
            }
            
            RegisterState();
            Save();
        }

        /// <summary>
        /// Создать сейв. (Срабатывает только если на устройстве нет файла с сохранениями)
        /// </summary>
        protected abstract void CreateNewState();
        
        /// <summary>
        /// Регистрируем его.
        /// </summary>
        private void RegisterState() => _stateProvider.RegisterState(GetType(), State);

        /// <summary>
        /// Сохраняем сейв.
        /// </summary>
        protected void Save(bool sendToServer = false)
        {
            _stateProvider.Save();
        }
    }
}
