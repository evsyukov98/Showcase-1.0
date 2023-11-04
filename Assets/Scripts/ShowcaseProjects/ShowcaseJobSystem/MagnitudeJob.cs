using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ShowcaseJobSystem
{
    public struct MagnitudeJob : IJob
    {
        public NativeArray<Vector2> Input;
        public NativeArray<float> Output;
        
        public void Execute() // Любой IJob начинается с этого как Start
        {
            for (int index = 0; index < Input.Length; index++)
            {
                Output[index] = Input[index].magnitude;
            }
        }
    }
}
