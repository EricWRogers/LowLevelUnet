using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int BulletSpeed = 50;
	void Update () {
        transform.Translate(Vector3.forward * Time.deltaTime * BulletSpeed);
	}
}
