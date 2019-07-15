
using UnityEngine;

public class RespawnPoint : MonoBehaviour {

    private Vector3 position;
    private FallPit fallPit;
    private AbilityFall playerFallHandler;

	// Use this for initialization
	void Start () {
        this.position = GetComponent<Transform>().transform.position;
        this.fallPit = GetComponentInParent<FallPit>();
        this.playerFallHandler = GameObject.Find("Player").GetComponent<AbilityFall>();
	}

    public Vector3 GetPosition() {
        return this.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Set active spawn point to the one corresponding to the one 
        //attached to this game object
        this.playerFallHandler.SetActiveRespawnPoint(this.position);
    }
}
