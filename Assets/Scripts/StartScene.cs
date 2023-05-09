using Services.LoadingServices;
using UnityEngine;
using Zenject;

public class StartScene : MonoBehaviour
{
    private LoadingManager _loadingManager;
    
    [Inject]
    private void Inject(LoadingManager loadingManager)
    {
        _loadingManager = loadingManager;
    }

    private void Start()
    {
        _loadingManager.Load(ConstantStatics.ConstantScenes.SAVE_SYSTEM);
    }
}
