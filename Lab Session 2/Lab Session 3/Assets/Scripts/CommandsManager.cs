using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Command
{
    None, 
    KickUser,
    Color,
    Help,
    Whisper,
    ChangeName,
    DeleteLast
}

public class CommandsManager
{
    public Command CheckMessage
        (string _message, CommandsList _cList)
    {
        Command _com = Command.None;
        char[] chars = new char[_message.Length];
        chars = _message.ToCharArray();

        if (chars[0] != '/')
        {
            Debug.Log("This is not a command!");
            return _com;
        }

        for (int i = 0; i< _cList.commands.Count; i++)
        {
            if (_message.Contains(_cList.commands[i].commandName))
            {
                switch (_cList.commands[i].commandid)
                {
                    case (0):
                        _com = Command.KickUser;
                        break;
                    case (1):
                        _com = Command.Color;
                        break;
                    case (2):
                        _com = Command.Help;
                        break;
                    case (3):
                        _com = Command.Whisper;
                        break;
                    case (4):
                        _com = Command.ChangeName;
                        break;
                    case (5): // Delete the last message
                        _com = Command.DeleteLast;
                        break;
                    default:
                        _com = Command.None;
                        break;
                }
                //Debug.Log("Command /" + _cList.commands[i].commandName + " DONE!");
                break;
            }
        }
        return _com;
    }
}
