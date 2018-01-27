#pragma strict

// Javascript. The FixedUpdate function is frame independent, and contains automated controls for the motion of the fish where needed.
var underwaterGravity : float = -3.0; // reduce gravity to account for bouyancy when underwater
var waterlevel : float = 13.964; // needs to be about the same as the y of the water planes
var moveSpeed : float = 0.02f;
var boundXmin : float = -8.0;
var boundXmax : float = 25.0;
var boundYmin : float = 6.0;
var boundYmax : float = 13.5;
var boundZmin : float = -4.0;
var boundZmax : float = 197.0;
var splash : Transform;

var yangle : float;
var isTurning : boolean;
var haveTurnedOffPhysics : boolean  = true;

var streambedMask : LayerMask;

private var normalGravity : float = -9.81;
private var lastY : float;
private var goingUp : boolean;
private var hasGoneAboveWater : boolean = false;
private var hasGoneUp : boolean = false;
private var currentAngles : Vector3; // may be able to move this to the moveInbounds() function
static var nonKinematicTime : float = 3.3;//3.0; // amount of time the fish will stay non-kinematic before switching to kinematic (if underwater)
var lookForObstaclesTimer : int = 5; // have lookForObstacles wait until it fires twice, to reduce jitter
private var kinematicTimer : float;
private var hit : RaycastHit;
static var dead : boolean;

static var health : float = 100.0;
static var bumped : boolean = false;

var camDummy : GameObject;
var camDummyChild : GameObject;
var waterplane : GameObject;
private var rotationHasChanged : boolean = false;
private var direction : Vector3;

function Start () { 
	//bug.Log("Timer: " + lookForObstaclesTimer);Debug.Log("dummy child is at:" + camDummyChild.transform.position);
	dead = false;
	turnOffChildCollidersPhysics(true);	
	//Physics.gravity = Vector3(0, underwaterGravity, 0);
	lastY = GetComponent.<Rigidbody>().position.y;
	currentAngles = GetComponent.<Rigidbody>().transform.rotation.eulerAngles;
	// due to mesh facing in the x direction, need to turn the fish initially so it's facing towards the logs
	var targetRotation = Quaternion.Euler (currentAngles.x, currentAngles.y - 90.0, 0.0);
	GetComponent.<Rigidbody>().transform.rotation = Quaternion.Slerp(GetComponent.<Rigidbody>().transform.rotation, targetRotation, 100 * Time.fixedDeltaTime);
}

