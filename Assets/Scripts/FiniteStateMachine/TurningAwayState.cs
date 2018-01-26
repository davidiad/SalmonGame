using UnityEngine;
using System.Collections;

public class TurningAwayState : FSMState 
{
	public TurningAwayState() 
	{
		stateID = FSMStateID.TurningAway;
	}
	
	public override void Reason(Transform fish)
	{
		Debug.Log("On the TURNING AWAY");
		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
		if (fishController.turnIsComplete)
		{
			fishController.turnIsComplete = false;
			fishController.updateMoveSpeed();
			fishController.SetTransition(Transition.TurnIsComplete);
		}
	}
	
	public override void Act(Transform fish)
	{
		Debug.Log("On the TURNING AWAY");
		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
		fishController.turning(fishController.newDirection); 
	}
}