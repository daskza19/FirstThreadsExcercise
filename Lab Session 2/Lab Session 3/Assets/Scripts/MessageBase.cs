using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MessageBase : MonoBehaviour
{
    public string userName;
    public string message;
    public Color userColor;
    private MemoryStream stream;

    public MessageBase(string _userName, Color _userColor, string _message)
    {
        userName=_userName;
        userColor=_userColor;
        message=_message;
    }

    public void SetMessage(string _userName, Color _userColor, string _message)
    {
        userName = _userName;
        userColor = _userColor;
        message = _message;
    }

    public void SerializeMessage()
    {
        var _m = new MessageBase(this.userName, this.userColor, this.message);
        string json = JsonUtility.ToJson(_m);

        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
        Debug.Log(json);
        Debug.Log("Message Serialized!");
    }


    public MessageBase DeserializeMessage()
    {
        var _m = new MessageBase("Deafult Name", Color.white, "Default Message");
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        string json = reader.ReadString();
        _m = JsonUtility.FromJson<MessageBase>(json);
        Debug.Log("Message Deserialized!");

        return _m;
    }
}