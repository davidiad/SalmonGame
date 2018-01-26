using UnityEngine;
using System.Collections;

public class BumpState : FSMState
{
	public float bumpForce = -20.0f;

    public BumpState() 
    {
        stateID = FSMStateID.Bump;
    }

    public override void Reason(Transform fish)
    {
		// Can I not use the Transform fish instead of searching for GameObject?
		// Keep a count of # of times bumped within an amount of time, so if it's stuck in bumping over and over, it does
		// something to get out of that, like turning around and facing the other way, or perhaps some random direction
		GameObject theFish = GameObject.FindWithTag("Fishy");
		FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
		fishController.fishWasTapped = false;
		fish.GetComponent<FSMFishController>().SetTransition(Transition.HasBumped);
    }

    public override void Act(Transform fish)
    {
		float turnAmount = 5.0f;
		Debug.Log("BUMP");
		GameObject GO = GameObject.FindWithTag("Fishy"); // why search for GO, can just use Transform fish?

		FSMFishController fishController  = fish.GetComponent<FSMFishController>();

		// if a bump has recently happened, turn the fish to the opposite direction. Should this be a separate state?
		if ( (Time.time - fishController.timeAtPreviousBump) < fishController.timeBetweenBumps )
		{
			// rotate 180 degrees more or less
			turnAmount = 171.0f;
		}
		Vector3 bumpDirection =  new Vector3(GO.GetComponent<Rigidbody>().transform.forward.x, GO.GetComponent<Rigidbody>().transform.forward.y + turnAmount, GO.GetComponent<Rigidbody>().transform.forward.z);
		bumpDirection.Normalize();
		// rotate the fish around when it bumps so it moves away from obstacle
		//Vector3 moveDirection =  new Vector3(GO.rigidbody.transform.forward.x, GO.rigidbody.transform.forward.y + turnAmount, GO.rigidbody.transform.forward.z);
		//moveDirection.Normalize();
		//Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
		//fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, targetRotation, 10.0f * Time.fixedDeltaTime);
		fish.transform.Rotate(Vector3.up, turnAmount);

		GO.GetComponent<Rigidbody>().isKinematic = false;
		if (GO.GetComponent<Animation>()) {
			GO.GetComponent<Animation>().Stop();
		}
		fishController.setRagdollState(true);
		GO.GetComponent<Rigidbody>().AddForce (bumpDirection * bumpForce);
		//FishScript.health -= 1.0f; //jumping takes energy so health takes a hit
    }
}
