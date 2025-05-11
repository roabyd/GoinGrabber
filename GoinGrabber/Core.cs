using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Reflection;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppRUMBLE.Players;
using MelonLoader;
using RumbleModdingAPI;
using System.Collections;
using UnityEngine;
using HarmonyLib;
using Il2Cpp;
using System.Data;
using static RumbleModdingAPI.Calls.GameObjects.Map0.LightingAndEffects;
using UnityEngine.VFX;
using Il2CppRUMBLE.Pools;
using System.Reflection;
using UnityEngine.SceneManagement;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppRUMBLE.Managers;




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
            Logger.Msg("StoneMatchTracker: Initialized.");
        }


        public override void OnApplicationStart()
        {
            MelonLoader.MelonLogger.Msg("GoinGrabber Loaded!");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            lookingForNewObjects = false;
            Logger.Msg($"Scene loaded: {currentScene}");

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
            if (Input.GetKeyDown(KeyCode.Space) && currentScene.Equals("Gym"))
            {
                GoinReplacerObject.GetComponent<GoinReplacer>().TossGoin();
                ModResources.InstantiateFistbumpRing(gymGoinPosition);
            }
        }

        public override void OnLateInitializeMelon()
        {
            Calls.onRoundEnded += matchEnded;
        }

        private void matchEnded()
        {

        }

        private static void InstantiateGoinReplacer(Vector3 position)
        {
            Logger.Msg("Instantiating GoinReplacer at position: " + position);
            GoinReplacerObject = new GameObject("GoinReplacer");
            GoinReplacerObject.transform.position = position; // Set the position to the center of the scene
            GoinReplacerObject.AddComponent<GoinReplacer>();
            MelonLogger.Msg("GoinReplacer Loaded!");
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

                Logger.Msg($"Fist bump effects disabled on {__instance.gameObject.name}");
                return true;
            }
        }
    }
}

