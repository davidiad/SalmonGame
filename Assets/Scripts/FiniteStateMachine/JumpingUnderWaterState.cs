using UnityEngine;
using System.Collections;

public class JumpingUnderWaterState : FSMState
{
    public JumpingUnderWaterState() 
    {
        stateID = FSMStateID.JumpingUnderWater;
    }

    public override void Reason(Transform fish)
    {
		Debug.Log("JUMPING but Underwater");
		GameObject theFish = GameObject.FindWithTag("Fishy");
		FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
		if (theFish.transform.position.y > 14.0f) // substutute waterlevel variable later
		{ 
			Debug.Log("About to go ABOVE water");
			fish.GetComponent<FSMFishController>().SetTransition(Transition.AboveWater);
		}
    }

    public override void Act(Transform fish)
    {
		Vector3 currentAngles = fish.transform.rotation.eulerAngles;
		Debug.Log (currentAngles);
		Quaternion targetRotation = Quaternion.Euler(0.0f, currentAngles.y, 0.0f);
		fish.transform.rotation = Quaternion.Lerp(fish.transform.rotation, targetRotation, Time.fixedDeltaTime);
		fish.GetComponent<FSMFishController>().returnToSwimming();
    }
}
