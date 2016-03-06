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
        Vector3 dir = transform.forward.normalized;

        //pitch
        if (Input.GetKey(KeyCode.W))
            rot -= transform.right * TURN_DELTA;
        if (Input.GetKey(KeyCode.S))
            rot += transform.right * TURN_DELTA;

        //yaw
        if (Input.GetKey(KeyCode.A))
            rot -= transform.up * TURN_DELTA;
        if (Input.GetKey(KeyCode.D))
            rot += transform.up * TURN_DELTA;

        //roll
        if (Input.GetKey(KeyCode.E))
            rot -= transform.forward * TURN_DELTA;
        if (Input.GetKey(KeyCode.Q))
            rot += transform.forward * TURN_DELTA;

        rb.AddTorque(rot);

        //thrust
        if (Input.GetKey(KeyCode.DownArrow))
            dir *= -SPEED_DELTA;
        else if (Input.GetKey(KeyCode.UpArrow))
            dir *= SPEED_DELTA;
        else return;

        rb.AddForce(dir);
    }
}
