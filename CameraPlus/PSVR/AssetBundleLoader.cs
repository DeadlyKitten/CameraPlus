using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Collections;

namespace CameraPlus.PSVR
{
    

    class AssetBundleLoader : MonoBehaviour
    {
        Shader fisheyeShader;
        Shader chromabberationShader;
        Texture2D vignetteTexture;

        public static AssetBundleLoader Instance;

        public Shader FisheyeShader { get => fisheyeShader; private set => fisheyeShader = value; }
        public Shader ChromabberationShader { get => chromabberationShader; private set => chromabberationShader = value; }
        public Texture2D VignetteTexture { get => vignetteTexture; private set => vignetteTexture = value; }

        public static void OnLoad()
        {
            if (Instance != null) return;
            new GameObject("AssetBundleLoader").AddComponent<AssetBundleLoader>();
        }

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;

            DontDestroyOnLoad(gameObject);


            LoadFromFile();
        }

        private void LoadFromFile()
        {
            Log("Loading assetbundle");
            // load shader
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Environment.CurrentDirectory, "UserData/CameraPlus"), "psvr.bundle"));

            if (myLoadedAssetBundle == null)
            {
                Log("Failed to load AssetBundle! Make sure PluginsContent/LUTSaber.bundle exists!");
                return;
            }

            ChromabberationShader = myLoadedAssetBundle.LoadAsset<Shader>("Assets/psvr/Chromabberation.shader");
            Log($"Loaded {ChromabberationShader.name}");
            FisheyeShader = myLoadedAssetBundle.LoadAsset<Shader>("Assets/psvr/FisheyeShader.shader");
            Log($"Loaded {FisheyeShader.name}");
            VignetteTexture = myLoadedAssetBundle.LoadAsset<Texture2D>("Assets/psvr/Vignette 2.png");
            Log($"Loaded {VignetteTexture.name}");

        }

        private void Log(string s)
        {
            Console.WriteLine("[AssetBundleLoader] " + s);
        }


    }
}
