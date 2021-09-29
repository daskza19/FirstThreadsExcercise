using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Animal", menuName = "Scriptable Objects/Animal")]
public class ScriptableAnimal : ScriptableObject
{
    public Sprite sprite;
    public string animalName = "";
    public string description = "";
    public bool isMoving;
    [Range(0.5f, 10)]
    public float maxLife = 5;
    public float creationCooldown = 2;
}
