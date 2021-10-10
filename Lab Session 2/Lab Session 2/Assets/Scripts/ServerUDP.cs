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
    public int loops = 5;
    private int actualloops;
    public int delayTime = 5000;
    private bool isEnded;

    // Start is called before the first frame update
    void Start()
    {
        isEnded = false;
        actualloops = 0;
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 7000);
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
            Debug.Log("Received: " + System.Convert.ToBase64String(buffer));

            Thread.Sleep(delayTime); //Do a delay (like the statement says)

            newSocket.SendTo(System.Convert.FromBase64String(message), recv, SocketFlags.None, sendEnp);

            //Actualize the counter and the bool to exit the while
            //actualloops++;
            //if (actualloops>= loops)
            //    isEnded = true;
        }
    }

    private void OnDestroy()
    {
        newSocket.Close();
    }
}
