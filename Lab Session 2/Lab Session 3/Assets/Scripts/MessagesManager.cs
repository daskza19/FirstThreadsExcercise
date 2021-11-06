using System.Collections;
using System.Collections.Generic;
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
    #endregion

    public void Awake()
    {
        messagePrefab = (GameObject)Resources.Load("Message");
        userListPrefab = (GameObject)Resources.Load("UserPanel");
        serverListPrefab = (GameObject)Resources.Load("ServerPanel");
        cList = (CommandsList)Resources.Load("Commands");
    }

    #region ManageMessages
    public void SendMessage(MessageBase _message)
    {
        if (!comManager.CheckMessage(_message.message, cList, messagesList, usersList))
        {
            //Instantiate the new Message GameObject to see in screen
            GameObject newMessage = Instantiate(messagePrefab, new Vector3(0, 0, 0), Quaternion.identity, sendArea.transform);
            newMessage.GetComponent<MessageContainer>().SetMessageToPrefab(_message.userName, _message.userColor, _message.message);

            //Create the new Message Class and add to our current messages list
            MessageBase _toSendMessage = new MessageBase(_message.userName, _message.userColor, _message.message);
            messagesList.Add(_toSendMessage);

            //To send the message to the other clients

        }
        //Reset Input Field text
        sendText.text = "";
    }

    public void DeleteMessage(int index)
    {
        Destroy(messagesList[index]);
    }
    #endregion

    #region ManageUsers
    public void AddUserToList(UserBase _user)
    {
        GameObject go;
        Debug.Log("Hols");
        if (_user.isServer)
            go = Instantiate(serverListPrefab, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);
        else
            go = Instantiate(userListPrefab, new Vector3(0, 0, 0), Quaternion.identity, userListArea.transform);

        go.GetComponent<User>().SetUserPrefab(_user);
        usersList.Add(_user);
    }
    public void DeleteUserToList(int index)
    {
        Destroy(usersList[index]);
    }
    #endregion
}
