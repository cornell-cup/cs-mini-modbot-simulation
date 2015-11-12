using UnityEngine;
using System.Collections.Generic;
using System;

public class MoveFromVisionData1 : MonoBehaviour {
	public Queue<Coord> testCoordinates;
	public Coord initialCoord;
	const float START_SPEED = 3f;
    public float moveSpeed;
    
	Queue<Coord> GetCoord() {
		string line;
		
		Queue<Coord> list = new Queue<Coord> ();
		// Read the file and display it line by line.
		System.IO.StreamReader file = 
			new System.IO.StreamReader(Application.dataPath + "/Scripts/WriteToFile.txt");
        print(Application.dataPath);
		while((line = file.ReadLine()) != null)
		{
            String[] values = line.Split('\t');

            //values[0] is robot id
            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);
            float r = float.Parse(values[4]);
            
            list.Enqueue(new Coord (x,y,z,r));
		}
		file.Close();
		// Suspend the screen.
		//Console.ReadLine();
		return list;
	}

	// Use this for initialization
	void Start () {
		//moveSpeed = 3f;
        moveSpeed = START_SPEED;
		testCoordinates = GetCoord();
		initialCoord = testCoordinates.Dequeue();

	}

	// Update is called once per frame
	void Update () 
	{
		float rot;
		//reading vision data
		if(testCoordinates.Count > 0){
            Coord c = testCoordinates.Dequeue();
			rot = (c.getRot () );
			transform.position = new Vector3 (c.getXCoord (), 
				c.getYCoord(), c.getZCoord ());
			transform.rotation = Quaternion.Euler (0, rot, 0);
			c.toString ();
		}
	}

    public class Coord
    {
        float xCoord;
        float yCoord;
        float zCoord;
        float rot;

        public Coord()
        {
            xCoord = 0;
            yCoord = 0;
            zCoord = 0;
            rot = 0;
        }

        public Coord(float x, float y, float z, float r)
        {
            xCoord = x;
            yCoord = y;
            zCoord = z;
            rot = r;
        }

        public float getXCoord()
        {
            return xCoord;
        }

        public float getYCoord()
        {
            return yCoord;
        }

        public float getZCoord()
        {
            return zCoord;
        }

        public float getRot()
        {
            return rot;
        }

        public string toString()
        {
            //Debug.Log ("(" + xCoord + "," + yCoord + ")");
            return "(" + xCoord + "," + yCoord + ")";
        }
    }
}
