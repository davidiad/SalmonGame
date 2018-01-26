using UnityEngine;
using System.Collections;

public class TurnLScript : StateMachineBehaviour {
	private GameObject fish;
	private float moveSpeed = 0.05f;
    private Rigidbody rb;
	
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        fish = animator.gameObject;
        rb = fish.GetComponent<Rigidbody>();
	}
	
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		float turnAmount = animator.GetFloat ("turnAmount");
		Vector3 moveDirection = new Vector3();
		moveDirection = fish.transform.forward + -0.5f * fish.transform.right;
		//Vector3 rot = new Vector3(0.0f, fish.transform.localRotation.y, fish.transform.localRotation.z);
		moveDirection.Normalize();
		Vector3 movement = new Vector3();
		movement = moveDirection * moveSpeed;
		fish.transform.localPosition += movement;
		// Update fish rotation
		rb.transform.Rotate(0.0f, -turnAmount, 0.0f);
	}
	
}