function FixedUpdate () {
	// decrease the health slightly due to passage of time
	health -= .005;
	if (health <= 0.0 && !dead) {
		die();
	}
	// if the fish is at the water surface, get the position of the fish, and instantiate a splash particle animation there and run it one shot
	if (Mathf.Abs(GetComponent.<Rigidbody>().position.y - waterlevel) < 0.9) {
		//if (!splash) {
			Splash();
		//}
	}
	if (GetComponent.<Rigidbody>().isKinematic) {
	// in case the user, or automated controls, "drive" the fish above the water surface, s/he falls back into the water
		if (GetComponent.<Rigidbody>().position.y > (waterlevel + 0.7)) {
			Debug.Log("Gone ABOVE!");
			nonKinematicTime = 0.5; // but set the timer to a shorter interval so the fish recovers more quickly than from a jump
			GetComponent.<Rigidbody>().isKinematic = false;
			Physics.gravity = Vector3(0, normalGravity, 0);
		}
		/*
		if ( inBounds() ) {
			currentAngles = rigidbody.transform.rotation.eulerAngles;
			if (Mathf.Abs(currentAngles.x) > 0.5) { //IDK why the z angle gets tilted at times, but this is a fix
				var targetRotation = Quaternion.Euler (0.0, currentAngles.y, currentAngles.z);
				rigidbody.transform.rotation = Quaternion.Slerp(rigidbody.transform.rotation, targetRotation, 8 * Time.fixedDeltaTime);			
			}
		} else {
			moveInBounds();
		}
		*/
		//if (!inBounds()) { moveInBounds(); }
		//else { // only moveInBounds or turnUpRight and lookForObstacles, but not all in the same update
			turnUpright();
			//lookForObstacles();
			avoidObstacles();
		//}
		move();
		GetComponent.<Animation>()["walk"].wrapMode = WrapMode.Loop;
	}
	/*
	// alternative method for returning the fish to its kinematic state after a jump. 
	// Tests for conditions, that the fish jumped above water, started to come back down, and hit the water again
	// Replace by returnToKinematic(), below, which uses a timer to return the fish to kinematic state
	if (!rigidbody.isKinematic) {
		// 1. Check whether fish has gone above water
		if (rigidbody.position.y > waterlevel) {
			hasGoneAboveWater = true;
			//Debug.Log("hasGoneAboveWater ought to be true");
		}
		// 2. Check whether the fish has gone Up since going about the water
		if (hasGoneAboveWater) {
			if (rigidbody.position.y <= lastY) {
				goingUp = false;
			} else {
			 	goingUp = true;
			}
			lastY = rigidbody.position.y;
		}
		// 3. Check whether the fish goes thru the water surface again, this time presumably on the way down
		if (hasGoneAboveWater && !goingUp && (rigidbody.position.y < (waterlevel - 2.5))) { 
			//Debug.Log("Ought to be Kinematic again");
			rigidbody.isKinematic = true;
			resetFishStatus();
		}
	}
	*/
	if (GetComponent.<Rigidbody>().position.y > waterlevel) {
		// recenter collider once the initial force has been applied
	    var col : CapsuleCollider = gameObject.GetComponent.<Collider>() as CapsuleCollider;
    	col.center = Vector3(-0.34,0.0,0.0);
    	
		Physics.gravity = Vector3(0, normalGravity, 0);
		hasGoneAboveWater = true;
		if (GetComponent.<Rigidbody>().position.y <= lastY) {
			goingUp = false;
		} else {
			goingUp = true;
		}
		lastY = GetComponent.<Rigidbody>().position.y;
	}
	/*
	// wait until fish is above the water to switch to ragdoll state
	if (haveTurnedOffPhysics && !rigidbody.isKinematic && rigidbody.transform.localPosition.y > waterlevel) {
		//if (haveTurnedOffPhysics) { // makes sure that state is changed just once, not every update
			setRagdollState(true);
		//}
	}*/
	if (!GetComponent.<Rigidbody>().isKinematic && GetComponent.<Rigidbody>().transform.localPosition.y < waterlevel + 0.6) {
		//Debug.Log("rigidbody.detectCollisions: " + rigidbody.detectCollisions);
		
		if (haveTurnedOffPhysics) { // makes sure that state is changed just once, not every update
			setRagdollState(true);
		}
		Physics.gravity = Vector3(0, underwaterGravity, 0);
		if (hasGoneAboveWater && !goingUp) {
			//Debug.Log("velocity.y is: " + rigidbody.velocity.y);
			GetComponent.<Rigidbody>().AddForce (Vector3(0,1,0) * GetComponent.<Rigidbody>().velocity.y * 5.0); // mimic the force of the fish falling into the water. Should vary with velocity.
			hasGoneAboveWater = false;
		}
		if (!dead) {
			returnToKinematic();
		}
	}
}

function turnUpright() {
	currentAngles = GetComponent.<Rigidbody>().transform.rotation.eulerAngles;
	var targetRotation : Quaternion;
	var needToTurn : boolean = false;
	if (bumped) { // do a quick turn to the side if fish has bumped into something (fishtrigger has been triggered)
		//Debug.Log("BUMPED!!!!!!!!!!!!!!!!!!");
		//Debug.Log("CA before: " + currentAngles.y);
		currentAngles = Vector3(currentAngles.x, currentAngles.y + 110.0, currentAngles.z);
		//Debug.Log("CA after: " + currentAngles.y);
		targetRotation = Quaternion.Euler (currentAngles.x, currentAngles.y, currentAngles.z);
		//rigidbody.transform.rotation = targetRotation;
		//rigidbody.transform.rotation = Quaternion.Slerp(rigidbody.transform.rotation, targetRotation, 4 * Time.fixedDeltaTime);
		needToTurn = true;
	}
	if (Mathf.Abs(currentAngles.x) > 2.5) { //IDK why the z angle gets tilted at times, but this is a fix
		targetRotation = Quaternion.Euler (0.0, currentAngles.y, currentAngles.z);
		needToTurn = true;
	}
	if (Mathf.Abs(currentAngles.z) > 40 && Mathf.Abs(currentAngles.z) <= 320) {
		targetRotation = Quaternion.Euler (0.0, currentAngles.y, 0.0);
		Debug.Log("Uprighting Z");
		needToTurn = true;
	}
	if (needToTurn) {
		GetComponent.<Rigidbody>().transform.rotation = Quaternion.Slerp(GetComponent.<Rigidbody>().transform.rotation, targetRotation, 2 * Time.fixedDeltaTime);	
	}
	if (bumped) {
		yield WaitForSeconds (0.9);
		bumped = false;
	}
}	

