﻿using System;
using System.Reflection;
using System.Collections;
using System.Collections.Concurrent;
using IPA;
using IPALogger = IPA.Logging.Logger;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CameraPlus
{
    public class Plugin : IBeatSaberPlugin
    {
        private bool _init;
        private HarmonyInstance _harmony;

        public Action<Scene, Scene> ActiveSceneChanged;
        public ConcurrentDictionary<string, CameraPlusInstance> Cameras = new ConcurrentDictionary<string, CameraPlusInstance>();

        public static Plugin Instance { get; private set; }
        public static string PluginName => "CameraPlus";
        public static string MainCamera => Plugin.PluginName.ToLower();
        
        public void OnApplicationStart()
        {
            Logger.log.Debug("OnApplicationStart");

            if (_init) return;
            _init = true;
            Instance = this;

            _harmony = HarmonyInstance.Create("com.brian91292.beatsaber.cameraplus");
            try
            {
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Failed to apply harmony patches! {ex}");
            }
            
            // Add our default cameraplus camera
            CameraUtilities.AddNewCamera(Plugin.MainCamera);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void OnActiveSceneChanged(Scene from, Scene to)
        {
            SharedCoroutineStarter.instance.StartCoroutine(DelayedActiveSceneChanged(from, to));
        }

        IEnumerator DelayedActiveSceneChanged(Scene from, Scene to)
        {
            yield return new WaitForSeconds(0.5f);

            // If any new cameras have been added to the config folder, render them
            CameraUtilities.ReloadCameras();

            try
            {
                if (ActiveSceneChanged != null)
                {
                    // Invoke each activeSceneChanged event
                    foreach (var func in ActiveSceneChanged?.GetInvocationList())
                    {
                        try
                        {
                            func?.DynamicInvoke(from, to);
                        }
                        catch (Exception ex)
                        {
                            Logger.log.Error($"Exception while invoking ActiveSceneChanged! {ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Warn($"'{ex.TargetSite}' threw an exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;

            _harmony.UnpatchAll("com.brian91292.beatsaber.cameraplus");
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
        }

        public void OnSceneUnloaded(Scene scene)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
            // Fix the cursor when the user resizes the main camera to be smaller than the canvas size and they hover over the black portion of the canvas
            if (CameraPlusBehaviour.currentCursor != CameraPlusBehaviour.CursorType.None && !CameraPlusBehaviour.anyInstanceBusy && 
                CameraPlusBehaviour.wasWithinBorder && CameraPlusBehaviour.GetTopmostInstanceAtCursorPos() == null)
            {
                CameraPlusBehaviour.SetCursor(CameraPlusBehaviour.CursorType.None);
                CameraPlusBehaviour.wasWithinBorder = false;
            }
        }

        public void Init(IPALogger logger)
        {
            Logger.log = logger;
            Logger.log.Debug("Logger prepared");
        }
    }
}