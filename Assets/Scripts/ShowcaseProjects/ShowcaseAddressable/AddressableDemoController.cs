using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ShowcaseAddressable
{
    public class AddressableDemoController : MonoBehaviour
    {
        [SerializeField] private LogMemory logMemory;
        [Space]
        [SerializeField] private GameObject materialTarget;
        [Header("Asset reference"), Space]
        [SerializeField] private AssetReference prefabReference;
        [SerializeField] private AssetReference materialReference;

        private AsyncOperationHandle<GameObject> _prefabHandle; // Ссылка на асинхронную операцию загрузки ресурса
        private AsyncOperationHandle<Material> _materialHandle; 
    
        private GameObject _instantiatedPrefab;                 // созданный префаб как обьект
        private Material _loadedMaterial;

        public void LoadPrefab()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long totalAllocatedMemoryBefore = Profiler.GetTotalAllocatedMemoryLong();

            if (_prefabHandle.IsValid()) 
                Addressables.Release(_prefabHandle);

            _prefabHandle = prefabReference.LoadAssetAsync<GameObject>();
            _prefabHandle.Completed += OnPrefabHandleOnCompleted;

            void OnPrefabHandleOnCompleted(AsyncOperationHandle<GameObject> handle)
            {
                _instantiatedPrefab = Instantiate(handle.Result, transform.position, Quaternion.identity);

                stopwatch.Stop();
                logMemory.LogResourceLoadInfo("Prefab", stopwatch, totalAllocatedMemoryBefore, handle);
            }
        }
        
        public void LoadMaterial()
        {
            Stopwatch stopwatch = Stopwatch.StartNew(); //начало таймера (для лога)
            long totalAllocatedMemoryBefore = Profiler.GetTotalAllocatedMemoryLong();

            if (_materialHandle.IsValid()) 
                Addressables.Release(_materialHandle);

            _materialHandle = materialReference.LoadAssetAsync<Material>();
            _materialHandle.Completed += OnCompleted;

            void OnCompleted(AsyncOperationHandle<Material> handle)
            {
                _loadedMaterial = handle.Result;
                materialTarget.GetComponent<Renderer>().material = _loadedMaterial;
            
                stopwatch.Stop();
                logMemory.LogResourceLoadInfo("Material", stopwatch, totalAllocatedMemoryBefore, handle);
            }
        }

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

        public void UnloadMaterial()
        {
            if (_loadedMaterial != null)
            {
                materialTarget.GetComponent<Renderer>().material = null;  // Данные поля нужны чтобы все нормально работало в эдиторе (в игре достаточно Addressables.Release(_materialHandle);)
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
    }
}
