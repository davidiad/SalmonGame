#pragma strict

private var moveDirection : Vector3;
private var angleBetweenDirectionAndCollisionNormal : float;
private var angleToTurn : int = 90;

function OnCollisionEnter(collision : Collision) {
	
	moveDirection = transform.parent.gameObject.GetComponent.<Rigidbody>().transform.forward;
    // Debug-draw all contact points and normals
    for (var contact : ContactPoint in collision.contacts) {
        Debug.DrawRay(contact.point, contact.normal, Color.red, 8, false);
        Debug.Log("thisCollider: " + contact.thisCollider);
        //Debug.Log("contact point is: " + contact.point);
        //Debug.Log("Direction of travel is: " + transform.parent.gameObject.rigidbody.transform.forward);
        angleBetweenDirectionAndCollisionNormal = Vector3.Angle(moveDirection, contact.normal);
        if (angleBetweenDirectionAndCollisionNormal <= 90)
        {
        	angleToTurn = 110;
        }
        else if (angleBetweenDirectionAndCollisionNormal > 90)
        {
        	angleToTurn = -110;
        }
        //Debug.Log("Angle Change is: " + angleBetweenDirectionAndCollisionNormal);
        GetComponent.<Rigidbody>().transform.parent.gameObject.GetComponent(FishScript).yangle = Vector3.Angle(Vector3.right, contact.normal) + angleToTurn;
        GetComponent.<Rigidbody>().transform.parent.gameObject.GetComponent(FishScript).moveSpeed *= 0.5;
    }
    //Debug.Log("I am: " + rigidbody.tag);
    //Debug.Log("I am colliding with: " + collision.collider);
    //Debug.Log("Contact: " + collision.contacts[0].point);
    
    /*
    Debug.Log("My parent is: " + rigidbody.transform.parent.gameObject.tag);
    if (rigidbody.tag == "fishCollider03") {
    	rigidbody.transform.parent.gameObject.GetComponent(KinematicWithJumping_02).yangle -= 40;
    }
    else if (rigidbody.tag == "fishCollider02") {
    	rigidbody.transform.parent.gameObject.GetComponent(KinematicWithJumping_02).yangle -= 90;
    }
    else if (rigidbody.tag == "fishCollider01") {
    	rigidbody.transform.parent.gameObject.GetComponent(KinematicWithJumping_02).yangle -= 180;
    }
    */
    /*
    var whichColliderHitFirst : String;
	var angleToTurn : int;
	
    switch (whichColliderHitFirst) {

    case "fishCollider01":
    	angleToTurn = 180;
    	break;
    case "fishCollider02":
    	angleToTurn = 90;
    	break;
    case "fishCollider03":
    	angleToTurn = 40;
    	break;
    }
    */
    GetComponent.<Rigidbody>().transform.parent.gameObject.GetComponent(FishScript).isTurning = true;
    GetComponent.<Rigidbody>().transform.parent.gameObject.GetComponent(FishScript).turnOffChildCollidersPhysics(true);
    //rigidbody.transform.parent.gameObject.GetComponent(KinematicWithJumping_02).yangle -= angleToTurn;
    
}

function OnCollisionExit(collision : Collision) {
	Debug.Log("Exited collision");
}