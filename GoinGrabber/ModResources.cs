using UnityEngine;

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

        public static GameObject InstantiateHandSlapper(Transform parent, bool localPlayer, bool rightHand)
        {
            GameObject handSlapper = GameObject.Instantiate(Bundle.LoadAsset<GameObject>("HandSlapper"), parent);
            handSlapper.name = "HandSlapper";
            Transform catcher = handSlapper.transform.Find("GoinCatcher");
            if (catcher != null)
            {
                string catcherName;
                catcherName = localPlayer ? "local" : "remote";
                catcherName += rightHand ? "RightCatcher" : "LeftCatcher";
                catcher.name = catcherName;
            }
            if (!rightHand)
            {
                handSlapper.transform.localRotation = Quaternion.Euler(0, 165, 15);
                Vector3 localPos = handSlapper.transform.localPosition;
                localPos.x = -localPos.x;
                handSlapper.transform.localPosition = localPos;
            }
            Transform slapper = handSlapper.transform.Find("SlapCollider");
            if (slapper != null)
            {
                slapper.gameObject.AddComponent<HandVelocityTracker>();
                string slapperName;
                slapperName = localPlayer ? "local" : "remote";
                slapperName += rightHand ? "RightSlapper" : "LeftSlapper";
                slapper.name = slapperName;
            }

            //remove the visuals used for testing
            Transform catcherVisual = handSlapper.transform.Find("VisualCapsule");
            catcherVisual.gameObject.SetActive(false);
            Transform slapperVisual = handSlapper.transform.Find("VisualHand");
            slapperVisual.gameObject.SetActive(false);

            return handSlapper;
        }
    }
}
