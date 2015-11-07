using UnityEngine;
using System.Collections;
using System;

public class MoveFromVisionData1 : MonoBehaviour {
	public Queue testCoordinates;
	public Coord initialCoord;
	const float PI = (float)Math.PI;

	public float moveSpeed;

	public class Coord{
		float xCoord;
		float yCoord;
		float zCoord;
		float rot;

		public Coord(){
			xCoord = 0;
			yCoord = 0;
			zCoord = 0;
			rot = 0;
		}

		public Coord(float x, float y, float z, float r){
			xCoord = x;
			yCoord = y;
			zCoord = z;
			rot = r;
		}

		public float getXCoord(){
			return xCoord;
		}

		public float getYCoord(){
			return yCoord;
		}

		public float getZCoord()
		{
			return zCoord;
		}

		public float getRot(){
			return rot;
		}

		public string toString(){
			//Debug.Log ("(" + xCoord + "," + yCoord + ")");
			return "(" + xCoord + "," + yCoord + ")";
		}
	}

	Queue GetCoord() {
		string line;
		string xcoord = "initial x";
		string ycoord = "initial y";
		string zcoord = "initial z";
		string rotation = "initial rot";
		int first, second, third, fourth, fifth;
		Queue list = new Queue ();
		// Read the file and display it line by line.
		System.IO.StreamReader file = 
			new System.IO.StreamReader(Application.dataPath + "/Scripts/WriteToFile.txt");
        print(Application.dataPath);
		while((line = file.ReadLine()) != null)
		{
			if(!line.Equals("ID\tXPOS(m)\tYPOS(m)\tROT(rad)\tXVEL(m/s)\tYVEL(m/s)\tROTVEL(rad/s)\tTIME(s)")){
				//string[] values = line.Split("\t");
				first = line.IndexOf('\t');
				second = line.IndexOf ('\t', first+1);
				third = line.IndexOf ('\t', second+1);
				fourth = line.IndexOf ('\t', third+1);
				fifth = line.IndexOf ('\t', fourth + 1);
				xcoord = line.Substring (first+1, second - first - 1);
				ycoord = line.Substring (second+1, third - second - 1);
				zcoord = line.Substring(third+1, fourth - third -1);
				rotation = line.Substring (fourth + 1, fifth - fourth - 1);
				//Debug.Log (xcoord + "," + ycoord+","+rotation);
				list.Enqueue (new Coord (float.Parse(xcoord), float.Parse(ycoord), float.Parse (zcoord), float.Parse(rotation)));
			}
		}
		file.Close();
		// Suspend the screen.
		//Console.ReadLine();
		return list;
	}

	// Use this for initialization
	void Start () {
		moveSpeed = 3f;
		testCoordinates = GetCoord ();
		initialCoord = (Coord)testCoordinates.Dequeue();

	}

	// Update is called once per frame
	void Update () 
	{
		float rot;
		//reading vision data
		if(testCoordinates.Count>0){
			Coord c = (Coord)testCoordinates.Dequeue();
			rot = (c.getRot () );
			transform.position = new Vector3 (c.getXCoord (), 
				c.getYCoord(), c.getZCoord ());
			transform.rotation = Quaternion.Euler (0, rot, 0);
			c.toString ();
		}
	}
}
