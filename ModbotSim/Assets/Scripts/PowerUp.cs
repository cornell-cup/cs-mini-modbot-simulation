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
	public GameObject itemObject;
    private GetInput input;

	// Use this for initialization
	void Start () {
        input = GetComponent<GetInput>();
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
        Debug.Log("Your Powerup: " + powerUp);
		if (isActive) {
			if (powerUp == "Boost") {
				time += Time.deltaTime;
				if (time > 2f) {
					GetComponent<Movement> ().boost = 1f;
					GetComponent<Movement> ().MAX_SPEED = 15;
					Deactivate ();
				}
			}

			if (powerUp == "Fake") {
                GetComponent<Movement>().studder = 70;
                time += Time.deltaTime;
				if (time > 1f) {
                    GetComponent<Movement>().MAX_SPEED = 15;
                    GetComponent<Movement>().studder = 0;
                    Deactivate();
				}
			}

			if (powerUp == "Banana" || powerUp == "Green Shell") {
				if (input.isFiring()) {
					Deactivate ();
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Boost" || other.transform.tag == "Fake Item" || other.transform.tag == "Banana" || other.transform.tag == "Green Shell") {
            if (powerUp == "")
            {
                if (other.transform.tag == "Boost")
                {
                    powerUp = "Boost";
                    GetComponent<Movement>().boost += 10.4f;
                    Activate();
                    Destroy(other.gameObject);
                }
                if (other.transform.tag == "Fake Item")
                {
                    powerUp = "Fake";
                    Activate();
                    Destroy(other.gameObject);
                }

                if (other.transform.tag == "Banana")
                {
                    Debug.Log("Picked up a Banana");
                    powerUp = "Banana";
                    Vector3 kartPos = transform.position;
                    Vector3 kartDir = -1f * transform.forward;
                    Vector3 spawnPos = transform.position + kartDir * 2.0f;
                    itemObject = Instantiate(banana, spawnPos, transform.rotation) as GameObject;
                    Material material = Resources.Load("Materials/orange-plastic", typeof(Material)) as Material;
                    itemObject.GetComponent<MeshRenderer>().material = material;
                    //currentShell.GetComponent<BoxCollider> ().enabled = false;
                    itemObject.AddComponent<Banana>();
                    itemObject.GetComponent<Banana>().target = gameObject;
                    Activate();
                    Destroy(other.gameObject);
                }

                if (other.transform.tag == "Green Shell")
                {
                    powerUp = "Green Shell";
                    Vector3 kartPos = transform.position;
                    Vector3 kartDir = -1f * transform.forward;
                    Vector3 spawnPos = transform.position + kartDir * 1.8f;
                    itemObject = Instantiate(shell, spawnPos, transform.rotation) as GameObject;
                    Material material = Resources.Load("Materials/orange-plastic", typeof(Material)) as Material;
                    itemObject.GetComponent<MeshRenderer>().material = material;
                    //currentShell.GetComponent<BoxCollider> ().enabled = false;
                    itemObject.AddComponent<Shell>();
                    itemObject.GetComponent<Shell>().target = gameObject;
                    Activate();
                    Destroy(other.gameObject);
                }
            }
        }
	}
}

