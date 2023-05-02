using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace ShowcaseJobBoids
{
    public class BoidsBehavior : MonoBehaviour
    {
        [SerializeField] private int numberOfEntities;
        [SerializeField] private float destinationThreshold;
        [SerializeField] private GameObject entityPrefab;

        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velocities;
        private NativeArray<Vector3> _accelerations;

        private TransformAccessArray _transformsAccess;

        private void Start()
        {
            _positions = new NativeArray<Vector3>(numberOfEntities, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(numberOfEntities, Allocator.Persistent);
            _accelerations = new NativeArray<Vector3>(numberOfEntities, Allocator.Persistent);

            Transform[] transforms = new Transform[numberOfEntities];

            for (int i = 0; i < numberOfEntities; i++)
            {
                transforms[i] = Instantiate(entityPrefab).transform;
                _positions[i] = transforms[i].position; 
                _velocities[i] = Random.insideUnitSphere;
            }

            _transformsAccess = new TransformAccessArray(transforms); // обертка для IJobParallelForTransform 
        }
        
        private void Update()
        {
            AccelerationJob accelerationJob = new AccelerationJob()
            {
                Positions = _positions, 
                Velocities = _velocities,
                Accelerations = _accelerations,
                DestinationThreshold = destinationThreshold
            };
            
            MoveJob moveJob = new MoveJob()
            {
                Positions = _positions,     // Позиция у всех начальная 0
                Velocities = _velocities,   // Случайные направления 
                Accelerations = _accelerations,
                DeltaTime = Time.deltaTime  // Скорость перемещения к направлению
            };

            JobHandle accelerationHandle = accelerationJob.Schedule(numberOfEntities, 0);
            JobHandle moveHandel = moveJob.Schedule(_transformsAccess, accelerationHandle); // выполниться после вычисления Acceleration
            moveHandel.Complete();
        }

        private void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _accelerations.Dispose();
            _transformsAccess.Dispose();
        }
    }
    
    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Accelerations;
        
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 velocity = Velocities[index] + Accelerations[index] * DeltaTime;
            Vector3 direction = velocity.normalized;
            
            transform.position += velocity * DeltaTime;
            transform.rotation = Quaternion.LookRotation(direction);

            Positions[index] = transform.position;
            Velocities[index] = velocity;
        }
    }

    public struct AccelerationJob : IJobParallelFor
    {
        // У данного интерфейса есть ограничения на использование Vector
        // дать доступ на чтение можно атрибутом ReadOnly
        [ReadOnly] 
        public NativeArray<Vector3> Positions;
        [ReadOnly] 
        public NativeArray<Vector3> Velocities;
        
        public NativeArray<Vector3> Accelerations;

        public float DestinationThreshold;

        private int Count => Positions.Length - 1;

        public void Execute(int index)
        {
            Vector3 myPosition = Positions[index];                               // позиция текущего обьекта
            
            Vector3 averageSpread = Vector3.zero;
            Vector3 averageVelocity = Vector3.zero;
            Vector3 averagePosition = Vector3.zero;

            int neighborCount = 0;                                               // кол-во рядом стоящих соседей

            for (int i = 0; i < Count; i++)
            {
                if (i == index)                                                  // себя пропустить
                    continue;

                Vector3 neighbourPosition = Positions[i];                        // позиция соседнего обьекта
                Vector3 positionsDifference = myPosition - neighbourPosition;    // вектор растояние к соседу
                
                if (positionsDifference.magnitude > DestinationThreshold)        // если слишком далеко, пропустить сосед
                    continue;

                neighborCount++;
                
                averageSpread += positionsDifference.normalized;                 // находим среднее направление + растояние до соседей
                averageVelocity += Velocities[i];                                // находим среднюю вектор направлений + скоростей соседей
                averagePosition += neighbourPosition;                            // находим среднюю позицию соседей
            }

            Accelerations[index] = averageSpread / neighborCount +               //    
                                   averageVelocity / neighborCount +             // балансирует скорость чтобы двигался как все + в томже направлении
                                   averagePosition / neighborCount - myPosition; // задает в основном скорость и вектор направления к стае
        }
    }
}
