// C# script to respond to gestures, attaches to the Gestures game object

using UnityEngine;
using System.Collections;

public class GestureResponses : MonoBehaviour {
	public GameObject GO; // The object to affect, in this case, the fish
	//public GameObject GOchild; // try applying forces to the root bone // not using now
	public GameObject CameraDummy;
	public GameObject CameraDummyChild;
	public GameObject CameraBG;
	public GameObject Waterplane;
	public Camera mainCam;
	public float dragSpeed = 0.3f;
	//public float jumpForce = 1050.0f;
	// The rotational angles of the fish. (The Z angle (banking) will stay at 0 at this point, so it doesn't need a variable)
	float yangle = 0.0f;
	float xangle = 0.0f;

    private Animator animator;
    private FishManager fishManager;
    private Rigidbody rb;
	private float _cumulativeDragAmount;
	private float _waterlevel;

    void Start()
    {
        animator = GO.GetComponent<Animator>();
        rb = GO.GetComponent<Rigidbody>();
        fishManager = GO.GetComponent<FishManager>();
        _waterlevel = fishManager.waterlevel;
        _cumulativeDragAmount = fishManager.cumulativeDragAmount;
    }

    void OnTap( TapGesture gesture ) { 
		Debug.Log("onTap");
		if( gesture.Selection == GO) {// && (FishScript.dead != true) ) { // requires tapping on the fish object to make it jump
			// Set the transition in the State Machine
			FSMFishController fishController  = GO.GetComponent<FSMFishController>();
			//fishController.jumpForce = 2500.0f;
			fishController.fishWasTapped = true;

			/* MOVE THE CODE TO TURN OFF KINEMATIC AND ADDFORCE TO JumpState.cs IN STATE MACHINE
			// To make it easy and fun to play, set the angle of the fish pointing automatically upwards for a good jumping angle (currently -40°)
			Vector3 currentAngles = GO.rigidbody.transform.rotation.eulerAngles;
			Quaternion targetRotation = Quaternion.Euler(0.0f, currentAngles.y, 55.0f);
			GO.rigidbody.transform.rotation = Quaternion.Lerp(GO.rigidbody.transform.rotation, targetRotation, 33.0f * Time.fixedDeltaTime);
			// need to add an event listener so that the rotation completes before the force is added.
			// In the meantime, multiplied fixedDeltaTime by 33 so that the rotation is finished. It rotates too fast though, almost instant.
			Vector3 moveDirection =  new Vector3(GO.rigidbody.transform.right.x, GO.rigidbody.transform.right.y, GO.rigidbody.transform.right.z);
			FishScript.nonKinematicTime = 3.4f;
			TurnOffKinematic(GO);
			GO.rigidbody.AddForce (moveDirection * jumpForce);
			FishScript.health -= 1.0f; //jumping takes energy so health takes a hit
			*/ // END MOVE THE CODE TO STATE MACHINE
		}
	}
	
	// helper function to toggle isKinematic state (User controlled vs. Physics engine controlled)
	void ToggleKinematic( GameObject currentObject ) {
		if (currentObject.GetComponent<Rigidbody>().isKinematic == false) {
			currentObject.GetComponent<Rigidbody>().isKinematic = true;
			if (currentObject.GetComponent<Animation>()) 
			{
				currentObject.GetComponent<Animation>().Play();
			}
			return;
		}
		if (currentObject.GetComponent<Rigidbody>().isKinematic == true) {
			currentObject.GetComponent<Rigidbody>().isKinematic = false;
			if (currentObject.GetComponent<Animation>()) {
				currentObject.GetComponent<Animation>().Stop();
			}
		}
	}

	void TurnOffKinematic( GameObject currentObject ) {
		if (currentObject.GetComponent<Rigidbody>().isKinematic == true) {
			Waterplane.GetComponent<Collider>().enabled = false;
			currentObject.GetComponent<Rigidbody>().isKinematic = false;
			if (currentObject.GetComponent<Animation>()) {
				currentObject.GetComponent<Animation>().Stop();
			}
		}
	}
	
	// Disabled for now, but Swipe gestures will allow the user to control how much force is given to a jump, as well as the approximate direction
    void OnSwipe( SwipeGesture gesture ) { 
		/*
		// gesture velocity in screen units per second
    	float velocity = gesture.Velocity;
 		ToggleKinematic( GO );
    	// Approximate swipe direction
    	FingerGestures.SwipeDirection direction = gesture.Direction;
		
   		if( gesture.Selection ) {
			
        	//Debug.Log( "Swiped object: " + gesture.Selection.name );
			if (GO.rigidbody) {
				//GO.rigidbody.AddForce (Vector3.right * velocity);
				if( gesture.Direction == FingerGestures.SwipeDirection.Left ) {
      	  			GO.rigidbody.AddForce (Vector3.left * velocity);
   		 		} else if( gesture.Direction == FingerGestures.SwipeDirection.Right ) {
      	  			GO.rigidbody.AddForce (Vector3.right * velocity);
   				} else if( gesture.Direction == FingerGestures.SwipeDirection.Up ) {
      	  			GO.rigidbody.AddForce (Vector3.forward * velocity);
   		 		} else if( gesture.Direction == FingerGestures.SwipeDirection.Down ) {
      	  			GO.rigidbody.AddForce (Vector3.back * velocity);
   		 		} else if( gesture.Direction == FingerGestures.SwipeDirection.UpperRightDiagonal ) {
      	  			GO.rigidbody.AddForce ((Vector3.forward + Vector3.right) * (0.7f) * velocity);
   		 		} else if( gesture.Direction == FingerGestures.SwipeDirection.UpperLeftDiagonal ) {
      	  			GO.rigidbody.AddForce ((Vector3.forward + Vector3.left) * (0.7f) * velocity);
   		 		} else if( gesture.Direction == FingerGestures.SwipeDirection.LowerLeftDiagonal ) {
      	  			GO.rigidbody.AddForce ((Vector3.back + Vector3.left) * (0.7f) * velocity);
   		 		} else if( gesture.Direction == FingerGestures.SwipeDirection.LowerRightDiagonal ) {
      	  			GO.rigidbody.AddForce ((Vector3.back + Vector3.right) * (0.7f) * velocity);   		 		}
			}
			
		} else {
        	Debug.Log( "No object was swiped at " + gesture.Position );
    	}*/
	}

