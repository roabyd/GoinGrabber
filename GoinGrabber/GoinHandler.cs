using UnityEngine;
using MelonLoader;
using Il2CppRUMBLE.Managers;
using RumbleModdingAPI;
using UnityEngine.XR;

namespace GoinGrabber
{
    [RegisterTypeInIl2Cpp]
    public class GoinHandler : MonoBehaviour
    {
        public float initialUpwardVelocity = 4f;
        public float tossGravity = -9.81f;
        public float spinSpeed = 500f; // degrees per second
        public AudioSource flipAudioSource;
        public GameObject catchPoint;
        public float hoverDuration = 0.5f;
        private float hoverTimer = 0f;
        private bool isHovering = false;

        private float verticalVelocity;
        private bool isFalling = false;
        private bool isLaunched = false;
        private Rigidbody rb;
        private bool caught = false;

        private Transform remotePlayerLeftHand;
        private Transform remotePlayerRightHand;
        private Transform localPlayerLeftHand;
        private Transform localPlayerRightHand;
        private Transform remotePlayerRightMiddleFinger;
        private Transform remotePlayerLeftMiddleFinger;

        private bool localRightHandTriggerPressedEarly = false;
        private bool localLeftHandTriggerPressedEarly = false;
        private bool remoteRightHandTriggerPressedEarly = false;
        private bool remoteLeftHandTriggerPressedEarly = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            flipAudioSource = gameObject.GetComponent<AudioSource>();

            if (PlayerManager.instance.AllPlayers.Count > 1)
            {
                remotePlayerRightHand = PlayerManager.instance.AllPlayers[1].Controller.transform.GetChild(0)
                    .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
                remotePlayerLeftHand = PlayerManager.instance.AllPlayers[1].Controller.transform.GetChild(0)
                    .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
                remotePlayerRightMiddleFinger = remotePlayerRightHand.GetChild(0).GetChild(0);
                //X rotation will be 72
                remotePlayerLeftMiddleFinger = remotePlayerLeftHand .GetChild(0).GetChild(0);
            }
            localPlayerRightHand = PlayerManager.instance.localPlayer.Controller.transform.GetChild(0)
                .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            localPlayerLeftHand = PlayerManager.instance.localPlayer.Controller.transform.GetChild(0)
                .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
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
                        verticalVelocity += tossGravity * Time.deltaTime;

                        // Move vertically
                        transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
                        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
                        // Detect apex
                        if (verticalVelocity <= 0)
                        {
                            isHovering = true;
                            hoverTimer = hoverDuration;
                            verticalVelocity = 0f; // Freeze vertical motion
                            rb.isKinematic = false;
                            rb.angularVelocity = new Vector3(0, 10, 0);
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
                }
                else
                {
                    rb.useGravity = true;
                    isLaunched = false;
                }
            }

            //Cleanup if goin makes it below the floor
            if (transform.position.y < -20 && !caught)
            {
                GoinManager.Instance.DestroyGoin(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!rb.isKinematic)
            {
                if (other.name == "localRightCatcher" && Calls.ControllerMap.RightController.GetGrip() > 0.6)
                {
                    localRightHandTriggerPressedEarly = true;
                }
                else if (other.name == "localLeftCatcher" && Calls.ControllerMap.LeftController.GetGrip() > 0.6)
                {
                    localLeftHandTriggerPressedEarly = true;
                }
                else if (other.name == "remoteRightCatcher" && (remotePlayerRightMiddleFinger.transform.rotation.x > 70 &&
                                    remotePlayerRightMiddleFinger.transform.rotation.x < 90))
                {
                    remoteRightHandTriggerPressedEarly = true;
                }
                else if (other.name == "remoteLeftCatcher" && remotePlayerLeftMiddleFinger.transform.rotation.x > 60 &&
                                    remotePlayerLeftMiddleFinger.transform.rotation.x < 80)
                {
                    remoteLeftHandTriggerPressedEarly = true;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!rb.isKinematic)
            {
                if (other.name == "localRightCatcher" && Calls.ControllerMap.RightController.GetGrip() > 0.6 &&
                    !localRightHandTriggerPressedEarly)
                {
                    VibrationUtil.Vibrate(XRNode.RightHand, 0.5f, 0.2f);
                    TriggerCatchAnimation();
                }
                else if (other.name == "localLeftCatcher" && Calls.ControllerMap.LeftController.GetGrip() > 0.6 &&
                         !localLeftHandTriggerPressedEarly)
                {
                    VibrationUtil.Vibrate(XRNode.LeftHand, 0.5f, 0.2f);
                    TriggerCatchAnimation();
                }
                else if (other.name == "remoteRightCatcher" && (remotePlayerRightMiddleFinger.transform.rotation.x > 70 &&
                         remotePlayerRightMiddleFinger.transform.rotation.x < 90) && !remoteRightHandTriggerPressedEarly)
                {
                    TriggerCatchAnimation();
                }
                else if (other.name == "remoteLeftCatcher" && remotePlayerLeftMiddleFinger.transform.rotation.x > 60 &&
                         remotePlayerLeftMiddleFinger.transform.rotation.x < 80 && !remoteLeftHandTriggerPressedEarly)
                {
                    TriggerCatchAnimation();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!rb.isKinematic)
            { 
                if (other.name == "localRightCatcher")
                {
                    localRightHandTriggerPressedEarly = false;
                }
                else if (other.name == "localLeftCatcher")
                {
                    localLeftHandTriggerPressedEarly = false;
                }
                else if (other.name == "remoteRightCatcher")
                {
                    remoteRightHandTriggerPressedEarly = false;
                }
                else if (other.name == "remoteLeftCatcher")
                {
                    remoteLeftHandTriggerPressedEarly = false;
                }
            }
        }

        private void TriggerCatchAnimation()
        {
            catchPoint = ModResources.InstantiateCatchPoint(transform.position);
            catchPoint.GetComponent<AudioSource>().Play();
            Destroy(catchPoint, catchPoint.GetComponent<AudioSource>().clip.length);
            isLaunched = false;
            isFalling = false;
            GoinManager.Instance.DestroyGoin(gameObject);
            caught = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //If the player does not close their hand before hitting the moving hand, the goin will be slapped away
            if (collision.gameObject.name.EndsWith("Slapper") && !caught &&
                collision.gameObject.TryGetComponent<HandVelocityTracker>(out var velocityTracker))
            {
                hoverTimer = 0;
                ContactPoint contact = collision.contacts[0];
                rb.AddForceAtPosition(velocityTracker.Velocity * velocityTracker.slapForce, contact.point, ForceMode.Impulse);
                if (collision.gameObject.name.StartsWith("localRight"))
                {
                    VibrationUtil.Vibrate(XRNode.RightHand, 0.8f, 0.3f);
                }
                else if (collision.gameObject.name.StartsWith("localLeft"))
                {
                    VibrationUtil.Vibrate(XRNode.LeftHand, 0.8f, 0.3f);
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
    }
}
