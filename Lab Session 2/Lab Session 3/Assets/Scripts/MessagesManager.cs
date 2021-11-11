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
    public GameObject helpMessagePrefab;
    public GameObject userListArea;
    private List<GameObject> userListPrefabs = new List<GameObject>();
    private List<GameObject> messagesListPrefabs = new List<GameObject>();
    public GameObject sendArea;
    public InputField sendText;

    [Header("Actual Lists")]
    public List<MessageBase> messagesList;
    public List<UserBase> usersList;
    public CommandsList cList;
    private CommandsManager comManager = new CommandsManager();

    [Header("Other")]
    public CommandsList commands;
    public MemoryStream newStream;
    private TCPServer tcpServer;
    #endregion

    public void Awake()
    {
        tcpServer = GetComponent<TCPServer>();
        messagePrefab = (GameObject)Resources.Load("Message");
        welcomeMessagePrefab = (GameObject)Resources.Load("WelcomeMessage");
        helpMessagePrefab = (GameObject)Resources.Load("HelpMessage");
        userListPrefab = (GameObject)Resources.Load("UserPanel");
        serverListPrefab = (GameObject)Resources.Load("ServerPanel");
        cList = (CommandsList)Resources.Load("Commands");
    }

    #region ManageMessages
    public bool SendMessage(MessageBase _message, bool isClient = false)
    {
        bool isComand = false;

        if (!isClient) //If the user is not a client check if the message contains a command
        {
            isComand = ManageCommands(_message, isComand);
        }
        else //If is client not actualize the UI and add it to a list, because the server will send the data after
        {
            SerializeMessage(_message);
        }

        sendText.text = ""; //Reset Input Field text
        return isComand;
    }

    public bool ManageCommands(MessageBase _message, bool isComand)
    {
        isComand = true;
        switch (comManager.CheckMessage(_message.message, cList))
        {
            case (Command.KickUser): //Error
                for(int i = 0; i < userListPrefabs.Count; i++)
                {
                    if (_message.message.Contains(userListPrefabs[i].GetComponent<User>().userName.text))
                    {
                        //TODO
                        break;
                    }
                }
                Debug.Log("No user encountered with this name, not deleted");
                break;
            case (Command.Color): //Command DONE
                UserBase _user = GetUserFromUserID(_message.userid);
                if (_message.message.Contains("White")) _user.userColor = Color.white;
                else if (_message.message.Contains("Blue")) _user.userColor = Color.blue;
                else if (_message.message.Contains("Red")) _user.userColor = Color.red;
                else if (_message.message.Contains("Green")) _user.userColor = Color.green;
                else if (_message.message.Contains("Yellow")) _user.userColor = Color.yellow;
                List<UserBase> newListColor = new List<UserBase>();
                for(int i = 0; i < usersList.Count; i++)
                {
                    newListColor.Add(usersList[i]);
                }
                CreateNewUserList(newListColor);
                tcpServer.wantToSendUsersList = true;
                break;
            case (Command.Help): //Command DONE
                Debug.Log("Command Help DONE");
                Instantiate(helpMessagePrefab, new Vector3(0, 0, 0), Quaternion.identity, sendArea.transform);
                break;
            case (Command.Whisper):

                break;
            case (Command.ChangeName): //Command DONE
                var newName = _message.message.Substring(12);
                UserBase _usermes = GetUserFromUserID(_message.userid);
                _usermes.userName = newName;

                List<UserBase> newListName = new List<UserBase>();
                for (int i = 0; i < usersList.Count; i++)
                {
                    newListName.Add(usersList[i]);
                }
                CreateNewUserList(newListName);
                tcpServer.wantToSendUsersList = true;
                break;
            case (Command.DeleteLast): //Command DONE
                if (messagesList.Count - 1 > 0)
                {
                    Destroy(messagesListPrefabs[messagesListPrefabs.Count - 1]);
                    messagesList.RemoveAt(messagesList.Count - 1);
                }
                break;
            default:
                InstantiateNewMessageToUI(_message);
                AddMessageToList(_message);
                SerializeMessage(_message);
                isComand = false;
                break;
        }
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
        messagesListPrefabs.Add(newMessage);
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
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(0);
        writer.Write(_m.userid);
        writer.Write(_m.message);
        writer.Write(_m.isWelcomeMessage);

    }
    public void SerializeUser(UserBase _user)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(1);
        writer.Write(_user.userName);
        writer.Write(_user.userid);
        writer.Write((int)_user.userColor.r);
        writer.Write((int)_user.userColor.g);
        writer.Write((int)_user.userColor.b);
        writer.Write((int)_user.userColor.a);
        writer.Write(_user.userIP);
        writer.Write(_user.port);
        writer.Write(_user.isServer);

        Debug.Log("User Serializated!");
    }
    public void SerializeUserList(List<UserBase> _user)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);
        writer.Write(2);
        writer.Write(_user.Count);

        for (int i = 0; i < _user.Count; i++)
        {
            writer.Write(_user[i].userName);
            writer.Write(_user[i].userid);
            writer.Write((double)_user[i].userColor.r);
            writer.Write((double)_user[i].userColor.g);
            writer.Write((double)_user[i].userColor.b);
            writer.Write((double)_user[i].userColor.a);
            writer.Write(_user[i].userIP);
            writer.Write(_user[i].port);
            writer.Write(_user[i].isServer);
        }

        Debug.Log("User List Serializated!");
    }
    public void Deserialize()
    {
        BinaryReader reader = new BinaryReader(newStream);
        newStream.Seek(0, SeekOrigin.Begin);
        reader.BaseStream.Seek(0, SeekOrigin.Begin);

        int whois = reader.ReadInt32();

        switch (whois)
        {
            case (1): //New User Case
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

                newStream.Flush();
                newStream.Close();

                //Get the new message and actualize the UI and the list of messages
                AddUserToList(_user);
                break;
            case (2): //New User List Case
                List<UserBase> _userList = new List<UserBase>();

                int listSize = reader.ReadInt32();
                Debug.Log("List size: " + listSize);
                for (int i = 0; i < listSize; i++)
                {
                    UserBase _newUser = new UserBase("Default User Name", Color.red);
                    _newUser.userName = reader.ReadString();
                    _newUser.userid = reader.ReadInt32();
                    _newUser.userColor.r = (float)reader.ReadDouble();
                    _newUser.userColor.g = (float)reader.ReadDouble();
                    _newUser.userColor.b = (float)reader.ReadDouble();
                    _newUser.userColor.a = (float)reader.ReadDouble();
                    _newUser.userIP = reader.ReadString();
                    _newUser.port = reader.ReadInt32();
                    _newUser.isServer = reader.ReadBoolean();
                    _userList.Add(_newUser);
                }
                newStream.Flush();
                newStream.Close();

                //Get the new message and actualize the UI and the list of messages
                CreateNewUserList(_userList);
                break;
            default: //Message Case (default)
                var _m = new MessageBase(0, "Default Message");
                _m.userid = reader.ReadInt32();
                _m.message = reader.ReadString();
                _m.isWelcomeMessage = reader.ReadBoolean();
                newStream.Flush();
                newStream.Close();

                //Get the new message and actualize the UI and the list of messages
                AddMessageToList(_m);
                InstantiateNewMessageToUI(_m);
                break;
        }
    }
    #endregion
}
