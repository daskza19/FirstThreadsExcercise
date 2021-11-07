using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MessageBase
{
    public string message;
    public int userid;
    public bool isWelcomeMessage;

    public MessageBase()
    {
        userid = 0;
        message = "Default Message Text";
        isWelcomeMessage = false;
    }

    public MessageBase(int _userid, string _message, bool _isWelcome = false)
    {
        userid = _userid;
        message = _message;
        isWelcomeMessage = _isWelcome;
    }

    public void SetMessage(int _userid, string _message, bool _isWelcome = false)
    {
        userid = _userid;
        message = _message;
        isWelcomeMessage = _isWelcome;
    }
}