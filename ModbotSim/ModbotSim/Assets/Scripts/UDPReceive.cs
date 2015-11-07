using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour {
	
	// receiving Thread
	Thread receiveThread;
	
	// udpclient object
	UdpClient client;
	
	// public
	// public string IP = "127.0.0.1"; default local
	public int port; // define > init
	
	// infos
	public Vector3 lastReceivedUDPPacket;


	// start from unity3d
	public void Start()
	{
		
		init ();
	}
	
	// init
	private void init()
	{
		print("UDPSend.init()");
		
		// define port
		port = 8051;
		
		// status
		print("Sending to 127.0.0.1 : "+port);
		print("Test-Sending to this Port: nc -u 127.0.0.1  "+port+"");
		
		
		receiveThread = new Thread(
			new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		
	}
	
	// receive thread
	private  void ReceiveData()
	{
		client = new UdpClient(port);
		while (true)
		{
			try
			{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.UTF8.GetString(data);
				string[] message = text.Split(new char[] {','});
				Vector3 result = new Vector3 (float.Parse(message[0]), float.Parse(message[1]), float.Parse(message[2]));

				print(">> " + result);
				
				lastReceivedUDPPacket=result;
				
				
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}
	
	// getLatestUDPPacket
	// cleans up the rest
	public Vector3 getLatestUDPPacket()
	{
		return lastReceivedUDPPacket;
	}

	public void OnApplicationQuit() {
        if (receiveThread.IsAlive) {
			receiveThread.Abort();
		}
		client.Close ();
	}

} 