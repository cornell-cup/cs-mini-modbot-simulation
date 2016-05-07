using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class ObstacleAvoid : MonoBehaviour {
	
	public float angleRange =  90f;
	//public bool avoidObstacle;
	//public float angleObstacle;
	//public Vector3 obstaclePosition;
	public bool leftObs;
	public bool rightObs;
	public bool centerObs;
	public float LeftDis;
	public float CenterDis;
	public float RightDis; 
	public float carRadiusApprox = 1;

	private SphereCollider col;

	void Awake () {
		col = GetComponent<SphereCollider>();
	}

	void Update () {
		//print (avoidObstacle);
	}

	void OnTriggerStay(Collider other) {
		// make sure front wheels are first two components of the car 
		Vector3 localVector = new Vector3 (0, 0, 4);
		Vector3 frontposition = transform.TransformPoint (localVector);

		RaycastHit hitleft;
		RaycastHit hitmiddle; 
		RaycastHit hitright; 
		Vector3 leftBeam = 10 * transform.TransformDirection (-0.3f, 0, 0.5f);
		Vector3 rightBeam = 10 * transform.TransformDirection (0.3f, 0, 0.5f); 
		Debug.DrawRay (frontposition, leftBeam, Color.red, 0.03f);
		Debug.DrawRay (frontposition, 10 * transform.forward, Color.green, 0.03f);	
		Debug.DrawRay (frontposition, rightBeam, Color.blue, 0.03f); 
		if (Physics.SphereCast (frontposition, carRadiusApprox, transform.forward, out hitmiddle, col.radius)) {
			if (isObstacle(hitmiddle.collider.gameObject.tag)) {
				centerObs = true; 
				CenterDis = hitmiddle.distance;
				Debug.Log ("MIDDLE ONE SAW YA " + hitmiddle.collider.gameObject.tag);
			} else {
				centerObs = false; 
				CenterDis = 0;
			}
		} else {
			centerObs = false;
			CenterDis = 0;
		}
		if (Physics.SphereCast (frontposition, carRadiusApprox, leftBeam, out hitleft, col.radius)) {
			if (isObstacle(hitleft.collider.gameObject.tag)) {
				leftObs = true; 
				LeftDis = hitleft.distance;
				Debug.Log ("LEFT ONE SAW YA" + hitleft.collider.gameObject.tag);
			} else {
				leftObs = false; 
				LeftDis = 0;
			}
		} else {
			leftObs = false; 
			LeftDis = 0;
		}
		if (Physics.SphereCast (frontposition, carRadiusApprox, rightBeam, out hitright, col.radius)) {
			if (isObstacle(hitright.collider.gameObject.tag)) {
				rightObs = true; 
				RightDis = hitright.distance;
				Debug.Log ("RIGHT ONE SAW YA" + hitright.collider.gameObject.tag);
			} else {
				rightObs = false; 
				RightDis = 0;
			}
		} else {
			rightObs = false; 
			RightDis = 0;
		}
	}

	private bool isObstacle(string gameObjectTag) {
		return !(gameObjectTag == "non-obstacle" || gameObjectTag == "Green Shell" || gameObjectTag == "Boost" ||
			gameObjectTag == "Banana");
	}


}