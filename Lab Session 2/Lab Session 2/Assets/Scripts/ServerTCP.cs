using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using System.Net;
using System.Net.Sockets;

public class ServerTCP : MonoBehaviour
{
    //UDP Things
    private Socket newSocket;
    private IPEndPoint ipep;
    private EndPoint sendEnp;

    private Thread mainThread;

    //Properties
    public string message = "PONG";
    public int delayTime = 100;
    private bool isEnded;

    // Start is called before the first frame update
    void Start()
    {
        isEnded = false;
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 1818);
        sendEnp = (EndPoint)ipep;

        newSocket.Bind(ipep);
        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        while (isEnded == false)
        {
            byte[] buffer = new byte[3];
            int recv = newSocket.ReceiveFrom(buffer, ref sendEnp); //Receive from a client and do the debug
            string receivedtext = System.Convert.ToBase64String(buffer);
            Debug.Log("(server) Received: " + receivedtext);

            Thread.Sleep(delayTime); //Do a delay (like the statement says)

            newSocket.SendTo(System.Convert.FromBase64String(message), System.Convert.FromBase64String(message).Length, SocketFlags.None, sendEnp);

            Debug.Log("(server) Sended: " + message);
        }
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }
}
