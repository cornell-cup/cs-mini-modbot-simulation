using UnityEngine;
using System.Collections;

public class SendModifiedInput : MonoBehaviour {
	Vector3 oldposition;
	Vector3 oldrotation;
	Vector3 posDisp;
	Vector3 rotDisp;

	float forwardinput;
	float turninput;

	// Use this for initialization
	void Start () {
		oldposition = new Vector3 ();
		oldrotation = new Vector3 ();
		posDisp = new Vector3 ();
		rotDisp = new Vector3 ();
	}
	
	// Update is called once per frame
	void Update () {
		//how much kart needs to be displaced in Time.deltaTime seconds
		posDisp = transform.position - oldposition;
		rotDisp = transform.position - oldrotation;

		//get output in some format that accomplishes the desired displacements in Time.deltaTime seconds
		forwardinput = posDisp.magnitude;
		if (forwardinput < -1)
			forwardinput = -1;
		if (forwardinput > 1)
			forwardinput = 1;

		turninput = rotDisp.y;
		if (turninput < -1)
			turninput = -1;
		if (turninput > 1)
			turninput = 1;


		//send that output to ECEs

		oldposition = transform.position;
		oldrotation = transform.rotation.eulerAngles;

	}

	public void simulateCrash(){
		forwardinput = 0;
		turninput = 0;
	}
		
}
