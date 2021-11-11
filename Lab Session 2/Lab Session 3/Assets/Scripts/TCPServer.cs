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

    MessageBase messageToSend;
    public bool wantToSendMessage = false;
    public bool wantToSendUsersList = false;
    public bool wantToActualizeReceive = false;

    //TCP Things
    private List<Socket> clientSocket;
    private int recv;
    private Thread mainThread;
    private Thread receiveUsersThread;
    private List<Socket> recuperateList = new List<Socket>();
    ArrayList acceptList = new ArrayList();
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //Get Manager to do all the things related to send/receive messages and actualize UI
        mManager = gameObject.GetComponent<MessagesManager>();
        clientSocket = new List<Socket>();

        //Inicialize the first user (this server) and send the first message
        serverUser = new UserBase(serverName, serverColor, true, host, 5); //TCP Inicialitions are done in the constructor of this user
        mManager.AddUserToList(serverUser);
        messageToSend = new MessageBase(serverUser.userid, "Server ON! Enjoy the experience :D", true);
        mManager.SendMessage(messageToSend);

        //Start the main thread
        mainThread = new Thread(MainLoop);
        mainThread.Start();
        receiveUsersThread = new Thread(ReceiveLoop);
        receiveUsersThread.Start();
    }
    private void DoSaveSocketsList(List<Socket> _list1, List<Socket> _list2)
    {
        _list1.Clear();
        for (int i=0;i< _list2.Count; i++)
        {
            _list1.Add(_list2[i]);
        }
    }

    private bool CheckIfSocketIsInList(Socket _socket)
    {
        for(int i = 0; i < clientSocket.Count; i++)
        {
            if (_socket == clientSocket[i])
            {
                return true;
            }
        }
        return false;
    }

    private void ReceiveLoop()
    {
        while (true)
        {
            try
            {
                DoSaveSocketsList(recuperateList, serverUser.userSockets);
                Socket.Select(serverUser.userSockets, null, null, 1000);

                if (serverUser.userSockets.Count > 0)
                {
                    for (int i = 0; i < serverUser.userSockets.Count; i++)
                    {
                        bool isInList = CheckIfSocketIsInList(serverUser.userSockets[i]);
                        Debug.Log(isInList);
                        if (!isInList)
                        {
                            Socket _newSocket = serverUser.userSockets[i].Accept();
                            clientSocket.Add(_newSocket);
                            Debug.Log("Added new client!");

                            //First receive the client user
                            byte[] userbuffer = new byte[2048];
                            recv = _newSocket.Receive(userbuffer, SocketFlags.None);
                            mManager.newStream = new System.IO.MemoryStream(userbuffer);
                            wantToActualizeReceive = true;
                            Debug.Log("Want to actulize the new user!");

                            Thread.Sleep(50); //Sleep for a little to get the user in the list

                            try
                            {
                                //Send the user list to the client
                                mManager.SerializeUserList(mManager.usersList);
                                for (int j = 0; j < clientSocket.Count; j++)
                                {
                                    clientSocket[j].Send(mManager.newStream.ToArray()); // Send to the clients
                                }
                                wantToSendUsersList = false;
                            }
                            catch
                            {
                                Debug.Log("Unable to send to this socket");
                            }

                            Thread.Sleep(250);

                            //First receive the client user
                            byte[] messagebuffer = new byte[2048];
                            recv = _newSocket.Receive(messagebuffer, SocketFlags.None);
                            if (recv == 0)
                            {
                                Debug.Log("User Disconnected from server");
                                break;
                            }
                            mManager.newStream = new System.IO.MemoryStream(messagebuffer);
                            for (int j = 0; j < clientSocket.Count; j++)
                            {
                                Debug.Log("Enviado lo recivido!");
                                clientSocket[j].Send(messagebuffer); // Send to the clients
                            }
                            wantToActualizeReceive = true;
                        }
                        else
                        {
                            //First receive the client user
                            byte[] messagebuffer = new byte[2048];
                            recv = serverUser.userSockets[i].Receive(messagebuffer, SocketFlags.None);
                            if (recv == 0)
                            {
                                Debug.Log("User Disconnected from server");
                                break;
                            }
                            mManager.newStream = new System.IO.MemoryStream(messagebuffer);
                            for (int j = 0; j < clientSocket.Count; j++)
                            {
                                Debug.Log("Enviado lo recivido!");
                                clientSocket[j].Send(messagebuffer); // Send to the clients
                            }
                            wantToActualizeReceive = true;
                        }
                    }
                }
                DoSaveSocketsList(serverUser.userSockets, recuperateList);
            }
            catch
            {
                Debug.Log("Some problems with that :(");
            }
        }
    }    

    private void MainLoop()
    {
        try
        { 
            Debug.Log("Opening server with listening mode");
            while (true)
            {
                while (true)
                {
                    if (wantToSendMessage)
                    {
                        try
                        {
                            for(int j = 0; j < clientSocket.Count; j++)
                            {
                                clientSocket[j].Send(mManager.newStream.ToArray()); // Send to the clients
                            }
                            wantToSendMessage = false;
                        }
                        catch
                        {
                            Debug.Log("Unable to send to this socket");
                        }
                    }
                    if (wantToSendUsersList)
                    {
                        try
                        {
                            Debug.Log("Want to send user list!");
                            //Send the user list to the client
                            mManager.SerializeUserList(mManager.usersList);
                            for (int j = 0; j < clientSocket.Count; j++)
                            {
                                clientSocket[j].Send(mManager.newStream.ToArray()); // Send to the clients
                            }
                            Debug.Log("User List Send to client");
                            wantToSendUsersList = false;
                        }
                        catch
                        {
                            Debug.Log("Unable to send to this socket");
                        }
                    }
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
        if (wantToActualizeReceive)
        {
            mManager.Deserialize();
            wantToActualizeReceive = false;
        }
    }

    private void OnDestroy()
    {
        if (mainThread != null) mainThread.Abort();
        if (receiveUsersThread != null) receiveUsersThread.Abort();
    }

    public void WantToSendMessage()
    {
        if (mManager.sendText.text == "")
            return;

        MessageBase newMessage = new MessageBase(serverUser.userid, mManager.sendText.text);
        bool isComand = mManager.SendMessage(newMessage);
        if(!isComand) wantToSendMessage = true;
    }
}