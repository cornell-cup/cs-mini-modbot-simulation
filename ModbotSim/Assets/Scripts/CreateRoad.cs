using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateRoad : MonoBehaviour {
	private float width = 8f;
	private Vector3[] vertices;
	private int counter = 1;
	private Vector2 temp = new Vector2();
	private static float MIN_DISTANCE = .03f;
	private static float FENCE_LENGTH = 2f;
	private Vector3 lastRightFence;
	private Vector3 lastLeftFence;
	private Vector3 lastLeftDxDy;
	private Vector3 lastRightDxDy;

//	void OnDrawGizmos () {
//		Debug.Log ("_________________________");
//		Debug.Log (counter);
//		Debug.Log ("_________________________");
//		if (vertices == null || counter == 5)
//			return;
//		Gizmos.color = Color.black;
//		for (int i = 0; i <vertices.Length; i++) {
//			//Debug.Log (i);
//			if (vertices [i] != null) {
//				Gizmos.DrawSphere (vertices [i], .05f);
//				Debug.Log (vertices[i]);
//				Debug.Log("Point: "+vertices[i/2]);
//			} else {
//				Debug.Log ("NULL");
//			}
//		}
//	}

	// Use this for initialization
	void Start () {
		lastRightFence = new Vector3 ();
		lastLeftFence = new Vector3 ();
		lastLeftDxDy = new Vector3 ();
		lastRightDxDy = new Vector3 ();
		Mesh mesh = new Mesh ();
		MeshFilter m = GetComponent<MeshFilter>();
		m.mesh.Clear ();
//									
		string text = System.IO.File.ReadAllText(Application.dataPath + "/Scripts/WriteToFile.txt");
		string[] lines = text.Split ('\n');
		string sep = "\t";
		Vector3 oldV = new Vector3();
		List<Vector3> array = new List<Vector3> ();
		for (int i=0; i<lines.Length-1; i++) {
			string s = lines [i];
			string[] split = s.Split (sep.ToCharArray());
			float x = float.Parse (split [0]);
			float y = float.Parse (split [1]);
			float z = float.Parse (split [2]);
			Vector3 tempp = new Vector3 (x, y, z);
			if (Vector3.Magnitude (tempp - oldV) < MIN_DISTANCE)
				continue;
			array.Add(tempp);
			oldV = tempp;
		}

		Vector3[] sampleinput = array.ToArray ();
		vertices = new Vector3[sampleinput.Length * 2];
		float dz = sampleinput[1].z - sampleinput[0].z;
		float dx = sampleinput[1].x - sampleinput[0].x;

		temp.Set (-1 * dz, dx);
		temp.Normalize();
		temp *= width;

		float moveZ = temp.y;
		float moveX = temp.x;

		vertices [0] = new Vector3 (sampleinput [0].x + moveX, 0, sampleinput [0].z + moveZ);
		vertices [1] = new Vector3 (sampleinput [0].x - moveX, 0, sampleinput [0].z - moveZ);

		//-------------------------------------------------------
		Transform fence = GameObject.FindGameObjectWithTag("FenceHolder").transform;
		ArrayList fences = new ArrayList ();
		for (int i = 1; i < sampleinput.Length-1; i++) {
//			if (i % 10 == 0) {
//				GameObject o = Instantiate (Resources.Load ("BetterFence"), sampleinput [i], Quaternion.identity) as GameObject;
//				o.transform.SetParent (fence);
//				fences.Add (o);
//			}
			dz = sampleinput[i+1].z - sampleinput[i-1].z;
			dx = sampleinput[i+1].x - sampleinput[i-1].x;
			temp.Set (-1 * dz, dx);
			temp.Normalize();
			temp *= width;

			moveZ = temp.y;
			moveX = temp.x;

			vertices [i*2] = new Vector3 (sampleinput [i].x + moveX, 0, sampleinput [i].z + moveZ);
			vertices [i*2 + 1] = new Vector3 (sampleinput [i].x - moveX, 0, sampleinput [i].z - moveZ);
			if (lastLeftFence == null || Vector3.Distance(lastLeftFence,vertices[i*2]) > FENCE_LENGTH) {
				Vector3 tempp;
				if (lastLeftDxDy == null) {
					tempp = vertices [i * 2];
					lastLeftDxDy.Set (moveX, 0, moveZ);
					lastLeftDxDy.Normalize ();
				} else {
					tempp = lastLeftFence + (lastLeftDxDy * FENCE_LENGTH / 2);
					lastLeftDxDy.Set (moveX, 0, moveZ);
					lastLeftDxDy.Normalize ();
					tempp += (lastLeftDxDy * FENCE_LENGTH / 2);
				}
				lastLeftFence = vertices [i * 2];

				float angle = Mathf.Atan (lastLeftDxDy.x / lastLeftDxDy.z);
				Quaternion q = new Quaternion ();
				q.SetEulerRotation (0, angle, 0);
				GameObject o1 = Instantiate (Resources.Load ("BetterFence"), tempp,q) as GameObject;
				//GameObject o2 = Instantiate (Resources.Load ("BetterFence"), vertices[i*2+1],q) as GameObject;
				o1.transform.SetParent (fence);
				//o2.transform.SetParent (fence);
				fences.Add (o1);
				//fences.Add (o2);
				//lastLeftDxDy.Set(moveX,0,moveZ);
			}
			if (lastRightFence == null || Vector3.Distance(lastRightFence,vertices[i*2+1]) > FENCE_LENGTH) {
				Vector3 tempp;
				if (lastRightDxDy == null) {
					tempp = vertices [i * 2+1];
					lastRightDxDy.Set (moveX, 0, moveZ);
					lastRightDxDy.Normalize ();
				} else {
					tempp = lastRightFence + (lastRightDxDy * FENCE_LENGTH / 2);
					lastRightDxDy.Set (moveX, 0, moveZ);
					lastRightDxDy.Normalize ();
					tempp += (lastRightDxDy * FENCE_LENGTH / 2);
				}

				lastRightFence = vertices [i * 2+1];
				float angle = Mathf.Atan (temp.x / temp.y);
				Quaternion q = new Quaternion ();
				q.SetEulerRotation (0, angle, 0);
				//GameObject o1 = Instantiate (Resources.Load ("BetterFence"), vertices[i*2],q) as GameObject;
				GameObject o2 = Instantiate (Resources.Load ("BetterFence"), vertices[i*2+1],q) as GameObject;
				//o1.transform.SetParent (fence);
				o2.transform.SetParent (fence);
				//fences.Add (o1);
				fences.Add (o2);
			}

		}

		//setup last one
		dz = sampleinput[sampleinput.Length-1].z - sampleinput[sampleinput.Length-2].z;
		dx = sampleinput[sampleinput.Length-1].x - sampleinput[sampleinput.Length-2].x;
		temp.Set (-1 * dz, dx);
		temp.Normalize();
		temp *= width;
		moveZ = temp.y;
		moveX = temp.x;

		vertices [2*(sampleinput.Length-1)] = new Vector3 (sampleinput [sampleinput.Length-1].x + moveX, 0, sampleinput [sampleinput.Length-1].z + moveZ);
		vertices [2*(sampleinput.Length-1)+1] = new Vector3 (sampleinput [sampleinput.Length-1].x - moveX, 0, sampleinput [sampleinput.Length-1].z - moveZ);
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
			if (i % 2 == 0 && i!= sampleinput.Length-1) {
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

		//UnityEditor.AssetDatabase.CreateAsset (GameObject.FindGameObjectWithTag ("FenceHolder"), "Assets/createdmesh/fences.asset");
		//Object prefab = EditorUtility.CreateEmptyPrefab("Assets/Temporary/"+t.gameObject.name+".prefab");
		//Object prefab = UnityEditor.PrefabUtility.FindPrefabRoot(GameObject.FindGameObjectWithTag("AllFences")); //Prefab prefab = Resources.Load("AllFences");
		//UnityEditor.PrefabUtility.ReplacePrefab(GameObject.FindGameObjectWithTag("FenceHolder"),prefab,UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
		UnityEditor.AssetDatabase.SaveAssets ();


		GetComponent<MeshCollider> ().sharedMesh = mesh;
		print ("Here");
		this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
