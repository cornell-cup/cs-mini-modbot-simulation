using UnityEngine;
<<<<<<< HEAD
using System.Collections.Generic;

using System;
using System.Net;
using System.Net.Sockets;

public class UDPSend
{
	//prefs
	private string IP;
	private int port;

	//connection stuff
	private static IPEndPoint computerEP;
	private static UdpClient unity_sender;

	private static byte[] packet;

	public UDPSend(string ip, int p)
	{
		//IP
		IP = ip;
		port = p;

		computerEP = new IPEndPoint(IPAddress.Parse(IP), port);
		unity_sender = new UdpClient();
		unity_sender.Connect(computerEP);
	}

	public static void newPacket(){
		packet = null;
	}

	public static void addVector(Vector3 v)
	{
		addFloat(v.x);
		addFloat(v.y);
		addFloat(v.z);
	}

	public static float addFloat(float f){
		byte[] newFloatByte = BitConverter.GetBytes(f);
		if(packet != null){
			List<byte> oldlist = new List<byte>(packet);
			List<byte> newlist = new List<byte>(newFloatByte);
			oldlist.AddRange(newlist);
			packet = oldlist.ToArray();
		}
		else{
			packet = newFloatByte;
		}
		return f;
	}

	public static void sendPacket(){
		unity_sender.Send(packet, packet.Length);
	}
}
=======
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class UDPSend : MonoBehaviour
{
    //private static int localport;
    public static Socket unity;
    public static int ct;
    //prefs
    private string IP;
    public int port;
    public Boolean DEBUG = false;

    //connection stuff
    IPEndPoint remoteEndPoint;
    UdpClient client;
    SendModifiedInput sd;

    // Use this for initialization
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        print("Initiating start sequence");
        sd = GetComponent<SendModifiedInput>();
        init();
    }

    public void init()
    {
        print("UDPSend.init()");

        //IP
        IP = "192.168.4.12"; //193.168.1.2
        port = 607;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

    }

    // Update is called once per frame
    void Update()
    {
        sendCommand();
    }

    private void sendCommand()
    {
        
        if (DEBUG)
        {
            return;
        }
        try
        {
            List<byte[]> data = addCommand(); ;
            byte[] dataf = data
                .SelectMany(a => a)
                .ToArray();
            //Debug.Log(BitConverter.ToString(dataf));
            client.Send(dataf, dataf.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    private List<byte[]> addCommand()
    {
        //Debug.Log("Here in udpsenddata");
        List<byte[]> b = new List<byte[]>();
        //placeholder values should be replaced with the sims current values
        // float magnitudex = 5.0f;
        // float magnitudey = 6.0f;
        float speed = sd.speed;
        int direction = sd.forward;
        int orientation = (int)sd.orien;
        //b.Add(BitConverter.GetBytes(magnitudex));
        // b.Add(BitConverter.GetBytes(magnitudey));
        b.Add(BitConverter.GetBytes(orientation));
        b.Add(BitConverter.GetBytes(direction));
        b.Add(BitConverter.GetBytes(speed));
        return b;
    }

    private void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            Debug.Log(data);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch
        {
            print("Error");
        }
    }
}
>>>>>>> 2fc66ecc298e40669e0b912287d9b92e6667045d
