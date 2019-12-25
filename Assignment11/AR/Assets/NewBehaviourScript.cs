using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Vuforia;

public class NewBehaviourScript : MonoBehaviour, IVirtualButtonEventHandler
{
    public VirtualButtonBehaviour[] vbs;
    public GameObject cube;
    public GameObject button;
    public Color[] colors;
    public int index;

    void Start()
    {
        vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; i++)
        {
            vbs[i].RegisterEventHandler(this);
        }
        colors = new Color[3];
        index = 0;

        colors[0] = Color.red;
        colors[1] = Color.yellow;
        colors[2] = Color.blue;

        cube = GameObject.Find("ImageTarget/Cube");
        button = GameObject.Find("ImageTarget/VirtualButton/Plane");
    }
    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        button.GetComponent<Renderer>().material.color = Color.red;
        if (index == 3)
            index = 0;
        cube.GetComponent<Renderer>().material.color = colors[index++];
    }
    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        button.GetComponent<Renderer>().material.color = Color.red;
    }
}

