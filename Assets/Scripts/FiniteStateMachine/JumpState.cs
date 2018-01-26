using UnityEngine;
using System.Collections;

public class JumpState : FSMState
{
	private float jumpForce = 100.0f;

    public JumpState() 
    {
        stateID = FSMStateID.Jump;
    }

    public override void Reason(Transform fish)
    {
		//if (Input.GetKey(KeyCode.J))
		//{
		GameObject theFish = GameObject.FindWithTag("Fishy");
		FSMFishController fishController  = theFish.GetComponent<FSMFishController>();
		fishController.fishWasTapped = false;
		fish.GetComponent<FSMFishController>().SetTransition(Transition.HasJumped);
		//}
    }

    public override void Act(Transform fish)
    {
		Debug.Log("JUMP");
		GameObject GO = GameObject.FindWithTag("Fishy");
		GameObject downwater = GameObject.FindWithTag("WaterDown");
		//GameObject upwater = GameObject.FindWithTag("WaterUp");
		Vector3 currentAngles = GO.GetComponent<Rigidbody>().transform.rotation.eulerAngles;
		Quaternion targetRotation = Quaternion.Euler(-55.0f, currentAngles.y, 0.0f);
		GO.GetComponent<Rigidbody>().transform.rotation = Quaternion.Lerp(GO.GetComponent<Rigidbody>().transform.rotation, targetRotation, 33.0f * Time.fixedDeltaTime);
		
		// need to add an event listener so that the rotation completes before the force is added.
		// In the meantime, multiplied fixedDeltaTime by 33 so that the rotation is finished. It rotates too fast though, almost instant.
		
		Vector3 moveDirection =  new Vector3(GO.GetComponent<Rigidbody>().transform.forward.x, GO.GetComponent<Rigidbody>().transform.forward.y, GO.GetComponent<Rigidbody>().transform.forward.z);

		//FishScript.nonKinematicTime = 3.4f;
		//if (GO.rigidbody.isKinematic == true) {
		downwater.GetComponent<Collider>().enabled = false;
		//upwater.collider.enabled = false;
		GO.GetComponent<Rigidbody>().isKinematic = false;
		GO.GetComponent<FSMFishController>().setRagdollState(true);
		if (GO.GetComponent<Animation>()) 
		{
			GO.GetComponent<Animation>().Stop();
		}
		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
		jumpForce = fishController.jumpForce;
		GO.GetComponent<Rigidbody>().AddForce (moveDirection * jumpForce);
		//FishScript.health -= 1.0f; //jumping takes energy so health takes a hit
    }
}
