using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class SocketController : MonoBehaviour
{
    // setup the server IP address
    [SerializeField] private string serverIP = "127.0.0.1";
    // setup the server port
    [SerializeField] private int serverPort = 65432;

    // setup the bridge controller
    private BridgeController bridgeController;
    // setup the model controller
    private ModelController modelController;

    // register thread for socket
    private System.Threading.Thread thread;
    volatile private bool keepSending = false;
    // register landmark model list
    volatile private LandmarkModelList landmarkModelList;

    volatile private byte[] imageBytes;

    // Start is called before the first frame update
    void Start()
    {
        // Program is suspended if bridgeController is null
        if (!TryGetComponent<BridgeController>(out bridgeController))
        {
            Debug.LogError("[SocketController] BridgeController is null");
            return;
        }

        // Program is suspended if modelController is null
        if (!TryGetComponent<ModelController>(out modelController))
        {
            Debug.LogError("[SocketController] ModelController is null");
            return;
        }

        // setup the application to run in background
        Application.runInBackground = true;
        // create a new thread for the socket
        thread = new System.Threading.Thread(() => HandleSocketConnection(serverIP, serverPort))
        {
            IsBackground = true
        };
        thread.Start();
    }

    void Update()
    {
        // getting new image bytes from the bridge controller
        imageBytes = bridgeController.GetImageBytes();

        // restart the thread if it is not running
        if (thread == null || !thread.IsAlive)
        {
            // create a new thread for the socket
            thread = new System.Threading.Thread(() => HandleSocketConnection(serverIP, serverPort))
            {
                IsBackground = true
            };
            thread.Start();
        }

        // update the landmark model list to the model controller if it is not null or empty
        if (landmarkModelList != null && landmarkModelList.FaceBlendshapes != null && landmarkModelList.FaceBlendshapes.Count > 0)
        {
            modelController.landmarkModels = landmarkModelList.FaceBlendshapes;
        }
        

        // if (imageBytes != null)
        // {
        //     Debug.Log("[SocketController] Image bytes length = " + imageBytes.Length);
        //     // generate new guid
        //     string guid = System.Guid.NewGuid().ToString();
        //     // create directory if it does not exist
        //     if (!System.IO.Directory.Exists(Application.persistentDataPath + "/ignored"))
        //     {
        //         System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/ignored");
        //     }
        //     // save image bytes to the file in png format
        //     System.IO.File.WriteAllBytes(Application.persistentDataPath + "/ignored/send-image-" + guid + ".png", imageBytes);
        //     Debug.Log("[SocketController] Image saved to " + Application.persistentDataPath + "/ignored/send-image-" + guid + ".png");
        // }
    }

    // socket listener
    private Socket client;

    // Create socket connection to the server with the server IP and server port
    void HandleSocketConnection(string serverIP, int serverPort)
    {
        // setup the server IP address
        System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(serverIP);
        // setup the server port
        System.Net.IPEndPoint localEndPoint = new System.Net.IPEndPoint(ipAddress, serverPort);

        // create a TCP/IP socket to send the image bytes
        client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // bind the socket to the local endpoint and connecting for sending data
        try
        {
            client.Connect(localEndPoint);

            keepSending = true;
            while (keepSending)
            {
                if (imageBytes != null)
                {
                    // send the image bytes length to the server
                    byte[] imageLengthBytes = System.BitConverter.GetBytes(imageBytes.Length);
                    int bytesSentLength = client.Send(imageLengthBytes);
                    // send the image bytes to the server
                    int bytesSent = client.Send(imageBytes);

                    // receive the response from the server
                    byte[] resultLengthBytes = new byte[8];
                    int resultLength = client.Receive(resultLengthBytes);

                    byte[] resultBytes = new byte[System.BitConverter.ToInt32(resultLengthBytes)];
                    int result = client.Receive(resultBytes);

                    // convert the result bytes to string
                    string resultString = System.Text.Encoding.UTF8.GetString(resultBytes);
                    // convert the result string to landmark model list object using JSON deserialization from snake case
                    LandmarkModelList landmarkModelListResult = JsonUtility.FromJson<LandmarkModelList>(resultString);

                    // do nothing if the the landmark model list has an error
                    if (landmarkModelListResult.Error != null)
                    {
                        Debug.Log("[SocketController] " + landmarkModelListResult.Error);
                    }
                    else
                    {
                        // update landmark model list
                        landmarkModelList = landmarkModelListResult;
                    }

                    // set image bytes to null
                    imageBytes = null;
                }

                        // sleep for 800 milliseconds
                        System.Threading.Thread.Sleep(800);
                    }
                }

            }
        }
        catch (System.Exception e)
        {
            Debug.Log("[SocketController] " + e.ToString());

            // stop the socket connection
            StopSocketConnection();
        }
    }

    private void StopSocketConnection()
    {
        // set keep sending to false
        keepSending = false;
        // stop thread if it is running
        if (thread != null && thread.IsAlive)
        {
            thread.Abort();
        }
        // close the socket
        client?.Close();
    }

    // on disable
    void OnDisable()
    {
        StopSocketConnection();
    }
}
