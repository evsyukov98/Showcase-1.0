using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace ShowcaseJobBoids
{
    public class BoidsBehavior : MonoBehaviour
    {
        [SerializeField] private int numberOfEntities = 100;
        [SerializeField] private GameObject entityPrefab;
        [Header("Acceleration Job:")]
        [SerializeField] private float destinationThreshold = 20;
        [SerializeField] private Vector3 weights;
        [Header("Bound Job:")]
        [SerializeField] private Vector3 areaSize;
        [SerializeField] private float boundsThreshold = 5;
        [SerializeField] private float boundsMultiplier = 8;
        [Header("Move Job:")]
        [SerializeField] private float velocityLimit = 3;
        

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
                DestinationThreshold = destinationThreshold,
                Weights = weights
            };
            
            BoundsJob boundsJob = new BoundsJob()
            {
                Positions = _positions,
                Accelerations = _accelerations,
                AreaSize = areaSize,
                BoundsThreshold = boundsThreshold,
                BoundsMultiplier = boundsMultiplier
            };
            
            MoveJob moveJob = new MoveJob()
            {
                Positions = _positions,     
                Velocities = _velocities,   
                Accelerations = _accelerations,
                DeltaTime = Time.deltaTime,  
                VelocityLimit = velocityLimit
            };

            var accelerationHandle = accelerationJob.
                Schedule(numberOfEntities, 128);
            
            var boundsHandle = boundsJob.
                Schedule(numberOfEntities, 128, accelerationHandle);

            var moveHandel = moveJob.
                Schedule(_transformsAccess, boundsHandle); 
            
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
