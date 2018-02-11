using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the cool down timers for each player ability
//So far includes (animation) timers for:
//  dash
//  attack
//  dodge
public class AbilitiesHandler : MonoBehaviour {
	//Timers
	private float dashCoolDownTimer;
	private float dashChainWindowTimer; //Currently unused
	private float attackCooldownTimer;
	private float dodgeTimer; 

	public int comboCount; //private

	//Public settings -> set by the user
	public float dashCoolDownTime;
	public float dashChainWindowTime;   //Currently unused
	public float attackCoolDownTime;    //How long the cool down time for an attack should last (after combo is performed
	public float attackComboWindowTime; //How long the player has between attacks to chain together a combo
	public float dodgeRollTime;
	public int maxComboCount;

	// Use this for initialization
	void Start () {
		dashCoolDownTimer = 0f;
		attackCooldownTimer = 0f;
	}

	//=============Public Functions====================
	public void HandleTimers() {
		if (!(dashCoolDownTimer <= 0f))
			dashCoolDownTimer -= Time.deltaTime;
		
		if (!(attackCooldownTimer <= 0f)) {
			attackCooldownTimer -= Time.deltaTime;
		} else {
			//Attack combo window is up, so reset comboCount
			comboCount = 0;
		}
		if (!(dodgeTimer <= 0f))
			dodgeTimer -= Time.deltaTime;
	}

	//=============Timer Functions=======================
	public void activateDashTimer() {
		dashCoolDownTimer = dashCoolDownTime;
	}

	public void activateComboWindowTimer() {
		//An attack has just been executed, so increase combo count
		comboCount++;

		if (comboCount == maxComboCount) {
			attackCooldownTimer = attackCoolDownTime;
		} else {
			attackCooldownTimer = attackComboWindowTime;
		}
	}

	public void activateDodgeTimer() {
		dodgeTimer = dodgeRollTime;
	}

	//==============Checker Functions=======================

	//Determines whether Dash ability is available for use
	public bool isDashAvailable() {
		if (dashCoolDownTimer <= 0f)
			return true;
		else
			return false;
	}

	//Determines whether Attack ability is available for use
	//If this is called, it means that an attack has been requested
	public bool isAttackAvailable() {
		//if (attackCooldownTimer <= 0f || comboCount != maxComboCount) {
		if (attackCooldownTimer <= 0f || comboCount != maxComboCount) {
			return true;
		} else {
			return false;
		}
	}

	//Check if timer is up
	public bool isCooldownTimerUp() {
		if (attackCooldownTimer <= 0f)
			return true;
		else
			return false;
	}

	//Checks if the combo max has been reached
	public bool comboMaxReached() {
		if (comboCount == maxComboCount)
			return true;
		else
			return false;
	}
}
