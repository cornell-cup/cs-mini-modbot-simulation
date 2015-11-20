using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class MasterUDPReceive : MonoBehaviour
{

    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    // public
    public int port;
    public bool started = false;

    public ArrayList players = new ArrayList();
    public ArrayList playerIPs = new ArrayList();
    private int playersLen = 0;
    public GameObject kart;

    // infos
    public ArrayList lastReceivedUDPPacket = new ArrayList();

    public void Start()
    {
        init();
    }

    // init
    private void init()
    {
        print("MasterUDPReceive.init()");

        // define port
        port = 8051;
        ;

        // status
        print("Listening on everywhere : " + port);


        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    // receive thread
    private void ReceiveData()
    {
        client = new UdpClient(port);
        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
        started = true;

        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref localEp);
                lastReceivedUDPPacket = new ArrayList();
                string text = Encoding.UTF8.GetString(data);
                string[] message = text.Split(new char[] { ',' });
                Vector3 result = new Vector3(float.Parse(message[0]), float.Parse(message[1]), float.Parse(message[2]));
                string playerIP = message[3];
                string playerName = message[4];
                if (playerIPs.IndexOf(playerIP) == -1)
                {
                    players.Add(playerName);
                    playerIPs.Add(playerIP);
                }

                print(">> " + result + ", " + playerIP);

                lastReceivedUDPPacket.Add(playerIP);
                lastReceivedUDPPacket.Add(result);

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
    
    public void Update()
    {
        if (players.Count != playersLen)
        {
            GameObject clone = (GameObject)Instantiate(kart);
            clone.GetComponentInChildren<Camera>().enabled = false;
            clone.GetComponentInChildren<MeshCollider>().convex = true;
            clone.GetComponentInChildren<WheelCollider>().enabled = false;
            clone.name = "Kart UDP";
            clone.GetComponent<CarNavigator>().phoneUpdate((string) playerIPs[playersLen], (string) players[playersLen]);
            playersLen += 1;
        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public ArrayList getLatestUDPPacket()
    {
        return lastReceivedUDPPacket;
    }

    public void OnApplicationQuit()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort();
            client.Close();
        }
    }

}