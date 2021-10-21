using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandsList", menuName = "ScriptableObjects/CommandsList", order = 1)]
public class CommandsList : ScriptableObject
{
    public List<string> commands;
}
