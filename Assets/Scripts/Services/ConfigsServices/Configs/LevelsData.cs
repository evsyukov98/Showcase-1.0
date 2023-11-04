using UnityEngine;

[CreateAssetMenu(fileName = "LevelsData", menuName = "CompanyName/Configs/LevelsData")]
public class LevelsData : ScriptableObject
{
    [SerializeField] private Level[] levels;

    public Level[] GetLevels => levels;
}