//Calculate the new directional vector to avoid the obstacle
function avoidObstacles() {
	var hit : RaycastHit;
	var hitLength: float = 9.0;

        //Only detect layer 8 (Obstacles)
        //int layerMask = 1 << 8;

    //Check that the fish hit with the obstacles is within its minimum distance to avoid
	if (Physics.Raycast(transform.position, transform.right, hit, hitLength, streambedMask)) {
		Debug.DrawRay(transform.position, GetComponent.<Rigidbody>().transform.right, Color.yellow, hitLength);
    	//Get the normal of the hit point to calculate the new direction
		var hitNormal : Vector3 = hit.normal;

        //Get the new directional vector by adding force to vehicle's current forward vector
        direction = transform.right + hitNormal;// * force;
        direction.Normalize();
        //Rotate the fish to its target directional vector
        var rot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2 * Time.fixedDeltaTime);
    }
}

function lookForObstacles() {
	if (rotationHasChanged && lookForObstaclesTimer > 0) {
		lookForObstaclesTimer -= 1;
	} else if (lookForObstaclesTimer <= 0) {
		lookForObstaclesTimer = 1;
		rotationHasChanged = false;
	}
	//Debug.Log("Timer: " + lookForObstaclesTimer);
	currentAngles = GetComponent.<Rigidbody>().transform.rotation.eulerAngles;
	//turnUpright(); // first, make sure fish is upright, else all the angles will be off
	var hitLength: float = 5.0;
	// 1. check if there is a hit straight ahead
	Debug.DrawRay(transform.position, GetComponent.<Rigidbody>().transform.right, Color.yellow, hitLength);
	if (lookForObstaclesTimer > 0 && Physics.Raycast (transform.position, GetComponent.<Rigidbody>().transform.right, hit, 2 * hitLength, streambedMask)) {
	var hitNormal : Vector3 = hit.normal;
	var hitDistance : float = hit.distance;
	Debug.Log("hitNorm: " + hitNormal);
	Debug.Log("hitDistance: " + hitDistance);
	//if (lookForObstaclesTimer >= 2) {
		//moveSpeed = 0.08;
		//Debug.Log("a hit: " + hit.collider);
		//Debug.DrawRay(transform.position, hit.normal, Color.blue, 3);
		// hit test at alternating angles, expanding out, and then head in the first direction with no hit.. chk up ad down separately from horizontal
		var yRotChange : float = 40.0; // if this is in degrees, then needs to be converted with trig to a vector
		var zRotChange : float = 25.0;
		// update the current angles
		var dirChange : Vector3 = Vector3(0.0, 0.0, yRotChange);
		//var dirChangeNormalized : Vector3 = Vector3.Normalize(dirChange);
		//var newDirection : Vector3 = rigidbody.transform.right + Vector3.Normalize(Vector3(0.0, 0.0, yRotChange));
		var dirChanges = new Vector3[4];
		var newDirections = new Vector3[4];
		var dirHits = new boolean[4]; // whether or not a ray in a particular direction has hit
		//direChanges[0] = Vector3( Mathf.Sin(yRotChange), 0.0, Mathf.Cos(yRotChange) );
		/*
		newDirections[0] = rigidbody.transform.right + Vector3.Normalize(Vector3(0.0, 0.0, yRotChange));;
		newDirections[1] = rigidbody.transform.right + Vector3.Normalize(Vector3(0.0, 0.0, -yRotChange));
		newDirections[2] = rigidbody.transform.right + Vector3.Normalize(Vector3(0.0, zRotChange, 0.0 ));;
		newDirections[3] = rigidbody.transform.right + Vector3.Normalize(Vector3(0.0, -zRotChange, 0.0));
		*/
		//newDirections[0] = Vector3(Mathf.Cos((currentAngles.y + yRotChange) * Mathf.Deg2Rad), 0.0, Mathf.Sin((currentAngles.y + yRotChange) *  Mathf.Deg2Rad));
		newDirections[0] = Quaternion.AngleAxis(yRotChange, Vector3.up) * GetComponent.<Rigidbody>().transform.right;
		newDirections[1] = Quaternion.AngleAxis(-yRotChange, Vector3.up) * GetComponent.<Rigidbody>().transform.right;
		newDirections[2] = Quaternion.AngleAxis(zRotChange, Vector3.forward) * GetComponent.<Rigidbody>().transform.right;
		newDirections[3] = Quaternion.AngleAxis(-zRotChange, Vector3.forward) * GetComponent.<Rigidbody>().transform.right;
		//newDirections[1] = rigidbody.transform.right + Vector3.Normalize(Vector3(0.0, Mathf.Sin(-yRotChange), 0.0));
		//newDirections[2] = rigidbody.transform.right + Vector3(Mathf.Cos(zRotChange), Mathf.Sin(zRotChange), 0.0);
		//newDirections[3] = rigidbody.transform.right + Vector3(Mathf.Cos(-zRotChange), Mathf.Sin(-zRotChange), 0.0);
		
		//Debug.Log("currentAngles: " + angles);
		//Debug.Log("moveDirection: " + moveDirection);

		//var targetRotation = Quaternion.LookRotation(moveDirection);
		
		var hitColor : Color;
		for (var i:int=0; i<4; i++) {
			if (Physics.Raycast(transform.position, newDirections[i], hit, hitLength, streambedMask)) {
				dirHits[i] = true;
			}
			else {
				dirHits[i] = false;
			}
			if (i<2) { hitColor = Color.green; } else {hitColor = Color.cyan;}
			Debug.DrawRay(transform.position, newDirections[i], hitColor, hitLength);
		}
		if (!rotationHasChanged && dirHits[0] && !dirHits[1]) { 
			yRotChange = -yRotChange;
			rotationHasChanged = true;
		} else if (!rotationHasChanged && !dirHits[0] && dirHits[1]) { 
			rotationHasChanged = true;
		} else if (!dirHits[0] && !dirHits[1]) { yRotChange = 0.0; } // reset the angles to turn to 0 if no obstacles in that plane
		else if (dirHits[0] && dirHits[1]) {
			yRotChange *= 2;
			rotationHasChanged = true;
		}

		// if both directions hit, or if negative direction hit, keep turn direction the same
		// reduce speed if both hit?
		if (dirHits[2] && !dirHits[3]) { zRotChange = -zRotChange; rotationHasChanged = true; }
		else if (!dirHits[2] && dirHits[3]) { rotationHasChanged = true; }
		else if (!dirHits[2] && !dirHits[3]) { zRotChange = 0.0; } // reset the angles to turn to 0 if no obstacles in that plane
		if (dirHits[0] && dirHits[1] && dirHits[2] && dirHits[3]) { moveSpeed = 0.075; }
		if (yRotChange == 0.0 && zRotChange == 0.0) {
			lookForObstaclesTimer = 1;
			rotationHasChanged = false;
		} else {
			rotationHasChanged = true;
		}
		var targetAngles : Vector3 = Vector3(currentAngles.x, currentAngles.y + yRotChange, currentAngles.z + zRotChange);
		var targetRotation : Quaternion = Quaternion.Euler(targetAngles);
			//yRotChange = 0.0;
			//Debug.Log("Other way");
			//dirChange = Vector3(0.0, 0.0, yRotChange);
			//newDirection = rigidbody.transform.right + Vector3.Normalize(dirChange);
			//targetAngles = Vector3(currentAngles.x, currentAngles.y + yRotChange, 0.0);
			//targetRotation = Quaternion.Euler(targetAngles);
		
		//dirChange = Vector3(0.0, 0.0, yRotChange);
		//moveDirection = rigidbody.transform.right + Vector3.Normalize(dirChange);
		// slow down if hit
		//targetAngles = Vector3(currentAngles.x, currentAngles.y + yRotChange, 0.0);
		////targetRotation = Quaternion.Euler (targetAngles);
		//targetRotation = Quaternion.LookRotation(moveDirection);
		//Debug.Log("before" + targetRotation);
		//if (Mathf.Abs(currentAngles.z) < 60 || Mathf.Abs(currentAngles.z) >= 300) {

		//if (lookForObstaclesTimer >= 9) { // reduce jitter by only updating direction every tenth time
			GetComponent.<Rigidbody>().transform.rotation = Quaternion.Slerp(GetComponent.<Rigidbody>().transform.rotation, targetRotation, 2 * Time.fixedDeltaTime);
		//}
		//}
		//Debug.Log("after" + targetRotation);
		//}
	} else {
		//Debug.Log("no hit");
		// back to normal speed if no hit
		moveSpeed = 0.1;
		//lookForObstaclesTimer = 10;
	}
//	Debug.Log("rotationHasChanged: " + rotationHasChanged);
}

