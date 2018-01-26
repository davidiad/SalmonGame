using UnityEngine;
using System.Collections;

public class TurningAndMoving : StateMachineBehaviour {

	private GameObject fish;
	private float turningMoveSpeed = 0.05f;
	
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		fish = animator.gameObject;
		//animator.Play ("turnR", 0, 0.5f);
        turningMoveSpeed = fish.GetComponent<FishManager> ().turningMoveSpeed;

	}
	
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		Vector3 moveDirection = new Vector3();
		moveDirection = fish.transform.forward;
		//Vector3 rot = new Vector3(0.0f, fish.transform.localRotation.y, fish.transform.localRotation.z);
		moveDirection.Normalize();
		Vector3 movement = new Vector3();
		movement = moveDirection * turningMoveSpeed;
		fish.transform.localPosition += movement;
	}
	
}
