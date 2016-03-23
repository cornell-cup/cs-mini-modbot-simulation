using UnityEngine;
using System;
using System.Collections.Generic;

public class Kart : MonoBehaviour 
{
	public List<Vector3> wayPoints; //list of waypoints for the car
	public int current_point; //the current waypoint
	PathPlanning pathPlanner; // Represents the path planning object


	public void SetUpKart () {
		//Set initial waypoint to 0
		current_point = 0;
		//Perform path planning
		GameObject pathPlanGameObject = GameObject.FindGameObjectWithTag("navmeshcontainer");
		pathPlanner = pathPlanGameObject.GetComponent<PathPlanning>();
		pathPlanner.SetUpPathPlanning (transform.position);
		wayPoints = new List<Vector3> ();
		int i = 0;
		foreach (GenerateGraph.Node pathNode in pathPlanner.path) { 
				wayPoints.Add (pathNode.point);
			Debug.Log ("Waypoint " + i + ": " + pathNode.point);
			i++;
		}
	}

}