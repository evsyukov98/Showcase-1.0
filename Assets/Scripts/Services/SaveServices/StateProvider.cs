using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.SaveServices
{
    public interface IStateProvider
    {
        bool TryGetState<TState>(Type type, out TState state) where TState : class, IState;

        void RegisterState<TState>(Type type, TState state) where TState : class, IState;

        void Save(bool sendToServer = false);
    }
    
    public class StateProvider :  IStateProvider
    {
        private SaveData _saveData;

        public bool TryGetState<TState>(Type type, out TState state) where TState : class, IState
        {
            if (_saveData != null) return _saveData.TryGetState(type, out state);
            
            if (!TryLoadSave(out _saveData))
            {
                CreateEmptySaveData();
            }
            return _saveData.TryGetState(type, out state);
        }

        /// <summary>
        /// Если сохранения нет на текущий момент то создать.
        /// Если есть то добавить его под определенный ключь.
        /// </summary>
        public void RegisterState<TState>(Type type, TState state) where TState : class, IState
        {
            var key = type.ToString();

            if (state == null)
            {
                Debug.LogWarning($"Save was not created. State name: ,{key}");
                return;
            }

            if (!_saveData.States.ContainsKey(key))
            {
                _saveData.States.Add(key, state);
            }
            else
            {
                _saveData.States[key] = state;
            }
        }

        public void Save(bool sendToServer = false)
        {
            LocalSaveProvider.SaveObjectToJson(_saveData);
        }

        public void DeleteSaves()
        {
            LocalSaveProvider.RemoveSaves();
        }

        private bool TryLoadSave(out SaveData saveData)
        {
            saveData = LocalSaveProvider.LoadSave();

            return saveData != null;
        }

        private void CreateEmptySaveData()
        {
            _saveData = new SaveData();
            Save();
        }
    }
    
    public class SaveData
    {
        public SaveData()
        {
            States = new Dictionary<string, IState>();
        }
        
        public readonly Dictionary<string, IState> States;

        public bool TryGetState<TState>(Type type, out TState state) where TState : class, IState
        {
            state = null;

            var key = type.ToString();

            if (!States.ContainsKey(key)) return false;
            state = (TState) States[key];
            return true;
        }
    }
    
    /// <summary>
    /// Наследуемся если это сохранение(состояние)
    /// </summary>
    public interface IState { }
}
