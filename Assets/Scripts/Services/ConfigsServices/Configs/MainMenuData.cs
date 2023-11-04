using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShowcaseProjectData", menuName = "CompanyName/Configs/ShowcaseProjectData")]
public class MainMenuData : ScriptableObject
{
    [SerializeField] private List<ShowcaseProjectData> allProjects;

    public List<ShowcaseProjectData> AllProjects => allProjects;
}


[Serializable]
public class ShowcaseProjectData
{
    [SerializeField] private string titleName;
    [SerializeField] private string sceneName;
    [SerializeField] private Sprite defaultIcon;
    
    public string TitleName => titleName;
    public string SceneName => sceneName;
    public Sprite DefaultIcon => defaultIcon;
}