	void OnTwist(TwistGesture gesture) { 
		CameraDummy.transform.Rotate(0.0f, gesture.DeltaRotation, 0.0f);
		OrbitCamera.targetOffset = new Vector3(CameraDummyChild.transform.position.x, 0.15f, CameraDummyChild.transform.position.z); 
	}
	
	// Direct user control of the fish's direction, for when the GO isKinematic
    void OnDrag( DragGesture gesture ) { // drag anywhere on the screen, unless on the cam dummy to move the camera position



		ContinuousGesturePhase phase = gesture.Phase;

			animator.SetBool ("dragging", true);


//		if (phase == ContinuousGesturePhase.Ended) 
//		{
//			//Debug.Log("phase: " + phase);
//			//animator.SetBool("dragging", false);
//			// adjust root rotation for ani rotation (should more ideally change gradually as turn ani returns to orig position)
//			GO.GetComponent<Rigidbody>().transform.Rotate(0.0f, 35.0f, 0.0f);
//		}

		// drag rotates dummy camera
		if (gesture.Position.y > (Screen.height * 0.845f)) {
			CameraDummy.transform.Rotate(0.0f, 2.0f * dragSpeed * gesture.DeltaMove.x, 0.0f);
			OrbitCamera.targetOffset = new Vector3(CameraDummyChild.transform.position.x, 0.15f, CameraDummyChild.transform.position.z);
		}
		// drag is happening above the botton area reserved for UX. Therefore, turning can happen
		else if ( gesture.Position.y > (Screen.height * 0.1f) && GO.GetComponent<Rigidbody>().isKinematic) {
			FSMFishController fishController  = GO.GetComponent<FSMFishController>();

            // if the animator is in seesObstacle state, if the user forces the fish to turn (via this function)
            // turn off auto-turning by moving to seesObstacleNoTurningAllowed state. That way, the fish is allowed 
            // to get close to an obstacle, giving the user more control (i.e. if they want to get close enough to 
            // jump over the obstacle. And need to set canAutoTurn before switching to turn state.
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("seesObstacle"))
            {
                animator.SetBool("canAutoTurn", false);
            }
			animator.SetBool("gotoTurnState", true);
			//fishController.turnIsComplete = true; // if a drag to turn is started, it should cancel any automated turning by returning to the SwimState

			// Drag is horizontal
			if ( Mathf.Abs(gesture.DeltaMove.y) <= Mathf.Abs(gesture.DeltaMove.x) ) {
				_cumulativeDragAmount += dragSpeed * gesture.DeltaMove.x;
                fishManager.cumulativeDragAmount = _cumulativeDragAmount;
				//Debug.Log("dcumulativeDragAmount: " + cumulativeDragAmount);

				//animator.SetBool("dragging", true);
				//if (cumulativeDragAmount > 0) {
					GO.GetComponent<Rigidbody>().transform.Rotate(0.0f, -dragSpeed * gesture.DeltaMove.x, 0.0f); // using quaternions for accurate rotation
				//}
				if (gesture.DeltaMove.x < 0) {
					animator.SetBool("turnL", false);
					animator.SetBool("turnR", true);
				} else if (gesture.DeltaMove.x > 0) {
					animator.SetBool("turnR", false);
					animator.SetBool("turnL", true);
				} else { // If drag movement stops, return to turn idle state
					//animator.SetBool("turnL", false);
					//animator.SetBool("turnR", false);
				}

				//Debug.Log ("What is DX?? :" + gesture.DeltaMove.x);

			// Drag is vertical
			} else if ( Mathf.Abs(gesture.DeltaMove.y) > Mathf.Abs(gesture.DeltaMove.x) ) {
				animator.SetBool("turnL", false);
				animator.SetBool("turnR", false);
				float rotateAmount = dragSpeed * gesture.DeltaMove.y;
				if ( rb.transform.position.y > (_waterlevel - 0.2f) )
				{
					rotateAmount = - Mathf.Abs(rotateAmount); // only let the rotation go Down, not Up, if near the surface
				}
				GO.GetComponent<Rigidbody>().transform.Rotate(rotateAmount, 0.0f, 0.0f); // rotating the z
			}
		}
		// touch has ended, leave the 
		if (phase == ContinuousGesturePhase.Ended) {
			animator.SetBool("turnL", false);
			animator.SetBool("turnR", false);
			animator.SetBool("dragging", false);

			_cumulativeDragAmount = 0.0f;
            fishManager.cumulativeDragAmount = 0.0f;
		}
	}
	/*
	void OnPinch(PinchGesture gesture) {
		float fov;
		float delta = gesture.Delta;
		if (mainCam.fieldOfView <= 130.0f && mainCam.fieldOfView >= 30.0f) {
			fov = 0.005f * delta;
		} else {
			delta = 0.0f;
		}
		mainCam.fieldOfView += delta;
		if (mainCam.fieldOfView < 30.0f) {mainCam.fieldOfView = 30.0f;}
		if (mainCam.fieldOfView > 130.0f) {mainCam.fieldOfView = 130.0f;}
	}*/
}