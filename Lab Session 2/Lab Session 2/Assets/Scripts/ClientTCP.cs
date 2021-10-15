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

    private Thread mainThread;

    //Properties
    public string message = "PING";
    private byte[] buffer;
    public int loops = 5;
    private int actualloops;
    public int delayTime = 5000;
    private bool isEnded;

    // Start is called before the first frame update
    void Start()
    {
        isEnded = false;
        buffer = new byte[3];
        actualloops = 0;

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 59050);

        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        try
        {
            Debug.Log("Trying to connect with a server");
            newSocket.Connect(ipep);
            Debug.Log("Connection with server " + ipep.Address + " at port " + ipep.Port);

            while (isEnded == false) // Do all the loops if the counts not reach the number yet, when finish disconnect from server
            {
                if (!newSocket.Connected)
                    return;

                newSocket.Send(System.Convert.FromBase64String(message)); // Send to the server
                Debug.Log("(client) Sended: " + message); // Do the debug

                int recv = newSocket.Receive(buffer); //Receive from a client and do the debug
                string receivedtext = System.Convert.ToBase64String(buffer); // Save the received text in new string, convert the bytes to a string
                Debug.Log("(client) Received: " + receivedtext); // Do the debug

                //Actualize the counter and the bool to exit the while
                actualloops++;
                if (actualloops >= loops)
                    isEnded = true;
            }

            newSocket.Disconnect(false);
            Debug.Log("Disconnected from server");
            Application.Quit();
        }
        catch (SocketException e)
        {
            Debug.Log("Unable to connect with the server: "+ e);
            return;
        }
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }
}
