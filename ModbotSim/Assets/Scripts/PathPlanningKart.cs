using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// <summary>
// The PathPlanningKart class contains the state and operations necessary for the kart to 
// perform path planning and related decision making.
// </summary>
public class PathPlanningKart : MonoBehaviour 
{
	//list of waypoints of the current path segment
	public List<Vector3> currentWayPoints;
	//list of waypoints of the next path segment
	public List<Vector3> nextWayPoints;
	//list of waypoints for savior
	public List<Vector3> dynamicWayPoints;
	//the current waypoint
	public int current_waypoint;
	//represents the start node in the map
	public Node startNode;
	//represents the current thread job calculating the next path segment
	public DynamicPathThreadJob currentThreadJob; 
	//indicates whether a thread job is in progress or not
	public bool jobInProgress;
	//Marks already visited nodes in the A* traversal
	public HashSet<Vector3> closedNodes = new HashSet<Vector3> ();
	//Current waypoint node in progress
	public Vector3 nodeInProgress;
	//used for dynamic replanning
	public bool dynamicReplan;
	//object for keeping track of elapsed time to ensure an item isn't fired off immediately
	public Stopwatch stopWatch = new Stopwatch();
	//indicates whether the kart is allowed to use the item yet
	public bool canUseItem = false;

	// <summary>
	// Performs path planning for the first path segment by utilizing a DynamicPathThreadJob
	// and waiting for it to complete
	// </summary>
	public void PathPlanInitialSegment () {
		//first triggered thread job for this car
		startNode = PathPlanningDataStructures.graph.getClosestNode (transform.position);
		currentThreadJob = new DynamicPathThreadJob (startNode, PathPlanningDataStructures.graph.endNode, closedNodes, 15.0f);
		currentThreadJob.Start();
		currentThreadJob.Join();
		currentWayPoints = currentThreadJob.getPathWayPoints();
		closedNodes = currentThreadJob.getClosedNodes ();
		//indicate that next path segment needs to calculated
		jobInProgress = false;
		dynamicReplan = false;
	}

	// <summary>
	// Performs path planning for the next path segment by utilizing a DynamicPathThreadJob
	// that calculates the path in the background
	// </summary>
	public void PathPlanNextSegment () {
		//Check if the next path segment needs to be calculated in a thread
		if (jobInProgress == false && nextWayPoints == null && dynamicReplan == false) {
			//trigger thread job for this car to obtain the next set of waypoints
			Node pathStartNode;
			if (currentThreadJob.destinationNode == PathPlanningDataStructures.graph.endNode) {
				pathStartNode = startNode;
			} else {
				pathStartNode = currentThreadJob.destinationNode;
			}
			currentThreadJob = new DynamicPathThreadJob (pathStartNode, PathPlanningDataStructures.graph.endNode, closedNodes, 12.0f);
			currentThreadJob.Start ();
			jobInProgress = true;
		} else if (jobInProgress == false && dynamicReplan) {
			Node pathStartNode;
			if (currentThreadJob.destinationNode == PathPlanningDataStructures.graph.endNode) {
				pathStartNode = startNode;
			} else {
				pathStartNode = PathPlanningDataStructures.graph.getClosestNode(transform.position + 3 * transform.forward);
			}
			currentThreadJob = new DynamicPathThreadJob(pathStartNode, PathPlanningDataStructures.graph.endNode, closedNodes, 5.0f);
			currentThreadJob.Start ();
			jobInProgress = true;
		}
		//Check if in progress thread has completed the path calculation
		if (jobInProgress) {
			if (currentThreadJob.isFinished()) {
				if (dynamicReplan) {
					dynamicWayPoints = currentThreadJob.getPathWayPoints ();
					nextWayPoints = null;
				} else {
					nextWayPoints = currentThreadJob.getPathWayPoints();
					dynamicWayPoints = null;
				}
				closedNodes = currentThreadJob.getClosedNodes ();
				jobInProgress = false;
			}
		}
	}

	// <summary>
	// Implements logic for the AI kart to an item if it currently has one
	// </summary>
	public void UseItem () {
		PowerUp currentPowerUp = GetComponent<PowerUp> ();
		if (currentPowerUp.itemObject != null) {
			if (stopWatch.IsRunning == false && canUseItem == false) {
				stopWatch.Start ();
			}
			if (stopWatch.ElapsedMilliseconds >= 1000 && canUseItem == false) {
				stopWatch = new Stopwatch ();
				canUseItem = true;
			} else if (canUseItem == false) {
				return;
			}
			if (currentPowerUp.powerUp == "Banana") {
				Banana currentBanana = currentPowerUp.itemObject.GetComponent<Banana> ();
				if (currentBanana.fired == false) {
					//Iterate through all existing karts and check if another kart is behind
					//this kart
					GameObject[] currentKarts = GameObject.FindGameObjectsWithTag ("kart");
					for (int i = 0; i < currentKarts.Length; i++) {
						if (currentKarts [i].activeInHierarchy && currentKarts [i] != gameObject) {
							Vector3 inversePoint = transform.InverseTransformPoint (currentKarts [i].transform.position);
							if (inversePoint.z < 0) {
								currentBanana.Fire ();
								currentPowerUp.Deactivate ();
								canUseItem = false;
								break;
							}
						}
					}
				}
			} else if (currentPowerUp.powerUp == "Green Shell") {
				Shell currentShell = currentPowerUp.itemObject.GetComponent<Shell> ();
				if (currentShell.fired == false) {
					//Iterate through all existing karts and check if another kart is in
					//front of this kart and somewhat inline with this kart's direction
					GameObject[] currentKarts = GameObject.FindGameObjectsWithTag ("kart");
					for (int i = 0; i < currentKarts.Length; i++) {
						if (currentKarts [i].activeInHierarchy && currentKarts [i] != gameObject) {
							Vector3 inversePoint = transform.InverseTransformPoint (currentKarts [i].transform.position);
							if (inversePoint.z > 0) {
								Vector3 targetDirection = currentKarts[i].transform.position - transform.position; 
								Vector3 forwardDirection = transform.forward; 
								float angle = Vector3.Angle(targetDirection, forwardDirection); 
								if (angle < 10.0F) {
									currentShell.Fire();
									currentPowerUp.Deactivate ();
									canUseItem = false;
									break;
								}
							}
						}
					}
				}
			}
		}
	}
}