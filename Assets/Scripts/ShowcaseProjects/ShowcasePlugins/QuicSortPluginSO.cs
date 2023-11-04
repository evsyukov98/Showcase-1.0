using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class QuicSortPluginSO : MonoBehaviour
{
    private const string PLUGIN_NAME = "NativeQuickSort";
    
    [DllImport(PLUGIN_NAME)]
    private static extern void QuickSort(int[] arr, int left, int right);
    
    [SerializeField] private Button pluginButton;
    [SerializeField] private TMP_InputField inputArraySizeButton;

    private int[] _randomArray;

    private void Start()
    {
        pluginButton.onClick.AddListener(SortCompare);
    }

    private void OnDestroy()
    {
        pluginButton.onClick.RemoveAllListeners();
    }

    private void SortCompare()
    {
        if (int.TryParse(inputArraySizeButton.text, out var size))
        {
            _randomArray = GenerateRandomArray(size);
        }

        Invoke(nameof(DefaultSortCSharp), 1);
        Invoke(nameof(PluginSortCPP), 2);
    }

    private void DefaultSortCSharp()
    {
        int[] arr = (int[])_randomArray.Clone();

        Stopwatch stopwatch = Stopwatch.StartNew();
        QuickSortCSharp.Run(arr, 0, arr.Length - 1);
        stopwatch.Stop();

        Debug.Log($"C# QuickSort executed in: {stopwatch.ElapsedMilliseconds} ms");
    }
    
    private void PluginSortCPP()
    {
        int[] arr = (int[])_randomArray.Clone();

        Stopwatch stopwatch = Stopwatch.StartNew();
        QuickSort(arr, 0, arr.Length - 1);
        stopwatch.Stop();

        Debug.Log($"CPP QuickSort executed in: {stopwatch.ElapsedMilliseconds} ms");
    }
    
    public int[] GenerateRandomArray(int size)
    {
        int[] arr = new int[size];
        System.Random rand = new System.Random();

        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next();
        }

        return arr;
    }
}