function returnToKinematic() {
	// the fish has been given over to control of the physics engine, and now has re-entered (or is still in) the water,
	// so set a timer for how long to wait before returning to isKinematic state
    
    // add to time on the Kinematic timer
    //rigidbody.useGravity = false;
    if (nonKinematicTime > kinematicTimer) {
    	kinematicTimer += Time.deltaTime;
    	//Debug.Log("kinematicTimer = " + kinematicTimer);
    	if (nonKinematicTime <= kinematicTimer) {
    		GetComponent.<Rigidbody>().isKinematic = true;
    		avoidObstacles();
    		waterplane.GetComponent.<Collider>().enabled = true; // enabled so that raycasts will 'see' it
    		if (!bumped) { // trying to avoid conflicts between turning upright and turning after a bump
    			turnUpright();
    		}
    		setRagdollState(false);
 			if (GetComponent.<Animation>()) {
				GetComponent.<Animation>().Play();
			}
    		kinematicTimer = 0;	
    		nonKinematicTime = 3.18;//3.0; // reset to original amount; used a shorter amount for when the fish is "driven" by user above water
    		// reset the fish's vertical position to upright:
			GetComponent.<Rigidbody>().transform.localEulerAngles.x = Mathf.Lerp(GetComponent.<Rigidbody>().transform.localEulerAngles.x, 0, Time.fixedDeltaTime);
			//if (bumped) {
			//	rigidbody.transform.localEulerAngles.y = Mathf.Lerp(rigidbody.transform.localEulerAngles.y + 90.0, 0, Time.fixedDeltaTime);
    		//}
    		GetComponent.<Rigidbody>().transform.localEulerAngles.z = Mathf.Lerp(GetComponent.<Rigidbody>().transform.localEulerAngles.z, 0, Time.fixedDeltaTime);
    		//var targetRotation : Quaternion = Quaternion.Euler(rigidbody.transform.forward);
			//rigidbody.transform.rotation = Quaternion.Slerp(rigidbody.transform.rotation, targetRotation, Time.fixedDeltaTime);
    		/*
    		// loop through all the bones and smoothly reset the rotations to 0
    		var allChildren = gameObject.GetComponentsInChildren(Transform);
			for (var child : Transform in allChildren) {
   				 child.transform.localEulerAngles = Vector3.Slerp(rigidbody.transform.localEulerAngles, Vector3(0,0,0), Time.fixedDeltaTime);
			}	
			*/
    	}
    }
}

