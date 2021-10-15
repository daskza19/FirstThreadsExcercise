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
    private Socket newSocket;
    private IPEndPoint ipep;
    private Socket clientSocket;
    private IPEndPoint clientipep;

    private int recv;
    private byte[] buffer;

    private Thread mainThread;

    //Properties
    public string message = "PONG";
    public int delayTime = 200;

    // Start is called before the first frame update
    void Start()
    {
        buffer = new byte[3];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Any, 59050);

        try
        {
            newSocket.Bind(ipep);
            mainThread = new Thread(MainLoop);
            mainThread.Start();
        }
        catch
        {
            Debug.Log("Bind error");
        }
    }

    private void MainLoop()
    {
        try
        {
            newSocket.Listen(10);
            Debug.Log("Opening server with listening mode");
            clientSocket = newSocket.Accept();
            clientipep = (IPEndPoint)clientSocket.RemoteEndPoint;
            Debug.Log("Connection with a client " + clientipep.Address + " at port " + clientipep.Port);

            while (true)
            {

                recv = clientSocket.Receive(buffer, SocketFlags.None); //Receive from a client and do the debug

                if(recv == 0)
                {
                    Debug.Log("Client disconnected from server");
                    break;
                }

                Debug.Log("(server) Received: " + System.Convert.ToBase64String(buffer));

                Thread.Sleep(2000);

                recv = clientSocket.Send(System.Convert.FromBase64String(message), SocketFlags.None);
                Debug.Log("(server) Sended: " + message);

                if (recv == 0)
                {
                    Debug.Log("Client disconnected from server");
                    break;
                }

                Thread.Sleep(2000);
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
        newSocket.Close();
    }
}
