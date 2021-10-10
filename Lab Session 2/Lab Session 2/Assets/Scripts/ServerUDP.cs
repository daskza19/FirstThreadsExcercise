using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using System.Net;
using System.Net.Sockets;

public class ServerUDP : MonoBehaviour
{
    //UDP Things
    private Socket newSocket;
    private IPEndPoint ipep;
    private EndPoint sendEnp;

    private Thread mainThread;

    //Properties
    public string message = "PONG";
    public int delayTime = 1000;
    private bool isEnded;

    // Start is called before the first frame update
    void Start()
    {
        isEnded = false;
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 3000);
        sendEnp = (EndPoint)ipep;

        newSocket.Bind(ipep);
        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        while (isEnded == false)
        {
            byte[] buffer = new byte[256];
            int recv = newSocket.ReceiveFrom(buffer, ref sendEnp); //Receive from a client and do the debug
            Debug.Log("(server) Received: " + System.Convert.ToBase64String(buffer));

            Thread.Sleep(delayTime); //Do a delay (like the statement says)

            newSocket.SendTo(System.Convert.FromBase64String(message), recv, SocketFlags.None, sendEnp);
            Debug.Log("(server) Sended: " + message);
        }
    }

    private void OnDestroy()
    {
        newSocket.Close();
    }
}
