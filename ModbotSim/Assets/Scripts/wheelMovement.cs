using UnityEngine;
using System.Collections;

public class wheelMovement : MonoBehaviour {
	float motorForce=4000;
	float turnForce=25;
	float brakeForce=8000;
	WheelCollider fr;
	WheelCollider fl;
	WheelCollider br;
	WheelCollider bl;
	private float turnInput;
	private float forwardInput;
	private float brakeInput;
	public float MAX_SPEED=15;
	public float boost = 1f;
	GetInput input;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		
		input = GetComponent<GetInput> ();
		fr = GameObject.FindGameObjectWithTag ("wcfr").GetComponent<WheelCollider> ();
		fl = GameObject.FindGameObjectWithTag ("wcfl").GetComponent<WheelCollider> ();
		br = GameObject.FindGameObjectWithTag ("wcbr").GetComponent<WheelCollider> ();
		bl = GameObject.FindGameObjectWithTag ("wcbl").GetComponent<WheelCollider> ();
		fr.enabled = true;
		fl.enabled = true;
		br.enabled = true;
		bl.enabled = true;
		rb = GameObject.FindGameObjectWithTag ("kart").GetComponent<Rigidbody> ();
		boost = 1f;

	}
	
	// Update is called once per frame
	void Update () {
		turnInput = input.getTurnInput ();
		forwardInput = input.getForwardInput ();
		brakeInput = input.getBraking ();
		if (rb.velocity.magnitude < MAX_SPEED && forwardInput!=0) {
			print ("Y" + rb.velocity.magnitude+ " X:"+forwardInput+" Z:"+boost);

			br.motorTorque = forwardInput * motorForce * boost;
			bl.motorTorque = forwardInput * motorForce * boost;
		} else {
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


	}

	public bool isArtificialIntelligence(){
		return false;
	}
}
