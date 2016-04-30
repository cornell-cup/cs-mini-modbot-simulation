using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// ItemsAI is a class that contains the essential data structures and functions necessary
// for implementing item retrieval and detection for an AI

public class ItemsAI
{
	//represents all the items on the map
	public static GameObject[] itemList;
	//represents a mapping from each item game object to its associated Bounds object
	public static Dictionary<GameObject, Bounds> objectToBounds;
	//represents a mapping from each item game object to its position
	public static Dictionary<GameObject, Vector3> objectToPosition;

	public static Object itemsUpdateLock = new Object();
	public static Stopwatch stopWatch = new Stopwatch();

	// <summary>
	// Initializes/Updates items by retrieving and storing them in a list; also stores a mapping
	// of each item to its position
	// </summary>
	public static void updateItems () {
		lock (itemsUpdateLock) {
			if (stopWatch.IsRunning == false || stopWatch.ElapsedMilliseconds >= 1000) {
				GameObject[] speedBoosts = GameObject.FindGameObjectsWithTag ("Speed Boost");
				GameObject[] greenShells = GameObject.FindGameObjectsWithTag ("Green Shell");
				itemList = new GameObject[speedBoosts.Length + greenShells.Length];
				speedBoosts.CopyTo (itemList, 0);
				greenShells.CopyTo (itemList, speedBoosts.Length);
				objectToBounds = new Dictionary<GameObject, Bounds> ();
				objectToPosition = new Dictionary<GameObject, Vector3> ();
				foreach (GameObject item in itemList) {
					objectToBounds.Add (item, item.GetComponent<BoxCollider> ().bounds);
					objectToPosition.Add (item, item.transform.position);
				}
				stopWatch.Stop ();
				stopWatch.Start ();
			}
		}
	}

	// <summary>
	// Given a node object representing a possible waypoint, calculate a reduction for
	// the heuristic if there is an item nearby
	// </summary>
	// <param name="n"> a Node representing a possible waypoint</param>
	public static float getReduction(Node n) {
		lock (itemsUpdateLock) {
			float reduction = 1;
			foreach (GameObject item in itemList) {
				if (Vector3.Distance (objectToPosition [item], n.position) <= 5) {
					UnityEngine.Debug.Log ("Node contains item!!!!");
					Vector3 itemPosition = objectToPosition [item];
					reduction = 20;
					UnityEngine.Debug.Log ("Current item size: " + itemList.Length);
				}
			}
			return reduction;
		}
	}


}


