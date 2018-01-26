using UnityEngine;
using System.Collections;

public class BumpStateScript : StateMachineBehaviour
{
	public float bumpForce = -20.0f;
	private GameObject GO;
	private Rigidbody GORB;
	private Vector3 reboundDirection;


	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GO = animator.gameObject;
		GORB = GO.GetComponent<Rigidbody>();

		reboundDirection = -GORB.transform.forward;
	}


	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

			float turnAmount = 25.0f;
			Debug.Log("BUMP");
	//		GameObject GO = GameObject.FindWithTag("Fishy"); // why search for GO, can just use Transform fish?
	//
	//		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
	//
	//		// if a bump has recently happened, turn the fish to the opposite direction. Should this be a separate state?
	//		if ( (Time.time - fishController.timeAtPreviousBump) < fishController.timeBetweenBumps )
	//		{
	//			// rotate 180 degrees more or less
	//			turnAmount = 171.0f;
	//		}
			Vector3 bumpDirection =  new Vector3(GORB.transform.forward.x, GORB.transform.forward.y + turnAmount, GORB.transform.forward.z);
			bumpDirection.Normalize();
			// rotate the fish around when it bumps so it moves away from obstacle
	//		//Vector3 moveDirection =  new Vector3(GO.rigidbody.transform.forward.x, GO.rigidbody.transform.forward.y + turnAmount, GO.rigidbody.transform.forward.z);
	//		//moveDirection.Normalize();
	//		//Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
	//		//fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, targetRotation, 10.0f * Time.fixedDeltaTime);
			GO.transform.Rotate(Vector3.up, turnAmount);
	//

			if (GO.GetComponent<Animation>()) {
				GO.GetComponent<Animation>().Stop();
			}
			GORB.isKinematic = false;
			setRagdollState(true);
			animator.enabled = false;

			GORB.AddForce (bumpDirection * bumpForce);
	//		//FishScript.health -= 1.0f; //jumping takes energy so health takes a hit




	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Debug.Log ("Exiting bump state");
		animator.enabled = true;

	}

	// Ragdoll functions repeated from fishAni script. Consolidate in one place
	public void setRagdollState(bool state) {
		// set the parent game object collision detection to opposite of "state"
		// set parent and children isKinematic to opposite of "state"

		// define the opposite boolean and set
		bool oppositeBoolean  = true;
		if (state) { oppositeBoolean = false; }

		GORB.detectCollisions = oppositeBoolean;
		turnOffChildCollidersPhysics(oppositeBoolean);
	}


	public void turnOffChildCollidersPhysics (bool turnOff) {
		//haveTurnedOffPhysics = turnOff;
		foreach (Transform child in GO.transform) {
			turnOffPhysics(child, turnOff);
		}
	}

	public void turnOffPhysics(Transform obj, bool turnOff) {
		if(obj.GetComponent<Animation>()) {
			obj.GetComponent<Animation>().Stop();
		}
		if (obj.GetComponent<Rigidbody>()) 
		{
			obj.GetComponent<Rigidbody>().isKinematic = turnOff;
			obj.GetComponent<Rigidbody>().transform.localPosition = new Vector3(obj.GetComponent<Rigidbody>().transform.localPosition.x, 0.0f, 0.0f);
		}
		// recursively check children (bones)
		foreach (Transform trans in obj)  {
			turnOffPhysics(trans, turnOff);  
		}  
	}

}








	//  REFERENCE from previous bump trigger code
	//		if (!FishScript.bumped) { // don't turn on bumped flag, and hence an automated turn, until a prvious bumped cycle has completed. Trying to avoid too much bumping and turning
	//			FishScript.bumped = true;
	//		}

	//reboundDirection = -GO.GetComponent.<Rigidbody>().transform.right;
	//FishScript.nonKinematicTime = 0.5;
	//GO.GetComponent.<Rigidbody>().isKinematic = false;

	//GO.GetComponent.<Rigidbody>().AddForce (reboundDirection * 100);

	//FishScript.health -= 1.0f; //jumping takes energy so health takes a hit




	// REFERENCE from original version of hand-rolled state machine
//    public BumpState() 
//    {
//        /stateID = FSMStateID.Bump;
//    }

//    public override void Reason(Transform fish)
//    {
//		// Can I not use the Transform fish instead of searching for GameObject?
//		// Keep a count of # of times bumped within an amount of time, so if it's stuck in bumping over and over, it does
//		// something to get out of that, like turning around and facing the other way, or perhaps some random direction
//		GameObject theFish = GameObject.FindWithTag("Fishy");
//		FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
//		fishController.fishWasTapped = false;
//		fish.GetComponent<FSMFishController>().SetTransition(Transition.HasBumped);
//    }

//    public override void Act(Transform fish)
//    {
//		float turnAmount = 5.0f;
//		Debug.Log("BUMP");
//		GameObject GO = GameObject.FindWithTag("Fishy"); // why search for GO, can just use Transform fish?
//
//		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
//
//		// if a bump has recently happened, turn the fish to the opposite direction. Should this be a separate state?
//		if ( (Time.time - fishController.timeAtPreviousBump) < fishController.timeBetweenBumps )
//		{
//			// rotate 180 degrees more or less
//			turnAmount = 171.0f;
//		}
//		Vector3 bumpDirection =  new Vector3(GO.GetComponent<Rigidbody>().transform.forward.x, GO.GetComponent<Rigidbody>().transform.forward.y + turnAmount, GO.GetComponent<Rigidbody>().transform.forward.z);
//		bumpDirection.Normalize();
//		// rotate the fish around when it bumps so it moves away from obstacle
//		//Vector3 moveDirection =  new Vector3(GO.rigidbody.transform.forward.x, GO.rigidbody.transform.forward.y + turnAmount, GO.rigidbody.transform.forward.z);
//		//moveDirection.Normalize();
//		//Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
//		//fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, targetRotation, 10.0f * Time.fixedDeltaTime);
//		fish.transform.Rotate(Vector3.up, turnAmount);
//
//		GO.GetComponent<Rigidbody>().isKinematic = false;
//		if (GO.GetComponent<Animation>()) {
//			GO.GetComponent<Animation>().Stop();
//		}
//		fishController.setRagdollState(true);
//		GO.GetComponent<Rigidbody>().AddForce (bumpDirection * bumpForce);
//		//FishScript.health -= 1.0f; //jumping takes energy so health takes a hit
//    }
