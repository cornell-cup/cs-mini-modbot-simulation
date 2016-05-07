using UnityEngine;
using System.Collections;

public class SetPosFromVisionData : MonoBehaviour
{

    Transform start;
    UDPVisionReceive rec;
    bool done = false;

    // Use this for initialization
    void Start()
    {
        start = GameObject.Find("StartLine").GetComponent<Transform>();
        rec = GameObject.FindGameObjectWithTag("udpvision").GetComponent<UDPVisionReceive>();
    }

    void Update()
    {
        if (!done)
        {
            VisionData dataObj = rec.getDataObj(0);
            if (dataObj != null)
            {
                Vector3 v = new Vector3(dataObj.positionX, 0, dataObj.positionY) * MoveFromVisionData1.SCALE;
                transform.position = v - start.localPosition + new Vector3(0, 0.02f, 0);
                done = true;
            }
        }
    }
}