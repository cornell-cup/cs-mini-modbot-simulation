using UnityEngine;
using System;
using System.Collections.Generic;

// <summary>
// The PathPlanningKart class contains the state and operations necessary for the kart to 
// perform path planning
// </summary>
public class PathPlanningKart : MonoBehaviour 
{
	//list of waypoints of the current path segment
	public List<Vector3> currentWayPoints;
	//list of waypoints of the next path segment
	public List<Vector3> nextWayPoints;
	//the current waypoint
	public int current_waypoint;
	//represents the start node in the map
	public Node startNode;
	//represents the current thread job calculating the next path segment
	public DynamicPathThreadJob currentThreadJob; 
	//indicates whether or not the next path segment has been calculated yet
	public bool pathCalculated;
	//indicates whether a thread job is in progress or not
	public bool jobInProgress;
	//Marks already visited nodes in the A* traversal
	public HashSet<Node> closedNodes = new HashSet<Node> ();


	// <summary>
	// Performs path planning for the first path segment by utilizing a DynamicPathThreadJob
	// and waiting for it to complete
	// </summary>
	public void PathPlanInitialSegment () {
		//first triggered thread job for this car
		Debug.Log("Starting path planning for initial segment");
		startNode = PathPlanningDataStructures.graph.getClosestNode (transform.position);
		currentThreadJob = new DynamicPathThreadJob (startNode, PathPlanningDataStructures.graph.endNode, closedNodes);
		currentThreadJob.Start();
		currentThreadJob.Join();
		currentWayPoints = currentThreadJob.getPathWayPoints();
		closedNodes = currentThreadJob.getClosedNodes ();
		//indicate that next path segment needs to calculated
		pathCalculated = false;
		jobInProgress = false;
	}

	// <summary>
	// Performs path planning for the next path segment by utilizing a DynamicPathThreadJob
	// that calculates the path in the background
	// </summary>
	public void PathPlanNextSegment () {
		//Check if the next path segment needs to be calculated in a thread
		if (pathCalculated == false && jobInProgress == false) {
			//trigger thread job for this car to obtain the next set of waypoints
			Node pathStartNode;
			if (currentThreadJob.destinationNode == PathPlanningDataStructures.graph.endNode) {
				pathStartNode = startNode;
			} else {
				pathStartNode = currentThreadJob.destinationNode;
			}
			currentThreadJob = new DynamicPathThreadJob(pathStartNode, PathPlanningDataStructures.graph.endNode, closedNodes);
			currentThreadJob.Start();
			jobInProgress = true;
		}
		//Check if in progress thread has completed the path calculation
		if (jobInProgress) {
			if (currentThreadJob.isFinished()) {
				nextWayPoints = currentThreadJob.getPathWayPoints();
				closedNodes = currentThreadJob.getClosedNodes ();
				pathCalculated = true;
				jobInProgress = false;
				Debug.Log ("Finished next thread job. Size: " + nextWayPoints.Count);
			}
		}
	}
}