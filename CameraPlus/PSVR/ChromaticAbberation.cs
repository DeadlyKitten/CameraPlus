using UnityEngine;
using CameraPlus.PSVR;

[AddComponentMenu("Indie Effects/Chromatic Abberation C#")]
public class ChromaticAbberation : MonoBehaviour
{
    public Shader shader;
    private Material chromMat;
    public Texture2D vignette;

    public void Start()
    {
        AssetBundleLoader.OnLoad();
        shader = AssetBundleLoader.Instance.ChromabberationShader;
        vignette = AssetBundleLoader.Instance.VignetteTexture;

        chromMat = new Material(shader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        chromMat.SetTexture("_MainTex", source);
        chromMat.SetTexture("_Vignette", vignette);
        Graphics.Blit(source, destination, chromMat);
    }
}