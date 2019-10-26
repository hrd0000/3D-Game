using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour {
    public GameObject target;
    public float followSpeed = 5f;         
    Vector3 distance;

	void Start () {
        distance = transform.position - target.transform.position;
	}

	void Update () {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + distance, followSpeed * Time.deltaTime);
	}
}
