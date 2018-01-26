using UnityEngine;
using System.Collections;

public class FallingState : FSMState
{
    public FallingState() 
    {
        stateID = FSMStateID.Falling;
    }

    public override void Reason(Transform fish)
    {
		if (fish.transform.position.y < 14.4f)
		{
			GameObject theFish = GameObject.FindWithTag("Fishy");
			FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
			fishController.SetTransition(Transition.AtSurface);
		}
    }

    public override void Act(Transform fish)
    {
		Debug.Log("Help Me I think I'm Falling...");
		//GameObject upwater = GameObject.FindWithTag("WaterUp");
		//upwater.collider.enabled = true;
		// try to force the fish to not fall asleep, or get stuck in the falling state
		/*
		if (fish.transform.position.y >= 14.4f)
		{
			Debug.Log ("Don't get stuck in FallingState");
			Vector3 extraGravityVector = new Vector3(0.0f, -10.0f, 0.0f);
			fish.rigidbody.AddForce(extraGravityVector);
		}
		*/
    }
}
