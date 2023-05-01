using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ShowcaseJobSystem
{
    public struct PowJob : IJob
    {
        public NativeArray<int> Bridge;

        public void Execute()
        {
            Bridge[1] = (int)Mathf.Pow(Bridge[0], 0.5f);
        }
    }
}