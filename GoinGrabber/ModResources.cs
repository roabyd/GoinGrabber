using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RumbleModdingAPI.Calls.GameObjects.Gym.Tutorial.WorldTutorials.CombatCarvings;

namespace GoinGrabber
{
    internal class ModResources
    {
        public static Il2CppAssetBundle Bundle;

        public static GameObject GoinPrefab;

        private static bool initialized = false;
        public static bool Initialized { get { return initialized; } }

        public static void LoadResources(bool reload = false)
        {
            if (initialized && !reload) return;

            Bundle = Il2CppAssetBundleManager.LoadFromFile(@"UserData/goingrabberbundle");

            initialized = true;
        }

        public static GameObject InstantiateGoin(Vector3 position)
        {
            GameObject goinObject = GameObject.Instantiate(Bundle.LoadAsset<GameObject>("Goin"));
            goinObject.transform.position = position;

            goinObject.name = "CustomGoin";
            return goinObject;
        }

        public static GameObject InstantiateCatchPoint(Vector3 position)
        {
            GameObject goinObject = GameObject.Instantiate(Bundle.LoadAsset<GameObject>("CatchPoint"));
            goinObject.transform.position = position;

            goinObject.name = "CatchPoint";
            return goinObject;
        }

        public static GameObject InstantiateFistbumpRing(Vector3 position)
        {
            GameObject fistbumpRing = GameObject.Instantiate(Bundle.LoadAsset<GameObject>("FistbumpRing"));
            fistbumpRing.transform.position = position;
            fistbumpRing.AddComponent<RingDissolver>();

            fistbumpRing.name = "FistbumpRing";
            return fistbumpRing;
        }
    }
}
