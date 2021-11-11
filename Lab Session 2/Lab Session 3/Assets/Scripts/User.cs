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

[System.Serializable]
public class UserBase
{
    public string userName;
    public Color userColor;
    public bool isServer = false;
    public int userid; //We use this id to not duplicate users in messages list, the messages contains a userid to link them
    public int maxUsers = 5;

    public List<Socket> userSockets;
    public IPEndPoint ipep;
    public string userIP = "127.0.0.1";
    public int port = 7777;

    public UserBase(string _userName, Color _userColor, bool _isServer = false, int _port = 7777, int _maxUsers = 5, string _ip = "127.0.0.1")
    {
        userName = _userName;
        userColor = _userColor;
        isServer = _isServer;
        userIP = _ip;
        port = _port;
        userid = Random.Range(0, int.MaxValue);
        userSockets = new List<Socket>();
        maxUsers = _maxUsers;

        //TCP Start Inicialization
        if (_isServer)
        {
            for(int i = 0;i< maxUsers; i++)
            {
                userSockets.Add(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                ipep = new IPEndPoint(IPAddress.Any, port);
                try
                {
                    userSockets[i].Bind(ipep);
                    userSockets[i].Listen(10);
                }
                catch
                {
                    Debug.Log("User Bind error");
                }
            }
        }
        else
        {
            userSockets.Add(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            ipep = new IPEndPoint(IPAddress.Parse(_ip), port);
        }
    }

    public void SetUser(string _userName, Color _userColor, bool _isServer = false, int _port = 7777, int _maxUsers = 5, string _ip = "127.0.0.1")
    {
        userName = _userName;
        userColor = _userColor;
        isServer = _isServer;
        userIP = _ip;
        port = _port;
        maxUsers = _maxUsers;

        //TCP Start Inicialization
        if (_isServer)
        {
            for (int i = 0; i < maxUsers; i++)
            {
                userSockets[i] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ipep = new IPEndPoint(IPAddress.Any, port);
                try
                {
                    userSockets[i].Bind(ipep);
                    userSockets[i].Listen(10);
                }
                catch
                {
                    Debug.Log("User Bind error");
                }
            }
        }
        else
        {
            userSockets[0] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipep = new IPEndPoint(IPAddress.Parse(_ip), port);
        }
    }

    public void RestablecerListaSockets(List<Socket> _lista)
    {
        userSockets.Clear();
        for(int i = 0; i < _lista.Count; i++)
        {
            userSockets[i] = _lista[i];
        }
    }

    private void OnDestroy()
    {
        for(int i = 0; i < maxUsers; i++)
        {
            userSockets[i].Close();
        }
    }
}