using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading;

using System.Net;
using System.Net.Sockets;

public class TCPServer : MonoBehaviour
{
    #region Variables
    //Server Info
    [Header("Server Info")]
    public string serverName = "Server Name";
    public Color serverColor;
    public string welcomeMessage = "Hello new user!";
    public int host = 7777;
    private MessagesManager mManager;
    public Button sendbutton;

    public bool wantToSend = false;
    private MessageBase messageToSend = new MessageBase("Defalut Name", Color.white, "Default Message");

    //TCP Things
    private Socket newSocket;
    private IPEndPoint ipep;
    private List<Socket> clientSocket;
    private int recv;
    private byte[] buffer;
    private Thread mainThread;
    ArrayList listenList = new ArrayList();
    ArrayList acceptList = new ArrayList();
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        mManager = gameObject.GetComponent<MessagesManager>();
        //mManager.AddUserToList(serverName, serverColor, userListArea, true);
        messageToSend.SetMessage(serverName, serverColor, "Server ON! Enjoy the experience :D");
        mManager.SendMessage(messageToSend);


        buffer = new byte[3];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Any, host);

        try
        {
            //newSocket.Bind(ipep);
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
            //newSocket.Listen(10);
            //Debug.Log("Opening server with listening mode");

            while (true)
            {
                if (wantToSend == true)
                {
                    mManager.SendMessage(messageToSend);
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void Update()
    {
        //Why I did this?
        //actualCounterTime += Time.deltaTime;
        //if (actualCounterTime >= maxCounterTime)
        //{
        //    Debug.Log("refresh!!");
        //    actualCounterTime = 0;
        //}

        //for (int i = 0; i < 3; i++)
        //{
        //    listenList[i] = new Socket(AddressFamily.InterNetwork,
        //                               SocketType.Stream,
        //                               ProtocolType.Tcp);
        //    ((Socket)listenList[i]).Bind(ipep);
        //    ((Socket)listenList[i]).Listen(10);
        //}
        //
        //Socket.Select(listenList, null, null, 1000);
        //
        //for(int i = 0; i < listenList.Count; i++)
        //{
        //    acceptList[i] = ((Socket)listenList[i]).Accept();
        //}
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }

    public void WantToSendMessage()
    {
        if (mManager.sendText.text == "")
            return;

        //wantToSend = true;
        //messageToSend.SetMessage(serverName, serverColor, mManager.sendText.text);
        //mManager.sendText.text = "";
        MessageBase newMessage = new MessageBase(serverName, serverColor, mManager.sendText.text);
        mManager.SendMessage(newMessage);
    }
}