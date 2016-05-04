using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPVisionReceive : MonoBehaviour
{
    // Address
    private string localIP;
    private string remoteIP;
    private int port;

    // Connection
    private static IPEndPoint unityEP;
    private static IPEndPoint computer_sender_EP;
    private static UdpClient unity_receiver;

    private static byte[] packet;

    private static bool abortThread;

    // receiving Thread
    private Thread receiveThread;
	private static VisionData[] dataObjs = new VisionData[4];

	public VisionData getDataObj(int i){
		return dataObjs [i];
	}

    public void Start()
    {
		for(int i = 0; i< 4; i++)
			dataObjs[i] = new VisionData();
		
        localIP = "127.0.0.1";
        remoteIP = "127.0.0.1";
        port = 607;

        unityEP = new IPEndPoint(IPAddress.Parse(localIP), port);
        computer_sender_EP = new IPEndPoint(IPAddress.Parse(remoteIP), port);

        unity_receiver = new UdpClient();
        unity_receiver.Client.Bind(unityEP);
        unity_receiver.Client.ReceiveTimeout = 1000;

        packet = new byte[68];

        abortThread = false;

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {
        while(!abortThread)
        {
            try
            {
                packet = unity_receiver.Receive(ref unityEP);
            }
            catch(Exception err)
            {
                //print(err.ToString());
            }            
        }
    }

    public static void decodeDatagram()
    {
        byte[] data = packet;

        int id = BitConverter.ToInt32(data, 0);
		dataObjs[id].time = (float)BitConverter.ToDouble(data, 4);
		dataObjs[id].positionX = (float)BitConverter.ToDouble(data, 12);
		dataObjs[id].positionY = (float)BitConverter.ToDouble(data, 20);
        int innerColor = BitConverter.ToInt32(data, 28);
        int outerColor = BitConverter.ToInt32(data, 32);

		dataObjs[id].orientation = (float)BitConverter.ToDouble(data, 36);
		dataObjs[id].velocityRot = (float)BitConverter.ToDouble(data, 44);
		dataObjs[id].velocityx = (float)BitConverter.ToDouble(data, 52);
		dataObjs[id].velocityy = (float)BitConverter.ToDouble(data, 60);

//        string msg = "id: " + id + " time: " + time + " x: " + x + " y: " + y +
//                     " innerColor: " + innerColor + " outerColor: " + outerColor +
//                     " orientation: " + orientation + " velocityRot: " + velocityRot +
//                     " velocityx: " + velocityx + " velocityy: " + velocityy;
//        Debug.Log(msg);
    }

    public void OnApplicationQuit()
    {
        abortThread = true;
    }
}