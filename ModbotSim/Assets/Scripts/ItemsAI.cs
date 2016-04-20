using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ItemsAI is a class that contains the essential data structures and functions necessary
// for implementing item retrieval and detection for an AI

public class ItemsAI
{
	//represents all the speed boost objects
	public static GameObject[] speedBoosts;
	//represents all the green shell objects
	public static GameObject[] greenShells;
	//represents a mapping from each item game object to its associated Bounds object
	public static Dictionary<GameObject, Bounds> objectToBounds;
	//represents a mapping from each item game object to its position
	public static Dictionary<GameObject, Vector3> objectToPosition;

	public static void intializeItems () {
		speedBoosts = GameObject.FindGameObjectsWithTag("Speed Boost");
		greenShells = GameObject.FindGameObjectsWithTag ("Green Shell");
		objectToBounds = new Dictionary<GameObject, Bounds>();
		objectToPosition = new Dictionary<GameObject, Vector3> ();
		foreach (GameObject item in speedBoosts) {
			objectToBounds.Add (item, item.GetComponent<BoxCollider> ().bounds);
			objectToPosition.Add (item, item.transform.position);
		}
		foreach (GameObject item in greenShells) {
			objectToBounds.Add (item, item.GetComponent<BoxCollider> ().bounds);
			objectToPosition.Add (item, item.transform.position);
		}
	}

	public static float getReduction(Node n) {
		float reduction = 1;
		GameObject[] itemList = new GameObject[speedBoosts.Length + greenShells.Length];
		speedBoosts.CopyTo(itemList, 0);
		greenShells.CopyTo(itemList, speedBoosts.Length);
		foreach (GameObject item in itemList) {
			if (Vector3.Distance(objectToPosition [item], n.position) <= 5) {
				Debug.Log ("Node contains item!!!!");
				Vector3 itemPosition = objectToPosition [item];
				reduction = 20;
			}
		}
		return reduction;
	}


}


