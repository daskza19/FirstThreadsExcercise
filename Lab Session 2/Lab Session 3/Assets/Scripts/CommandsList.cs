using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Commands", menuName = "ScriptableObjects/CommandsList")]
public class CommandsList : ScriptableObject
{
    [System.Serializable]
    public class Command
    {
        public string commandName;
        public int commandid;
    }

    [SerializeField]
    public List<Command> commands;
}