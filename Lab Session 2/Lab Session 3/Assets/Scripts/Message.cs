using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public string userName = "Default User Name";
    public Color userColor;
    public string message = "Default Message";

    public Message(string _userName, Color _userColor, string _message)
    {
        userName = _userName;
        userColor = _userColor;
        message = _message;
    }

    public void InstantiateNewMessage(GameObject _prefab, GameObject _parent, MessagesList _MList)
    {
        GameObject go = Instantiate(_prefab, new Vector3(0, 0, 0), Quaternion.identity, _parent.transform);
        Text[] texts = new Text[2];
        texts = go.GetComponentsInChildren<Text>();
        texts[0].text = userName;
        texts[1].text = message;
        texts[1].color = userColor;
        Image[] images = go.GetComponentsInChildren<Image>();
        images[1].color = userColor;
        _MList.messagesList.Add(this);
    }
}