using UnityEngine;
using System.Collections;

public class Banana : MonoBehaviour {
	public GameObject target;
	private bool fired = false;
	private bool isColliderEnabled = false;

	// Use this for initialization
	void Start () {		
	}

	public void Fire() {
		fired = true;
		isColliderEnabled = true;
		gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		Vector3 kartDir = -1f*transform.forward;
		Vector3 spawnPos = transform.position + kartDir * 0.3f;
		transform.position = spawnPos;
		isColliderEnabled = true;
		//GetComponent<BoxCollider> ().enabled = true;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("e") && !fired) {
			Fire ();
		}

		if(target!=null && !fired){
			//gameObject.GetComponent<BoxCollider> ().enabled = false;
			Vector3 kartDir = -1f*target.transform.forward;
			Vector3 followPos = target.transform.position + kartDir * 1.8f;
			transform.position = followPos;
			transform.LookAt(target.transform);
		}
	}


	void OnCollisionEnter (Collision collision) {
		if (isColliderEnabled) {
			if (collision.gameObject.tag == "kart" ) {
				collision.gameObject.GetComponent<PowerUp> ().powerUp = "Fake";
				collision.gameObject.GetComponent<PowerUp> ().Activate ();
				Destroy (gameObject);
			} else {
				//Destroy Object Maybe
			}
		}
	}
}
