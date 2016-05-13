using UnityEngine;
using System.Collections;

public class Place : MonoBehaviour {
    public int carPlace=0;
    public int index=0;
    public int numNodes=0;
    public int numLaps=0;
    public string[] arr = new string[2];
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {
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
        if (int.Parse(arr[1]) - index < 40 && index != int.Parse(arr[1])) { 
            numNodes++;
            index = int.Parse(arr[1]);
        }
            

        if (numNodes > 40 && index > 80)
        {
            numLaps++;
            numNodes = 0;
        }
    }
}
