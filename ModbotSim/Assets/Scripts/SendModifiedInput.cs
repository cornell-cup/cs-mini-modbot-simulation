using UnityEngine;
using System.Collections;

public class SendModifiedInput : MonoBehaviour {
	Vector3 oldposition;
	Vector3 oldrotation;
	Vector3 posDisp;
	Vector3 rotDisp;

	float forwardinput;
	float turninput;
    float velX;
    float velY;

    public float speed;
    public float orien;
    public int forward;
    Rigidbody rb;
    Movement m;

	// Use this for initialization
	void Start () {
		oldposition = new Vector3 ();
		oldrotation = new Vector3 ();
		posDisp = new Vector3 ();
		rotDisp = new Vector3 ();
        rb = GetComponent<Rigidbody>();
        m = GetComponent<Movement>();
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

        //orien = transform.rotation.eulerAngles.y;
        orien = m.getWheelOrien();
        velX = rb.velocity.x;
        velY = rb.velocity.y;
        speed = rb.velocity.magnitude;
        float MIN_SPEED = 1f;
        //dead zone should be anything less than 1 because 1 means we are moving
        if (speed < MIN_SPEED)
            speed = 0;
        else
        {
            //the motor take in a speed value between 14 and 95. 
            //This does the conversion from simulation to motors: ~[0-30]->[14,95]
            speed = 14 + (81 / 30) * speed;
        }
        forward = m.isForward() ? 1 : 0;
		//send that output to ECEs

		oldposition = transform.position;
		oldrotation = transform.rotation.eulerAngles;

	}

	public void simulateCrash(){
		forwardinput = 0;
		turninput = 0;
	}
		
}
