using System.Collections;
using System.Collections.Generic;
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

    // register thread for socket
    private System.Threading.Thread thread;
    volatile bool keepSending = false;

    volatile byte[] imageBytes;

    // Start is called before the first frame update
    void Start()
    {
        // Program is suspended if bridgeController is null
        if (!TryGetComponent<BridgeController>(out bridgeController))
        {
            Debug.LogError("[SocketController] BridgeController is null");
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
    }

    // socket listener
    private Socket client;
    // socket handler
    private Socket handler;

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
            Debug.Log("[SocketController] Socket connected to " + client.RemoteEndPoint.ToString());
            while (true)
            {
                keepSending = true;
                while (keepSending)
                {
                    if (imageBytes != null)
                    {
                        // send the image bytes to the server
                        client.Send(imageBytes);

                        // receive the response from the server
                        byte[] bytes = new byte[1024];
                        int bytesRec = client.Receive(bytes);
                        Debug.Log("[SocketController] Echoed test = " + System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        // set image bytes to null
                        imageBytes = null;

                        // sleep for 100 milliseconds
                        System.Threading.Thread.Sleep(100);
                    }
                }

            }
        }
        catch (System.Exception e)
        {
            Debug.Log("[SocketController] " + e.ToString());
        }
    }
    // on disable
    void OnDisable()
    {
        // set keep reading to false
        keepSending = false;
        // stop thread if it is running
        if (thread != null && thread.IsAlive)
        {
            thread.Abort();
        }
        // close the socket
        client?.Close();
        handler?.Close();
    }
}
