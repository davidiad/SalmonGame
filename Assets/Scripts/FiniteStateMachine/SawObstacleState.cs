using UnityEngine;
using System.Collections;

public class SawObstacleState : FSMState
{
	public SawObstacleState() 
	{
		stateID = FSMStateID.SawObstacle;
	}

	public override void Reason(Transform fish)
	{
		Debug.Log("I taut I taw...");
		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
		if (fishController.foundClearDirection)
		{
			fishController.foundClearDirection = false;
			fishController.SetTransition(Transition.FoundClearDirection);
		}
	}
	
	public override void Act(Transform fish)
	{/*
		fish.rigidbody.isKinematic = false;
		Debug.Log(fish.rigidbody.isKinematic);
	*/	
		Debug.Log("I taut i taw a obstacle");
		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
		fishController.findClearDirection();
	}
}
