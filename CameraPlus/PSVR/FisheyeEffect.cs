using UnityEngine;
using CameraPlus.PSVR;

/* 
						---Fisheye Image Effect---
This Indie Effects script is an adaption of the Unity Pro Fisheye Effect done by me.
If you want me to attempt to convert a unity pro image effect, consult the manual for my
forum link and email address.
*/

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Indie Effects/Fisheye C#")]
public class FisheyeEffect : MonoBehaviour
{
    public float strengthX = 0.2f;
    public float strengthY = 0.2f;
    public Shader fishEyeShader;
    private Texture2D tex;
    private Material fisheyeMaterial;

    public void Start()
    {
        AssetBundleLoader.OnLoad();
        fishEyeShader = AssetBundleLoader.Instance.FisheyeShader;

        fisheyeMaterial = new Material(fishEyeShader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        float oneOverBaseSize = 80.0f / 512.0f;

        float ar = (Screen.width * 1.0f) / (Screen.height * 1.0f);

        fisheyeMaterial.mainTexture = source;
        fisheyeMaterial.SetVector("intensity", new Vector4(strengthX * ar * oneOverBaseSize, strengthY * oneOverBaseSize, strengthX * ar * oneOverBaseSize, strengthY * oneOverBaseSize));
        Graphics.Blit(source, destination, fisheyeMaterial);
    }
}