using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    // register Image reference to webcam texture
    [SerializeField]
    private UnityEngine.UI.RawImage rawImage;

    // image getter from the webcam texture and return png format
    public byte[] GetImageBytes()
    {
        // get the webcam texture
        WebCamTexture webcamTexture = (WebCamTexture)rawImage.texture;
        // get the camera resolution
        int width = webcamTexture.width;
        int height = webcamTexture.height;
        // get the camera pixels
        Color32[] pixels = webcamTexture.GetPixels32();
        // create a new texture2D
        Texture2D tex = new Texture2D(width, height);
        // set the pixels to the texture2D
        tex.SetPixels32(pixels);
        // apply the pixels
        tex.Apply();
        // return the texture2D to png format
        return tex.EncodeToPNG();
    }

    // Start is called before the first frame update
    void Start()
    {
        // getting devies from WebCamTexture
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices)
        {
            Debug.Log("[BridgeController] devicename:" + device.name);
        }

        // getting the first device
        WebCamTexture webcamTexture = new WebCamTexture(devices[0].name)
        {
            // set the fps to 15
            requestedFPS = 15,
            // set resolution to 640x480
            requestedWidth = 640,
            requestedHeight = 480
        };

        rawImage.texture = webcamTexture;
        // flip the camera in horizontal
        rawImage.transform.localScale = new Vector3(-1, 1, 1);
        // play the camera
        webcamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // get camera resolution
        Debug.Log("[BridgeController] width:" + rawImage.texture.width + " height:" + rawImage.texture.height);
        // update rawImage resolution to camera resolution with scale to fit one half of the screen
        rawImage.rectTransform.sizeDelta = new Vector2(rawImage.texture.width / 2, rawImage.texture.height / 2);
        
    }
}
