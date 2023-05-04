using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ShowcaseJobBoids
{
    public struct BoundsJob : IJobParallelFor
    {
        [ReadOnly] 
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Accelerations;

        public Vector3 AreaSize;
        
        public void Execute(int index)
        {
            Vector3 pos = Positions[index];
            Vector3 size = AreaSize * 0.5f; // если куб равен 10 то если центр 0 значит края от -5 до 5
            
            Accelerations[index] += Compensate(-size.x - pos.x, Vector3.right) +
                                    Compensate(size.x - pos.x, Vector3.left) +
                                    Compensate(-size.y - pos.y, Vector3.up) +
                                    Compensate(size.y - pos.y, Vector3.down) +
                                    Compensate(-size.z - pos.z, Vector3.forward) +
                                    Compensate(size.z - pos.z, Vector3.back);
        }

        /// <summary>
        /// Меняет скорость в зависимости от близости к границам
        /// </summary>
        /// <param name="delta">расстояние до границы</param>
        /// <param name="direction">Направление компенсации (если Right значит это проверка левой границы)</param>
        /// <returns></returns>
        private Vector3 Compensate(float delta, Vector3 direction)
        {
            const float threshold = 5f;     // за сколько клеток до границы начнет действовать сила 
            const float multiplier = 8f;    
            
            delta = Mathf.Abs(delta); 
            
            if(delta > threshold)
                return Vector3.zero;

            // чем ближе тем выше меньше дельта тем больше наша сила компенсации
            float compensationPower = (1 - delta / threshold); 
            
            return direction * compensationPower * multiplier;
        }
    }
}