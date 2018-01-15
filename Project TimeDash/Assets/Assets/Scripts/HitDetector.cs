using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: This class will be used by Enemies to detect hits, since I can't use OnCollisionEnter2D because 
//  the sword collider is a Trigger. 
public class HitDetector : MonoBehaviour {
	
	public bool wasHit; //must be accessable to other classes that handle collisions

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
