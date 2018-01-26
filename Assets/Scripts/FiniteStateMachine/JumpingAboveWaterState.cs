using UnityEngine;
using System.Collections;


public class JumpingAboveWaterState : FSMState
{
	private float lastY = -100.0f; // arbitrary value, but needs to be defined

    public JumpingAboveWaterState() 
    {
        stateID = FSMStateID.JumpingAboveWater;
    }

    public override void Reason(Transform fish)
    {
		Debug.Log("JUMPING ABOVE WATER");
		GameObject theFish = GameObject.FindWithTag("Fishy");
		FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
		if (fish.transform.position.y <= lastY) // fish has reached the apex of jump, so transition to falling state
		{
			fishController.SetTransition(Transition.ReachedApex);
		}
		lastY = fish.transform.position.y;
    }

    public override void Act(Transform fish)
    {
		Debug.Log(lastY);

    }
}
