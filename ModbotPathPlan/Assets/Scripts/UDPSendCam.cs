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

    public int port;
    UdpClient client;

    Camera cam;
    Texture2D tex;
    RenderTexture rt;

	// Use this for initialization
	void Start () {
	    if (this.transform.parent.gameObject.GetComponent<UDPControl>().usingPhone)
        {
            port = 8050;
            usingPhone = true;

            cam = GetComponent<Camera>();
            tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            rt = new RenderTexture(width, height, 24);

            sendThread = new Thread(
                new ThreadStart(sendData));
            sendThread.IsBackground = true;
            sendThread.Start();
        }
	}

    // Update is called once per frame
    void sendData ()
    {
        remoteEP = new IPEndPoint(IPAddress.Parse(this.transform.parent.gameObject.GetComponent<CarNavigator>().playerIP), port);

        client = new UdpClient();

        if (usingPhone)
        {
            while (true)
            {
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

                try
                {
                    //client.Send(toSend, toSend.Length, remoteEP);
                    client.Send(toSend, toSend.Length, remoteEP);
                }
                catch (Exception err)
                {
                    print(err.ToString());
                }
            }
        }

    }

    public void OnApplicationQuit()
    {
        if (sendThread != null)
        {
            client.Close();
            sendThread.Abort();
        }
    }
}

