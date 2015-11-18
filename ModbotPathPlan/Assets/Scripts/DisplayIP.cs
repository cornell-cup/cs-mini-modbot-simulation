using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;

public class DisplayIP : MonoBehaviour
{

    Text IPText;
    public ArrayList players;
    public int playersLen = 0;
    public GameObject scriptLocation;

    // Use this for initialization
    void Start()
    {
        IPText = GetComponent<Text>();
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        IPText.text += host.AddressList[0] + "\n\n";
        players = new ArrayList();
        scriptLocation = GameObject.FindGameObjectWithTag("MasterUDP");
    }

    // Update is called once per frame
    void Update()
    {

        players = scriptLocation.GetComponent<MasterUDPReceive>().players;
        if (players.Count != playersLen)
        {
            IPText.text += players[playersLen] + "\n";
            playersLen += 1;
        }

    }
}
