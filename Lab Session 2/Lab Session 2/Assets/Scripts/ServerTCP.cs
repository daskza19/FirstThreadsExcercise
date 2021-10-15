using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

using System.Net;
using System.Net.Sockets;

public class ServerTCP : MonoBehaviour
{
    //UDP Things
    private Socket serverSocket;
    private IPEndPoint serveripep;
    private EndPoint serverSendEnp;

    private Socket clientSocket;
    private IPEndPoint clientipep;
    private EndPoint clientSendEnp;

    private Thread mainThread;

    //Properties
    public string message = "PONG";
    public int delayTime = 100;
    private bool isEnded;

    // Start is called before the first frame update
    void Start()
    {
        isEnded = false;
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serveripep = new IPEndPoint(IPAddress.Any, 7777);
        serverSendEnp = (EndPoint)serveripep;

        serverSocket.Bind(serveripep);
        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        try
        {
            Debug.Log("Opening server with listening mode");
            serverSocket.Listen(10);

            clientSocket = serverSocket.Accept();
            Debug.Log("OMG connection with a client!");

            while (isEnded == false)
            {
                byte[] buffer = new byte[3];
                Debug.Log("A");
                int recv = serverSocket.ReceiveFrom(buffer, ref serverSendEnp); //Receive from a client and do the debug
                string receivedtext = System.Convert.ToBase64String(buffer);
                Debug.Log("(server) Received: " + receivedtext);

                Thread.Sleep(delayTime); //Do a delay (like the statement says)

                serverSocket.SendTo(System.Convert.FromBase64String(message), System.Convert.FromBase64String(message).Length, SocketFlags.None, serverSendEnp);

                Debug.Log("(server) Sended: " + message);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        serverSocket.Close();
    }
}
