using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RumbleModdingAPI.Calls.GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard;
using UnityEngine;
using MelonLoader;
using Il2CppInterop.Runtime.InteropTypes;
using System.Reflection;
using Il2CppRUMBLE.Players;

namespace GoinGrabber
{
    [RegisterTypeInIl2Cpp]
    internal class GoinTosser : MonoBehaviour
    {
        public float initialUpwardVelocity = 4f;
        public float gravity = -9.81f;
        public float spinSpeed = 360f; // degrees per second
        public AudioSource flipAudioSource;
        public GameObject catchPoint;
        public float velocityThreshold = -5f; // Threshold for falling velocity
        public float hoverDuration = 0.5f;
        private float hoverTimer = 0f;
        private bool isHovering = false;

        private float verticalVelocity;
        private bool isFalling = false;
        private bool isLaunched = false;
        private Rigidbody rb;
        private bool caught = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            flipAudioSource = gameObject.GetComponent<AudioSource>();
            int interactorLayer = 31;

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), interactorLayer, false);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Floor"), false);
        }

        void Update()
        {
            if (isLaunched)
            {
                // Ascend or hover phase
                if (!isFalling)
                {
                    if (!isHovering)
                    {
                        // Apply gravity
                        verticalVelocity += gravity * Time.deltaTime;

                        // Move vertically
                        transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

                        // Detect apex
                        if (verticalVelocity <= 0)
                        {
                            isHovering = true;
                            hoverTimer = hoverDuration;
                            verticalVelocity = 0f; // Freeze motion
                        }
                    }
                    else
                    {
                        // Hovering at apex
                        hoverTimer -= Time.deltaTime;

                        if (hoverTimer <= 0f)
                        {
                            isHovering = false;
                            isFalling = true;
                            verticalVelocity = 0f; // Reset velocity before applying gravity
                        }
                    }

                    // Always rotate during ascent/hover
                    transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
                }
                else
                {
                    // Falling phase begins after hover
                    verticalVelocity += -9.81f * Time.deltaTime;
                    transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
                    transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);

                    if (verticalVelocity < velocityThreshold)
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;
                        rb.velocity = verticalVelocity * Vector3.up;
                        isLaunched = false; // End manual control
                    }
                }
            }

            if (transform.position.y < -20 && !caught)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isFalling || isHovering)
            {  
                if (other.name.Equals("InteractionHandTrigger") && !caught)
                {
                    catchPoint = ModResources.InstantiateCatchPoint(transform.position);
                    catchPoint.GetComponent<AudioSource>().Play();
                    Destroy(catchPoint, catchPoint.GetComponent<AudioSource>().clip.length);
                    isLaunched = false;
                    isFalling = false;
                    Destroy(gameObject);
                    caught = true;
                    CleanUpGameChanges();
                }
            }           
        }

        public void Launch()
        {
            verticalVelocity = initialUpwardVelocity;
            isLaunched = true;
            isFalling = false;
            caught = false;
            flipAudioSource.Play();
            rb.isKinematic = true; // Start as kinematic to prevent physics interactions
            rb.useGravity = false;
        }

        private void CleanUpGameChanges()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Interactor"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Floor"), true);
        }
    }
}
