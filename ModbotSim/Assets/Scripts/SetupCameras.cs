using UnityEngine;
using System.Collections;

public class SetupCameras : MonoBehaviour {
	public int numPlayers = 2;
	// Use this for initialization
	void Start () {
		GameObject[] cameraObjects = GameObject.FindGameObjectsWithTag ("Camera");
		GameObject[] karts = GameObject.FindGameObjectsWithTag ("kart");
		Camera[] cameras = new Camera[4];
		for (int i = 0; i < 4; i++) {
			cameras [i] = karts [i].GetComponentInChildren<Camera> ();
		}
		if (numPlayers == 1) {
			cameras [0].rect = new Rect (0f, 0f, 1f, 1f);
		} else if (numPlayers == 2) {
			cameras [0].rect = new Rect (0f, .5f, .7f, .5f);
			cameras [2].rect = new Rect (.3f, 0f, .7f, .5f);
			karts [1].SetActive (false);
			karts [3].SetActive (false);
		} else if (numPlayers == 3) {
			cameras [0].rect = new Rect (0f, .5f, 1f, .5f);
			cameras [1].rect = new Rect (0f, 0f, .5f, .5f);
			cameras [2].rect = new Rect (.5f, 0f, .5f, .5f);
		} else if (numPlayers == 4) {
			cameras [0].rect = new Rect (0f, 0f, .5f, .5f);
			cameras [1].rect = new Rect (.5f, 0f, .5f, .5f);
			cameras [2].rect = new Rect (0f, .5f, .5f, .5f);
			cameras [3].rect = new Rect (.5f, .5f, .5f, .5f);
		}


		//for (int i = numPlayers; i < 4; i++) {
		//	karts [i].SetActive(false);
		//}


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
