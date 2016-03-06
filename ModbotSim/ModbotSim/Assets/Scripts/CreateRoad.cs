using UnityEngine;
using System.Collections;

public class CreateRoad : MonoBehaviour {
	private float width = 2f;


	// Use this for initialization
	void Start () {
		Mesh mesh = new Mesh ();
		MeshFilter m = GetComponent<MeshFilter>();
		m.mesh.Clear ();
		//generate 300 points that go in a circle
		Vector3[] sampleinput = new Vector3[300];
		float angle = 0;
		float radius = 30; 

		for (int i = 0; i < sampleinput.Length-1; i++) {
			float dxR = radius * Mathf.Sin (angle);
			float dyR = radius * Mathf.Cos (angle);
			sampleinput [i] = new Vector3 (dxR, 0, dyR);
			//Debug.Log ("i: " + i + "   " + sampleinput [i]);
			angle += (2 * Mathf.PI / (sampleinput.Length-1));
		}
		sampleinput [sampleinput.Length - 1] = new Vector3(sampleinput [0].x, 0, sampleinput[0].z);
		Debug.Log ("0: " + sampleinput [0]);
		Debug.Log ("last: " + sampleinput [sampleinput.Length - 1]);
		for(int i=0; i<sampleinput.Length; i++)
			Debug.Log ("i: "+i+"  "+sampleinput[i]);
		Debug.Log ("------------------");

//		Vector3[] sampleinput = {
//			new Vector3 (0, 0, 0),
//			new Vector3 (0, 0, .1f),
//			new Vector3 (.1f, 0, .3f),
//			new Vector3 (.15f, 0, .4f),
//			new Vector3 (.3f, 0, .4f)
//		};

		Vector3[] vertices = new Vector3[sampleinput.Length * 2];


		//setup the first two points for basecase reference
		//-------------------------------------------------------
		//Debug.Log ("b4: "+(sampleinput [1].z - sampleinput [0].z) / (sampleinput [1].x - sampleinput [0].x));
		//get angle between point 0 and 1
		float a = Mathf.Atan((sampleinput[1].z - sampleinput[0].z)/(sampleinput[1].x - sampleinput[0].x));
		//Debug.Log (a);

		//get delta x and z for wing vertices
		float dz = Mathf.Abs(width * Mathf.Cos(a));
		float dx = Mathf.Abs(width * Mathf.Sin(a));
		float sign_x = Mathf.Sign (sampleinput [0].x);
		float sign_z = Mathf.Sign (sampleinput [0].z);
		vertices [0] = new Vector3 (sampleinput [0].x + sign_x*dx, 0, sampleinput [0].z + sign_z*dz);
		vertices [1] = new Vector3 (sampleinput [0].x + -1*sign_x*dx, 0, sampleinput [0].z + -1*sign_z*dz);
		sign_x = -1*Mathf.Sign (sampleinput [1].x);
		sign_z = -1*Mathf.Sign (sampleinput [1].z);
		vertices [2] = new Vector3 (sampleinput [1].x + -1*sign_x*dx, 0, sampleinput [1].z + -1*sign_z*dz);
		vertices [3] = new Vector3 (sampleinput [1].x + sign_x*dx, 0, sampleinput [1].z + sign_z*dz);
		//-------------------------------------------------------

		for (int i = 2; i < sampleinput.Length-1; i++) {
			a = Mathf.Atan((sampleinput[i+1].z - sampleinput[i-1].z)/(sampleinput[i+1].x - sampleinput[i-1].x));
			dz = Mathf.Abs(width * Mathf.Cos(a));
			dx = Mathf.Abs(width * Mathf.Sin(a));
			sign_x = Mathf.Sign (sampleinput [i].x);
			sign_z = Mathf.Sign (sampleinput [i].z);
			vertices[i*2] = new Vector3 (sampleinput [i].x + sign_x*dx, 0, sampleinput [i].z + sign_z*dz);
			vertices[i*2+1] = new Vector3 (sampleinput [i].x + -1*sign_x*dx, 0, sampleinput [i].z + -1*sign_z*dz);
		}

		//setup last one
		a = Mathf.Atan((sampleinput[sampleinput.Length-1].z - sampleinput[sampleinput.Length-2].z)/(sampleinput[sampleinput.Length-1].x - sampleinput[sampleinput.Length-2].x));
		dz = Mathf.Abs(width * Mathf.Cos(a));
		dx = Mathf.Abs(width * Mathf.Sin(a));
		sign_x = Mathf.Sign (sampleinput [(sampleinput.Length-1)].x);
		sign_z = Mathf.Sign (sampleinput [(sampleinput.Length-1)].z);
		vertices[(sampleinput.Length-1)*2] = new Vector3 (sampleinput [(sampleinput.Length-1)].x + sign_x*dx, 0, sampleinput [(sampleinput.Length-1)].z + sign_z*dz);
		vertices[(sampleinput.Length-1)*2+1] = new Vector3 (sampleinput [(sampleinput.Length-1)].x + -1*sign_x*dx, 0, sampleinput [(sampleinput.Length-1)].z + -1*sign_z*dz);
//
//		vertices[(sampleinput.Length-1)*2 + 1] = new Vector3(vertices[0].x,0,vertices[0].z);
//		vertices [(sampleinput.Length - 1) * 2] = new Vector3(vertices[1].x,0,vertices[1].z);
		mesh.vertices = vertices;



		//num triangles

		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i = 0; i < uvs.Length - 3; i += 4) {
			uvs [i] = new Vector2 (1, 1);
			uvs [i+1] = new Vector2 (0, 1);
			uvs [i+2] = new Vector2 (1, 0);
			uvs [i+3] = new Vector2 (0, 0);
		}

		int[] indices = new int[(sampleinput.Length * 2-2)*3];
		for (int i = 0; i < sampleinput.Length * 2 - 2; i++) {
			if (i % 2 == 0) {
				indices [i * 3] = i;
				indices [i * 3 + 1] = i + 3;
				indices [i * 3 + 2] = i + 1;
			} else {
				indices [i * 3] = i - 1;
				indices [i * 3 + 1] = i + 1;
				indices [i * 3 + 2] = i + 2;
			}
		}

		mesh.triangles = indices;
		mesh.uv = uvs;
		UnityEditor.AssetDatabase.CreateAsset (mesh, "Assets/createdmesh/trackmesh.asset");
		UnityEditor.AssetDatabase.SaveAssets ();




		GetComponent<MeshCollider> ().sharedMesh = mesh;
		this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
