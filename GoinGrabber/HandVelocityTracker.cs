using MelonLoader;
using UnityEngine;

namespace GoinGrabber
{
    [RegisterTypeInIl2Cpp]
    public class HandVelocityTracker : MonoBehaviour
    {
        public Vector3 Velocity { get; private set; }

        public float slapForce = 0.5f;

        private Vector3 lastPosition;

        void Start()
        {
            lastPosition = transform.position;
        }

        void Update()
        {
            Velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
        }
    }
}