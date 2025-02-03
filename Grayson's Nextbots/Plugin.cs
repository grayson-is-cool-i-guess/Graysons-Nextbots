using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using GraysonsNextbots.Nextbots;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

namespace GraysonsNextbots
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public bool InRoom = false;

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void Log(string message)
        {
            Logger.Log("[Grayson's Nextbots] " + message);
        }

        public static Texture2D nextbotLogo;
        public static AudioClip ambienceAudio;

        private void Awake()
        {
            StartCoroutine(LoadAssetsAtRuntime());
        }

        private void Update()
        {
            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                if (!InRoom) InRoom = true;
            }
            else
            {
                if (InRoom)
                {
                    InRoom = false;
                }

                foreach (GameObject nextbots in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
                {
                    if (nextbots.name.Contains("Nextbot"))
                    {
                        nextbots.Destroy();
                    }
                }
            }

            if (InRoom)
            {
                NextbotManager.Summon();
            }
        }

        private IEnumerator LoadAssetsAtRuntime()
        {
            string GraysonsNextbotsFolder = Path.Combine(Paths.PluginPath, "Grayson's Nextbots");
            if (!Directory.Exists(GraysonsNextbotsFolder))
            {
                Directory.CreateDirectory(GraysonsNextbotsFolder);
                Log("The folder 'Grayson's Nexbots' could not be found, had to create it as it is necissary for the mod: " + GraysonsNextbotsFolder);
            }







            // --- Image Fetching ---
            string[] logoFilenames = { "logo.png", "logo.jpg" };
            string imagePathIGuess = null;
            foreach (string filename in logoFilenames)
            {
                string path = Path.Combine(GraysonsNextbotsFolder, filename);
                if (File.Exists(path))
                {
                    imagePathIGuess = path;
                    break;
                }
            }
            if (imagePathIGuess != null)
            {
                try
                {
                    byte[] imageData = File.ReadAllBytes(imagePathIGuess);
                    nextbotLogo = new Texture2D(2, 2);
                    if (!nextbotLogo.LoadImage(imageData))
                    {
                        Log("Either logo.png or logo.jpg failed to load! Are they created?\n" + imagePathIGuess);
                    }
                    else
                    {
                        Log("Done loading image!");
                    }
                }
                catch (Exception uhgaiwoyhgioeuhiwea)
                {
                    Log("Exception while loading logo: " + uhgaiwoyhgioeuhiwea);
                }
            }
            else
            {
                Texture2D blankimagebecausetheactualonecouldntbeloaded = new Texture2D(564, 564);
                Color[] ahgjogujwaeiougahiouwheog = new Color[4] { Color.white, Color.white, Color.white, Color.white };
                blankimagebecausetheactualonecouldntbeloaded.SetPixels(ahgjogujwaeiougahiouwheog);
                blankimagebecausetheactualonecouldntbeloaded.Apply();
                nextbotLogo = blankimagebecausetheactualonecouldntbeloaded;
                Log("Logo file not found. Created a blank texture.");
                File.WriteAllBytes(Path.Combine(GraysonsNextbotsFolder, "logo.png"), blankimagebecausetheactualonecouldntbeloaded.EncodeToPNG());
            }







            // --- Ambient Audio Fetching ---
            string audioFilename = "ambience.ogg";
            string audioPath = Path.Combine(GraysonsNextbotsFolder, audioFilename);
            if (File.Exists(audioPath))
            {
                string uri = "file://" + audioPath;
                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result != UnityWebRequest.Result.Success)
                    {
                        Log("ambience.ogg fetching FAILED! " + uwr.error);
                    }
                    else
                    {
                        ambienceAudio = DownloadHandlerAudioClip.GetContent(uwr);
                        Log("done loading audio!!!!!!!!!!!!");
                    }
                }
            }
            else
            {
                byte[] blankAudio = new byte[0];
                File.WriteAllBytes(audioPath, blankAudio);
                Log("No ambience.ogg file found, created an empty one!!!!!");
            }
        }
    }
}