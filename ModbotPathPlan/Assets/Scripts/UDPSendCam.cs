using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;

public class UDPSendCam : MonoBehaviour {

    Thread sendThread;

    public int width = 150;
    public int height = 75;

    public byte[] toSend;

    public bool usingPhone = false;
    IPEndPoint remoteEP;
    public string playerIP;

    public int port;
    UdpClient client;

    Camera cam;

	// Use this for initialization
	void Start () {
	    if (this.transform.parent.gameObject.GetComponent<UDPControl>().usingPhone)
        {
            port = 8050;
            usingPhone = true;

            cam = GetComponent<Camera>();
            playerIP = this.transform.parent.gameObject.GetComponent<CarNavigator>().playerIP;

            sendThread = new Thread(
                new ThreadStart(sendData));
            sendThread.IsBackground = true;
            sendThread.Start();
        }
	}

    // Update is called once per frame
    void Update()
    {
        if (usingPhone)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            RenderTexture rt = new RenderTexture(width, height, 24);

            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;

            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            toSend = tex.EncodeToPNG();

            print(toSend.Length);

            cam.targetTexture = null;
            RenderTexture.active = null; // added to avoid errors 
            DestroyImmediate(rt);
        }

    }

    void sendData ()
    {
        remoteEP = new IPEndPoint(IPAddress.Parse(playerIP), port);
        client = new UdpClient();

        while (true)
        {
            try
            {
                byte[] data = getToSend();
                client.Send(data, data.Length, remoteEP);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }

    }

    public byte[] getToSend()
    {
        return toSend;
    }

    public void OnApplicationQuit()
    {
        if (sendThread != null)
        {
            sendThread.Abort();
            client.Close();
        }
    }
}

