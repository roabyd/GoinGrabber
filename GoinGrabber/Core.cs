using Il2CppRUMBLE.Players.Subsystems;
using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace GoinGrabber
{
    public class Core : MelonMod
    {
        private string currentScene = "Loader";
        private static GameObject GoinReplacerObject;
        private bool lookingForNewObjects = false;
        private HashSet<int> knownInstanceIDs = new HashSet<int>();
        private static Vector3 gymGoinPosition = new Vector3(0, 1.6f, 0);

        public static MelonLogger.Instance Logger { get; private set; }
        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            Logger.Msg("GoinGrabber: Initialized.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            lookingForNewObjects = false;

            if (!ModResources.Initialized)
            {
                ModResources.LoadResources();
            } 
            if (currentScene.Equals("Gym"))
            {
                InstantiateGoinReplacer(gymGoinPosition);
            }
        }

        public override void OnUpdate()
        {
            //For Testing purposes only
            //if (Input.GetKeyDown(KeyCode.Space) && currentScene.Equals("Gym"))
            //{
            //    GoinReplacerObject.GetComponent<GoinReplacer>().TossGoin();
            //    ModResources.InstantiateFistbumpRing(gymGoinPosition);
            //}
        }

        private static void InstantiateGoinReplacer(Vector3 position)
        {
            GoinReplacerObject = new GameObject("GoinReplacer");
            GoinReplacerObject.transform.position = position; // Set the position to the center of the scene
            GoinReplacerObject.AddComponent<GoinReplacer>();
        }

        private static void AnimateGoinReplacer(Vector3 position)
        {
            InstantiateGoinReplacer(position);
            GoinReplacerObject.GetComponent<GoinReplacer>().TossGoin();
            ModResources.InstantiateFistbumpRing(position);
        }

        [HarmonyPatch(typeof(PlayerBoxInteractionSystem))]
        [HarmonyPatch("ExecuteFistBumpReward")]
        public static class FistBumpBonusPatch
        {
            // This method will run before the original Awake method
            [HarmonyPrefix]
            public static bool Prefix(PlayerBoxInteractionSystem __instance, Vector3 interactionPosition)
            {
                // Disable the fist bump effects immediately when the object is initialized
                if (__instance.onFistBumpBonusVFX != null)
                {
                    AnimateGoinReplacer(interactionPosition);     
                    __instance.onFistBumpBonusVFX = null;
                }
                if (__instance.onFistBumpBonusSFX != null)
                {
                    __instance.onFistBumpBonusSFX = null;
                }
                return true;
            }
        }
    }
}

