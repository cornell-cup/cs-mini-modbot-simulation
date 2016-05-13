using UnityEngine;
using System.Collections.Generic;

public class PlaceMaster : MonoBehaviour {
    Rect position = new Rect(10, 10, 100, 20);
    List<PlaceMapping> nameToIndex = new List<PlaceMapping>();
    //Dictionary<GameObject, int> nameToIndex = new Dictionary<GameObject, int>();
    GameObject[] karts;
    GUIStyle myStyle = new GUIStyle();
    private float time=0;

    private class PlaceMapping : System.IComparable<PlaceMapping>
    {
        public GameObject kart;
        public int index;
        public string name;

        public PlaceMapping(GameObject o, int i)
        {
            kart = o;
            index = i;
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
    
    // Use this for initialization
    void Start() {
        Font f = new Font();
        myStyle.normal.textColor = Color.white;
        myStyle.fontSize = 24;
        myStyle.fontStyle = FontStyle.Bold;
         karts = GameObject.FindGameObjectsWithTag("kart");
        foreach (GameObject o in karts){
            PlaceMapping k = new PlaceMapping(o, 0);
            nameToIndex.Add(k);
            k.name = o.GetComponent<KartInfo>().name;
        }

    }

    // Update is called once per frame
    void Update () {
        time += Time.deltaTime;
        if (time < 1)
            return;
        time = 0;
        for ( int i=0; i< nameToIndex.Count; i++)
        {
            PlaceMapping o = nameToIndex[i];
            int ii = o.kart.GetComponent<Place>().index + 83* o.kart.GetComponent<Place>().numLaps;
            o.index = ii;
        }
        nameToIndex.Sort((x, y) => x.CompareTo(y));
    }

    void OnGUI()
    {
        for (int i = 0; i < nameToIndex.Count; i++)
        {
            position.y = 10 + 30 * i;
            string x = "";
            if (i == 0)
                x = "1st";
            if (i == 1)
                x = "2nd";
            if (i == 2)
                x = "3rd";
            if (i == 4)
                x = "4th";
            GUI.Label(position, nameToIndex[i].name+": "+x, myStyle);
        }
        
    }
}
