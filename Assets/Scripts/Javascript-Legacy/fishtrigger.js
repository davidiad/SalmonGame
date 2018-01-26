
#pragma strict

var GO : GameObject;
private var reboundDirection : Vector3;

function OnTriggerEnter() {
	//Debug.Log("fish trigger");
	if (!FishScript.bumped) { // don't turn on bumped flag, and hence an automated turn, until a prvious bumped cycle has completed. Trying to avoid too much bumping and turning
		FishScript.bumped = true;
	}
	//Debug.Log("bumped: " + FishScript.bumped);
	reboundDirection = -GO.GetComponent.<Rigidbody>().transform.right;
	FishScript.nonKinematicTime = 0.5;
	GO.GetComponent.<Rigidbody>().isKinematic = false;
	//GO.rigidbody.AddForce (moveDirection * jumpForce);
	GO.GetComponent.<Rigidbody>().AddForce (reboundDirection * 100);
	//GO.rigidbody.transform.Rotate(0.0, 0.0, 90.0);
	//var targetRotation : Quaternion = Quaternion.Euler(GO.rigidbody.transform.forward);
	//GO.rigidbody.transform.rotation = Quaternion.Slerp(GO.rigidbody.transform.rotation, targetRotation, Time.fixedDeltaTime);
	FishScript.health -= 1.0f; //jumping takes energy so health takes a hit
	//var impactFactor : float = collision.gameObject.rigidbody.velocity.magnitude;
	//Debug.Log(impactFactor);
	//FishScript.health -= 0.2 * impactFactor;
}