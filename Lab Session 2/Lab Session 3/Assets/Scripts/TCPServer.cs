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
    public Button sendbutton;
    private UserBase serverUser;
    private MessagesManager mManager;

    public bool receivedInfo = false;
    public bool wantToSendNewMessage = false;
    public bool wantToAddNewUser = false;

    //TCP Things
    private Socket clientSocket;
    private int recv;
    private Thread mainThread;
    private Thread receiveThread;
    ArrayList listenList = new ArrayList();
    ArrayList acceptList = new ArrayList();
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //Get Manager to do all the things related to send/receive messages and actualize UI
        mManager = gameObject.GetComponent<MessagesManager>();

        //Inicialize the first user (this server) and send the first message
        serverUser = new UserBase(serverName, serverColor, true, host); //TCP Inicialitions are done in the constructor of this user
        mManager.AddUserToList(serverUser);
        MessageBase messageToSend = new MessageBase(serverUser.userid, "Server ON! Enjoy the experience :D", true);
        mManager.SendMessage(messageToSend);

        //Start the main thread
        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        try
        { 
            serverUser.newSocket.Listen(10);
            Debug.Log("Opening server with listening mode");
            while (true)
            {
                clientSocket = serverUser.newSocket.Accept();

                //First receive the client user
                byte[] userbuffer = new byte[2048];
                recv = clientSocket.Receive(userbuffer, SocketFlags.None);
                mManager.usersStream = new System.IO.MemoryStream(userbuffer);

                //When the client is received, add to our list
                Debug.Log("User received");
                wantToAddNewUser = true;

                Thread.Sleep(50); //Sleep for a little to get the user in the list

                //Send the user list to the client
                mManager.SerializeUserList(mManager.usersList);
                clientSocket.Send(mManager.userListStream.ToArray());
                Debug.Log("User List Send to client");

                receiveThread = new Thread(ReceiveMessagesLoop);
                receiveThread.Start();
                while (true)
                {
                    if (wantToSendNewMessage)
                    {
                        clientSocket.Send(mManager.messagesStream.ToArray()); // Send to the server
                        wantToSendNewMessage = false;
                        Debug.Log("Message Sent!");
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void ReceiveMessagesLoop()
    {
        while (true)
        {
            byte[] messagebuffer = new byte[2048];
            recv = clientSocket.Receive(messagebuffer, SocketFlags.None);
            mManager.messagesStream = new System.IO.MemoryStream(messagebuffer);
            Debug.Log("Received message from client");
            clientSocket.Send(messagebuffer);
            Debug.Log("Info sent to the client too");

            //Add the info to the screen (deserialize the message and update UI)
            receivedInfo = true;
        }
    }

    private void Update()
    {
        if (receivedInfo)
        {
            mManager.DeserializeMessage();
            receivedInfo = false;
        }
        if (wantToAddNewUser)
        {
            mManager.DeserializeUser();
            wantToAddNewUser = false;
        }
    }

    private void OnDestroy()
    {
        if (mainThread != null) mainThread.Abort();
    }

    public void WantToSendMessage()
    {
        if (mManager.sendText.text == "")
            return;

        MessageBase newMessage = new MessageBase(serverUser.userid, mManager.sendText.text);
        bool isComand = mManager.SendMessage(newMessage);
        if(!isComand) wantToSendNewMessage = true;
    }
}