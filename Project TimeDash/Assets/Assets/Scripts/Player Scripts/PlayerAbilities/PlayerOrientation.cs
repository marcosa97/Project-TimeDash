using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: This class probably doesn't need the game object reference because just using "this" or saying "transform...." 
//       will already reference the object this script is attached to
//Keeps track of the directin the player faces
public class PlayerOrientation : MonoBehaviour {

	public float angle; //remove this later
	private GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
	}

	//Assigns a vector direction to one of 8 determined directions, each with a specific angle range
	//Returns one of the 8 possible PlayerDirections
	//  -> Direction will be used to determine animation to use
	public PlayerDirections GetDirection(Vector3 direction) {
		float angleRatio;

		var angle1 = Mathf.Atan2 (player.transform.right.y, player.transform.right.x);
		var dir = direction - player.transform.position;
		var angle2 = Mathf.Atan2 (dir.y, dir.x);

		//                     deltaAngle = -90
		//                            |       -45
		//                            |      
		// deltaAngle = -180 ---------0----------- deltaAngle = 0
		//                            |      
		//                            |        45
		//                 deltaAngle = 90
		var deltaAngle = angle1- angle2;
		angle = deltaAngle * Mathf.Rad2Deg;

		angleRatio = (deltaAngle * Mathf.Rad2Deg) / 45;

		if ( (angleRatio >= 0) && (angleRatio < 1))
			return PlayerDirections.RightDown;
	
		else if ( ((angleRatio >= 1f) && (angleRatio < 2f)) )
			return PlayerDirections.DownRight;

		else if (angleRatio >= 2f && angleRatio < 3f)
			return PlayerDirections.DownLeft;

		else if (angleRatio >= 3f && angleRatio < 4f)
			return PlayerDirections.LeftDown;

		else if (angleRatio >= -4f && angleRatio < -3f)
			return PlayerDirections.LeftUp;

		else if (angleRatio >= -3f && angleRatio < -2f)
			return PlayerDirections.UpLeft;

		else if (angleRatio >= -2f && angleRatio < -1f)
			return PlayerDirections.UpRight;

		else if (angleRatio >= -1f && angleRatio < 0f)
			return PlayerDirections.RightUp;
	    
		//default
		return PlayerDirections.RightUp;
	}

}

//The way the player faces, split into 45 degree angles
//Split into 8 positions, starting at the top moving clockwise
public enum PlayerDirections {
	RightUp, //0
	UpRight, //1
	UpLeft,  //2
	LeftUp,  //3
	LeftDown,//4
	DownLeft,//5
	DownRight, //6
	RightDown  //7
}
	
