using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

//In this system, an object can be facing in one of 8 directions
//A direction is determined by taking a vector facing in the direction
//  the object is facing, calculating its angle relative to the vector (1, 0)
//  and determining in which of the 8 directions the angle falls in
public class OrientationSystem : MonoBehaviour {

	//NOTE: OLD FUNCTION, NEWER VERSION IS DetermineDirectionFromVector
	//Assigns a vector direction to one of 8 determined directions, each with a specific angle range
	//Returns one of the 8 possible PlayerDirections
	//  -> Direction will be used to determine animation to use
	//  @Vector 3 direction: 
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

	/// <summary>
	///   Determines the direction (one of the EightDirections) 
	///   that vector is pointing to. 
	/// </summary>
	/// <returns>The direction from vector.</returns>
	/// <param name="dir"> Vector pointing in the direction we want
	///  to transform into an EightDirections direction.</param>
	public EightDirections DetermineDirectionFromVector(Vector2 dir) {

		//varAngle = the angle where the vector of reference is (1, 0)
		//Debug.Log(dir);
		var dirAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		//Debug.Log (dirAngle);

		//                     dirAngle =  90
		//                            |        45
		//                            |      
		// dirAngle =    180 ---------0----------- dirAngle = 0
		//                            |      
		//                            |       -45
		//                 dirAngle = -90

		//===============THE DIRECTIONS=================
		//                    112.5 North   67.5  
		//          157.5          -  |  -          -  22.5
		//                          - | -  -   
		//              West ---------0----------- East
		//                            |      
		//                            |        
		//                          South       


		if ( (dirAngle >= -112.5) && (dirAngle < -67.5) )
			return EightDirections.South;

		else if ( (dirAngle >= -67.5) && (dirAngle < -22.5) )
			return EightDirections.SouthEast;

		else if (dirAngle >= -22.5 && dirAngle < 22.5)
			return EightDirections.East;

		else if (dirAngle >= 22.5 && dirAngle < 67.5)
			return EightDirections.NorthEast;

		else if (dirAngle >= 67.5 && dirAngle < 112.5)
			return EightDirections.North;

		else if (dirAngle >= 112.5 && dirAngle < 157.5)
			return EightDirections.NorthWest;

		else if (dirAngle >= -157.5 && dirAngle < -112.5)
			return EightDirections.SouthWest;

		else if (dirAngle >= 157.5 || dirAngle < -157.5)
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