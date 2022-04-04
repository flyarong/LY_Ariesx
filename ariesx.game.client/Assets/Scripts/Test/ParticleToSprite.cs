using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleToSprite : MonoBehaviour {

    //private Camera mainCamera;
    //private RenderTexture renderTexture;

    private ParticleSystem particle;

    private string path = "/Arts/Temp/";

    public Camera blackCam;
    public Camera whiteCam;

    public int framesToCapture = 25;
    public int frameRate = 25;

    private RenderTexture blackCamRenderTexture;
    private RenderTexture whiteCamRenderTexture;

    private Texture2D texb;
    private Texture2D texw;
    private Texture2D outputtex;

    private float originaltimescaleTime;
    private int videoframe = 0;

    [HideInInspector]
    public bool start = false;

    void Awake() {
        Application.targetFrameRate = 60;
        this.particle = this.GetComponent<ParticleSystem>();
        originaltimescaleTime = Time.timeScale;

    }

    void Update() {
        if (this.start) {
            StartCoroutine(this.GenerateTexture());
        }
    }

    public void StartGenerating() {
        this.videoframe = 0;
        this.start = true;
        this.particle.Play();
    }

    public IEnumerator GenerateTexture() {
        if (videoframe < framesToCapture) {
            string filename = Application.dataPath + this.path + this.name + "_" + videoframe + ".png";
            Time.timeScale = 0;
            yield return new WaitForEndOfFrame();
            blackCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            whiteCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

            blackCam.targetTexture = blackCamRenderTexture;
            blackCam.Render();
            RenderTexture.active = blackCamRenderTexture;
            texb = GetTex2D(true);

            //Now do it for Alpha Camera
            whiteCam.targetTexture = whiteCamRenderTexture;
            whiteCam.Render();
            RenderTexture.active = whiteCamRenderTexture;
            texw = GetTex2D(true);
            // If not using render textures then simply get the images from both cameras
            // If we have both textures then create final output texture
            if (texw && texb) {
                
                int width = Screen.width;
                int height = Screen.height;

                // If we are not using a render texture then the width will only be half the screen
                outputtex = new Texture2D(width, height, TextureFormat.ARGB32, false);

                //// Create Alpha from the difference between black and white camera renders
                for (int y = 0; y < outputtex.height; ++y) { // each row
                    for (int x = 0; x < outputtex.width; ++x) { // each column
                        float alpha;
                        alpha = texw.GetPixel(x, y).r - texb.GetPixel(x, y).r;
                        alpha = 1.0f - alpha;
                        Color color;
                        if (alpha == 0) {
                            color = Color.clear;
                        } else {
                            color = texb.GetPixel(x, y) / alpha;
                        }
                        color.a = alpha;
                        outputtex.SetPixel(x, y, color);
                    }
                }

                // Encode the resulting output texture to a byte array then write to the file
                byte[] pngShot = outputtex.EncodeToPNG();
                Debug.LogError("Save: " + filename);
                File.WriteAllBytes(filename, pngShot);

                // Reset the time scale, then move on to the next frame.
                Time.timeScale = originaltimescaleTime;
                videoframe++;
            }
        } else {
            Debug.Log("Complete! " + videoframe + " videoframes rendered (0 indexed)");
            this.start = false;
        }
    }


    private Texture2D GetTex2D(bool renderAll) {
        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        if (!renderAll) {
            width = width / 2;
        }

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        return tex;
    }
}
