using System.Diagnostics;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace ShowcaseJobSystem
{
    public class DefaultJobExample : MonoBehaviour
    {
        [SerializeField] private Button defaultJobButton;
        [SerializeField] private int elementsCount = 1000;
        [SerializeField] private TextMeshProUGUI resultText;
        
        private void Start()
        {
            defaultJobButton.onClick.AddListener(DefaultJobStart);
        }

        private void DefaultJobStart()
        {
            NativeArray<Vector2> input = new NativeArray<Vector2>(elementsCount, Allocator.TempJob);
            NativeArray<float> output = new NativeArray<float>(elementsCount, Allocator.TempJob);

            input = InitVectorArray(input);
            
            MagnitudeJob defaultJob = new MagnitudeJob()
            {
                Input = input,
                Output = output
            };

            Stopwatch stopwatch = new Stopwatch(); //для измерения времени выполнения задачи
            stopwatch.Start();

            JobHandle jobHandle = defaultJob.Schedule();

            // останавливает текущий поток пока не будет выполнен FactorialJob
            jobHandle.Complete();
            
            stopwatch.Stop();

            resultText.text = $"DefaultJob execution time: " +
                              $"{(double) (stopwatch.ElapsedTicks) / Stopwatch.Frequency * 1000.0:0.000} ms";
            
            Debug.Log($"Example: magnitude by index[{25}] is {output[25]}");

            // вызываем обязательно после использования массива так как он находиться в неуправляемой куче
            // также можно вызывать только после Complete так как ресурсы могут быть все еще использоваться
            input.Dispose();
            output.Dispose();
        }
        
        private static NativeArray<Vector2> InitVectorArray(NativeArray<Vector2> input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new Vector2(i, i);
            }

            return input;
        }
        
        
        // просто обьяснение
        private void AllocatorTempExample() 
        {
            NativeArray<int> tempContainer = new NativeArray<int>(100, Allocator.Temp);
            // Allocator.Temp - это временный выделитель памяти для массивов, предназначенных для кратковременного использования.
            // Он предназначен для ситуаций, когда данные нужны только на короткое время в основном потоке.
            // Не рекомендуется использовать Allocator.Temp для задач, выполняемых в других потоках или долгосрочных операций,
            // так как память, выделенная с Allocator.Temp, автоматически освобождается в конце каждого кадра.
            // Для задач, выполняемых в других потоках или требующих сохранения данных между кадрами,
            // используйте Allocator.TempJob или Allocator.Persistent.
            tempContainer.Dispose();
        }
    }
}