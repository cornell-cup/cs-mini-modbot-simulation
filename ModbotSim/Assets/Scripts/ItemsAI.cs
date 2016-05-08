using UnityEngine;
using System;
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

	public static object itemsUpdateLock = new object();
	public static Stopwatch stopWatch = new Stopwatch();

	// <summary>
	// Initializes/Updates items by retrieving and storing them in a list; also stores a mapping
	// of each item to its position
	// </summary>
	public static void updateItems () {
		lock (itemsUpdateLock) {
			if (stopWatch.IsRunning == false || stopWatch.ElapsedMilliseconds >= 1000) {
				GameObject[] boosts = GameObject.FindGameObjectsWithTag ("Boost");
				GameObject[] greenShells = GameObject.FindGameObjectsWithTag ("Green Shell");
				GameObject[] bananas = GameObject.FindGameObjectsWithTag ("Banana");
				itemList = new GameObject[boosts.Length + greenShells.Length + bananas.Length];
				boosts.CopyTo (itemList, 0);
				greenShells.CopyTo (itemList, boosts.Length);
				bananas.CopyTo (itemList, boosts.Length + greenShells.Length);
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
			float reduction = 1.0f;
			foreach (GameObject item in itemList) {
				float itemDistance = Vector3.Distance (objectToPosition [item], n.position);
				if (itemDistance <= 5.0f) {
					Vector3 itemPosition = objectToPosition [item];
					reduction = Math.Min(5.0f / itemDistance, 1.5f);
				}
			}
			return reduction;
		}
	}


}


