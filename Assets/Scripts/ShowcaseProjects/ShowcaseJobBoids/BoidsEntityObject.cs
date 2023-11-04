using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShowcaseProjects.ShowcaseJobBoids
{
    public class BoidsEntityObject : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Vector2 speedRange = new Vector2(1,4);
        

        private void Start()
        {
            animator.speed = Random.Range(speedRange.y, speedRange.x);
        }
    }
}
