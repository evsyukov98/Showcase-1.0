using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ShowcaseJobSystem
{
    public struct MagnitudeParallelJob : IJobParallelFor
    {
         public NativeArray<Vector2> Input;
         public NativeArray<float> Output;

        public void Execute(int index)
        {
            Output[index] = Input[index].magnitude;
        }
    }
}