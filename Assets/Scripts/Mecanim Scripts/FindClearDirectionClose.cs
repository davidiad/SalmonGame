using UnityEngine;
using System.Collections;

public class FindClearDirectionClose : StateMachineBehaviour {

	public Vector3[] directionsToCheck;

    private GameObject fish;
    private Vector3 moveDirection = new Vector3();
    private Quaternion rot;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		initDirectionsToCheck ();
		fish = animator.gameObject;

		// Set new direction for fish
		moveDirection = fish.transform.forward + 0.5f * fish.transform.right;
		rot = Quaternion.LookRotation(moveDirection);

	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		moveDirection = findBestDirection ();
		rot = Quaternion.LookRotation(moveDirection);
		fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, rot, 2.0f * Time.deltaTime);

		// check for the rotation being close (enough) to the target ("best direction") rotation
		// If rotation is close to the target rotation, allow the state to change
		if (Vector3.Dot (fish.transform.forward, moveDirection) > 0.96f) {
			animator.SetBool ("foundClearDirection", true);
		}
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		animator.SetBool ("obstacleIsClose", false);
        animator.SetBool("hasIdled", false);
	}

	private Vector3 findBestDirection() {
		Vector3 bestDirection = new Vector3 (0f, 0f, 0f);
		float[] hitDistances;
		hitDistances = new float[24];
		// set default of -1 for length (means no hit found for length of hitTest)
		for (int i=0; i<hitDistances.Length; i++) {
			hitDistances[i] = -1.0f;
		}
		// cast an array of Rays
		// get the one -- if any -- with the longest clear direction
		RaycastHit hit = new RaycastHit ();
		float hitLength = 16.0f;
		int layermask = (1 << 15) | (1 << 4); // layer 13 is the fish trigger, don't want the ray to detect that
		for (int i=0; i<directionsToCheck.Length; i++) {
			if (Physics.Raycast (fish.transform.position, directionsToCheck[i], out hit, hitLength, layermask)) {
				Debug.DrawRay (fish.transform.position, directionsToCheck[i], Color.red, 2.0f);
				//Debug.Log ("hit dist: " + i + " : " + hit.distance);
				// save hit distances in an array, with a default value of -1.0
				hitDistances[i] = hit.distance;

			// if a direction is found that doesn't return a hit, then use that as best direction
			// That saves from having to check all 24 directions if one is already found
			// That also saves us from, later, having to select on of the no hit directions randomly
			} else { 
				return directionsToCheck[i];
			}
		}
		// find the max value(s) in that array
		int bestDirectionIndex = 0;
		float maxValue = 0.0f;
		for (int i=0; i<hitDistances.Length; i++) {
			if (hitDistances[i] > maxValue) {
				maxValue = hitDistances[i];
				bestDirectionIndex = i;
			}
		}
		// set best direction to that vector with the max value
		return directionsToCheck [bestDirectionIndex];
	}

	// Find a more efficient place to move this so it doesn't need to be calculated more than once.
	private void initDirectionsToCheck() {
		directionsToCheck = new Vector3[24];
		directionsToCheck [0] = Vector3.forward;
		directionsToCheck [1] = (Vector3.forward + Vector3.right).normalized;
		directionsToCheck [2] = Vector3.right;
		directionsToCheck [3] = (Vector3.right + Vector3.back).normalized;
		directionsToCheck [4] = Vector3.back;
		directionsToCheck [5] = (Vector3.back + Vector3.left).normalized;
		directionsToCheck [6] = Vector3.left;
		directionsToCheck [7] = (Vector3.left + Vector3.forward).normalized;
		for (int i=0; i<8; i++) {
			directionsToCheck[i+8] = (directionsToCheck[i] + Vector3.up).normalized;
			directionsToCheck[i+16] = (directionsToCheck[i] + Vector3.down).normalized;
		}
	}
}
