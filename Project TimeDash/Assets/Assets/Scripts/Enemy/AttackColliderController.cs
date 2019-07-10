using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderController : MonoBehaviour {

    public PlayerController playerController;
    private AttackInfoContainer attackInfo;
    public PolygonCollider2D attackCollider;
    private bool hitPlayer; //To avoid hitting multiple times
                            //in one attack

    // Use this for initialization
    void Start()
    {
        hitPlayer = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Only check collisions when in attacking state
        if (other.tag == "Player" && hitPlayer == false)
        {
            //Get attack info
            attackInfo = GetComponentInParent<AttackInfoContainer>();

            Debug.Log("Hit player");
            Debug.Log(attackInfo.damage);
            playerController.HurtPlayer(attackInfo);
            hitPlayer = true;
            return;
        }

        //If the player's shield was hit, stop attack
        if (other.tag == "Player Shield")
        {
            //controllerScript.AttackBlocked ();

            //Instantiate some particles
        }
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        hitPlayer = false;
        attackCollider.enabled = false;
    }
}
