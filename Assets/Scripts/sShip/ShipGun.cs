using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGun : MonoBehaviour {

    public GameObject bullet;

    void Start()
    {
        PoolManager.instance.CreatePool(bullet, 20);
    }
    // Update is called once per frame
    void Update () {
        Vector3 self = transform.position;
        Quaternion selfR = transform.rotation;
        if (Input.GetButtonDown("Submit"))
        {
            PoolManager.instance.ReuseObject(bullet, self, selfR);
        }
	}
}
