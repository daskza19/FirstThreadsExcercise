using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using System.Net;
using System.Net.Sockets;

public class ClientTCP : MonoBehaviour
{
    //UDP Things
    private Socket newSocket;
    private IPEndPoint ipep;
    private EndPoint sendEnp;

    private Thread mainThread;

    //Properties
    public string message = "PING";
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
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1818);
        sendEnp = (EndPoint)ipep;

        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        while (isEnded == false)
        {
            newSocket.SendTo(System.Convert.FromBase64String(message), System.Convert.FromBase64String(message).Length, SocketFlags.None, sendEnp); // Send to the server
            Debug.Log("(client) Sended: " + message); // Do the debug

            byte[] buffer = new byte[3];
            int recv = newSocket.ReceiveFrom(buffer, ref sendEnp); //Receive from a client and do the debug
            string receivedtext = System.Convert.ToBase64String(buffer); // Save the received text in new string, convert the bytes to a string
            Debug.Log("(client) Received: " + receivedtext); // Do the debug

            Thread.Sleep(delayTime); //Do a delay (like the statement says)

            //Actualize the counter and the bool to exit the while
            actualloops++;
            if (actualloops >= loops)
                isEnded = true;
        }
        Debug.Log("Desconnecting client from actual server"); // Debug desconnection
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }
}
