using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Movement : MonoBehaviour {
	//whether or not this movement script is attached to an AI or player controlled car
	public bool isAI;
    public int kartNum;
	//basic movement variables that both an AI car and player controlled car utilizes
	private float turnInput;
	private float forwardInput;
	private float brakeInput;
    public bool writePositionsToFile = false;

    //Player controlled car movement variables
    float motorForce=4000;
	float turnForce=45;
	float brakeForce=8000;
	WheelCollider fr;
	WheelCollider fl;
	WheelCollider br;
	WheelCollider bl;
	public float MAX_SPEED=15;
	public float boost = 1f;
	GetInput input;
	Rigidbody rb;

	//AI car movement variables
	private float currentDeltaTurn = 0f;
	private float speed = 0f;
	private float turnValue = 0f;
	public static float ACCEL = 3.4f/4.5f; // m/s^2
	[System.NonSerialized]
	public static float DELTA_TURN = .5f*4.5f; 
	[System.NonSerialized]
	public static float MAX_TURN = 5.91f*4.5f;// turn radius in meters ~ 64 / MAX_TURN (car is 1 meter long, .5 meters wide)
	[System.NonSerialized]
	public static float IDLE_ACCEL = ACCEL / 10;
	private CarController carController = new CarController();

    public float getWheelOrien()
    {
        return turnInput * turnForce;
    }

    private Vector3 rotationVector;
	private Vector3 direction = Vector3.forward;
	private bool goingBackwards = false;
    PathPlanningKart kartp;

    public float studder = 0;
	private float startTime = 0;
	private bool start = false;

    // <summary>
    // Use this for initialization
    // </summary>
    void Start () {
		startTime = Time.realtimeSinceStartup;
		input = GetComponent<GetInput> ();
        //GameObject o = GameObject.Find("Kart " + kartNum);
        Debug.Log("here");
        //Debug.Log("rrr"+o.transform);
		rb = GetComponent<Rigidbody> ();
        Debug.Log("rrr" + rb);
        boost = 1f;
        fr = GameObject.Find("wcfr "+kartNum).GetComponent<WheelCollider>();
        fl = GameObject.Find("wcfl " + kartNum).GetComponent<WheelCollider>();
        br = GameObject.Find("wcbr " + kartNum).GetComponent<WheelCollider>();
        bl = GameObject.Find("wcbl " + kartNum).GetComponent<WheelCollider>();
        fr.enabled = true;
        fl.enabled = true;
        br.enabled = true;
        bl.enabled = true;

        if (isAI) {
			ItemsAI.updateItems ();
			PathPlanningDataStructures.initializePathPlanning ();
			kartp = GetComponentInChildren<PathPlanningKart> ();
			kartp.PathPlanInitialSegment ();
            MAX_SPEED *= .8f;
		} 

	}

    // <summary>
    // Update is called once per frame
    // </summary>
    void FixedUpdate() {
		if (Time.realtimeSinceStartup - startTime > 4.5f) {
			print ("counting down");
			start = true;
		}
		
		if (start) {
			//Debug.Log("Delta Time: " + Time.deltaTime);
			Debug.Log("boost: " + boost);
			WheelHit hit;
			float travelL = 1.0f;
			float travelR = 1.0f;
			float AntiRoll = 5000;

			bool groundedL = fl.GetGroundHit(out hit);
			if (groundedL)
				travelL = (-fl.transform.InverseTransformPoint(hit.point).y - fl.radius) / fl.suspensionDistance;

			bool groundedR = fr.GetGroundHit(out hit);
			if (groundedR)
				travelR = (-fr.transform.InverseTransformPoint(hit.point).y - fr.radius) / fr.suspensionDistance;

			var antiRollForce = (travelL - travelR) * AntiRoll;

			if (groundedL)
				rb.AddForceAtPosition(fl.transform.up * -antiRollForce,
					fl.transform.position);
			if (groundedR)
				rb.AddForceAtPosition(fr.transform.up * antiRollForce,
					fr.transform.position);

			bool bgroundedL = bl.GetGroundHit(out hit);
			if (bgroundedL)
				travelL = (-bl.transform.InverseTransformPoint(hit.point).y - bl.radius) / bl.suspensionDistance;

			bool bgroundedR = br.GetGroundHit(out hit);
			if (bgroundedR)
				travelR = (-br.transform.InverseTransformPoint(hit.point).y - br.radius) / br.suspensionDistance;


			if (bgroundedL)
				rb.AddForceAtPosition(bl.transform.up * -antiRollForce,
					bl.transform.position);
			if (bgroundedR)
				rb.AddForceAtPosition(br.transform.up * antiRollForce,
					br.transform.position);
			if (isAI){
				//PathPlanningKart k = GetComponentInChildren<PathPlanningKart>();
				kartp.PathPlanNextSegment();
				Tuple<float, float> t = carController.speedAndTurn(this.gameObject);
				turnInput = (float)t.Second;
				forwardInput = (float)t.First;
				//Debug.Log("Forward Input:" + forwardInput + " Turn Input:" + turnInput);
				ItemsAI.updateItems();
				kartp.UseItem();
			} else {
				turnInput = input.getTurnInput();
				forwardInput = input.getForwardInput();
				//Debug.Log("Forward Input:" + forwardInput + " Turn Input:" + turnInput);
			}

			brakeInput = input.getBraking();
			if (rb.velocity.magnitude < MAX_SPEED && forwardInput != 0)
			{
				//print("Y" + rb.velocity.magnitude + " X:" + forwardInput + " Z:" + boost);

				br.motorTorque = forwardInput * motorForce * boost;
				bl.motorTorque = forwardInput * motorForce * boost;

			}
			else
			{
				br.motorTorque = 0;
				bl.motorTorque = 0;

			}
			//		} else {
			//			br.brakeTorque = -1*forwardInput * brakeForce;
			//			bl.brakeTorque = -1*forwardInput * brakeForce;
			//			fr.brakeTorque = -1*forwardInput * brakeForce;
			//			fl.brakeTorque = -1*forwardInput * brakeForce;
			//		}
			//		if (brakeInput > 0) {
			//			br.motorTorque = 0;
			//			bl.motorTorque = 0;
			//		}

			fr.steerAngle = turnInput * turnForce;
			fl.steerAngle = turnInput * turnForce;

			br.brakeTorque = brakeInput * brakeForce * boost;
			bl.brakeTorque = brakeInput * brakeForce * boost;
			fr.brakeTorque = brakeInput * brakeForce * boost;
			fl.brakeTorque = brakeInput * brakeForce * boost;

			br.brakeTorque += studder * brakeForce;
			bl.brakeTorque += studder * brakeForce;
			fr.brakeTorque += studder * brakeForce;
			fl.brakeTorque += studder * brakeForce;


			if (writePositionsToFile)
				WriteToRepo();

			Quaternion temp = transform.rotation;
			Vector3 tt = temp.eulerAngles;
			if (tt.x > 300 && tt.x < 357)
				tt.x = 359.5f;
			if (tt.x < 50 && tt.x > .5f)
				tt.x = .5f;

			if (tt.z > 300 && tt.z < 359.5f)
				tt.z = 359.5f;
			if (tt.z < 50 && tt.z > .5f)
				tt.z = .5f;
			tt.Set(tt.x, tt.y, tt.z);
			temp.eulerAngles = tt;
			transform.rotation = temp;
		}
    }

    public bool isForward()
    {
        //Debug.Log("YYY" + rb.velocity);
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        if (localVel.z > 0)
            return true;
        //  if (fr.motorTorque > 0)
        //   return true;

      //  if (forwardInput > 0)
           // return true;
        return false;
    }

    void WriteToRepo()
    {
        int id = 1;
        var sw = new System.IO.StreamWriter(Application.dataPath + "/Scripts/WriteToFile.txt", true);
        sw.Write(string.Format("{0}\t{1}\t{2}\n", transform.position.x, transform.position.y, transform.position.z));
        sw.Close();
    }

    // <summary>
    // Returns whether the car is an AI car or a player controlled car

    // </summary>
    public bool isArtificialIntelligence(){
		return isAI;
	}

	// <summary>
	// Draws the current waypoints for an AI car
	// </summary>
	public void OnDrawGizmosSelected()
	{
		if (isAI) {
			PathPlanningKart k = GetComponent<PathPlanningKart> ();
			List<Vector3> currentWayPoints = k.currentWayPoints;
			if (currentWayPoints == null) {
				return;
			}
			for (int i = 0; i < currentWayPoints.Count; i++) {
				Vector3 point = currentWayPoints [i];
				Gizmos.color = new Color (0.0f, 0.0f, 1.0f, 0.3f);
				Gizmos.DrawCube (point, new Vector3 (3.0f, 3.0f, 3.0f));

				int x = i + 1;
				if (x < currentWayPoints.Count) {
					Gizmos.color = Color.magenta;
					Gizmos.DrawLine (point, currentWayPoints [x]);
				}
			}
		}
	}

//	private void sendData(){
//		UDPSend.newPacket();
//		UDPSend.addFloat(scale * Movement.instance.getLinVel().magnitude);
//		UDPSend.addVector(scale * Movement.instance.getAngVel());
//		UDPSend.addVector(scale * Movement.instance.getLinAcc());
//		UDPSend.addVector(scale * Movement.instance.getAngAcc());
//		UDPSend.addVector(transform.rotation.eulerAngles);
//		UDPSend.sendPacket();
//	}
}