// i think this function is not needed when using returnToKinematic() with a timer
function resetFishStatus() {
	hasGoneAboveWater = false;
	goingUp = false;
	//var currentXangle = rigidbody.transform.localEulerAngles.x;
	//var currentYangle = rigidbody.transform.localEulerAngles.y;
	//var currentZangle = rigidbody.transform.localEulerAngles.z;
	//rigidbody.transform.localEulerAngles = Vector3(Mathf.Lerp(currentXangle, 0, Time.fixedDeltaTime), currentYangle, Mathf.Lerp(currentZangle, 0, Time.fixedDeltaTime));
	GetComponent.<Rigidbody>().transform.localEulerAngles.x = 0.0;
	GetComponent.<Rigidbody>().transform.localEulerAngles.z = 0.0;
	moveInBounds(); // need to check again for isKinematic?
}

// for now, the bounds are simply rectangular boxes, which results in a lot of area where the fish is prevented from going
// and sometimes the fish goes under the terrain where it sticks above the bounds.
// in the future, attempt to check bounds by creating a mesh object roughly the size of the water channel, and testing for collision with that object
// (e.g. the fish is inside the bounds object)
// then, to move inbounds, would have to find the nearest point on the bounds object, and move the fish inside, along the normal of that point
function inBounds() {
	// test for object being inside bounds
	/*
	if (rigidbody.transform.position.y > boundYmin &&
		rigidbody.transform.position.y < boundYmax &&
		rigidbody.transform.position.x > boundXmin &&
		rigidbody.transform.position.x < boundXmax &&
		rigidbody.transform.position.z > boundZmin &&
		rigidbody.transform.position.z < boundZmax &&
		// avoid area with logs
		(rigidbody.transform.position.z < 38.0 || rigidbody.transform.position.z > 52.0) ) {
		return true;
	}
	*/
	// simpler version since we are using raycast "vision" (lookForObstacles()
	if (GetComponent.<Rigidbody>().transform.position.y < boundYmax &&
		GetComponent.<Rigidbody>().transform.position.z > boundZmin &&
		GetComponent.<Rigidbody>().transform.position.z < boundZmax) {
		return true;
	}
	return false;
}

