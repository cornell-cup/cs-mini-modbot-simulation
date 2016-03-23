using UnityEngine;
using System.Collections;
using System.IO;

public class Movement : MonoBehaviour
{

	public bool isAI=false;
	public bool writePositionsToFile = false;
    [System.NonSerialized]
    public bool usingPhone = false;
    [System.NonSerialized]
    public static float ACCEL = 3.4f/4.5f; // m/s^2
    [System.NonSerialized]
    public static float DELTA_TURN = .5f*4.5f; 
    [System.NonSerialized]
	public static float MAX_TURN = 5.91f*4.5f;// turn radius in meters ~ 64 / MAX_TURN (car is 1 meter long, .5 meters wide)
	[System.NonSerialized]
	public static float IDLE_ACCEL = ACCEL / 10;
//	[System.NonSerialized]
//	public static float MAX_TURN = 0.5f;
    [HideInInspector]
    [System.NonSerialized]
    public float MAX_SPEED = 40f / 4.5f; // this is meters per second! woo



    private float currentDeltaTurn = 0f;
    private float speed = 0f;
    private float turnValue = 0f;
    private float turnInput = 0f;
    private Vector3 direction = Vector3.forward;
    private Vector3 rotationVector;
    private bool goingBackwards = false;
    private float forwardInput = 0;
	public float boost = 1f;

    private string horizontal;
    private string vertical;
	GetInput input;

	//Log Stuff
	private Vector3 position = new Vector3();
	private Vector3 oldPosition = new Vector3();
	private Vector3 velocity = new Vector3();
	private Vector3 acceleration = new Vector3();
	private Vector3 rotation = new Vector3();
	private Vector3 oldvelocity = new Vector3();


	private Vector3 velocityX = new Vector3();
	private Vector3 velocityY = new Vector3();
	private Vector3 moveVector = new Vector3();

	private float yAngleOffsetVelocity=0;
	private float yAngleOffset=0;


	private float height=.5f;
	private float width=.5f;
	private float motorOutForce;
	private float resistForce;
	private float rollingResistForce;
	private float airResistForce;
	private float centripetalForce;

	private float Ic;
	private float mTor = .1f;
	private float effec = 1;
	private float gearRatio = 1;
	private float angularSpeed = 350;
	private float wheelRad = .035f;

	//kg
	private float mass = 2;
	private float g = 9.81f;
	private float gradAngle = 0;

	private float rollResist = .015f;

	private float airDensity = 1.21f;
	private float dragCoef = 1.2f;
	private float areaCross = .004f;

	private float lW = .125f;

	private float maxTurnAngle = Mathf.PI /6;

	private Matrix4x4 posMatrix = new Matrix4x4 ();
	private Matrix4x4 velMatrix = new Matrix4x4 ();
	private Vector4 temp = new Vector4();

	private Vector3 temp3 = new Vector3();
	private Quaternion tempQ = new Quaternion();

	private CarController carController = new CarController();


	//movement variable descriptions

	//force from motor
	//	maxTourque (scaling for forward input)
	//	Effeciency (assume 1)
	//	Gear Ratio (1 - 150 some int)
	//	Angular speed (rpms)
	//	Wheel Radius

	//	uphill vs downhill (1 for uphill -1 for downhill)
	//	m =mass
	//	g = 9.8
	//	Gradient Angle (Gradient alpha) (angle of slope - 0)
	//	
		//rolling resistance
	//	rolling resistance coeffecient (fr)
	//	wieight
	//	cos(alpha) slope
	//	
		//drag force
	//	density of air (p=1.21 kg/m^3)
	//	drag coeffecient (cd=1.2)
	//	Area (cross sectional = .004 m^2)
	//	Velocity vector (y) squared magnitude
	//  Lw = car geoemetry = 0.15



	//mass m - ydot squared = velocity squared in forward direction
	//tan(phi) phi is max turning angle (30 degrees) * turning input to get current phi


