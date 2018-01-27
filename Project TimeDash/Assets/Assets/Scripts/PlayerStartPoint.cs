using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPoint : MonoBehaviour {

	private PlayerController player;
	private CameraController camera;

	//The direction the player should face when entering a new area
	public Vector2 startDirection;

	// Use this for initialization
	void Start () {
		//Finds object in our world that is a playerController
		player = FindObjectOfType<PlayerController>();
		//Make player position the same position as the Start Point
		player.transform.position = transform.position;
		///////player.lastMove = startDirection;

		camera = FindObjectOfType<CameraController> ();
		camera.transform.position = new Vector3 (transform.position.x, transform.position.y,
			camera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
