using System;
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
        [SerializeField] private Vector3 areaSize;
        [SerializeField] private float velocityLimit = 3;
        [SerializeField] private Vector3 weights;
        

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
            BoundsJob boundsJob = new BoundsJob()
            {
                Positions = _positions,
                Accelerations = _accelerations,
                AreaSize = areaSize
            };
            
            AccelerationJob accelerationJob = new AccelerationJob()
            {
                Positions = _positions, 
                Velocities = _velocities,
                Accelerations = _accelerations,
                DestinationThreshold = destinationThreshold,
                Weights = weights
            };
            
            MoveJob moveJob = new MoveJob()
            {
                Positions = _positions,     // Позиция у всех начальная 0
                Velocities = _velocities,   // Случайные направления 
                Accelerations = _accelerations,
                DeltaTime = Time.deltaTime,  // Скорость перемещения к направлению
                VelocityLimit = velocityLimit
            };

            var boundsHandle = boundsJob.
                Schedule(numberOfEntities, 128);
            
            var accelerationHandle = accelerationJob.
                Schedule(numberOfEntities, 128, boundsHandle);
            
            var moveHandel = moveJob.
                Schedule(_transformsAccess, accelerationHandle); // выполниться после вычисления Acceleration
            
            moveHandel.Complete();
        }

        private void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _accelerations.Dispose();
            _transformsAccess.Dispose();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Vector3.zero, areaSize);
        }
    }
}
