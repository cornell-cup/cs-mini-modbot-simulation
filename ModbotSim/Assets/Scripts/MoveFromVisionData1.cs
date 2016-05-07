using UnityEngine;
using System.Collections.Generic;
using System;

public class MoveFromVisionData1 : MonoBehaviour
{
	private Vector3 pos;
	private float time;
	private float rotation;
	public static float SCALE;
	private float elapsedTime;
	public bool didCollide;
	Vector3 m = new Vector3 ();
	Vector3 s = new Vector3 ();

	VisionData dataObj;
	// Use this for initialization
	void Start(){
		pos = new Vector3 ();
		dataObj = GameObject.FindGameObjectWithTag ("udpvision").GetComponent<UDPVisionReceive>().getDataObj (0);
	}

	// Update is called once per frame
	void Update() {
		elapsedTime += Time.deltaTime;
		time = 0; 
		if (elapsedTime > dataObj.time) { 
			pos.Set (SCALE * dataObj.positionX, 0, SCALE * dataObj.positionY);
			time = dataObj.time;
			rotation = dataObj.orientation;
			transform.position = pos;
			transform.rotation = Quaternion.Euler (0, 5, 0);
			m.Set (pos.x + transform.position.x / 2, pos.y + transform.position.y / 2, pos.z + transform.position.z / 2);
			didCollide = Physics.CheckBox (m, s);
		}
	}

}