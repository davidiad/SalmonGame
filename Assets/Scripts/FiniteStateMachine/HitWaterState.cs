using UnityEngine;
using System.Collections;

public class HitWaterState : FSMState
{
    public HitWaterState() 
    {
        stateID = FSMStateID.HitWater;
    }

    public override void Reason(Transform fish)
    {
		Debug.Log("HIT THE WATER");
		GameObject theFish = GameObject.FindWithTag("Fishy");
		FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
		fishController.SetTransition(Transition.UnderWater);
    }

    public override void Act(Transform fish)
    {
		// add a bounce force as fish hits water, before transitioning to jumpingUnderWater state
		GameObject theFish = GameObject.FindWithTag("Fishy");
		//Vector3 currentVelocity = new Vector3(theFish.rigidbody.velocity.x, theFish.rigidbody.velocity.y, theFish.rigidbody.velocity.z);
		theFish.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f , -1.0f ,0.0f) * theFish.GetComponent<Rigidbody>().velocity.y * 40.0f); // mimic the force of the fish falling into the water. Should vary with velocity.
    }
}