    // Use this for initialization
    void Start()
    {
		input = GetComponent<GetInput>();
		position.Set (50, 50, 0);
		motorOutForce = (mTor * effec * gearRatio * angularSpeed/6)/wheelRad;
		resistForce = 0;
		rollingResistForce = rollResist * mass * g * Mathf.Cos (gradAngle);
		rollingResistForce = 0;
		airResistForce = .5f * airDensity * dragCoef * areaCross;
		centripetalForce = mass / lW;

		if (isAI) {
			Kart k = GetComponent<Kart> ();
			k.SetUpKart ();
			MAX_SPEED = MAX_SPEED * .95f;
		}
    }


    void Update()
    {
		if (isAI) {
			Tuple<float, float> t = carController.speedAndTurn (this.gameObject);
			//Debug.Log ("Receiving data: " + t.First + "  " + t.Second);
			turnInput = (float)t.Second;
			forwardInput = (float)t.First;
		} else {
			turnInput = input.getTurnInput ();
			forwardInput = input.getForwardInput ();
		}

        //how quickly do we want to turn
        currentDeltaTurn = DELTA_TURN * turnInput;
        //takes care of ideling to go back straight && also turning

		//Debug.Log ("(turnValue + currentDeltaTurn): "+(turnValue + currentDeltaTurn)+"  MAXTU: "+(MAX_TURN)+"  speed: "+speed);
		if (Mathf.Abs(turnInput) <.1 && turnValue > 0)
        {
			turnValue = 0;
           // Debug.Log("Idle Pos");
            if (turnValue < DELTA_TURN)
                turnValue = 0;
            else 
                turnValue -= DELTA_TURN;
        }
		else if (Mathf.Abs(turnInput) <.1  && turnValue < 0)
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
        //Debug.Log("DeltaTurn: " + currentDeltaTurn);
        //Debug.Log("turnValue: " + turnValue);

        //Debug.Log("Vert: "+Input.GetAxis(vertical));
        //which way do we want to go


        //takes care of ACCELeration
		speed+= ACCEL *forwardInput;
		if (speed < -MAX_SPEED) {
			speed = -MAX_SPEED;
		}
		if (speed > MAX_SPEED) {
			speed = MAX_SPEED;
		}

		if (forwardInput == 1 && speed < MAX_SPEED) {
			speed += ACCEL;
		} else if (forwardInput == -1 && speed > -MAX_SPEED) {
			speed -= ACCEL;
		} else if (forwardInput == 0 && speed < 0) {
			if (speed > -IDLE_ACCEL)
				speed = 0;
			else
				speed += IDLE_ACCEL;
		} else if (forwardInput == 0 && speed > 0) {
			if (speed < IDLE_ACCEL)
				speed = 0;
			else
				speed -= IDLE_ACCEL;
		} else if (Mathf.Abs (speed) > MAX_SPEED) {
			speed = Mathf.Sign (speed) * MAX_SPEED;
		}

		//applies boost, if any
		speed = speed * boost;

        //calculates the direction displacement vector
		rotationVector.Set(0, turnValue * speed * Time.deltaTime, 0);
        direction = Quaternion.Euler(rotationVector) * direction;

        //Debug.Log ("Direction: "+ direction);
        //Debug.Log ("Speed: " +speed);
    }

