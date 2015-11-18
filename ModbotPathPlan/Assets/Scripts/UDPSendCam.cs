using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System;

public class UDPSendCam : MonoBehaviour {

    public int width = 100;
    public int height = 50;

    public byte[] toSend;

    public bool usingPhone = false;
    private IPEndPoint remoteEP;

    public int port;
    private UdpClient client;

	// Use this for initialization
	void Start () {
	    if (this.transform.parent.gameObject.GetComponent<UDPControl>().usingPhone)
        {
            port = 8050;
            usingPhone = true;
            remoteEP = new IPEndPoint(IPAddress.Parse(this.transform.parent.gameObject.GetComponent<CarNavigator>().playerIP), port);

            client = new UdpClient();
        }
	}

    // Update is called once per frame
    void Update ()
    {
        if (usingPhone)
        {
            Camera cam = GetComponent<Camera>();
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            RenderTexture rt = new RenderTexture(width, height, 24);
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;

            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            toSend = tex.EncodeToPNG();


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
