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
    volatile public LandmarkModelList landmarkModelList;

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
                    byte[] resultLengthBytes = new byte[4];
                    int resultLength = client.Receive(resultLengthBytes);
                    // receive the result bytes from the server
                    byte[] resultBytes = new byte[System.BitConverter.ToInt32(resultLengthBytes)];
                    int result = client.Receive(resultBytes);

                    // convert the result bytes to string
                    string resultString = System.Text.Encoding.UTF8.GetString(resultBytes);
                    Debug.Log("resultString=" + resultString);

                    //string resultString = "{\n   \"face_blendshapes\":[\n      [\n         {\n            \"index\":0,\n            \"score\":2.725336571529624e-06,\n            \"display_name\":\"\",\n            \"category_name\":\"_neutral\"\n         },\n         {\n            \"index\":1,\n            \"score\":0.1256556361913681,\n            \"display_name\":\"\",\n            \"category_name\":\"browDownLeft\"\n         },\n         {\n            \"index\":2,\n            \"score\":0.10741706937551498,\n            \"display_name\":\"\",\n            \"category_name\":\"browDownRight\"\n         },\n         {\n            \"index\":3,\n            \"score\":0.00015090890519786626,\n            \"display_name\":\"\",\n            \"category_name\":\"browInnerUp\"\n         },\n         {\n            \"index\":4,\n            \"score\":0.05803176388144493,\n            \"display_name\":\"\",\n            \"category_name\":\"browOuterUpLeft\"\n         },\n         {\n            \"index\":5,\n            \"score\":0.029591241851449013,\n            \"display_name\":\"\",\n            \"category_name\":\"browOuterUpRight\"\n         },\n         {\n            \"index\":6,\n            \"score\":3.763048516702838e-05,\n            \"display_name\":\"\",\n            \"category_name\":\"cheekPuff\"\n         },\n         {\n            \"index\":7,\n            \"score\":2.0550120893858548e-07,\n            \"display_name\":\"\",\n            \"category_name\":\"cheekSquintLeft\"\n         },\n         {\n            \"index\":8,\n            \"score\":2.673679944109608e-07,\n            \"display_name\":\"\",\n            \"category_name\":\"cheekSquintRight\"\n         },\n         {\n            \"index\":9,\n            \"score\":0,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeBlinkLeft\"\n         },\n         {\n            \"index\":10,\n            \"score\":0,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeBlinkRight\"\n         },\n         {\n            \"index\":11,\n            \"score\":0.0249343104660511,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookDownLeft\"\n         },\n         {\n            \"index\":12,\n            \"score\":0.025372108444571495,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookDownRight\"\n         },\n         {\n            \"index\":13,\n            \"score\":0.05538523569703102,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookInLeft\"\n         },\n         {\n            \"index\":14,\n            \"score\":0.04630373418331146,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookInRight\"\n         },\n         {\n            \"index\":15,\n            \"score\":0.07394025474786758,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookOutLeft\"\n         },\n         {\n            \"index\":16,\n            \"score\":0.0958392471075058,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookOutRight\"\n         },\n         {\n            \"index\":17,\n            \"score\":0.2888622581958771,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookUpLeft\"\n         },\n         {\n            \"index\":18,\n            \"score\":0.2607914209365845,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeLookUpRight\"\n         },\n         {\n            \"index\":19,\n            \"score\":0.49653369188308716,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeSquintLeft\"\n         },\n         {\n            \"index\":20,\n            \"score\":0.27751195430755615,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeSquintRight\"\n         },\n         {\n            \"index\":21,\n            \"score\":0.010500242933630943,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeWideLeft\"\n         },\n         {\n            \"index\":22,\n            \"score\":0.011249943636357784,\n            \"display_name\":\"\",\n            \"category_name\":\"eyeWideRight\"\n         },\n         {\n            \"index\":23,\n            \"score\":9.395217784913257e-05,\n            \"display_name\":\"\",\n            \"category_name\":\"jawForward\"\n         },\n         {\n            \"index\":24,\n            \"score\":0.0003268206783104688,\n            \"display_name\":\"\",\n            \"category_name\":\"jawLeft\"\n         },\n         {\n            \"index\":25,\n            \"score\":0.00079760467633605,\n            \"display_name\":\"\",\n            \"category_name\":\"jawOpen\"\n         },\n         {\n            \"index\":26,\n            \"score\":1.848598003562074e-05,\n            \"display_name\":\"\",\n            \"category_name\":\"jawRight\"\n         },\n         {\n            \"index\":27,\n            \"score\":0.00010702616418711841,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthClose\"\n         },\n         {\n            \"index\":28,\n            \"score\":0.01620500721037388,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthDimpleLeft\"\n         },\n         {\n            \"index\":29,\n            \"score\":0.00832220260053873,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthDimpleRight\"\n         },\n         {\n            \"index\":30,\n            \"score\":0.0004258823173586279,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthFrownLeft\"\n         },\n         {\n            \"index\":31,\n            \"score\":0.0005020815879106522,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthFrownRight\"\n         },\n         {\n            \"index\":32,\n            \"score\":0.00029130952316336334,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthFunnel\"\n         },\n         {\n            \"index\":33,\n            \"score\":0.00028187493444420397,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthLeft\"\n         },\n         {\n            \"index\":34,\n            \"score\":0.00019980304932687432,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthLowerDownLeft\"\n         },\n         {\n            \"index\":35,\n            \"score\":0.00023617586703039706,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthLowerDownRight\"\n         },\n         {\n            \"index\":36,\n            \"score\":0.13394878804683685,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthPressLeft\"\n         },\n         {\n            \"index\":37,\n            \"score\":0.09576204419136047,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthPressRight\"\n         },\n         {\n            \"index\":38,\n            \"score\":0.00228625419549644,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthPucker\"\n         },\n         {\n            \"index\":39,\n            \"score\":0.0039457776583731174,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthRight\"\n         },\n         {\n            \"index\":40,\n            \"score\":0.004106540232896805,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthRollLower\"\n         },\n         {\n            \"index\":41,\n            \"score\":0.005921840667724609,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthRollUpper\"\n         },\n         {\n            \"index\":42,\n            \"score\":0.029016653075814247,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthShrugLower\"\n         },\n         {\n            \"index\":43,\n            \"score\":0.00828493945300579,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthShrugUpper\"\n         },\n         {\n            \"index\":44,\n            \"score\":0.648847758769989,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthSmileLeft\"\n         },\n         {\n            \"index\":45,\n            \"score\":0.5493622422218323,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthSmileRight\"\n         },\n         {\n            \"index\":46,\n            \"score\":0.007123016752302647,\n            \"display_name\":\"\",\n            ,\n            \"category_name\":\"mouthUpperUpLeft\"\n         },\n         {\n            \"index\":49,\n            \"score\":0.0004932832671329379,\n            \"display_name\":\"\",\n            \"category_name\":\"mouthUpperUpRight\"\n         },\n         {\n            \"index\":50,\n            \"score\":1.2277971563889878e-06,\n            \"display_name\":\"\",\n            \"category_name\":\"noseSneerLeft\"\n         },\n         {\n            \"index\":51,\n            \"score\":1.6909758642214001e-06,\n            \"display_name\":\"\",\n            \"category_name\":\"noseSneerRight\"\n         }\n      ]\n   ]\n}";

                    // convert the result string to landmark model list object using JSON deserialization from snake case
                    LandmarkModelList landmarkModelListResult = JsonUtility.FromJson<LandmarkModelList>(resultString);

                    // do nothing if the the landmark model list has an error
                    if (landmarkModelListResult?.Error != null)
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

                    // sleep for 800 milliseconds
                    System.Threading.Thread.Sleep(800);
                }

            }
        }
        catch (Exception e)
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
