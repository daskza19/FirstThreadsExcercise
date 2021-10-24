using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public Text userName;
    public Image userColor;

    public void Setuser(string _userName, Color _userColor)
    {
        userName.text = _userName;
        userColor.color = _userColor;
    }
}
