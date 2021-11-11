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
    public bool wantToActualizeReceive = false;
    MessageBase messageToSend;

    //TCP Things
    private int recv;
    private Thread mainThread;
    private Thread receiveThread;
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
                clientUser.newSocket.Send(mManager.newStream.ToArray());

                //When the client connects with the server, two threads will be avaible.
                //One of the threads (this) will be only to do the send work. The other thread will be the receive thread
                receiveThread = new Thread(ReceiveLoop);
                receiveThread.Start();

                while (mManager.usersList.Count <= 1)
                {
                    Thread.Sleep(20);
                }
                
                //After open the new thread to receive, start a infinite while to send the message when the user wants.
                //But before, put wantToSend true to send the welcome message from this user.
                messageToSend = new MessageBase(clientUser.userid, "Hi my name is " + clientName + " nice to meet you :)", true);
                mManager.SerializeMessage(messageToSend);
                wantToSendMessage = true; // Send to the server
                while (true)
                {
                    if (!clientUser.newSocket.Connected)
                        break;

                    if (wantToSendMessage)
                    {
                        clientUser.newSocket.Send(mManager.newStream.ToArray()); // Send to the server
                        wantToSendMessage = false;
                        Debug.Log("Message Sent!");
                    }
                }
                clientUser.newSocket.Disconnect(false);
                Debug.Log("Disconnected from server");
                Application.Quit();
            }
            catch (SocketException socketException)
            {
                Debug.Log("Is not possible to connect with server, restarting... (error: " + socketException + " )");
            }
        }
    }

    private void ReceiveLoop()
    {
        while (true)
        {
            if (!clientUser.newSocket.Connected)
                break;

            byte[] buffer = new byte[4096];
            recv = clientUser.newSocket.Receive(buffer, SocketFlags.None);
            if (recv == 0)
            {
                Debug.Log("User Disconnected from server");
                break;
            }
            mManager.newStream = new System.IO.MemoryStream(buffer);
            wantToActualizeReceive = true;
        }
        clientUser.newSocket.Disconnect(false);
        Debug.Log("Disconnected from server");
        Application.Quit();
    }

    private void Update()
    {
        if (wantToActualizeReceive)
        {
            Debug.Log("Want To Receive List");
            mManager.Deserialize();
            wantToActualizeReceive = false;
        }
    }

    private void OnDestroy()
    {
        if(receiveThread != null) receiveThread.Abort();
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
