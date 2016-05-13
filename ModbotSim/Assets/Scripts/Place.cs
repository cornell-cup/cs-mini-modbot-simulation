using UnityEngine;
using System.Collections;

public class Place : MonoBehaviour {
    public int carPlace;
    public int index;
    public int numLaps;
    public string[] arr = new string[2];
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("place");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        arr = closest.name.Split(' ');
        index = int.Parse(arr[1]);
    }
}
