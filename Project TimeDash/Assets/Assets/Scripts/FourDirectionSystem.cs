using UnityEngine;

public class FourDirectionSystem {

    public FourDirections GetDirectionFromVector(Vector2 dir) {
        //===============THE DIRECTIONS=================
        //                          North   
        //               135 -        |        - 45
        //                        -   |   -   
        //              West ---------0----------- East    - 0
        //                        -   |   -  
        //              -135 -        |        - -45
        //                          South  

        //Determine angle in degrees
        var dirAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if ( dirAngle >= 45 && dirAngle < 135 ) {
            return FourDirections.North;
        } else if ( dirAngle >= 135 || dirAngle < -135) {
            return FourDirections.West;
        } else if ( dirAngle >= -135 && dirAngle < -45) {
            return FourDirections.South;
        } else if ( dirAngle >= -45 && dirAngle < 45) {
            return FourDirections.East;
        }

        return FourDirections.South;
    }

}

public enum FourDirections
{
    North,
    East,
    South,
    West
}
