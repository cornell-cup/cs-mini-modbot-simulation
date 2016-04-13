using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private float oldDistance = 100000f;
	private bool justSwitchedWaypoint = false;

	public Tuple<float, float> speedAndTurn(GameObject car) {
		//Adjust steer accordingly if obstacles are present
		ObstacleAvoid obstacleAvoid = car.GetComponent<ObstacleAvoid> ();
		float speed = 0; 
		float steer = 0;
		if (obstacleAvoid.leftObs && !obstacleAvoid.rightObs)
			steer = Mathf.Max (1.2f/obstacleAvoid.LeftDis, 0.35f);
		if (obstacleAvoid.rightObs && !obstacleAvoid.leftObs) 
			steer = -1 * Mathf.Max (1.2f/obstacleAvoid.RightDis, 0.35f);
		if (obstacleAvoid.centerObs && !obstacleAvoid.rightObs && !obstacleAvoid.leftObs) {
			steer = Mathf.Max (1.2f/obstacleAvoid.CenterDis, 0.35f); 
		}
		if (obstacleAvoid.centerObs && (obstacleAvoid.leftObs || obstacleAvoid.rightObs)) {
			steer = steer * 1.42f; 
		}
			
		//Obtain current movement direction of the car
		//Determine current angle of car steer
		Vector3 forward = car.transform.forward;
		float carAngle = Mathf.Atan (forward.z / forward.x);
		//Determine desired angle of car steer
		PathPlanningKart kart = car.GetComponent<PathPlanningKart> ();
		Vector3 currentWayPoint = kart.currentWayPoints[kart.current_waypoint];
		Vector3 desiredDirection = null;
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
			if (kart.current_waypoint >= kart.currentWayPoints.Count) {
				//Current waypoints have been consumed; Move onto next set of waypoints
				kart.currentWayPoints = kart.nextWayPoints;
				kart.pathCalculated = false;
				kart.current_waypoint = 0;
			}
		} else {
			justSwitchedWaypoint = false;
		}

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