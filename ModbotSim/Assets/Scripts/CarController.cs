using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface CarControllerInt {
	Tuple<float, float> speedAndTurn(GameObject car);
}

public class CarController : CarControllerInt {
	private float oldDistance = 100000f;
	private bool justSwitchedWaypoint = false;

	public Tuple<float, float> speedAndTurn(GameObject car) {
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
		//Debug.Log ("steerHere: " + steer);
		Kart kart = car.GetComponent<Kart> ();
//		Vector3 travelDirection = car.transform.InverseTransformPoint(new Vector3 (kart.wayPoints[kart.current_point].x, 
//		                                                                           car.transform.position.y, 
//		                                                                           kart.wayPoints[kart.current_point].z));
		Vector3 travelDirection = kart.wayPoints[kart.current_point];
		travelDirection.x -= car.transform.position.x;
		travelDirection.z -= car.transform.position.z;

		Vector3 position = kart.wayPoints [kart.current_point];
		Vector3 mypos = car.transform.position;
		float distance = Vector3.Distance (position, mypos);
		Debug.Log ("current Waypoint: "+kart.current_point+" justS:"+ justSwitchedWaypoint);
		//Debug.Log ("distance: " + distance + " justS:" + oldDistance);
		//skipping logic --- BAD
//		if (distance > oldDistance && !justSwitchedWaypoint) {
//			Debug.Log ("SKIPPING waypoint "+kart.current_point);
//			int see_ahead = kart.current_point + 1;
//			if (see_ahead >= kart.wayPoints.Count)
//				see_ahead = 0;
//			//Vector3 seeDirection = car.transform.InverseTransformPoint (new Vector3 (kart.wayPoints[see_ahead].x, 
//			//  car.transform.position.y, 
//			// kart.wayPoints[see_ahead].z));
//			//if (seeDirection.z > 0) {
//			kart.current_point = see_ahead;
//			justSwitchedWaypoint = true;
//			return speedAndTurn (car); 
//			//} 
//		}
//		oldDistance = distance;
		// For skipping if the waypoint is behind the car
//		Vector3 relPosition = car.transform.InverseTransformPoint (kart.wayPoints [kart.current_point]);
//		if (relPosition.z <= 0) {
//			Debug.Log ("SKIPPING waypoint "+kart.current_point);
//			int see_ahead = kart.current_point + 1;
//			if (see_ahead >= kart.wayPoints.Count)
//				see_ahead = 0;
//			//Vector3 seeDirection = car.transform.InverseTransformPoint (new Vector3 (kart.wayPoints[see_ahead].x, 
//			                                                                   //  car.transform.position.y, 
//			                                                                    // kart.wayPoints[see_ahead].z));
//			//if (seeDirection.z > 0) {
//				kart.current_point = see_ahead;
//				return speedAndTurn (car); 
//			//} 
//		}
		//Debug.Log ("travelDirrection: " + travelDirection);
		Vector3 forward = car.transform.forward;
		float carAngle = Mathf.Atan (forward.z / forward.x);
		float desiredAngle = Mathf.Atan (travelDirection.z / travelDirection.x);
	Debug.Log ("carAngle: " + carAngle+"   desiredAngle: "+desiredAngle);
//		Debug.Log ("Current Waypoint: " + kart.current_point);
		//steer = travelDirection.x / travelDirection.magnitude + steer;
		if (carAngle - desiredAngle > Mathf.PI / 2)
			desiredAngle += Mathf.PI;
		if (carAngle - desiredAngle < -Mathf.PI / 2)
			carAngle += Mathf.PI;
			
		steer = (carAngle - desiredAngle)/(Mathf.PI/2);
		if (steer > 1) {
			//input_steer = 1;
			steer = Mathf.Min (steer, 1f);
		} 
		if (steer < -1) {
			//input_steer = -1; 
			steer = Mathf.Max (steer, -1f); 
		}
		
		if (travelDirection.magnitude < 5) {
			kart.current_point = kart.current_point + 1;
			justSwitchedWaypoint = true;
			if (kart.current_point >= kart.wayPoints.Count) {
				kart.current_point = 0;
			}
		} else {
			justSwitchedWaypoint = false;
		}

		speed = Mathf.Sqrt (1.05f - (steer * steer));
		Debug.Log ("Speed: " + speed + "   Turn: " + steer);

		//Debug.Log ("Current Waypoint: " + kart.current_point);
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