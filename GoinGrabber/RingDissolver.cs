using MelonLoader;
using UnityEngine;

namespace GoinGrabber
{
    [RegisterTypeInIl2Cpp]
    internal class RingDissolver : MonoBehaviour
    {
        public Material material; // assign in inspector or via script
        public float dissolveTime = 1.0f; // time to dissolve

        private float startTime;

        private void Start()
        {
            material = GetComponent<Renderer>().material;
            startTime = Time.time; // mark the time when object is instantiated
        }

        void Update()
        {
            float elapsed = Time.time - startTime;
            float t = Mathf.Clamp01(elapsed / dissolveTime); // normalized [0,1] time
            float dissolveValue = Mathf.Lerp(0.1f, 1, t);
            material.SetFloat("_DissolveAmount", dissolveValue);

            if (elapsed >= dissolveTime)
            {
                Destroy(gameObject); // destroy the object after dissolving
            }
        }
    }
}
