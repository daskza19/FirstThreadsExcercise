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
    public GameObject welcomeMessagePrefab;
    public GameObject userListArea;
    private List<GameObject> userListPrefabs = new List<GameObject>();
    public GameObject sendArea;
    public InputField sendText;

    [Header("Actual Lists")]
    public List<MessageBase> messagesList;
    public List<UserBase> usersList;
    public CommandsList cList;
    private CommandsManager comManager = new CommandsManager();

    [Header("Other")]
    public CommandsList commands;
    public MemoryStream messagesStream;
    public MemoryStream usersStream;
    public MemoryStream userListStream;
    #endregion

    public void Awake()
    {
        messagePrefab = (GameObject)Resources.Load("Message");
        welcomeMessagePrefab = (GameObject)Resources.Load("WelcomeMessage");
        userListPrefab = (GameObject)Resources.Load("UserPanel");
        serverListPrefab = (GameObject)Resources.Load("ServerPanel");
        cList = (CommandsList)Resources.Load("Commands");

        messagesStream = new MemoryStream();
        usersStream = new MemoryStream();
    }

    #region ManageMessages
    public bool SendMessage(MessageBase _message, bool isClient = false)
    {
        bool isComand = false;

        if (!comManager.CheckMessage(_message.message, cList, messagesList, usersList))
        {
            if (!isClient)
            {
                InstantiateNewMessageToUI(_message); //If is client not actualize the UI, because the server will send the data
                AddMessageToList(_message);
            }
            SerializeMessage(_message);
        }
        else isComand = true;

        sendText.text = ""; //Reset Input Field text
        return isComand;
    }
    public void AddMessageToList(MessageBase _m)
    {
        //Add the new message to our list
        messagesList.Add(_m);
    }
    public void InstantiateNewMessageToUI(MessageBase _m)
    {
        GameObject newMessage;
        //Instantiate the new Message GameObject to see in screen
        if (!_m.isWelcomeMessage) newMessage = Instantiate(messagePrefab, new Vector3(0, 0, 0), Quaternion.identity, sendArea.transform);
        else newMessage = Instantiate(welcomeMessagePrefab, new Vector3(0, 0, 0), Quaternion.identity, sendArea.transform);
        UserBase newuser = GetUserFromUserID(_m.userid);
        if (newuser != null)
        {
            newMessage.GetComponent<MessageContainer>().SetMessageToPrefab(newuser.userName, newuser.userColor, _m.message, _m.isWelcomeMessage);
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
        userListPrefabs.Add(go);
        usersList.Add(_user);
    }
    public void CreateNewUserList(List<UserBase> _userList)
    {
        GameObject go;
        //First clean and delete the last user list
        usersList.Clear();
        for(int j = 0; j < userListPrefabs.Count; j++)
        {
            Destroy(userListPrefabs[j]);
        }
        userListPrefabs.Clear();

        //Add all the users to this list
        for (int i=0; i < _userList.Count; i++)
        {
            if (_userList[i].isServer)
                go = Instantiate(serverListPrefab, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);
            else
                go = Instantiate(userListPrefab, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);

            go.GetComponent<User>().SetUserPrefab(_userList[i]);
            usersList.Add(_userList[i]);
            userListPrefabs.Add(go);
        }
    }
    public void DeleteUserToList(int index)
    {
        usersList.RemoveAt(index);
    }
    #endregion

    #region Serializables
    public void SerializeMessage(MessageBase _m)
    {
        messagesStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(messagesStream);
        writer.Write(_m.userid);
        writer.Write(_m.message);
        writer.Write(_m.isWelcomeMessage);
        usersStream.Flush();
        usersStream.Close();
    }
    public void DeserializeMessage()
    {
        var _m = new MessageBase(0, "Default Message");
        BinaryReader reader = new BinaryReader(messagesStream);
        messagesStream.Seek(0, SeekOrigin.Begin);
        reader.BaseStream.Seek(0, SeekOrigin.Begin);

        _m.userid = reader.ReadInt32();
        _m.message = reader.ReadString();
        _m.isWelcomeMessage = reader.ReadBoolean();
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
    public void SerializeUserList(List<UserBase> _user)
    {
        userListStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(userListStream);

        writer.Write(_user.Count);

        for (int i = 0; i < _user.Count; i++)
        {
            writer.Write(_user[i].userName);
            writer.Write(_user[i].userid);
            writer.Write((int)_user[i].userColor.r);
            writer.Write((int)_user[i].userColor.g);
            writer.Write((int)_user[i].userColor.b);
            writer.Write((int)_user[i].userColor.a);
            writer.Write(_user[i].userIP);
            writer.Write(_user[i].port);
            writer.Write(_user[i].isServer);
        }
        userListStream.Flush();
        userListStream.Close();

        Debug.Log("User List Serializated!");
    }
    public void DeserializeUserList()
    {
        BinaryReader reader = new BinaryReader(userListStream);
        userListStream.Seek(0, SeekOrigin.Begin);
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        List<UserBase> _userList = new List<UserBase>();

        int listSize = reader.ReadInt32();
        Debug.Log("List size: " + listSize);
        for (int i = 0; i < listSize; i++)
        {
            UserBase _newUser = new UserBase("Default User Name", Color.red);
            _newUser.userName = reader.ReadString();
            _newUser.userid = reader.ReadInt32();
            _newUser.userColor.r = reader.ReadInt32();
            _newUser.userColor.g = reader.ReadInt32();
            _newUser.userColor.b = reader.ReadInt32();
            _newUser.userColor.a = reader.ReadInt32();
            _newUser.userIP = reader.ReadString();
            _newUser.port = reader.ReadInt32();
            _newUser.isServer = reader.ReadBoolean();
            _userList.Add(_newUser);
        }
        userListStream.Flush();
        userListStream.Close();

        //Get the new message and actualize the UI and the list of messages
        CreateNewUserList(_userList);
    }
    #endregion
}
