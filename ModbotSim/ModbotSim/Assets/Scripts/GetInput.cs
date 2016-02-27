using UnityEngine;
using System.Collections;

public class GetInput : MonoBehaviour {
	public bool usingPhone = false;
	private string horizontal;
	private string vertical;
	private KeyCode left;
	private KeyCode right;
	private KeyCode up;
	private KeyCode down;

	// Use this for initialization
	void Start () {
		//this.
		//var sw = new StreamWriter(Application.dataPath + "/Scripts/WriteToFile.txt", false);
		//sw.Write(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t\n")); 
		//print("initializing " + this);
		string[] name = this.ToString().Split(' ');
		if (name[0] != "Kart")
		{
			Debug.Log(name[0]);
			throw new System.Exception("Movement script attached to non Kart object");
		}
		switch (name[1])
		{
		case "1":
			horizontal = "Horizontal";
			vertical = "Vertical";
			left = KeyCode.LeftArrow;
			right = KeyCode.RightArrow;
			up = KeyCode.UpArrow;
			down = KeyCode.DownArrow;
			break;
		case "2":
			horizontal = "Horizontal2";
			vertical = "Vertical2";
			left = KeyCode.A;
			right = KeyCode.D;
			up = KeyCode.W;
			down = KeyCode.S;
			break;
		case "3":
			horizontal = "Horizontal3";
			vertical = "Vertical3";
			break;
		case "4":
			horizontal = "Horizontal4";
			vertical = "Vertical4";
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	public float getTurnInput(){
		Vector3 Udp = Vector3.zero;
		if (!usingPhone) {
			if (Input.GetKey(right)){
				return 1;
			}
			if (Input.GetKey(left)){
				return -1;
			}
			return Input.GetAxis (horizontal);
		}
		else
		{
			Udp = GetComponent<UDPReceive>().getLatestUDPPacket();
			float turnInput = Udp.x;
			if (Mathf.Abs(turnInput) < .2)
				return 0;
			return turnInput;
		}
	}

	public int getForwardInput(){
		Vector3 Udp = Vector3.zero;
		int forwardInput;
		if (!usingPhone)
		{
			if (Input.GetAxis(vertical) > 0)
				forwardInput = 1;
			else if (Input.GetAxis(vertical) < 0)
				forwardInput = -1;
			else if (Input.GetKey (up)) {
				return 1;
			}
			else if (Input.GetKey (down)) {
				return -1;
			} 
			else
				forwardInput = 0;
		}
		else
		{
			float z = Udp.z;
			if (z < -.25)
				forwardInput = 1;
			else if (z > 0)
				forwardInput = -1;
			else
				forwardInput = 0;
		}
		return forwardInput;
	}

}
