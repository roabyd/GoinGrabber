using Il2CppRUMBLE.Players.Subsystems;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Il2CppRUMBLE.Managers;
using System.Collections;

namespace GoinGrabber
{
    public class Core : MelonMod
    {
        private string currentScene = "Loader";
        private bool lookingForNewObjects = false;
        private HashSet<int> knownInstanceIDs = new HashSet<int>();
        private static Vector3 gymGoinPosition = new Vector3(0, 1.6f, 0);

        public static MelonLogger.Instance Logger { get; private set; }
        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            Logger.Msg("GoinGrabber: Initialized.");
            // This sets GoinManager.Instance for internal and external use
            _ = new GoinManager();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            lookingForNewObjects = false;

            if (!ModResources.Initialized)
            {
                ModResources.LoadResources();
            } 
            if (currentScene.Equals("Gym") || currentScene.Equals("Map0") || currentScene.Equals("Map1"))
            {
                MelonCoroutines.Start(CreateGoinInteractionObjects());
            }
        }

        public override void OnUpdate()
        {
            //For Testing purposes only
            //if (Input.GetKeyDown(KeyCode.Space) && currentScene.Equals("Gym"))
            //{
            //    GoinManager.Instance.CreateReplacementFistBumpGoin(gymGoinPosition);
            //    ModResources.InstantiateFistbumpRing(gymGoinPosition);
            //}
        }

        private static void AnimateGoinReplacer(Vector3 position)
        {
            GoinManager.Instance.CreateReplacementFistBumpGoin(position);
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

        private IEnumerator CreateGoinInteractionObjects()
        {
            yield return new WaitForSeconds(5f);
            Transform playerRightHand = PlayerManager.instance.localPlayer.Controller.transform.GetChild(0)
                    .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            Transform playerLeftHand = PlayerManager.instance.localPlayer.Controller.transform.GetChild(0)
                .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
            ModResources.InstantiateHandSlapper(playerRightHand, true, true);
            ModResources.InstantiateHandSlapper(playerLeftHand, true, false);

            if (PlayerManager.instance.AllPlayers.Count > 1)
            {
                Transform remotePlayerRightHand = PlayerManager.instance.AllPlayers[1].Controller.transform.GetChild(0)
                    .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
                Transform remotePlayerLeftHand = PlayerManager.instance.AllPlayers[1].Controller.transform.GetChild(0)
                    .GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
                ModResources.InstantiateHandSlapper(remotePlayerRightHand, false, true);
                ModResources.InstantiateHandSlapper(remotePlayerLeftHand, false, false);
            }
        }
    }
}

