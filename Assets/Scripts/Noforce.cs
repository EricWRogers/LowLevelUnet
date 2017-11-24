using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noforce : MonoBehaviour {
	public Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate()
	{
		rb.velocity = new Vector3(0, 0, 0);
		rb.angularVelocity = new Vector3(0, 0, 0);
	}
}
