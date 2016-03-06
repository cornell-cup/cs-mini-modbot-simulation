using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    
    public float TURN_DELTA = 0.2f;
    public float SPEED_DELTA = 0.5f;
    public float MAX_TURN = 1.0f;
    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = MAX_TURN;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = new Vector3();
        Vector3 dir = new Vector3();

        Vector3 x = transform.right.normalized;
        Vector3 y = transform.up.normalized;
        Vector3 z = transform.forward.normalized;

        //pitch
        if (Input.GetKey(KeyCode.W))
            rot -= x;
        if (Input.GetKey(KeyCode.S))
            rot += x;

        //yaw
        if (Input.GetKey(KeyCode.A))
            rot -= y;
        if (Input.GetKey(KeyCode.D))
            rot += y;

        //roll
        if (Input.GetKey(KeyCode.E))
            rot -= z;
        if (Input.GetKey(KeyCode.Q))
            rot += z;

        rb.AddTorque(rot*TURN_DELTA);

        //sway   
        if (Input.GetKey(KeyCode.LeftArrow))
            dir -= x;
        else if (Input.GetKey(KeyCode.RightArrow))
            dir += x;

        //heave
        if (Input.GetKey(KeyCode.UpArrow))
            dir -= y;
        else if (Input.GetKey(KeyCode.DownArrow))
            dir += y;

        //surge
        if (Input.GetKey(KeyCode.Comma))
            dir -= z;
        else if (Input.GetKey(KeyCode.Period))
            dir += z;
        
        rb.AddForce(dir*SPEED_DELTA);
    }
}
