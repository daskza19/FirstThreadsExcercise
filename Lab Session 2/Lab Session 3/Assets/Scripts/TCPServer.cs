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
    //Server Info
    [Header("Server Info")]
    public string serverName = "Server Name";
    public Color serverColor;
    public string welcomeMessage = "Hello new user!";
    public int host = 7777;
    private MessagesList mList;

    //UI Things
    [Header("UI Things")]
    public GameObject sendArea;
    public GameObject serverList;
    public GameObject userList;
    public GameObject message;
    public Button sendbutton;
    public InputField sendText;
    public CommandsList commands;

    //TCP Things
    private Socket newSocket;
    private IPEndPoint ipep;
    private List<Socket> clientSocket;
    private int recv;
    private byte[] buffer;
    private Thread mainThread;

    // Start is called before the first frame update
    void Start()
    {
        mList = GetComponent<MessagesList>();
        buffer = new byte[3];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Any, host);

        try
        {
            newSocket.Bind(ipep);
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
            newSocket.Listen(10);
            Debug.Log("Opening server with listening mode");
            
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }

    public void SendMessage()
    {
        if (sendText.text == "")
            return;

        Message newMessage = new Message(serverName, serverColor, sendText.text);
        newMessage.InstantiateNewMessage(message, sendArea, mList);
        sendText.text = "";
    }
}