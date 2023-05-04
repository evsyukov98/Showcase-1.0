using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace ShowcaseJobBoids
{
    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Accelerations;
        
        public float DeltaTime;
        public float VelocityLimit;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 velocity = Velocities[index] + Accelerations[index] * DeltaTime;
            Vector3 direction = velocity.normalized;

            velocity = direction * Mathf.Clamp(velocity.magnitude, 1, VelocityLimit);
            
            transform.position += velocity * DeltaTime;
            transform.rotation = Quaternion.LookRotation(direction);

            Positions[index] = transform.position;
            Velocities[index] = velocity;
            Accelerations[index] = Vector3.zero;
        }
    }
}