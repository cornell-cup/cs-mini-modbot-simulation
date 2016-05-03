using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

interface CarControllerInt {
	// <summary>
	// Returns a Tuple pair of (speed, turn), where speed ranges from 0 (no speed) to 
	// 1 (max speed), and turn ranges from -1 (represents full left turn) to 1 (represents
	// full right turn)
	// </summary>
	// <param name="car"> 
	// Gameobject representing a car
	// </param>
	Tuple<float, float> speedAndTurn(GameObject car);
}

public class CarController : CarControllerInt {
	private bool justSwitchedWaypoint = false;

	public Tuple<float, float> speedAndTurn(GameObject car) {
		//Adjust steer accordingly if obstacles are present
		float speed = 0; 
		float steer = 0;

			
		//Obtain current movement direction of the car
		//Determine current angle of car steer
		Vector3 forward = car.transform.forward;
		float carAngle = Mathf.Atan (forward.z / forward.x);


		//Determine desired angle of car steer
		PathPlanningKart kart = car.GetComponent<PathPlanningKart> ();

		if (kart.dynamicReplan && kart.dynamicWayPoints != null && kart.dynamicWayPoints.Count > 0) {
			kart.currentWayPoints = kart.dynamicWayPoints;
			kart.nextWayPoints = null;
			kart.dynamicReplan = false;
		}

		if (kart.nextWayPoints != null && kart.nextWayPoints.Count == 0) {
			kart.nextWayPoints = null;
		}
	
		if (kart.current_waypoint >= kart.currentWayPoints.Count && kart.nextWayPoints != null) {
			kart.currentWayPoints = kart.nextWayPoints;
			kart.nextWayPoints = null;
			kart.current_waypoint = 0;
		} else if (kart.current_waypoint >= kart.currentWayPoints.Count) {
			kart.current_waypoint = kart.currentWayPoints.Count - 1;
		}

		Vector3 currentWayPoint = kart.currentWayPoints[kart.current_waypoint];
		Vector3 desiredDirection = new Vector3 (0,0,0);
		desiredDirection.y = currentWayPoint.y;
		desiredDirection.x = currentWayPoint.x - car.transform.position.x;
		desiredDirection.z = currentWayPoint.z - car.transform.position.z;
		float desiredAngle = Mathf.Atan (desiredDirection.z / desiredDirection.x);

		//Conversion from radians to a steer between -1 and 1
		if (carAngle - desiredAngle > Mathf.PI / 2)
			desiredAngle += Mathf.PI;
		if (carAngle - desiredAngle < -Mathf.PI / 2)
			carAngle += Mathf.PI;

		steer = (carAngle - desiredAngle)/(Mathf.PI/2);
		if (steer > 1) {
			steer = Mathf.Min (steer, 1f);
		} 
		if (steer < -1) {
			steer = Mathf.Max (steer, -1f); 
		}

		//If within small distance away to the current waypoint, move onto the next waypoint
		if (desiredDirection.magnitude < 5) {
			kart.current_waypoint = kart.current_waypoint + 1;
			justSwitchedWaypoint = true;
			if (kart.current_waypoint >= kart.currentWayPoints.Count && kart.nextWayPoints != null) {
				//Current waypoints have been consumed; Move onto next set of waypoints
				kart.currentWayPoints = kart.nextWayPoints;
				kart.nextWayPoints = null;
				kart.current_waypoint = 0;
			}
		} else {
			justSwitchedWaypoint = false;
		}
		if (kart.current_waypoint + 1 < kart.currentWayPoints.Count) {
			if (Vector3.Distance (kart.transform.position, kart.currentWayPoints [kart.current_waypoint]) > 10) {
				kart.dynamicReplan = true;
			}
		} else {
			if (Vector3.Distance (kart.transform.position, kart.currentWayPoints [kart.current_waypoint - 1]) > 10) {
				kart.dynamicReplan = true;
			}
		}

		//
		Vector3 inversePoint = (kart.transform.InverseTransformPoint (kart.currentThreadJob.endNode.position));
		if (inversePoint.z < 0) {
			if (inversePoint.x > 0) {
				steer = 1;
			} else {
				steer = -1;
			}
		}

		/*
		ObstacleAvoid obstacleAvoid = car.GetComponent<ObstacleAvoid> ();
		if (obstacleAvoid.leftObs && !obstacleAvoid.rightObs)
			steer = Mathf.Min (.5f/obstacleAvoid.LeftDis, 0.5f);
		if (obstacleAvoid.rightObs && !obstacleAvoid.leftObs) 
			steer = -1 * Mathf.Min (.5f/obstacleAvoid.RightDis, 0.5f);
		if (obstacleAvoid.centerObs && !obstacleAvoid.rightObs && !obstacleAvoid.leftObs) {
			steer = Mathf.Min (.5f/obstacleAvoid.CenterDis, 0.5f); 
		}
		if (obstacleAvoid.centerObs && (obstacleAvoid.leftObs || obstacleAvoid.rightObs)) {
			steer = Mathf.Max(Mathf.Min (steer * 1.42f, 0.5f), -0.5f);
		}
		*/


		speed = Mathf.Sqrt (1.05f - (steer * steer));

		return new Tuple<float, float> (speed, steer);
	}
}

public class Tuple<T1, T2> {
	public T1 First { get; private set;}
	public T2 Second {get; private set;}
	public Tuple(T1 first, T2 second) {
		First = first;
		Second = second;
	}
}