using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

//8 Directions that an object can be facing 
//Keeps track of the directin the object this script is attached to is facing
public class OrientationSystem : MonoBehaviour {

	//Assigns a vector direction to one of 8 determined directions, each with a specific angle range
	//Returns one of the 8 possible PlayerDirections
	//  -> Direction will be used to determine animation to use
	public EightDirections GetDirection(Vector3 direction) {

		var angle1 = Mathf.Atan2 (transform.right.y, transform.right.x);
		var dir = direction - transform.position;
		var angle2 = Mathf.Atan2 (dir.y, dir.x);

		//                     deltaAngle = -90
		//                            |       -45
		//                            |      
		// deltaAngle = -180 ---------0----------- deltaAngle = 0
		//                            |      
		//                            |        45
		//                 deltaAngle = 90

		//===============THE DIRECTIONS=================
		//                   -112.5 North  -67.5  
		//         -157.5          -  |  -          - -22.5
		//                          - | -  -   
		//              West ---------0----------- East
		//                            |      
		//                            |        
		//                          South       
		float deltaAngle = angle1- angle2;
		deltaAngle = (deltaAngle * Mathf.Rad2Deg);

		if ( (deltaAngle >= -112.5) && (deltaAngle < -67.5) )
			return EightDirections.North;

		else if ( (deltaAngle >= -67.5) && (deltaAngle < -22.5) )
			return EightDirections.NorthEast;

		else if (deltaAngle >= -22.5 && deltaAngle < 22.5)
			return EightDirections.East;

		else if (deltaAngle >= 22.5 && deltaAngle < 67.5)
			return EightDirections.SouthEast;

		else if (deltaAngle >= 67.5 && deltaAngle < 112.5)
			return EightDirections.South;

		else if (deltaAngle >= 112.5 && deltaAngle < 157.5)
			return EightDirections.SouthWest;

		else if (deltaAngle >= -157.5 && deltaAngle < -112.5)
			return EightDirections.NorthWest;

		else if (deltaAngle >= 157.5 || deltaAngle < -157.5)
			return EightDirections.West;

		//default
		return EightDirections.South;
	}

}

//The way the player faces, split into 45 degree angle slices
//Split into 8 positions, starting at the top moving clockwise
public enum EightDirections {
	North, //0
	NorthEast, //1
	East,  //2
	SouthEast,  //3
	South,//4
	SouthWest,//5
	West, //6
	NorthWest  //7
}