using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolCollider : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {
            this.gameObject.transform.parent.GetComponent<PatrolData>().ifFollowPlayer = true;
            this.gameObject.transform.parent.GetComponent<PatrolData>().player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player") {
            this.gameObject.transform.parent.GetComponent<PatrolData>().ifFollowPlayer = false;
            this.gameObject.transform.parent.GetComponent<PatrolData>().player = null;
        }
    }
}
