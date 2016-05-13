using UnityEngine;
using System.Collections.Generic;
using System;

public class MoveFromVisionData1 : MonoBehaviour
{
	private Vector3 pos;
	private float time;
	private float rotation;
	public static float SCALE = 15f;
	private float elapsedTime;
	public bool didCollide;
	Vector3 m = new Vector3 ();
	Vector3 s = new Vector3 ();
    Vector3 offset;
    Vector3 oldVel = Vector3.zero;
    Rigidbody rb;

	VisionData dataObj;
	// Use this for initialization
	void Start(){
		pos = new Vector3 ();
        dataObj = GetComponent<VisionData>();
        offset = new Vector3(0,0,0);
        rb = GameObject.FindGameObjectWithTag("kart").GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update() {
        if (dataObj != null && dataObj.time>0)
        {
           // Debug.Log("Here");
            elapsedTime += Time.deltaTime;
            time = 0;
            //&& dataObj.used == false
            //elapsedTime > dataObj.time && 
            if (dataObj.used == false && dataObj.positionX != transform.position.x && dataObj.positionY != transform.position.z)
            {
               
                dataObj.used = true;
                pos.Set(SCALE * dataObj.positionX - offset.x, 0, SCALE * dataObj.positionY- offset.z);
                time = dataObj.time;
                rotation = Math.Abs(dataObj.orientation) * ((float)(180f / Math.PI)) % 360;
                transform.position = pos;
                transform.rotation = Quaternion.Euler(0, rotation, 0);
                m.Set(pos.x + transform.position.x / 2, pos.y + transform.position.y / 2, pos.z + transform.position.z / 2);
                didCollide = Physics.CheckBox(m, s);
                Debug.Log("vel " + rb.velocity);
                rb.velocity = oldVel;

            }
        } else
        {
            //dataObj = GameObject.FindGameObjectWithTag("udpvision").GetComponent<UDPVisionReceive>().getDataObj(0);
            Debug.Log("non " + rb.velocity);
            oldVel = rb.velocity;
        }
        
        UDPVisionReceive.decodeDatagram();
        
	}

}