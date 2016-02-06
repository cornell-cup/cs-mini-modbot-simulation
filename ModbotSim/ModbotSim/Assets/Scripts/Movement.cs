using UnityEngine;
using System.Collections;
using System.IO;

public class Movement : MonoBehaviour
{
    [System.NonSerialized]
    public bool usingPhone = false;
    [System.NonSerialized]
    public static float ACCEL = .002f;
    [System.NonSerialized]
    public static float DELTA_TURN = .1f;
    [System.NonSerialized]
    public static float MAX_TURN = 0.5f;
    [HideInInspector]
    [System.NonSerialized]
    public float MAX_SPEED = .1f;

    private float currentDeltaTurn = 0f;
    private float speed = 0f;
    private float turnValue = 0f;
    private float turnInput = 0f;
    private Vector3 direction = Vector3.forward;
    private Vector3 rotationVector;
    private bool goingBackwards = false;
    private int forwardInput = 0;

    private string horizontal;
    private string vertical;

    // Use this for initialization
    void Start()
    {
        //this.
        //var sw = new StreamWriter(Application.dataPath + "/Scripts/WriteToFile.txt", false);
        //sw.Write(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t\n")); 
        //print("initializing " + this);
        string[] name = this.ToString().Split(' ');
        if (name[0] != "Kart")
        {
            throw new System.Exception("Movement script attached to non Kart object");
        }
        switch (name[1])
        {
            case "1":
                horizontal = "Horizontal";
                vertical = "Vertical";
                break;
            case "2":
                horizontal = "Horizontal2";
                vertical = "Vertical2";
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


    void Update()
    {
        Vector3 Udp = Vector3.zero;
        if (!usingPhone)
            turnInput = Input.GetAxis(horizontal);
        else
        {
            Udp = GetComponent<UDPReceive>().getLatestUDPPacket();
            turnInput = Udp.x;
            if (Mathf.Abs(turnInput) < .2)
                turnInput = 0;
        }

        //how quickly do we want to turn
        currentDeltaTurn = DELTA_TURN * turnInput;
        //takes care of ideling to go back straight && also turning
        if (turnInput == 0 && turnValue > 0)
        {
            Debug.Log("Idle Pos");
            if (turnValue < DELTA_TURN)
                turnValue = 0;
            else 
                turnValue -= DELTA_TURN;
        }
        else if (turnInput == 0 && turnValue < 0)
        {
            Debug.Log("Idle Neg");
            if (turnValue > -DELTA_TURN)
                turnValue = 0;
            else
                turnValue += DELTA_TURN;
        }
        else if ((turnValue + currentDeltaTurn) < (MAX_TURN * Mathf.Abs(turnInput)) && (turnValue + currentDeltaTurn) > (-MAX_TURN * Mathf.Abs(turnInput)))
        {
            Debug.Log("Turn");
            turnValue += currentDeltaTurn;
        }
        //Debug.Log("DeltaTurn: " + currentDeltaTurn);
        //Debug.Log("turnValue: " + turnValue);

        //Debug.Log("Vert: "+Input.GetAxis(vertical));
        //which way do we want to go
        if (!usingPhone)
        {
            if (Input.GetAxis(vertical) > 0)
                forwardInput = 1;
            else if (Input.GetAxis(vertical) < 0)
                forwardInput = -1;
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

        //takes care of ACCELeration
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
            if (speed > .5f * -ACCEL)
                speed = 0;
            else
                speed += .5f * ACCEL;
        }
        else if (forwardInput == 0 && speed > 0)
        {
            if (speed < .5f * ACCEL)
                speed = 0;
            else
                speed -= .5f * ACCEL;
        }

        //calculates the direction displacement vector
        rotationVector.Set(0, turnValue * speed * 25, 0);
        direction = Quaternion.Euler(rotationVector) * direction;

        //Debug.Log ("Direction: "+ direction);
        //Debug.Log ("Speed: " +speed);
    }

    //Update transform position and rotation - Write to file
    void FixedUpdate()
    {
        //update the transform
        transform.position += speed * direction;
        if (!goingBackwards)
            transform.rotation = Quaternion.LookRotation(direction);
        else
            transform.rotation = Quaternion.LookRotation(-1 * direction);
        //WriteToRepo ();
    }

    void WriteToRepo()
    {
        /* Vector3 pos = Vector3.zero;
         Quaternion rot = transform.rotation;

             pos = transform.position + speed * direction;
             rot = Quaternion.LookRotation (direction);

 */
        int id = 1;
        var sw = new StreamWriter(Application.dataPath + "/Scripts/WriteToFile.txt", true);
        //print (transform.rotation.eulerAngles.y);
        sw.Write(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t\n", id, transform.position.x, transform.position.y, transform.position.z, transform.rotation.eulerAngles.y));
        sw.Close();
        //Debug.Log("Wrote to files");     
    }
}
