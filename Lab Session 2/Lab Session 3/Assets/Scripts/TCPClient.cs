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
    MessageBase messageToSend;

    //TCP Things
    private int recv;
    private Thread mainThread;
    private Thread receiveThread;
    #endregion

    byte[] hola;

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

                //When the client already send the user, receive the list of actual users from the server
                //clientUser.newSocket.Receive(mManager.messagesSendStream.ToArray());

                //When the client connects with the server, starts another thread to receive the date.
                //With that the client will have two threads, one to send a message when they want and another to
                //stay receiving the new messages from the server.
                receiveThread = new Thread(ReceiveLoop);
                receiveThread.Start();

                //After open the new thread to receive, start a infinite while to send the message when the user wants.
                //But before, put wantToSend true to send the welcome message from this user.
                messageToSend = new MessageBase(clientUser.userid, "Hi my name is " + clientName + " nice to meet you :)");
                mManager.SerializeMessage(messageToSend);
                wantToSendMessage = true;
                while (true)
                {
                    if (!clientUser.newSocket.Connected)
                        return;

                    if (wantToSendMessage)
                    {
                        clientUser.newSocket.Send(mManager.messagesSendStream.ToArray()); // Send to the server
                        wantToSendMessage = false;
                        Debug.Log("Message Sent!");
                    }
                }
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
            clientUser.newSocket.Receive(mManager.messagesSendStream.ToArray());
            mManager.DeserializeMessage();
        }
    }

    private void OnDestroy()
    {
        if(receiveThread != null) receiveThread.Abort();
        mainThread.Abort();
    }

    public void WantToSendMessage()
    {
        if (mManager.sendText.text == "") //First we check that the input text is not empty to not send a empty message
            return;

        if (wantToSendMessage && clientUser.newSocket.Connected) //Second we check that this user is not already sending a message
            return;

        MessageBase newMessage = new MessageBase(clientUser.userid, mManager.sendText.text);
        mManager.SerializeMessage(newMessage);
        wantToSendMessage = true;
    }
}
