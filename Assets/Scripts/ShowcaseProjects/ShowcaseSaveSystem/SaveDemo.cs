using System;
using Services.SaveServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ShowcaseSaveSystem
{
    public class SaveDemo : StateManager<SaveDemoObject>
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button moveButton;
        [Header("Save objects: ")]
        [SerializeField] private GameObject cubeObject;
        [SerializeField] private TMP_InputField inputFieldName;
        [SerializeField] private TMP_InputField inputFieldID;
        
        protected override void CreateNewState()
        {
            State = new SaveDemoObject();
        }

        private void Start()
        {
            saveButton.onClick.AddListener(OnSaveButton);
            loadButton.onClick.AddListener(OnLoadButton);
            moveButton.onClick.AddListener(OnMoveButton);
        }

        private void OnMoveButton()
        {
            cubeObject.transform.position = new Vector3(
                Random.Range(-2, 2f),
                Random.Range(-2, 2f),
                Random.Range(-2, 2f));
        }

        private void OnSaveButton()
        {
            State.SavePosition(cubeObject.transform.position);
            
            State.savedName = inputFieldName.text;
            State.savedID = Int32.TryParse(inputFieldID.text, out var formatToInt) ? formatToInt : 0;

            Save();
        }

        private void OnLoadButton()
        {
            cubeObject.transform.position = State.GetPosition();
            
            inputFieldName.text = State.savedName;
            inputFieldID.text = State.savedID.ToString();
        }
    }

    public class SaveDemoObject : IState
    {
        public StateVector3 savedPosition;
        public string savedName;
        public int savedID;

        public SaveDemoObject()
        {
            savedName = "";
            savedID = 0;
            savedPosition = new StateVector3();
        }
        
        public void SavePosition(Vector3 vector3)
        {
            savedPosition.x = vector3.x;
            savedPosition.y = vector3.y;
            savedPosition.z = vector3.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(savedPosition.x, savedPosition.y, savedPosition.z);
        }
    }
    
    [Serializable]
    public class StateVector3
    {
        public StateVector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        
        public float x;
        public float y;
        public float z;
    }
}
