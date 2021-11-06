using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsManager : MonoBehaviour
{
    public bool CheckMessage(string _message, CommandsList _cList, List<MessageBase> _messagesL, List<UserBase> _usersL)
    {
        bool containCommand = false;
        char[] chars = new char[_message.Length];
        chars = _message.ToCharArray();

        if (chars[0] != '/')
        {
            Debug.Log("This is not a command!");
            return false;
        }

        for (int i = 0; i< _cList.commands.Count; i++)
        {
            if (_message.Contains(_cList.commands[i].commandName))
            {
                switch (_cList.commands[i].commandid)
                {
                    case (0):

                        break;
                    case (1):

                        break;
                    case (2):

                        break;
                    case (3):

                        break;
                    case (4):

                        break;
                    case (5): // Delete the last message
                        Destroy(_messagesL[_messagesL.Count-1]);
                        _messagesL.RemoveAt(_messagesL.Count-1);
                        break;
                    case (6):

                        break;
                    default:

                        break;
                }
                Debug.Log("Command /" + _cList.commands[i].commandName + " DONE!");
                containCommand = true;
                break;
            }
        }
        return containCommand;
    }
}
