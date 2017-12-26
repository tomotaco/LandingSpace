using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotController : MonoBehaviour {

    public Texture2D textureScreenShot;

    public static int widthScreenShot = 192;
    public static int heightScreenShot = 120;
    void Start () {
        this.textureScreenShot = new Texture2D(widthScreenShot, heightScreenShot, TextureFormat.RGB24, false, false);        
    }

    public void takeScreenShot()
    {
        Debug.Log("ScreenShotController.takeScreenShot()");
        StartCoroutine(this.takeScreenShotEnumerator());
    }
    private IEnumerator takeScreenShotEnumerator()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Screen(" + Screen.width.ToString() + ", " + Screen.height.ToString() + "), " +
                "tex(" + this.textureScreenShot.width.ToString() + ", " + this.textureScreenShot.height.ToString() + ")");
        //            this.textureScreenShot.ReadPixels(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), 0, 0);
        var widthCapture = Screen.width / 4;
        var heightCapture = widthCapture * heightScreenShot / widthScreenShot;
        var offsetX = (Screen.width / 2) - (widthCapture / 2);
        var offsetY = (Screen.height / 2) - (heightCapture / 2);
        
        var textureCapture = new Texture2D(widthCapture, heightCapture, TextureFormat.RGB24, false, false);
        textureCapture.ReadPixels(new Rect(offsetX, offsetY, widthCapture, heightCapture), 0, 0);
        TextureScale.Bilinear(textureCapture, widthScreenShot, heightScreenShot);
        textureCapture.Apply(false, false);
        this.textureScreenShot.LoadRawTextureData(textureCapture.GetRawTextureData());

//        this.textureScreenShot.ReadPixels(new Rect(offsetX, offsetY, widthScreenShot, heightScreenShot), 0, 0);
        this.textureScreenShot.Apply(false, false);
        Object.Destroy(textureCapture);
    }
}
