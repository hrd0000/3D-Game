using UFO.com;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {
    private IUserAction action;

	// Use this for initialization
	void Start () {
        action = SceneController.getInstance() as IUserAction;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            action.hitUFO(mousePos);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            action.launchUFO();
        }
    }
}
