using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MessageBase : MonoBehaviour
{
    public string message;
    public UserBase user;
    private MemoryStream stream;

    public MessageBase(UserBase _user, string _message)
    {
        if(user == null)
        {
            user = new UserBase(_user.userName, _user.userColor);
        }
        user.userName = _user.userName;
        user.userColor = _user.userColor;
        message = _message;
    }

    public void SetMessage(UserBase _user, string _message)
    {
        if (user == null)
        {
            user = new UserBase(_user.userName, _user.userColor);
        }
        user.userName = _user.userName;
        user.userColor = _user.userColor;
        message = _message;
    }

    public void SerializeMessage()
    {
        var _m = new MessageBase(this.user, this.message);
        string json = JsonUtility.ToJson(_m);

        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
        Debug.Log(json);
        Debug.Log("Message Serialized!");
    }

    public MessageBase DeserializeMessage()
    {
        UserBase _receivedUser = new UserBase("Default User Name", Color.white);
        var _m = new MessageBase(_receivedUser, "Default Message");
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        string json = reader.ReadString();
        _m = JsonUtility.FromJson<MessageBase>(json);
        Debug.Log("Message Deserialized!");

        return _m;
    }
}