function moveInBounds() {  // is there a way to combine this function with inBounds(), so that the bounds checking doesn't have to be done twice?
	if (GetComponent.<Rigidbody>().isKinematic && !inBounds() ) { // should be able to eliminate this !inBounds() check, since this function checks bounds again
		var xPosTarget : float;
		var yPosTarget : float;
		var zPosTarget : float;
		var xRotChange : float = 20.0;
		var yRotChange : float = 100.0;
		/*
		if (rigidbody.transform.position.x >= boundXmax) {
			xPosTarget = boundXmax - 1.0;
		} else if (rigidbody.transform.position.x < boundXmin) {
			xPosTarget = boundXmin + 1.0;
		} else {
			xPosTarget = rigidbody.transform.position.x;
		}
		*/
		xPosTarget = GetComponent.<Rigidbody>().transform.position.x;
		/*
		if (rigidbody.transform.position.y <= boundYmin) {
			yPosTarget = boundYmin + 0.5;
			xRotChange = -xRotChange; // if fish hits the bottom boundary, rotate up rather than down
		} else if (rigidbody.transform.position.y > boundYmax) {
			yPosTarget = boundYmax - 0.5;
		} else {
			yPosTarget = rigidbody.transform.position.y;
		}
		*/
		if (GetComponent.<Rigidbody>().transform.position.y > boundYmax) {
			yPosTarget = boundYmax - 0.5;
		} else {
			yPosTarget = GetComponent.<Rigidbody>().transform.position.y;
		}
		
		if (GetComponent.<Rigidbody>().transform.position.z >= boundZmax) {
			zPosTarget = boundZmax - 1.0;
		} else if (GetComponent.<Rigidbody>().transform.position.z < boundZmin) {
			zPosTarget = boundZmin + 1.0;
		// if the fish is caught in the logs
		//} else if (rigidbody.transform.position.z > 38.0 && rigidbody.transform.position.z < 52.0) {
		//	zPosTarget = 36.0;
		} else {
			zPosTarget = GetComponent.<Rigidbody>().transform.position.z;
		}
		
		// rotation back into bounds
		currentAngles = GetComponent.<Rigidbody>().transform.rotation.eulerAngles;
		//Debug.Log("currentAngles is: " + currentAngles);
		var targetRotation = Quaternion.Euler (currentAngles.x + xRotChange, currentAngles.y + yRotChange, 0.0);
		GetComponent.<Rigidbody>().transform.rotation = Quaternion.Slerp(GetComponent.<Rigidbody>().transform.rotation, targetRotation, Time.fixedDeltaTime);
		// move back into bounds
		GetComponent.<Rigidbody>().transform.position = Vector3( 
		Mathf.Lerp(GetComponent.<Rigidbody>().transform.position.x, xPosTarget, Time.fixedDeltaTime), 
		Mathf.Lerp(GetComponent.<Rigidbody>().transform.position.y, yPosTarget, Time.fixedDeltaTime), 
		Mathf.Lerp(GetComponent.<Rigidbody>().transform.position.z, zPosTarget, Time.fixedDeltaTime) );

		//Debug.Log("moveInBounds has happened");
	}
}

