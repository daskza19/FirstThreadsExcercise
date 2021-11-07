using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class MessagesManager : MonoBehaviour
{
    #region Variables
    [Header("UI Things")]
    public GameObject userListPrefab;
    public GameObject serverListPrefab;
    public GameObject messagePrefab;
    public GameObject userListArea;
    public GameObject sendArea;
    public InputField sendText;

    [Header("Actual Lists")]
    public List<MessageBase> messagesList;
    public List<UserBase> usersList;
    public CommandsList cList;
    private CommandsManager comManager = new CommandsManager();

    [Header("Other")]
    public CommandsList commands;
    public MemoryStream messagesSendStream;
    public MemoryStream usersStream;
    #endregion

    public void Awake()
    {
        messagePrefab = (GameObject)Resources.Load("Message");
        userListPrefab = (GameObject)Resources.Load("UserPanel");
        serverListPrefab = (GameObject)Resources.Load("ServerPanel");
        cList = (CommandsList)Resources.Load("Commands");

        messagesSendStream = new MemoryStream();
        usersStream = new MemoryStream();
    }

    #region ManageMessages
    public void SendMessage(MessageBase _message)
    {
        if (!comManager.CheckMessage(_message.message, cList, messagesList, usersList))
        {
            InstantiateNewMessageToUI(_message);
            AddMessageToList(_message);
            SerializeMessage(_message);
        }
        //Reset Input Field text
        sendText.text = "";
    }
    public void AddMessageToList(MessageBase _m)
    {
        //Add the new message to our list
        messagesList.Add(_m);
    }
    public void InstantiateNewMessageToUI(MessageBase _m)
    {
        //Instantiate the new Message GameObject to see in screen
        GameObject newMessage = Instantiate(messagePrefab, new Vector3(0, 0, 0), Quaternion.identity, sendArea.transform);
        UserBase newuser = GetUserFromUserID(_m.userid);
        if (newuser != null)
        {
            newMessage.GetComponent<MessageContainer>().SetMessageToPrefab(newuser.userName, newuser.userColor, _m.message);
        }
    }
    public void DeleteMessage(int index)
    {
        messagesList.RemoveAt(index);
    }
    public UserBase GetUserFromUserID(int _id)
    {
        foreach(var user in usersList)
        {
            if(user.userid == _id)
            {
                return user;
            }
        }
        Debug.Log("No user found with this ID: " + _id);
        return null;
    }
    #endregion

    #region ManageUsers
    public void AddUserToList(UserBase _user)
    {
        GameObject go;
        if (_user.isServer)
            go = Instantiate(serverListPrefab, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);
        else
            go = Instantiate(userListPrefab, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);

        go.GetComponent<User>().SetUserPrefab(_user);
        usersList.Add(_user);
    }
    public void DeleteUserToList(int index)
    {
        usersList.RemoveAt(index);
    }
    #endregion

    #region Serializables
    public void SerializeMessage(MessageBase _m)
    {
        messagesSendStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(messagesSendStream);
        writer.Write(_m.userid);
        writer.Write(_m.message);
        usersStream.Flush();
        usersStream.Close();
    }
    public void DeserializeMessage()
    {
        var _m = new MessageBase(0, "Default Message");
        BinaryReader reader = new BinaryReader(messagesSendStream);
        messagesSendStream.Seek(0, SeekOrigin.Begin);
        reader.BaseStream.Seek(0, SeekOrigin.Begin);

        _m.userid = reader.ReadInt32();
        _m.message = reader.ReadString();
        usersStream.Flush();
        usersStream.Close();

        //Get the new message and actualize the UI and the list of messages
        AddMessageToList(_m);
        InstantiateNewMessageToUI(_m);
    }

    public void SerializeUser(UserBase _user)
    {
        usersStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(usersStream);
        writer.Write(_user.userName);
        writer.Write(_user.userid);
        writer.Write((int)_user.userColor.r);
        writer.Write((int)_user.userColor.g);
        writer.Write((int)_user.userColor.b);
        writer.Write((int)_user.userColor.a);
        writer.Write(_user.userIP);
        writer.Write(_user.port);
        writer.Write(_user.isServer);

        usersStream.Flush();
        usersStream.Close();

        Debug.Log("User Serializated!" + usersStream.ToString());
    }

    public void DeserializeUser()
    {
        BinaryReader reader = new BinaryReader(usersStream);
        usersStream.Seek(0, SeekOrigin.Begin);
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        var _user = new UserBase("Default User Name", Color.red);

        _user.userName = reader.ReadString();
        _user.userid = reader.ReadInt32();
        _user.userColor.r = reader.ReadInt32();
        _user.userColor.g = reader.ReadInt32();
        _user.userColor.b = reader.ReadInt32();
        _user.userColor.a = reader.ReadInt32();
        _user.userIP = reader.ReadString();
        _user.port = reader.ReadInt32();
        _user.isServer = reader.ReadBoolean();

        usersStream.Flush();
        usersStream.Close();

        //Get the new message and actualize the UI and the list of messages
        AddUserToList(_user);
    }
    #endregion
}
