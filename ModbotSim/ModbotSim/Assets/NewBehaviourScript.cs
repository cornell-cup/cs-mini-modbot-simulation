using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public float moveSpeed;

	// Use this for initialization
	void Start () {
	
		moveSpeed = 4f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey ("left"))
		{
			transform.Rotate(0f, -.75f , 0f);
		}
		if (Input.GetKey ("right")) 
		{
			transform.Rotate (0f, .75f, 0f);
		}

		transform.Translate (0f, 0f, Input.GetAxis ("Vertical")*moveSpeed*Time.deltaTime);
	}
}
