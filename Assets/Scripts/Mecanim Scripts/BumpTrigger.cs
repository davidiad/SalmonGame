using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpTrigger : MonoBehaviour {


	public GameObject GO;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("fishtrig3"))
        {
            if (!other.gameObject.CompareTag("WaterDown"))
            {
                Animator animator = GO.GetComponent<Animator>();
                int bumpedHash = Animator.StringToHash("bumped");
                animator.SetTrigger(bumpedHash);
            }
        }
		// make the trigger sphere very small, and move back inside the fish's body, so that a bump isn't triggered again while in a kinematic state


//		SphereCollider triggerCollider = GetComponent<SphereCollider> ();
//		triggerCollider.radius = 0.01f;
//		float triggerCenterZ = 0.0f;
//		triggerCollider.center.z = triggerCenterZ;


//		if (!FishScript.bumped) { // don't turn on bumped flag, and hence an automated turn, until a previous bumped cycle has completed. Trying to avoid too much bumping and turning
//			FishScript.bumped = true;
//		}

		//reboundDirection = -GO.GetComponent.<Rigidbody>().transform.right;
		//FishScript.nonKinematicTime = 0.5;
		//GO.GetComponent.<Rigidbody>().isKinematic = false;

		//GO.GetComponent.<Rigidbody>().AddForce (reboundDirection * 100);

		//FishScript.health -= 1.0f; //jumping takes energy so health takes a hit

	}




}