function move() {
	var moveDirection = GetComponent.<Rigidbody>().transform.right;
	var distanceFromSurface : float = Mathf.Abs(waterlevel + 0.4 - transform.position.y);
	var downPush : Vector3 = Vector3(0, -0.5, 0); // if fish gets to the surface, push down so he can't be driven above the surface
	if (distanceFromSurface < 1.0) {
	//Debug.Log("CALLING downPush");
		turnUpright();
		moveDirection = GetComponent.<Rigidbody>().transform.right + ((1.0 - distanceFromSurface) * downPush);
	} else if (transform.position.y >= waterlevel + 0.3) {
		turnUpright();
		moveDirection = GetComponent.<Rigidbody>().transform.right + Vector3(0, -1.0, 0);
	}
	moveDirection.Normalize();
	var movement = moveDirection * moveSpeed;
	GetComponent.<Rigidbody>().transform.localPosition += movement;
}

function Splash () {
	// Instantiate(splash, Vector3 (rigidbody.position.x, waterlevel, rigidbody.position.z), Quaternion.identity);
}

function turnOffChildCollidersPhysics (turnOff : boolean) {
	haveTurnedOffPhysics = turnOff;
    // Turn off physics in child colliders when the fish is above water.
		for (var child : Transform in transform) {
 	   		turnOffPhysics(child, turnOff);
 	   }
}

function turnOffPhysics(obj : Transform, turnOff : boolean) {
	if(obj.GetComponent.<Animation>()) {
		obj.GetComponent.<Animation>().Stop();
	}
    if (obj.GetComponent.<Rigidbody>()) {
    	//obj.rigidbody.detectCollisions = turnOff;
    	obj.GetComponent.<Rigidbody>().isKinematic = turnOff;
    	//if (turnOff == true) {
    		//obj.rigidbody.isKinematic = turnOff;
    	//}    // reset positions and rotations of joints
    	obj.GetComponent.<Rigidbody>().transform.localPosition.y = 0.0;
    	obj.GetComponent.<Rigidbody>().transform.localPosition.z = 0.0;
    	// x position of bones is frozen at original place
    	if (!dead) {
    		obj.GetComponent.<Rigidbody>().transform.localEulerAngles = Vector3(0,0,0);
    	} else if (dead) {
			// if possible, set the spring of all joints to 0, so the fish will die a natural death, not all stiff
    	}
    }
    
    // recursively check children (bones)
    for (var trans : Transform in obj)  {
    	turnOffPhysics(trans, turnOff);
    	//Debug.Log(trans);   
    }  
}

function setRagdollState(state : boolean) {
	// set the parent game object collision detection to opposite of "state"
	// set parent and children isKinematic to opposite of "state"
	
	// define the opposite boolean and set
	var oppositeBoolean : boolean = true;
	if (state) { oppositeBoolean = false; }
	
	GetComponent.<Rigidbody>().detectCollisions = oppositeBoolean;
	turnOffChildCollidersPhysics(oppositeBoolean);
}

function resetJoints() {

}

function die() {
	dead = true;
	GetComponent.<Rigidbody>().isKinematic = false;
	 if (GetComponent.<Animation>()) {
		GetComponent.<Animation>().Stop();
	}
	underwaterGravity = normalGravity;
	setRagdollState(true);
	GetComponent.<Rigidbody>().velocity = Vector3(0.0,0.0,0.0);
}