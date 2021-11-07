using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MessageBase
{
    public string message;
    public int userid;

    public MessageBase()
    {
        userid = 0;
        message = "Default Message Text";
    }

    public MessageBase(int _userid, string _message)
    {
        userid = _userid;
        message = _message;
    }

    public void SetMessage(int _userid, string _message)
    {
        userid = _userid;
        message = _message;
    }
}