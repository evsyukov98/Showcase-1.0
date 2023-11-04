using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ShowcaseAddressable
{
    public class LogMemory : MonoBehaviour
    {
        [SerializeField] private Text ramText;
        [SerializeField] private TextMeshProUGUI loadLogText;
        [Space] 
        [SerializeField] private Button openBundleFolder;

        private void Start()
        {
#if UNITY_EDITOR
            openBundleFolder.gameObject.SetActive(true);
            openBundleFolder.onClick.AddListener(OpenBundleFolder);
#endif

            loadLogText.text = "Обратите внимание, что информация по загруженному префабу может быть не корректной " +
                               "т.к. префаб как ресурс практически ничего не весит, а вот прикрепленные обьекты(текстура, меш, материал и т.д.).";
        }

        private void OpenBundleFolder()
        {
            
#if UNITY_EDITOR
            string path = Path.GetFullPath(Application.dataPath + "/..//Library/com.unity.addressables/aa/Windows");
            UnityEditor.EditorUtility.RevealInFinder(path);
#endif
        }

        private void FixedUpdate()
        {
            LogMemoryUsage();
        }

        private void LogMemoryUsage()
        {
            long totalMemory = Profiler.GetTotalAllocatedMemoryLong();
            float totalMemoryMB = totalMemory / 1048576f;

            ramText.text = totalMemoryMB.ToString(CultureInfo.InvariantCulture);
        }

        [Button]
        public void LogFullMemoryUsage()
        {
            long totalMemory = System.GC.GetTotalMemory(false);
            float totalMemoryMB = totalMemory / 1048576f;
            Debug.Log($"Общий объем памяти, выделенной для управляемых объектов в куче .NET: {totalMemoryMB:F2} МБ");
            
            long reservedMemory = Profiler.GetTotalReservedMemoryLong();
            float reservedMemoryMB = reservedMemory / 1048576f;
            Debug.Log($"Общий объем памяти, зарезервированной Unity для всех объектов и ресурсов: {reservedMemoryMB:F2} МБ");

            long totalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            float totalAllocatedMemoryMB = totalAllocatedMemory / 1048576f;
            Debug.Log($"Общий объем памяти, фактически выделенной для всех объектов и ресурсов Unity: {totalAllocatedMemoryMB}");
        }
        
        public void LogResourceLoadInfo<T>(string resourceName, Stopwatch stopwatch, long totalAllocatedBefore, AsyncOperationHandle<T> handle)
        {
            long totalAllocatedAfter = Profiler.GetTotalAllocatedMemoryLong();
            float totalAllocatedDifferenceMB = (totalAllocatedAfter - totalAllocatedBefore) / 1048576f;
            
            long memorySize = Profiler.GetRuntimeMemorySizeLong(handle.Result as Object);
            string message = $"{resourceName} " +
                             $"загружен за {stopwatch.ElapsedMilliseconds} миллисекунд, " +
                             $"использует {memorySize} байт памяти, " +
                             $"память изменилась на: {totalAllocatedDifferenceMB}";
            loadLogText.text = message;

            stopwatch.Stop();
            
        }
        
        
    }
}