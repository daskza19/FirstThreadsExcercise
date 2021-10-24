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

    //UI Things
    [Header("UI Things")]
    public GameObject userListArea;
    public GameObject serverList;
    public GameObject userList;
    public GameObject sendArea;
    public GameObject message;
    public Button sendbutton;
    public InputField sendText;
    public CommandsList commands;

    //Messages
    [Header("Actual Lists")]
    public List<GameObject> messagesList;
    public List<GameObject> usersList;
    private CommandsManager comManager = new CommandsManager();

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
        AddUserToList(serverName, serverColor, true);
        SendMessage(serverName, "Server ON! Enjoy the experience");
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

    private void Update()
    {
        /*
        for (int i = 0; i < 3; i++)
        {
            listenList[i] = new Socket(AddressFamily.InterNetwork,
                                       SocketType.Stream,
                                       ProtocolType.Tcp);
            ((Socket)listenList[i]).Bind(ipep);
            ((Socket)listenList[i]).Listen(10);
        }

        Socket.Select(listenList, null, null, 1000);

        for(int i = 0; i < listenList.Count; i++)
        {
            acceptList[i] = ((Socket)listenList[i]).Accept();
        }

        */
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }

    #region ServerUIManagerLists
    public void SendMessage()
    {
        if (sendText.text == "")
            return;

        if (comManager.CheckMessage(sendText.text, commands, messagesList, usersList))
        {

        }
        else
        {
            GameObject newMessage = Instantiate(message, new Vector3(0, 0, 0), Quaternion.identity, sendArea.transform);
            newMessage.GetComponent<Message>().SetMessage(serverName, serverColor, sendText.text);
            messagesList.Add(newMessage);
        }

        sendText.text = "";
    }

    public void SendMessage(string userName, string _text)
    {
        GameObject newMessage = Instantiate(message, new Vector3(0, 0, 0), Quaternion.identity, sendArea.transform);
        newMessage.GetComponent<Message>().SetMessage(serverName, serverColor, _text);
        sendText.text = "";

        messagesList.Add(newMessage);
    }
    
    public void DeleteMessage(int index)
    {
        Destroy(messagesList[index]);
    }

    public void AddUserToList(string _userName, Color _userColor, bool _isServer = false)
    {
        GameObject go;

        if(_isServer)
            go = Instantiate(serverList, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);
        else
            go = Instantiate(userList, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);

        go.GetComponent<User>().Setuser(_userName, _userColor);
        usersList.Add(go);
    }

    public void DeleteUserToList(int index)
    {
        Destroy(usersList[index]);
    }
    #endregion
}