	void equations(){
		float F1 = motorOutForce * forwardInput;
		float F2 = resistForce;
		float F3 = rollingResistForce;
		float F4 = -1*Mathf.Sign(velocity.y)*airResistForce * (velocity.y * velocity.y);
		float F5 = centripetalForce * (velocity.y*velocity.y)* Mathf.Tan (maxTurnAngle * turnInput);

		float xAccel = F5 / mass;
		float yAccel = (F1 + F2 + F3 + F4) / mass;
		float angleAccel = (6 * velocity.y * velocity.y * Mathf.Tan (maxTurnAngle * turnInput))/(height * height + width * width);
		Debug.Log ("F5: " + F5);
		Debug.Log ("velocity: " + velocity);
		velocity.x = velocity.x + xAccel * Time.deltaTime;
		velocity.y = velocity.y +yAccel * Time.deltaTime;



		yAngleOffsetVelocity = yAngleOffsetVelocity + (angleAccel * Time.deltaTime);
		moveVector.Set (velocity.x, 0, velocity.y);
		Debug.Log ("transform forward: " + transform.forward);
		float forwardV = Vector3.Dot (moveVector, transform.forward);
		float perpV = Vector3.Dot (moveVector, transform.right);
		velocity.Set (perpV, forwardV, 0);		
		position.x = position.x + (forwardV * Time.deltaTime) + (.5f * xAccel * Time.deltaTime * Time.deltaTime);
		position.y = position.y + (perpV * Time.deltaTime) + (.5f * yAccel * Time.deltaTime * Time.deltaTime);
		yAngleOffset = yAngleOffset + (yAngleOffsetVelocity * Time.deltaTime) + (.5f * angleAccel * Time.deltaTime * Time.deltaTime);
		yAngleOffset %= 360;
		temp3.Set (position.x, position.z, position.y);
		transform.localPosition = temp3;
		//transform.position = temp3;


		temp3.Set (0, yAngleOffset,0);
		//Debug.Log (temp3);
		transform.localEulerAngles = temp3;
	}

//	void updateLogVectors(){
//		//turn input [-1,1], forward input [-1,1]
//
//		float F1 = forwardInput * (mTor * effec * gearRatio * angularSpeed/60) / wheelRad;
//		Debug.Log ("F1: " + F1);
//		float F2 = mass * g * Mathf.Sin (gradAngle);
//		Debug.Log ("F2: " + F2);
//		float F3 = Mathf.Sign(F1) * rollResist * mass * g * Mathf.Cos (gradAngle) ;
//		Debug.Log ("F3: " + F3);
//		float F4 = .5f * airDensity * dragCoef * areaCross * velocity.y*velocity.y;
//		Debug.Log ("F4: " + F4);
//
//		float accelY = (F1 - F2 - F3 - F4) / mass;
//
//		float accelX = (velocity.y* velocity.y * Mathf.Tan (turnInput * maxTurnAngle))/(lW*mass);
//
////		temp.Set (transform.position.y, velocity.y, posY, 0);
////		posMatrix.SetRow (0, temp);
////
////		temp.Set (transform.position.x, velocity.x, posX, 0);
////		posMatrix.SetRow (1, temp);
////
////		temp.Set (0, 0, 0, 0);
////		posMatrix.SetRow (2, temp);
////
////		temp.Set (0, 0, 0, 1);
////		posMatrix.SetRow (3, temp);
//
//		//temp.Set (1, Time.deltaTime, .5f * Time.deltaTime * Time.deltaTime, 0);
//		//position = posMatrix * temp;
//		position.y = position.y + velocity.y *Time.deltaTime + accelY * Time.deltaTime * Time.deltaTime * .5f;
//		position.x = position.x + velocity.x * Time.deltaTime + accelX * Time.deltaTime * Time.deltaTime * .5f;
//
//		Debug.Log (velocity);
//
//		velocity.x = velocity.x + Time.deltaTime * accelX;
//		velocity.y = velocity.y + Time.deltaTime * accelY;
//
//		temp3.Set (position.x, position.z, position.y);
//		transform.position = temp3;
//
//		//forces to look at displacement vector in order to rotate
////		temp3.Set (position.x - oldPosition.x,position.z - oldPosition.z,position.y - oldPosition.y);
////		transform.rotation = Quaternion.LookRotation(temp3);
////		oldPosition = position;
//
//
//
//
//	}

    //Update transform position and rotation - Write to file
    void FixedUpdate()
    {
       // update the transform
			transform.position += direction* (speed * Time.deltaTime);
        if (!goingBackwards)
            transform.rotation = Quaternion.LookRotation(direction);
        else
            transform.rotation = Quaternion.LookRotation(-1 * direction);

		//equations ();
		//updateLogVectors ();
		if(writePositionsToFile)
        	WriteToRepo ();
    }

    void WriteToRepo()
    {
        int id = 1;
        var sw = new StreamWriter(Application.dataPath + "/Scripts/WriteToFile.txt", true);
		sw.Write(string.Format("{0}\t{1}\t{2}\n",transform.position.x, transform.position.y, transform.position.z));
        sw.Close();
    }
}
