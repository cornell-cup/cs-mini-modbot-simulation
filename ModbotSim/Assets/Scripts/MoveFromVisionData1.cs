using UnityEngine;
using System.Collections.Generic;
using System;

public class MoveFromVisionData1 : MonoBehaviour
{
	private Vector3 pos;
	private float time;
	private float rotation;
	private static float SCALE;
	private float elapsedTime;

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
		}
	}

}