using System;
using System.Diagnostics;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace ShowcaseJobSystem
{
    public class ParallelJobExample : MonoBehaviour
    {
        [SerializeField] private Button parallelJobButton;
        [SerializeField] private int elementsCount = 1000;
        [SerializeField] private int innerLoopBatchCount = 128;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TMP_InputField batchCountField;

        private void Start()
        {
            parallelJobButton.onClick.AddListener(ParallelJobStart);
        }
        
        private void ParallelJobStart()
        {
            NativeArray<Vector2> input = new NativeArray<Vector2>(elementsCount, Allocator.TempJob);
            NativeArray<float> output = new NativeArray<float>(elementsCount, Allocator.TempJob);

            input = InitVectorArray(input);
            
            var parallelJob = new MagnitudeParallelJob()
            {
                Input = input,
                Output = output
            };
            
            Stopwatch stopwatch = new Stopwatch(); //для измерения времени выполнения задачи
            stopwatch.Start();
            
            // https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Unity.Jobs.IJobParallelFor.html
            // Первый параметр индексатор означает что будут вызваны индексы от 0 до 1000 (не обязательно в правильном порядке)
            // Второй параметр innerLoopBatchCount максимальное кол-во итераций которые могут будут обработаны одним потоком
            // (т.е. в идеале 5 потокв по 20 итераций)
            innerLoopBatchCount = Int32.TryParse(batchCountField.text, out var formatToInt) ? formatToInt : 1;
            JobHandle jobHandle = parallelJob.Schedule(elementsCount, innerLoopBatchCount);
            
            jobHandle.Complete();
            
            stopwatch.Stop();
            resultText.text = $"ParallelJob execution time: " +
                              $"{(double) (stopwatch.ElapsedTicks) / Stopwatch.Frequency * 1000.0:0.000} ms";

            Debug.Log($"Magnitude of {25} is {output[25]}");

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
    }
}