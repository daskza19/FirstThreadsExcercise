using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

using System.Net;
using System.Net.Sockets;

public class ServerUDP : MonoBehaviour
{
    public Socket newSocket;
    public IPEndPoint ipep;
    public EndPoint sendEnp;

    // Start is called before the first frame update
    void Start()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 7000);
        sendEnp = (EndPoint)ipep;

        newSocket.Bind(ipep);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
