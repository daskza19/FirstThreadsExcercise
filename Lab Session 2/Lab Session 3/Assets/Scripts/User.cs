using System.Collections;
using System.Collections.Generic;
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

    public void SetUser(string _userName, Color _userColor, bool _isServer = false)
    {
        userName = _userName;
        userColor = _userColor;
        isServer = _isServer;
    }
}