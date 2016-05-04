using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class itemImage : MonoBehaviour {
	public GameObject kart;
	public Sprite[] itemSprites;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		string powerUp = kart.GetComponent<PowerUp> ().powerUp;
		if (powerUp == "") {
			if (gameObject.GetComponent<Image> () != null) {
				Destroy (GetComponent<Image> ());
			}
		} else {
			if (gameObject.GetComponent<Image> () == null) {
				if (powerUp == "Boost") {
					gameObject.AddComponent<Image> ().sprite = itemSprites [0];
				}
				if (powerUp == "Green Shell") {
					gameObject.AddComponent<Image> ().sprite = itemSprites [1];
				}
				if (powerUp == "Banana") {
					gameObject.AddComponent<Image> ().sprite = itemSprites [2];
				}
			}
		}
	}
}
