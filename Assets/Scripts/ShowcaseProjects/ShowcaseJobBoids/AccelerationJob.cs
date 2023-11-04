using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ShowcaseJobBoids
{
    public struct AccelerationJob : IJobParallelFor
    {
        // У данного интерфейса есть ограничения на использование Vector
        // дать доступ на чтение можно атрибутом ReadOnly
        [ReadOnly] public NativeArray<Vector3> Positions;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Accelerations;

        public Vector3 Weights;
        public float DestinationThreshold;

        private int Count => Positions.Length - 1;

        public void Execute(int index)
        {
            Vector3 myPosition = Positions[index];                               // позиция текущего обьекта
            
            Vector3 totalSpread = Vector3.zero;
            Vector3 totalVelocity = Vector3.zero;
            Vector3 totalPosition = Vector3.zero;

            int neighborCount = 0;                                              // кол-во рядом стоящих соседей

            for (int i = 0; i < Count; i++)
            {
                if (i == index)                                                 // себя пропустить
                    continue;

                Vector3 neighbourPosition = Positions[i];                       // позиция соседнего обьекта
                Vector3 positionsDifference = myPosition - neighbourPosition;   // вектор растояние от соседнего к текущему
                
                if (positionsDifference.magnitude > DestinationThreshold)       // если слишком далеко, пропустить сосед
                    continue;

                neighborCount++;
                
                totalSpread += positionsDifference.normalized;                  // сумма всех направлений от седей к нам
                totalVelocity += Velocities[i];                                 // сумма всех векторов скоростей
                totalPosition += neighbourPosition;                             // сумма всех позиций соседей
            }

            Vector3 averageSpread = totalSpread / neighborCount;                // среднее направление от соседей к нам 
            Vector3 averageVelocity = totalVelocity / neighborCount;            // средний вектор скорости соседей
            Vector3 averagePosition = (totalPosition / neighborCount);          // среднее позиция соседей
            
            
            Accelerations[index] += 
                averageSpread * Weights.x +                                     // учитывает разделение, чтобы поддерживать расстояние между объектами
                averageVelocity * Weights.y +                                   // учитывает выравнивание, чтобы двигаться с похожим направлением, как и соседние объекты
                (averagePosition - myPosition) * Weights.z;                     // учитывает сближение, чтобы двигаться к средней позиции соседей и поддерживать стаю вместе
        }
    }
}