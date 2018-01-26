using UnityEngine;
using System.Collections;

public class jumpStateScript : StateMachineBehaviour {

	GameObject go;
	GameObject rotationDummy;
	Rigidbody gorb;
    // should move these vars to FishManager
	private float initialJumpforce = 1000.0f;
    public float jumpAngle = 55.0f;
	private Vector3 jumpDirection;
	Quaternion targetRotation;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		go = animator.gameObject;
		gorb = go.GetComponent<Rigidbody>();
		rotationDummy = new GameObject();
        rotationDummy.transform.eulerAngles = new Vector3(-jumpAngle, go.transform.eulerAngles.y, 0.0f);
		//Vector3 currentAngles = gorb.transform.rotation.eulerAngles;
		jumpDirection = rotationDummy.transform.forward;
		targetRotation = Quaternion.LookRotation (jumpDirection);
		//targetRotation = Quaternion.Euler(jumpDirection);
		//Vector3 moveDirection =  new Vector3(gorb.transform.forward.x, gorb.transform.forward.y, gorb.transform.forward.z);

		GameObject[] downwaters = GameObject.FindGameObjectsWithTag("WaterDown");
		foreach (GameObject downwater in downwaters) {
			downwater.GetComponent<Collider>().enabled = false;
		}
        animator.SetBool("canAutoTurn", true);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		gorb.transform.rotation = Quaternion.Slerp(gorb.transform.rotation, targetRotation, 3.5f * Time.deltaTime);

		if ((Vector3.Dot (go.transform.forward, jumpDirection)) > 0.98) {
			gorb.isKinematic = false;
			setRagdollState(true);
			animator.enabled = false;
            Vector3 forceVector = initialJumpforce * jumpDirection;
			gorb.AddForce(forceVector);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.enabled = true;
	}


	// Ragdoll functions repeated from fishAni script. Consolidate in one place
	public void setRagdollState(bool state) {
		// set the parent game object collision detection to opposite of "state"
		// set parent and children isKinematic to opposite of "state"
		
		// define the opposite boolean and set
		bool oppositeBoolean  = true;
		if (state) { oppositeBoolean = false; }
		
		go.GetComponent<Rigidbody>().detectCollisions = oppositeBoolean;
		turnOffChildCollidersPhysics(oppositeBoolean);
	}
	
	
	public void turnOffChildCollidersPhysics (bool turnOff) {
		//haveTurnedOffPhysics = turnOff;
		foreach (Transform child in go.transform) {
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
