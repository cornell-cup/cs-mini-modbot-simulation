using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// PathPlanningDataStructures is a class that contains the essential data structures necessary
// for implementing the path planning approach

public class PathPlanningDataStructures {
	// Heuristic object containing the dijkstra movement costs
	public static HeuristicD heuristic;
	// Graph object representing the map and containing all possible way points
	public static GenerateGraph graph; 
	//indicates for each node in the graph the number of AI cars that have it as a current waypoint
	public static Dictionary<Vector3, int> nodeToCount = new Dictionary<Vector3, int>();
	// Global lock for synchronizing access to the nodeToCount dictionary
	public static System.Object globalLock = new System.Object();
	//indicates whether or not the path planning data structures have been initialized
	public static bool pathPlanningInitialized = false;

	// <summary>
	// Constructor that initializes the data structures necessary for the path planning approach
	// </summary>
	public static void initializePathPlanning () {
		lock (globalLock) {
			if (pathPlanningInitialized == false) {
				graph = new GenerateGraph ();
				heuristic = new HeuristicD (graph);
				pathPlanningInitialized = true;
			}
		}
	}

}
