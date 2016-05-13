using UnityEngine;
using System.Collections;

public class SetupCameras : MonoBehaviour {
	public int numPlayers = 1;
	// Use this for initialization
	void Start () {
		GameObject[] cameraObjects = GameObject.FindGameObjectsWithTag ("Camera");
		GameObject[] karts = GameObject.FindGameObjectsWithTag ("kart");
		Camera[] cameras = new Camera[4];
        cameras[0] = karts[3].GetComponentInChildren<Camera>();
        cameras[1] = karts[2].GetComponentInChildren<Camera>();
        cameras[2] = karts[1].GetComponentInChildren<Camera>();
        cameras[3] = karts[0].GetComponentInChildren<Camera>();

        for(int i=0; i<4; i++)
        {
            Debug.Log(karts[i].name);
        }

		if (numPlayers == 1) {
			cameras [0].rect = new Rect (0f, 0f, 1f, 1f);
            karts[2].SetActive(false);
            karts[1].SetActive(false);
            karts[0].SetActive(false);
        } else if (numPlayers == 2) {
			cameras [0].rect = new Rect (0f, .5f, .7f, .5f);
			cameras [1].rect = new Rect (.3f, 0f, .7f, .5f);
<<<<<<< HEAD
<<<<<<< HEAD

=======
//			karts [2].SetActive (false);
//			karts [3].SetActive (false);
>>>>>>> e94c9b82731d91bc4b38bc933a604e2a80d7ed72
=======
			karts [1].SetActive (false);
			karts [0].SetActive (false);
>>>>>>> 2fc66ecc298e40669e0b912287d9b92e6667045d
		} else if (numPlayers == 3) {
			cameras [0].rect = new Rect (0f, .5f, 1f, .5f);
			cameras [1].rect = new Rect (0f, 0f, .5f, .5f);
			cameras [2].rect = new Rect (.5f, 0f, .5f, .5f);
            karts[0].SetActive(false);
        } else if (numPlayers == 4) {
			cameras [0].rect = new Rect (0f, 0f, .5f, .5f);
			cameras [1].rect = new Rect (.5f, 0f, .5f, .5f);
			cameras [2].rect = new Rect (0f, .5f, .5f, .5f);
			cameras [3].rect = new Rect (.5f, .5f, .5f, .5f);
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
