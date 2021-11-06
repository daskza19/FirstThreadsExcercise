using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public Text userName;
    public Image userColor;

    public void SetUserPrefab(UserBase _user)
    {
        userName.text = _user.userName;
        userColor.color = _user.userColor;
    }
}

public class UserBase : MonoBehaviour
{
    public string userName;
    public Color userColor;
    public bool isServer = false;
    public int userid; //We use this id to not duplicate users in messages list, the messages contains a userid to link them

    public Socket newSocket;
    private IPEndPoint ipep;
    public string userIP = "127.0.0.1";
    public int port = 7777;

    public UserBase(string _userName, Color _userColor, bool _isServer = false, int _port = 7777, string _ip = "127.0.0.1")
    {
        userName = _userName;
        userColor = _userColor;
        isServer = _isServer;
        userIP = _ip;
        port = _port;

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        if (_isServer)
        {
            ipep = new IPEndPoint(IPAddress.Any, port);
        }
        else
        {
            ipep = new IPEndPoint(IPAddress.Parse(_ip), port);
        }
    }

    public void SetUser(string _userName, Color _userColor, string _ip = "127.0.0.1", int _port = 7777, bool _isServer = false)
    {
        userName = _userName;
        userColor = _userColor;
        isServer = _isServer;
        userIP = _ip;
        port = _port;

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Parse(_ip), port);
    }

    private void OnDestroy()
    {
        newSocket.Close();
    }
}