
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorOpener : MonoBehaviour {

    //Get position of door on grid
    public Tilemap doorTilemap;
    public Vector3Int doorTilePosition_1;
    public Vector3Int doorTilePosition_2;

    public void RemoveDoorTile() {
        this.doorTilemap.SetTile(doorTilePosition_1, null);
        this.doorTilemap.SetTile(doorTilePosition_2, null);
    }
	
}
