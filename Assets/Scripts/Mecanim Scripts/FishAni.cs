using UnityEngine;
using System.Collections;

public class FishAni : MonoBehaviour 
{
	public Vector3 chdir;
	public float waterlevel = 14.7f;
	public float cumulativeDragAmount;
	public float moveSpeed;
	public float turningMoveSpeed; // speed of forward motion while turning under user control
	Animator anim;

	int tapHash = Animator.StringToHash("tap");
	float kinematicTimer;
	public float nonKinematicTime;
	bool goneAboveWater;
	private SphereCollider triggerCollider;

	void Start () {
		anim = GetComponent<Animator>();
		chdir = new Vector3 (0.0f, 0.0f, 0.0f);
		kinematicTimer = 0.0f;
		nonKinematicTime = 2.0f;
		goneAboveWater = false;
		GameObject bumpTrigger = GameObject.FindGameObjectWithTag("fishtrig3");
		triggerCollider = bumpTrigger.GetComponent<SphereCollider> ();
	}
	
	// Should use FixedUpdate here?
	void Update () 
	{
		if (transform.position.z > 166.5f) {
			waterlevel = 19.7f;
		} else {
			waterlevel = 14.7f;
	}

		if ((transform.position.y > (waterlevel + 0.8f)) && !(GetComponent<Rigidbody> ().isKinematic)) {
			goneAboveWater = true;
		}

		// mimic the force of the fish falling into the water. Should vary with velocity.
		if (goneAboveWater && (transform.position.y < (waterlevel + 0.8f)) && (transform.position.y > (waterlevel + 0.5f))) {
			float speed = 25.0f * GetComponent<Rigidbody> ().velocity.y;
			GetComponent<Rigidbody>().AddForce (Vector3.up * speed); 
		}

		if (!(GetComponent<Rigidbody> ().isKinematic)) 
		{
			MinimizeTrigger ();
			// start timer
			if (transform.position.y < (waterlevel + 0.3f)) {
				if (kinematicTimer < 0.5f) {
					kinematicTimer += Time.deltaTime;
				// if > .5s, and hasn't gone above water, return to kinematic
				} else if (!goneAboveWater) { 
					returnToKinematic();
				// if has gone above water, return to kin. in 2 or 3 s
				} else if (kinematicTimer < nonKinematicTime) {
					kinematicTimer += Time.deltaTime;
				} else {
					returnToKinematic();
				}
			}

		}



//		if (goneAboveWater) {
//			if (transform.position.y < 15.0f) {
//				if (kinematicTimer < nonKinematicTime) {
//					kinematicTimer += Time.deltaTime;
//				} else {
//					kinematicTimer = 0.0f;
//					goneAboveWater = false;
//					GetComponent<Rigidbody>().isKinematic = true;
//					// Turn the animator state machine back on, now that we are done with using physics engine
//					anim.enabled = true;
//					// set the state to swim, otherwise it picks up where it left off with jump
//					anim.CrossFade("swim", 0.0f);
//					GameObject downwater = GameObject.FindWithTag("WaterDown");
//					downwater.GetComponent<Collider>().enabled = true;
//					// need to turn kinematic back on
//					setRagdollState(false);
//				}
//			}
//		}// else { // Has not gone above water. // If non-Kinematic, and 1/2 second has passed, return it to kinematic


		// trigger to jump state
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		if(Input.GetMouseButtonDown(0))
		{
			GameObject fish = GameObject.FindGameObjectWithTag("Fishy");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit))
			{
				if ( GameObject.Find(hit.collider.gameObject.name) == fish)
				{
					anim.SetTrigger (tapHash);
				}
			}
		}
	}

	private void MinimizeTrigger() {
		
		if (triggerCollider != null) {
			triggerCollider.radius = 0.01f;
			triggerCollider.center = new Vector3 (0, 0, 0);
		}
	}

	private void MaximizeTrigger() {

		if (triggerCollider != null) {
			triggerCollider.radius = 0.15f;
			triggerCollider.center = new Vector3 (0f, 0.1f, 1.0f);
		}
	}

	public void returnToKinematic() {
		kinematicTimer = 0.0f;
		goneAboveWater = false;
		GetComponent<Rigidbody>().isKinematic = true;
		// Turn the animator state machine back on, now that we are done with using physics engine
		anim.enabled = true;
		// set the state to swim, otherwise it picks up where it left off with in jumping state
		anim.CrossFade("swim", 0.0f);

		GameObject[] downwaters = GameObject.FindGameObjectsWithTag("WaterDown");
		foreach (GameObject downwater in downwaters) {
			downwater.GetComponent<Collider>().enabled = true;
		}
		// need to turn kinematic back on
		setRagdollState(false);
		MaximizeTrigger ();
	}

	public void setRagdollState(bool state) {
		// set the parent game object collision detection to opposite of "state"
		// set parent and children isKinematic to opposite of "state"
		
		// define the opposite boolean and set
		bool oppositeBoolean  = true;
		if (state) { oppositeBoolean = false; }
		
		GetComponent<Rigidbody>().detectCollisions = oppositeBoolean;
		turnOffChildCollidersPhysics(oppositeBoolean);
	}
	
	
	public void turnOffChildCollidersPhysics (bool turnOff) {
		//haveTurnedOffPhysics = turnOff;
		foreach (Transform child in transform) {
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
