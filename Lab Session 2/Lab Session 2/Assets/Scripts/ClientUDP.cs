using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

using System.Net;
using System.Net.Sockets;

public class ClientUDP : MonoBehaviour
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
    public Text uiText;

    // Start is called before the first frame update
    void Start()
    {
        isEnded = false;
        actualloops = 0;

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Parse("147.83.144.2"), 1818);
        sendEnp = (EndPoint)ipep;

        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        while (isEnded == false)
        {
            newSocket.SendTo(System.Convert.FromBase64String(message), SocketFlags.None, sendEnp);
            Debug.Log("(client) Sended: " + message);

            byte[] buffer = new byte[256];
            int recv = newSocket.ReceiveFrom(buffer, ref sendEnp); //Receive from a client and do the debug
            Debug.Log("(client) Received: " + System.Convert.ToBase64String(buffer));
            //ChangeUIText(System.Convert.ToBase64String(buffer));

            Thread.Sleep(delayTime); //Do a delay (like the statement says)

            //Actualize the counter and the bool to exit the while
            actualloops++;
            if (actualloops >= loops)
                isEnded = true;
        }
    }

    private void ChangeUIText(string _text)
    {
        uiText.text = _text;
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }
}
