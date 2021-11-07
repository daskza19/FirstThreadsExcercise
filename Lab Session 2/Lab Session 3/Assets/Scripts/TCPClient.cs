using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TCPClient : MonoBehaviour
{
    #region Variables
    //Client Info
    [Header("Client Info")]
    public string clientName = "Client Name";
    public Color clientColor;
    public int host = 7777;
    public Button sendbutton;
    private UserBase clientUser;
    private MessagesManager mManager;

    public bool wantToSendMessage = false;
    public bool wantToReceiveMessage = false;
    public bool wantToReceiveUserList = false;
    MessageBase messageToSend;

    //TCP Things
    private int recv;
    private Thread mainThread;
    private Thread receiveMessagesThread;
    private Thread receiveUsersThread;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Get Manager to do all the things related to send/receive messages and actualize UI
        mManager = gameObject.GetComponent<MessagesManager>();

        clientUser = new UserBase(clientName, clientColor, false, host); //TCP Inicialitions are done in the constructor of this user

        //Start the main thread
        mainThread = new Thread(MainLoop);
        mainThread.Start();
    }

    private void MainLoop()
    {
        while (!clientUser.newSocket.Connected)
        {
            try
            {
                Debug.Log("Trying to connect with a server");
                clientUser.newSocket.Connect(clientUser.ipep);
                Debug.Log("Connection with server " + clientUser.ipep.Address + " at port " + clientUser.ipep.Port);

                //When the client connects with the server, send the user to server (to enter to their list)
                mManager.SerializeUser(clientUser);
                clientUser.newSocket.Send(mManager.usersStream.ToArray());

                //When the client connects with the server, starts two threads to receive the info (userlist and new messages).
                //With that the client will have three threads, one to send a message when they want, another to
                //stay receiving the new messages from the server and the last to get the entire users list.
                receiveMessagesThread = new Thread(ReceiveMessagesLoop);
                receiveMessagesThread.Start();

                byte[] userlistbuffer = new byte[4096];
                recv = clientUser.newSocket.Receive(userlistbuffer, SocketFlags.None);
                Debug.Log("User List Received");
                mManager.userListStream = new System.IO.MemoryStream(userlistbuffer);
                wantToReceiveUserList = true;

                //After open the new thread to receive, start a infinite while to send the message when the user wants.
                //But before, put wantToSend true to send the welcome message from this user.
                messageToSend = new MessageBase(clientUser.userid, "Hi my name is " + clientName + " nice to meet you :)", true);
                mManager.SerializeMessage(messageToSend);
                wantToSendMessage = true;
                while (true)
                {
                    if (!clientUser.newSocket.Connected)
                        break;

                    if (wantToSendMessage)
                    {
                        clientUser.newSocket.Send(mManager.messagesStream.ToArray()); // Send to the server
                        wantToSendMessage = false;
                        Debug.Log("Message Sent!");
                    }
                }
                clientUser.newSocket.Disconnect(false);
                Debug.Log("Disconnected from server");
            }
            catch (SocketException socketException)
            {
                Debug.Log("Is not possible to connect with server, restarting... (error: " + socketException + " )");
            }
        }
    }

    private void ReceiveMessagesLoop()
    {
        while (true)
        {
            byte[] messagebuffer = new byte[2048];
            recv = clientUser.newSocket.Receive(messagebuffer, SocketFlags.None);

            if (recv == 0)
            {
                Debug.Log("User Disconnected from server");
                break;
            }

            mManager.messagesStream = new System.IO.MemoryStream(messagebuffer);
            wantToReceiveMessage = true;
        }
    }

    private void ReceiveUsersLoop()
    {
        Debug.Log("Hola");
        while (true)
        {
            byte[] userlistbuffer = new byte[4096];
            recv = clientUser.newSocket.Receive(userlistbuffer, SocketFlags.None);
            Debug.Log("User List Received");
            mManager.userListStream = new System.IO.MemoryStream(userlistbuffer);
            wantToReceiveUserList = true;
        }
    }

    private void Update()
    {
        if (wantToReceiveUserList)
        {
            Debug.Log("Want To Receive List");
            mManager.DeserializeUserList();
            wantToReceiveUserList = false;
        }
        if (wantToReceiveMessage)
        {
            mManager.DeserializeMessage();
            wantToReceiveMessage = false;
        }
    }

    private void OnDestroy()
    {
        if(receiveMessagesThread != null) receiveMessagesThread.Abort();
        if (receiveUsersThread != null) receiveUsersThread.Abort();
        mainThread.Abort();
    }

    public void WantToSendMessage()
    {
        if (mManager.sendText.text == "") //First we check that the input text is not empty to not send a empty message
            return;

        if (wantToSendMessage && clientUser.newSocket.Connected) //Second we check that this user is not already sending a message
            return;

        MessageBase newMessage = new MessageBase(clientUser.userid, mManager.sendText.text);
        mManager.SendMessage(newMessage, true);
        wantToSendMessage = true;
    }
}
