using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MessageContainer : MonoBehaviour
{
    public Text userName;
    public Text message;
    public Image userColor;

    public void SetMessageToPrefab(string _userName, Color _userColor, string _message)
    {
        if(userName != null) userName.text = _userName;

        if (userColor != null) userColor.color = _userColor;

        if (message != null) message.text = _message;
    }
}