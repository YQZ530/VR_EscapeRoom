using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenshot : MonoBehaviour
{
    // Start is called before the first frame update
  
    public string fileName = "Screenshot.png";
    public Camera targetCamera; // Assign in inspector
    public int width = 1920;
    public int height = 1080;

    public void TakeScreenshot(string fileName)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        targetCamera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        targetCamera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes( fileName, bytes);

        Debug.Log("Screenshot saved to: " + fileName);
    }
    
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.P))
        {
            string filepath = Application.dataPath + "/Screenshot/" + fileName;
            TakeScreenshot(filepath);
            
        }
    }
    
}
