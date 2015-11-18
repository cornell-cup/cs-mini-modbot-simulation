using UnityEngine;
using System.Collections;

public class UDPControl : MonoBehaviour {

    public GameObject masterScript;
    public Vector3 move;
    public bool usingPhone = false;

	// Use this for initialization
	void Start ()
    {
        masterScript = GameObject.FindGameObjectWithTag("MasterUDP");
        if (this.GetComponent<CarNavigator>().usingPhone)
        {
            usingPhone = true;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (usingPhone)
        {
            ArrayList lastMessage = masterScript.GetComponent<MasterUDPReceive>().lastReceivedUDPPacket;
            if (lastMessage.Count != 0 && lastMessage[0].ToString() == this.GetComponent<CarNavigator>().playerIP)
            {
                move = (Vector3)lastMessage[1];
            }
        }
    }
}
