using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {
	public GameObject target;
	private bool fired = false;
	public float speed = 20;
	private Vector3 oldVelocity;
	private float time = 0;
	private bool isColliderEnabled = false;

	// Use this for initialization
	void Start () {		
	}

	public void Fire() {
		fired = true;
		isColliderEnabled = true;
		Vector3 kartDir = 1f*transform.forward;
		Vector3 spawnPos = transform.position + kartDir * 3.5f;
		transform.position = spawnPos;
		GetComponent<Rigidbody>().velocity = transform.TransformDirection( new Vector3( 0, 0, speed ) );
		//GetComponent<BoxCollider> ().enabled = true;
	}
		
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("e")) {
			Fire ();
		}

		if (fired) {
			time += Time.deltaTime;
			if (time > 5f) {
				Destroy (gameObject);
				time = 0;
			}
			GetComponent<Rigidbody>().velocity = speed * (GetComponent<Rigidbody>().velocity.normalized);

			print (GetComponent<Rigidbody> ().velocity);
			//GetComponent<Rigidbody>().AddForce(transform.forward * 20);
		} else if(target!=null){
			Vector3 kartDir = -1f*target.transform.forward;
			Vector3 followPos = target.transform.position + kartDir * 1.5f;
			transform.position = Vector3.MoveTowards(transform.position, followPos, 100f);
			transform.LookAt(target.transform);
		}
	}
		
	void FixedUpdate () {
		// because we want the velocity after physics, we put this in fixed update
		oldVelocity = GetComponent<Rigidbody>().velocity;
	}

	void OnCollisionEnter (Collision collision) {
		if (isColliderEnabled) {
			if (collision.gameObject.tag == "kart") {
				collision.gameObject.GetComponent<PowerUp> ().powerUp = "Fake";
				collision.gameObject.GetComponent<PowerUp> ().Activate ();
				Destroy (gameObject);
			} else {
				// get the point of contact
				ContactPoint contact = collision.contacts[0];

				// reflect our old velocity off the contact point's normal vector
				Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);        

				// assign the reflected velocity back to the rigidbody
				print(reflectedVelocity);
				GetComponent<Rigidbody>().velocity = reflectedVelocity;
			}
		}
	}
}
