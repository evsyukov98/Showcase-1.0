using System.Diagnostics;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class AddressableDemo : MonoBehaviour
{
    [SerializeField] private AssetReference prefabReference;
    [Space, Header("Material Load:")] 
    [SerializeField] private AssetReference materialReference;
    [SerializeField] private GameObject targetObject;
    [Space, Header("Profiling")] 
    [SerializeField] private Text ramText;
    
    private AsyncOperationHandle<GameObject> _prefabHandle; // загруженный в память перфаб 
    private AsyncOperationHandle<Material> _materialHandle; 
    
    private GameObject _instantiatedPrefab;                 // созданный префаб как обьект
    private Material _loadedMaterial;

    private void Update()
    {
        long totalMemory = System.GC.GetTotalMemory(false);
        float totalMemoryMB = totalMemory / 1048576f;

        ramText.text = totalMemoryMB.ToString(CultureInfo.InvariantCulture);
    }

    [Button]
    public void LoadPrefab()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        if (_prefabHandle.IsValid()) 
            Addressables.Release(_prefabHandle);

        _prefabHandle = prefabReference.LoadAssetAsync<GameObject>();
        _prefabHandle.Completed += OnPrefabHandleOnCompleted;

        void OnPrefabHandleOnCompleted(AsyncOperationHandle<GameObject> handle)
        {
            _instantiatedPrefab = Instantiate(handle.Result, transform.position, Quaternion.identity);

            stopwatch.Stop();
            Debug.Log($"Prefab загружен за {stopwatch.ElapsedMilliseconds} миллисекунд, использует {Profiler.GetRuntimeMemorySizeLong(handle.Result)} байт памяти");
        }
    }

    [Button]
    public void LoadMaterial()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        if (_materialHandle.IsValid()) 
            Addressables.Release(_materialHandle);

        _materialHandle = materialReference.LoadAssetAsync<Material>();
        _materialHandle.Completed += OnCompleted;

        void OnCompleted(AsyncOperationHandle<Material> handle)
        {
            _loadedMaterial = handle.Result;
            targetObject.GetComponent<Renderer>().material = _loadedMaterial;
            
            stopwatch.Stop();
            Debug.Log($"Материал загружен за {stopwatch.ElapsedMilliseconds} миллисекунд, использует {Profiler.GetRuntimeMemorySizeLong(handle.Result)} байт памяти");
        }
    }

    [Button]
    public void UnloadPrefab()
    {
        if (_instantiatedPrefab != null)
        {
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;

            if (_prefabHandle.IsValid())
            {
                Addressables.Release(_prefabHandle);
            }
        }
    }

    [Button]
    public void UnloadMaterial()
    {
        if (_loadedMaterial != null)
        {
            targetObject.GetComponent<Renderer>().material = null; // Данные поля нужны чтобы все нормально работало в эдиторе (в игре достаточно Addressables.Release(_materialHandle);)
            _loadedMaterial = null;
            
            if (_materialHandle.IsValid())
            {
                Addressables.Release(_materialHandle);
            }
        }
    }

    public void UnloadAll()
    {
        UnloadPrefab();
        UnloadMaterial();
    }

    [Button]
    public void LogMemoryUsage()
    {
        long totalMemory = System.GC.GetTotalMemory(false);
        float totalMemoryMB = totalMemory / 1048576f;

        long reservedMemory = Profiler.GetTotalReservedMemoryLong();
        float reservedMemoryMB = reservedMemory / 1048576f;

        Debug.Log($"Общее количество использованной оперативной памяти: {totalMemoryMB:F2} МБ");
        Debug.Log($"Общий объем зарезервированной памяти: {reservedMemoryMB:F2} МБ");
    }
}
