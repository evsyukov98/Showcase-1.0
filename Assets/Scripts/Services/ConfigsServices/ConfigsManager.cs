using UnityEngine;

namespace Services.ConfigsServices
{
    public class ConfigsManager : MonoBehaviour
    {
        [SerializeField] private CommonData commonData;
        [SerializeField] private LevelsData levelsData;
    
        public CommonData GetCommonData => commonData;
        public LevelsData GetLevelsData => levelsData;
    }
}
