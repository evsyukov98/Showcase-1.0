using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ShowcaseJobSystem
{
    public class JobExample : MonoBehaviour
    {
        private void Start()
        {
            NativeArray<int> bridge = new NativeArray<int>(2, Allocator.TempJob);
            bridge[0] = 11;
            
            FactorialJob factorialJob = new FactorialJob()
            {
                Bridge = bridge
            };
            
            PowJob powJob = new PowJob()
            {
                Bridge = bridge
            };

            Debug.Log("Before Schedule");
            
            JobHandle factJobHandle = factorialJob.Schedule();
            JobHandle powJobHandle = powJob.Schedule(factJobHandle);
            
            // останавливает текущий поток пока не будет выполнен FactorialJob
            factJobHandle.Complete();
            Debug.Log($"Factorial of {bridge[0]} is {bridge[1]}");
            powJobHandle.Complete();
            Debug.Log($"Pow of {bridge[0]} is {bridge[1]}");
            
            // вызываем обязательно после использования массива так как он находиться в неуправляемой куче
            // также можно вызывать только после Complete так как ресурсы могут быть все еще использоваться
            bridge.Dispose(); 
        }
    }
}