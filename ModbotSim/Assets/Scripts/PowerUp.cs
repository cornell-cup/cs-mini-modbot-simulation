using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUp : MonoBehaviour {
	[HideInInspector]
	public string powerUp = "";

	[HideInInspector]
	public bool isActive = false;
	[HideInInspector]
	private float time = 0;

	public GameObject shell; 
	public GameObject banana;
	private GameObject itemObject; 

	// Use this for initialization
	void Start () {
	}

	public void Deactivate() { 
		isActive = false;
		powerUp = "";
		time = 0;
	}

	public void Activate() {
		isActive = true;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("e")) {
			print ("use key was pressed");
			Activate ();
		}

		if (isActive) {
			if (powerUp == "Boost") {
				if (GetComponent<Movement> ().MAX_SPEED == 40f / 4.5f) {
					GetComponent<Movement> ().MAX_SPEED = 40f / 9f;
				}
				GetComponent<Movement> ().boost += 0.4f;
				time += Time.deltaTime;
				if (time > 2f) {
					GetComponent<Movement> ().boost = 1f;
					GetComponent<Movement> ().MAX_SPEED = 40f / 4.5f;
					Deactivate ();
				}
			}

			if (powerUp == "Fake") {
				GetComponent<Movement> ().MAX_SPEED = GetComponent<Movement> ().MAX_SPEED * 0.95f;
				time += Time.deltaTime;
				if (time > 2f) {
					GetComponent<Movement> ().MAX_SPEED = 40f / 4.5f;
					Deactivate ();
				}
			}

			if (powerUp == "Banana" || powerUp == "Green Shell") {
				if (Input.GetKeyDown ("e")) {
					Deactivate ();
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (!GetComponent<Movement> ().isArtificialIntelligence ()) {
			if (powerUp == "") {
				if (other.transform.tag == "Boost") {
					powerUp = "Boost";
					Destroy (other.gameObject);
				}

				if (other.transform.tag == "Fake Item") {
					powerUp = "Fake";
					Activate ();
					Destroy (other.gameObject);
				}

				if (other.transform.tag == "Banana") {
					powerUp = "Banana";
					Vector3 kartPos = transform.position;
					Vector3 kartDir = -1f * transform.forward;
					Vector3 spawnPos = transform.position + kartDir * 2.0f;
					itemObject = Instantiate (banana, spawnPos, transform.rotation) as GameObject;
					Material material = Resources.Load ("Materials/orange-plastic", typeof(Material)) as Material;
					itemObject.GetComponent<MeshRenderer> ().material = material;
					//currentShell.GetComponent<BoxCollider> ().enabled = false;
					itemObject.AddComponent<Banana> ();
					itemObject.GetComponent<Banana> ().target = gameObject;
					Activate ();
					Destroy (other.gameObject);
				}

				if (other.transform.tag == "Green Shell") {
					powerUp = "Green Shell";
					Vector3 kartPos = transform.position;
					Vector3 kartDir = -1f * transform.forward;
					Vector3 spawnPos = transform.position + kartDir * 1.8f;
					itemObject = Instantiate (shell, spawnPos, transform.rotation) as GameObject;
					Material material = Resources.Load ("Materials/orange-plastic", typeof(Material)) as Material;
					itemObject.GetComponent<MeshRenderer> ().material = material;
					//currentShell.GetComponent<BoxCollider> ().enabled = false;
					itemObject.AddComponent<Shell> ();
					itemObject.GetComponent<Shell> ().target = gameObject;
					Activate ();
					Destroy (other.gameObject);
				}
			} else {
				Destroy (other.gameObject);
			}
		} else {
			if (other.transform.tag == "Boost") {
				powerUp = "Boost";
				Destroy (other.gameObject);
			}
		}
		
	}

}

