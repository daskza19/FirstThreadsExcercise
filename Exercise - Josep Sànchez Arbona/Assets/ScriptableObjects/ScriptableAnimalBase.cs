using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "New Animal", menuName = "ScriptableObjects/Animal")]
public class ScriptableAnimalBase : ScriptableObject
{
    public string animalName = "";
    public string description = "";
    public bool isMoving = false;
    public float maxLifeTime = 5;
}
