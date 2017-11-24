using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour {
    [SerializeField] float movementSpeed = 50f;
    [SerializeField] float turnSpeed = 60f;
    private bool isMoving = false;
    //private float acceleration = 0.0f;
    private float speed = 0.0f;
    //private float maxSpeed = 2.0f;

    public Rigidbody rb;

    Transform myT;
	void Awake () {
        myT = transform;
	}

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        Turn();
        Thrust();
        
	}

    void FixedUpdate()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    void Turn()
    {
        if(isMoving)
        {
            float yaw = turnSpeed * Time.deltaTime * Input.GetAxis("RWSX");
            float pitch = (turnSpeed * speed) * Time.deltaTime * Input.GetAxis("Vertical");
            float roll = (turnSpeed * speed) * Time.deltaTime * Input.GetAxis("Horizontal");
            myT.Rotate(pitch, yaw, roll);
        }
    }
    void Thrust()
    {
		if (Input.GetAxis("LinuxLeftTrigger") > 0 && Input.GetAxis("LinuxLeftTrigger") > speed)
        {
			speed = Input.GetAxis("LinuxLeftTrigger");
        }
        //Slow Down
		if(speed > 0 || Input.GetAxis("LinuxLeftTrigger") == 0)
        {
            speed = speed - 0.1f * Time.deltaTime;
        }

        if(speed > 0)
        {
            myT.position += myT.forward * movementSpeed * Time.deltaTime * speed;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    
}
