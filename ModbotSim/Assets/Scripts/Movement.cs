using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour {
	//whether or not this movement script is attached to an AI or player controlled car
	public bool isAI = false;

	//basic movement variables that both an AI car and player controlled car utilizes
	private float turnInput;
	private float forwardInput;
	private float brakeInput;

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
	private Vector3 rotationVector;
	private Vector3 direction = Vector3.forward;
	private bool goingBackwards = false;
	private bool paused = false;

	// <summary>
	// Use this for initialization
	// </summary>
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

		if (isAI) {
			ItemsAI.updateItems ();
			PathPlanningKart k = GetComponent<PathPlanningKart> ();
			k.PathPlanInitialSegment ();
			MAX_SPEED = 40f / 4.5f;
		}

	}

	// <summary>
	// Update is called once per frame
	// </summary>
	void FixedUpdate () {
		if (isAI)
		{
			UpdateAI ();
		}
		else if (Input.GetKeyUp(KeyCode.P))
		{
			Time.timeScale = paused ? 1 : 0;
			paused = !paused;
		}
		else {
			UpdatePlayer ();
		}


	}

	public void UpdateAI() {
		PathPlanningKart k = GetComponent<PathPlanningKart>();
		k.PathPlanNextSegment();
		Tuple<float, float> t = carController.speedAndTurn(this.gameObject);
		turnInput = (float)t.Second;
		forwardInput = (float)t.First;
		ItemsAI.updateItems ();
		k.UseItem();

		//how quickly do we want to turn
		currentDeltaTurn = DELTA_TURN * turnInput;
		//takes care of ideling to go back straight && also turning

		//Debug.Log ("(turnValue + currentDeltaTurn): "+(turnValue + currentDeltaTurn)+"  MAXTU: "+(MAX_TURN)+"  speed: "+speed);
		if (Mathf.Abs(turnInput) < .1 && turnValue > 0)
		{
			turnValue = 0;
			// Debug.Log("Idle Pos");
			if (turnValue < DELTA_TURN)
				turnValue = 0;
			else
				turnValue -= DELTA_TURN;
		}
		else if (Mathf.Abs(turnInput) < .1 && turnValue < 0)
		{
			turnValue = 0;
			// Debug.Log("Idle Neg");
			if (turnValue > -DELTA_TURN)
				turnValue = 0;
			else
				turnValue += DELTA_TURN;
		}
		else if ((turnValue + currentDeltaTurn) < (MAX_TURN) && (turnValue + currentDeltaTurn) > (-MAX_TURN))
		{
			// Debug.Log("Turn");
			turnValue += currentDeltaTurn;
		}

		//takes care of Acceleration
		speed += ACCEL * forwardInput;
		if (speed < -MAX_SPEED)
		{
			speed = -MAX_SPEED;
		}
		if (speed > MAX_SPEED)
		{
			speed = MAX_SPEED;
		}
		if (forwardInput == 1 && speed < MAX_SPEED)
		{
			speed += ACCEL;
		}
		else if (forwardInput == -1 && speed > -MAX_SPEED)
		{
			speed -= ACCEL;
		}
		else if (forwardInput == 0 && speed < 0)
		{
			if (speed > -IDLE_ACCEL)
				speed = 0;
			else
				speed += IDLE_ACCEL;
		}
		else if (forwardInput == 0 && speed > 0)
		{
			if (speed < IDLE_ACCEL)
				speed = 0;
			else
				speed -= IDLE_ACCEL;
		}
		else if (Mathf.Abs(speed) > MAX_SPEED)
		{
			speed = Mathf.Sign(speed) * MAX_SPEED;
		}

		//applies boost, if any
		speed = speed * boost;

		//calculates the direction displacement vector
		rotationVector.Set(0, turnValue * speed * Time.deltaTime, 0);
		direction = Quaternion.Euler(rotationVector) * direction;

		transform.position += direction* (speed * Time.deltaTime);
		if (!goingBackwards)
			transform.rotation = Quaternion.LookRotation(direction);
		else
			transform.rotation = Quaternion.LookRotation(-1 * direction);
	}

	// <summary>
	// Updates a player controlled car's position and movement state
	// </summary>
	public void UpdatePlayer() {
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
}
