using UnityEngine;
using System.Collections;

public class TurnRScript : StateMachineBehaviour {

    private GameObject fish;
    private FishManager fishManager;
	private float moveSpeed = 0.05f;
	private float _cumulativeDragAmount;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		fish = animator.gameObject;
        fishManager = fish.GetComponent<FishManager>();
		_cumulativeDragAmount = 0.0f;
		//fish.GetComponent<FishAni> ().cumulativeDragAmount = 0.0f;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (animator.GetBool ("turnR")) {
            _cumulativeDragAmount = fishManager.cumulativeDragAmount;
			
			animator.Play ("turnR", 0, _cumulativeDragAmount / 50.0f);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//	//animator.Play ("turning", 0, 0.0f);
	//}

//	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
//	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
//	{
//
//	}

}
