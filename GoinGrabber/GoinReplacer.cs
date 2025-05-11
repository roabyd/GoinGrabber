using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoinGrabber
{
    [RegisterTypeInIl2Cpp]
    internal class GoinReplacer : MonoBehaviour
    {
        private GameObject goinInstance; // Second prefab to replace the object with
        public float initialUpwardVelocity = 3.5f;
        public float gravity = -15f;
        public float spinSpeed = 360f; // degrees per second
        public float velocityThreshold = -5f; // Threshold for falling velocity
        public float hoverDuration = 0.5f;

        private GoinTosser tossScript;

        public void TossGoin()
        {
            goinInstance = ModResources.InstantiateGoin(transform.position);
            tossScript = goinInstance.AddComponent<GoinTosser>(); // Add TossAndSpin component to the prefab

            tossScript.initialUpwardVelocity = initialUpwardVelocity; // Set the initial upward velocity
            tossScript.gravity = gravity; // Set the gravity
            tossScript.spinSpeed = spinSpeed; // Set the spin speed
            tossScript.velocityThreshold = velocityThreshold;
            tossScript.hoverDuration = hoverDuration; // Set the hover duration
            tossScript.Launch();
        }
    }
}
