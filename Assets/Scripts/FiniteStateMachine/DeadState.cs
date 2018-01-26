using UnityEngine;
using System.Collections;

public class DeadState : FSMState
{
    public DeadState() 
    {
        stateID = FSMStateID.Dead;
    }

    public override void Reason(Transform fish)
    {
		if (Input.GetKey(KeyCode.Z))
		{
			//fish.GetComponent<FSMFishController>().SetTransition(Transition.Z);
			GameObject theFish = GameObject.FindWithTag("Fishy");
			FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
			fishController.fishWasTapped = false;
			fish.GetComponent<FSMFishController>().SetTransition(Transition.Z);
		}
    }

    public override void Act(Transform fish)
    {
		fish.GetComponent<Rigidbody>().isKinematic = false;
		Debug.Log("I am dead");
		Debug.Log(fish.GetComponent<Rigidbody>().isKinematic);

    }
}
