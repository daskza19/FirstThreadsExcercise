using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public Text userName;
    public Text message;
    public Image userColor;

    public void SetMessage(string _userName, Color _userColor, string _message)
    {
        userName.text = _userName;
        userColor.color = _userColor;
        message.text = _message;
    }
}