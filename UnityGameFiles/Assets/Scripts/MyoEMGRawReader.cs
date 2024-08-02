using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MyoEMGRawReader : MonoBehaviour
{
    public string IP = "127.0.0.1";
    public int port = 12346;

    // read Thread
    Thread readThread;
    // udpclient object
    UdpClient client;
    String control = "";
    float speed = 0.0f;
    

    public void StartReadingData()
    {
        // create thread for reading messages
        readThread = new Thread(new ThreadStart(ReceiveData));
        readThread.IsBackground = true;
        readThread.Start();
    }

    // Unity Application Quit Function
    void OnApplicationQuit()
    {
        stopThread();
    }

    // Stop reading messages
    public void stopThread()
    {
        if (readThread.IsAlive)
        {
            readThread.Abort();
        }
        client.Close();
    }

    // receive thread function
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            // receive bytes
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
            byte[] buff = client.Receive(ref anyIP);

            // encode UTF8-coded bytes to text format
            string text = Encoding.UTF8.GetString(buff);
            string[] parts = text.Split(' ');
            control = parts[0];
            speed = float.Parse(parts[1]);
        }
    }

    public string ReadControlFromArmband() 
    {
        return control;
    }

    public float ReadSpeedFromArmband() 
    {
        return speed;
    }
}