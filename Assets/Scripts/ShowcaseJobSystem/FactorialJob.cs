using Unity.Collections;
using Unity.Jobs;

namespace ShowcaseJobSystem
{
    public struct FactorialJob : IJob
    {
        public NativeArray<int> Bridge;

        public void Execute() // Любой IJob начинается с этого как Start
        {
            Bridge[1] = GetFactorial(Bridge[0]);
        }

        private int GetFactorial(int f)
        {
            if (f == 0)
                return 1;

            return f * GetFactorial(f - 1);
        }
    }
}
