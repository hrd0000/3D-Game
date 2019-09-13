using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestifyBeh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("This Start!");
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("This Update!");
    }
    
    void Awake() 
    {
    	Debug.Log("This Awake!");
    }
    
    void FixedUpdate() 
    {
    	Debug.Log("This FixUpdate!");
    }
    
    void LateUpdate() 
    {
    	Debug.Log("This LateUpdate!");
    }
    
    void OnGUI() 
    {
    	Debug.Log("This OnGUI!");
    }
    
    void OnDisable() 
    {
    	Debug.Log("This Disabled!");
    }
    
    void OnEnable() 
    {
    	Debug.Log("This Enabled!");
    }
    
}
