using UnityEngine;
using System.Collections.Generic;

public class PlaceMaster : MonoBehaviour {

    private class PlaceMapping : System.IComparable<PlaceMapping>
    {
        public GameObject kart;
        public int index;

        public PlaceMapping(GameObject o, int i)
        {

        }

        public int CompareTo(PlaceMapping other)
        {
            if (other == null)
            {
                return 1;
            }

            //Return the difference in power.
            return other.index - index;
        }
    }
    List<PlaceMapping> nameToIndex = new List<PlaceMapping>();
    //Dictionary<GameObject, int> nameToIndex = new Dictionary<GameObject, int>();
    GameObject[] karts;
    // Use this for initialization
    void Start() {
         karts = GameObject.FindGameObjectsWithTag("kart");
        foreach (GameObject o in karts){
            PlaceMapping k = new PlaceMapping(o, 0);
            nameToIndex.Add(k);
        }       
	}
	
	// Update is called once per frame
	void Update () {
        for ( int i=0; i< nameToIndex.Count; i++)
        {
            PlaceMapping o = nameToIndex[i];
            int ii = o.kart.GetComponent<Place>().index;
            o.index = ii;
        }
        nameToIndex.Sort((x, y) => x.CompareTo(y));
    }
}
