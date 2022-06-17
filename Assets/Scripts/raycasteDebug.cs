using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycasteDebug : MonoBehaviour {
    private Camera Mcam;
	// Use this for initialization
	void Start () {
        Mcam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            Ray r = Mcam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(r.origin,r.direction*100);
        }
	}
}
