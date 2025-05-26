using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoinGrabber
{    
    public class GoinManager
    {
        public static GoinManager Instance { get; private set; }
        public List<GameObject> activeGoins { get; private set; } = new List<GameObject>();
        public int maxGoins { get; set; } = 10;

        public float initialTossUpwardVelocity = 3.5f;
        public float tossGravity = -15f;
        public float tossSpinSpeed = 500f; // degrees per second
        public float tossHoverDuration = 0.5f;

        public GoinManager()
        {
            Instance = this;
        }

        public void CreateReplacementFistBumpGoin(Vector3 position)
        {
            GameObject goinInstance = AddGoinToScene(position);
            GoinHandler goinHandler = goinInstance.GetComponent<GoinHandler>();

            goinHandler.initialUpwardVelocity = initialTossUpwardVelocity; // Set the initial upward velocity
            goinHandler.tossGravity = tossGravity; // Set the gravity
            goinHandler.spinSpeed = tossSpinSpeed; // Set the spin speed
            goinHandler.hoverDuration = tossHoverDuration; // Set the hover duration
            goinHandler.Launch();
        }

        public GameObject AddGoinToScene(Vector3 position)
        {
            GameObject newGoin = ModResources.InstantiateGoin(position);
            newGoin.AddComponent<GoinHandler>();
            if (activeGoins.Count == 0)
                EnableGoinPhysicsInteractions(true); // Enable physics interactions when the first Goin is added
            activeGoins.Add(newGoin);
            if (activeGoins.Count > maxGoins)
            {
                GameObject toRemove = activeGoins[0];
                activeGoins.RemoveAt(0);
                GameObject.Destroy(toRemove);
            }
            return newGoin;
        }

        public void DestroyGoin(GameObject target)
        {
            if (activeGoins.Remove(target))
            {
                GameObject.Destroy(target);
            }
            if (activeGoins.Count == 0)
                EnableGoinPhysicsInteractions(false); // Disable physics interactions when no Goins are left
        }

        public void ClearGoins()
        {
            activeGoins.Clear();
        }

        public void EnableGoinPhysicsInteractions(bool enable)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Default"), !enable);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("CombatFloor"), !enable);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Floor"), !enable);
        }

    }
}
