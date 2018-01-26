#pragma strict

var GO : GameObject;

function OnCollisionEnter(collision : Collision) {
	var impactFactor : float = collision.gameObject.GetComponent.<Rigidbody>().velocity.magnitude;
	//Debug.Log(impactFactor);
	FishScript.health -= 0.2 * impactFactor;
}