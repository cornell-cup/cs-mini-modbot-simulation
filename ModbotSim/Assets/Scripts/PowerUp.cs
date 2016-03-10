using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
	private string powerUp = "";

	[HideInInspector]
	public bool isActive = false;
	[HideInInspector]
	public float time = 0;

	public void Deactivate() { 
		isActive = false;
		powerUp = "";
		time = 0;
	}

	public void Activate() {
		isActive = true;
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("e")) {
			print ("use key was pressed");
			Activate ();
		}

		if (isActive) {
			if (powerUp == "Boost") {
				print ("Speed Up");
				if (GetComponent<Movement> ().MAX_SPEED == 40f / 4.5f) {
					GetComponent<Movement> ().MAX_SPEED = 40f / 6f;
				}
				GetComponent<Movement> ().boost += 0.2f;
				time += Time.deltaTime;
				if (time > 1.5f) {
					GetComponent<Movement> ().boost = 1f;
					GetComponent<Movement> ().MAX_SPEED = 40f / 4.5f;
					Deactivate ();
				}
			}

			if (powerUp == "Fake") {
				print ("Fake Item");
				GetComponent<Movement> ().MAX_SPEED = GetComponent<Movement> ().MAX_SPEED * 0.95f;
				time += Time.deltaTime;
				if (time > 2f) {
					GetComponent<Movement> ().MAX_SPEED = 40f / 4.5f;
					Deactivate ();
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.transform.tag == "Speed Boost") {
			powerUp = "Boost";
			Destroy (other.gameObject);
		}

		if (other.transform.tag == "Fake Item") {
			powerUp = "Fake";
			Activate ();
			Destroy (other.gameObject);
		}
	}

}

