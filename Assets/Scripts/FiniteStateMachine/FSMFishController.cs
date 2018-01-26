using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FSMFishController : AdvancedFSM 
{
    public GameObject Fish;
	//public GameObject frontTrigger;
	public float moveSpeed = 0.1f;
	public Vector3 newDirection;
	public float jumpForce;// = 1100.0f;
	public bool fishWasTapped = false; // set to true when user taps on fish
	public bool foundClearDirection = false;
	public bool turnIsComplete = false;
	private int rotCount = 0; // counting the number of frames it's taking to turn in a new direction. If too high, redo findClearDirection()
	public bool bumped = false; // set to true in onTriggerEnter when fish bumps into an object
	public float timeBetweenBumps = 0.8f;
	public float timeAtPreviousBump = 0.0f;
	public float timeAtCurrentBump = 0.0f;

	public float nonKinematicTime = 0.5f;
	public float kinematicTimer = 0.0f;
    private int health;
	// UI slider to control speed
	public Slider speedSlider; // shouldn't this be moved to start function and made private?

	private bool haveTurnedOffPhysics = true;

    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        health = 100;

        elapsedTime = 0.0f;
        shootRate = 2.0f;

        //Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Fishy");
        playerTransform = objPlayer.transform;
		turnOffChildCollidersPhysics(true);
        if (!playerTransform) 
		{
            print("Fishy doesn't exist.. Please add one with Tag named 'Fishy'");
		}
        //Get the turret of the tank
        // turret = gameObject.transform.GetChild(0).transform;
        // bulletSpawnPoint = turret.GetChild(0).transform;

        //Start Doing the Finite State Machine
        ConstructFSM();
    }

    //Update each frame
    protected override void FSMUpdate()
    {
        //Check for health
        elapsedTime += Time.deltaTime;
    }

    protected override void FSMFixedUpdate()
    {
        //CurrentState.Reason(playerTransform, transform);
        //CurrentState.Act(playerTransform, transform);
		CurrentState.Reason(playerTransform);
		CurrentState.Act(Fish.transform);
    }

    public void SetTransition(Transition t) 
    { 
        PerformTransition(t); 
    }

    private void ConstructFSM()
    {
		/*
       //Get the list of points
        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        Transform[] waypoints = new Transform[pointList.Length];
        int i = 0;
        foreach(GameObject obj in pointList)
        {
            waypoints[i] = obj.transform;
            i++;
        }

        PatrolState patrol = new PatrolState(waypoints);

        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
*/
/*        ChaseState chase = new ChaseState(waypoints);
        chase.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        chase.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        chase.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        AttackState attack = new AttackState(waypoints);
        attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        attack.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);

         */

       // AddFSMState(patrol);
/*        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(dead);*/

		SwimState swim = new SwimState();
		SawObstacleState sawObstacle = new SawObstacleState();
		TurningAwayState turningAway = new TurningAwayState();
		JumpState jump = new JumpState();
		JumpingUnderWaterState jumpingUnderWater = new JumpingUnderWaterState();
		JumpingAboveWaterState jumpingAboveWater  = new JumpingAboveWaterState();
		BumpState bump = new BumpState();
		FallingState falling = new FallingState();
		HitWaterState hitWater = new HitWaterState();
		DeadState dead = new DeadState();

		AddFSMState(swim);
		AddFSMState(sawObstacle);
		AddFSMState(turningAway);
		AddFSMState(jump);
		AddFSMState(jumpingUnderWater);
		AddFSMState(jumpingAboveWater);
		AddFSMState(falling);
		AddFSMState(hitWater);
		AddFSMState(bump);
		AddFSMState(dead);

		swim.AddTransition(Transition.Tapped, FSMStateID.Jump);
		swim.AddTransition(Transition.BumpedIntoSomething, FSMStateID.Bump);
		swim.AddTransition(Transition.SawObstacle, FSMStateID.SawObstacle);
		swim.AddTransition(Transition.SwimmingToTurning, FSMStateID.TurningAway);
		swim.AddTransition(Transition.GoneBelow, FSMStateID.Dead);

		sawObstacle.AddTransition(Transition.FoundClearDirection, FSMStateID.TurningAway);
		turningAway.AddTransition(Transition.TurnIsComplete, FSMStateID.Swimming);

		jump.AddTransition(Transition.HasJumped, FSMStateID.JumpingUnderWater);

		jumpingUnderWater.AddTransition(Transition.AboveWater, FSMStateID.JumpingAboveWater);
		jumpingUnderWater.AddTransition(Transition.ReturnToSwimming, FSMStateID.Swimming);

		jumpingAboveWater.AddTransition(Transition.ReachedApex, FSMStateID.Falling);
	
		falling.AddTransition(Transition.AtSurface, FSMStateID.HitWater);

		hitWater.AddTransition(Transition.UnderWater, FSMStateID.JumpingUnderWater);

		bump.AddTransition(Transition.HasBumped, FSMStateID.JumpingUnderWater);
	
		dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);
		dead.AddTransition(Transition.Z, FSMStateID.Swimming);

		//falling.AddTransition(Transition.GoneBelow, FSMStateID.Dead);



    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.tag == "Bullet")
        {
            health -= 50;

            if (health <= 0)
            {
                Debug.Log("Switch to Dead State");
                SetTransition(Transition.NoHealth);
                Explode();
            }
        }
    }

    protected void Explode()
    {
        float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            GetComponent<Rigidbody>().AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        }

        Destroy(gameObject, 1.5f);
    }
	
	public void OnTriggerEnter (Collider other) 
	{
		timeAtPreviousBump = timeAtCurrentBump;
		timeAtCurrentBump = Time.time;
		if (CurrentStateID == FSMStateID.Swimming)
		{
			bumped = true;
			//timeAtLastBump = Time;
			Debug.Log("Bumped into something");
		}
		//if (other.gameObject.tag == "fishy"){
		//	Debug.Log("Box went through!");
		//	}
	}

	public void returnToSwimming()
	{
		GameObject waterplane = GameObject.FindWithTag("WaterDown");
		Debug.Log ("returnToSwimming() called");
		Debug.Log ("nonKinematicTime is: " + nonKinematicTime);
		// the fish has been given over to control of the physics engine, and now has re-entered (or is still in) the water,
		// so set a timer for how long to wait before returning to isKinematic state
		if (kinematicTimer < nonKinematicTime) 
		{
			kinematicTimer += Time.deltaTime;
		}
		else 
		{
			setRagdollState(false);
			GetComponent<Rigidbody>().isKinematic = true;
			// avoidObstacles();
			waterplane.GetComponent<Collider>().enabled = true; // enabled so that raycasts will 'see' it, and the fish can collide with it
			/*if (!bumped) { // trying to avoid conflicts between turning upright and turning after a bump
				turnUpright();
			}
			setRagdollState(false);
			if (animation) 
			{
				animation.Play();
			} */
			kinematicTimer = 0.0f;
			updateMoveSpeed();
			SetTransition(Transition.ReturnToSwimming);
			//nonKinematicTime = 3.18;//3.0; // reset to original amount; used a shorter amount for when the fish is "driven" by user above water
			// reset the fish's vertical position to upright:
			//rigidbody.transform.localEulerAngles.x = Mathf.Lerp(rigidbody.transform.localEulerAngles.x, 0, Time.fixedDeltaTime);
			//rigidbody.transform.localEulerAngles.z = Mathf.Lerp(rigidbody.transform.localEulerAngles.z, 0, Time.fixedDeltaTime);
			/*
			Debug.Log(" BEFORE DOES THIS RUN???");
			Vector3 currentAngles = Fish.rigidbody.transform.rotation.eulerAngles;
			Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			Fish.rigidbody.transform.rotation = Quaternion.Lerp(Fish.rigidbody.transform.rotation, targetRotation, 33.0f * Time.fixedDeltaTime);
			Debug.Log("AFTER DOES THIS RUN???");
			*/
		}
	}

	public void move() 
	{
		Vector3 moveDirection = new Vector3();
		moveDirection = GetComponent<Rigidbody>().transform.forward;
		/* pushing down near surface -- not implementing for now, only is needed
		var distanceFromSurface : float = Mathf.Abs(waterlevel + 0.4 - transform.position.y);
		var downPush : Vector3 = Vector3(0, -0.5, 0); // if fish gets to the surface, push down so he can't be driven above the surface
		if (distanceFromSurface < 1.0) {
			//Debug.Log("CALLING downPush");
			turnUpright();
			moveDirection = rigidbody.transform.right + ((1.0 - distanceFromSurface) * downPush);
		} else if (transform.position.y >= waterlevel + 0.3) {
			turnUpright();
			moveDirection = rigidbody.transform.right + Vector3(0, -1.0, 0);
		}
		*/
		Vector3 rot = new Vector3(0.0f, GetComponent<Rigidbody>().transform.localRotation.y, GetComponent<Rigidbody>().transform.localRotation.z);
		//rigidbody.transform.rotation = Quaternion.Euler(rot);
		moveDirection.Normalize();
		Vector3 movement = new Vector3();
		movement = moveDirection * moveSpeed;
		GetComponent<Rigidbody>().transform.localPosition += movement;

		// set position of additional trigger object to prevent bumping into things (unneeded. Using raycast instead)
		//frontTrigger.rigidbody.transform.localRotation = rigidbody.transform.localRotation;
		//frontTrigger.rigidbody.transform.localPosition = rigidbody.transform.localPosition + (0.6f) * frontTrigger.rigidbody.transform.right;
	}

	public void updateMoveSpeed()
	{
		//Slider moveSpeedSlider = GetComponent<Slider>();
		//moveSpeedSlider = GameObject.Find("MoveSpeedSlider");
		moveSpeed = speedSlider.value;
	}

	// Refactoring avoidObstacles()
	//Calculate the new directional vector to avoid the obstacle
	public void avoidObstacles() 
	{
		Vector3 direction = new Vector3();
		direction = transform.forward;
		RaycastHit hit = new RaycastHit();
		float hitLength = 2.0f;
		int layermask = (1<<11) | (1<<4); // layer 13 is the fish trigger, don't want the ray to detect that

		if (Physics.Raycast(transform.position, transform.forward, out hit, hitLength, layermask))
		{
			if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, layermask))
			{
				Debug.DrawRay(transform.position, GetComponent<Rigidbody>().transform.forward * 0.5f, Color.magenta, 1.0f);
				moveSpeed = 0.0f;
				SetTransition(Transition.SawObstacle);
			}
			else if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f, layermask))
			{
				Debug.Log("Green Line");
				Debug.DrawRay(transform.position, GetComponent<Rigidbody>().transform.forward * 1.5f, Color.green, 1.5f);
				moveSpeed = 0.04f;
			}
			else
			{
				Debug.Log("Yellow Line");
				Debug.DrawRay(transform.position, GetComponent<Rigidbody>().transform.forward * hitLength, Color.yellow, hitLength);
				moveSpeed = speedSlider.value;
			}
			Vector3 hitNormal = hit.normal;
			direction = direction + hitNormal;
			direction.Normalize();
			direction = rightToForward(direction).normalized;
			newDirection = direction;
			foundClearDirection = true;
		}
		// split into separate state, swimming near surface??
		if (GetComponent<Rigidbody>().transform.position.y > 14.4f) // replace with waterlevel variable
		{
			Debug.Log ("near the surface");
			//Why zero out the Y????
			// Why not zero out the x???? We are trying to make it go flat, right?
			//Vector3 atSurfaceDirection = new Vector3(direction.x, 0.0f, direction.z);
			Vector3 atSurfaceDirection = new Vector3(direction.x, direction.y, 0.0f);
			direction = atSurfaceDirection;
			direction.Normalize();
			newDirection = rightToForward(direction).normalized;
			//foundClearDirection = true;
			Quaternion rot = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime);
		}

		if ( Mathf.Abs(transform.rotation.eulerAngles.z) > 10)
		{
			Debug.Log("TILTED");
			Vector3 uprightDirection = new Vector3(direction.x, direction.y, 0.0f);
			direction = uprightDirection;
			direction.Normalize();
			newDirection = rightToForward(direction).normalized;
			Quaternion rot = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime);
			//foundClearDirection = true;
		}
		//direction.Normalize();
		//Quaternion rot = Quaternion.Euler(direction);
		// Need to tranlate to right instead of forward
		//newDirection = rightToForward(direction).normalized;

	}

	/* // avoidObstacles works, but with some issues. Refactoring above.
	//Calculate the new directional vector to avoid the obstacle
	public void avoidObstacles() 
	{
		// javascript
		//var hit : RaycastHit;
		//var hitLength: float = 9.0;
		// C#
		Vector3 direction = new Vector3();
		direction = transform.forward.normalized;
		RaycastHit hit = new RaycastHit();
		float hitLength = 9.0f;
		int layermask = (1<<11) | (1<<4); // layer 13 is the fish trigger, don't want the ray to detect that
		
		
		
		//Check that the fish hit with the obstacles is within its minimum distance to avoid
		//if (Physics.Raycast(transform.position, transform.right, hit, hitLength, streambedMask)) {
		if ( Mathf.Abs(transform.rotation.eulerAngles.x) > 10)
		{
			Vector3 uprightDirection = new Vector3(0.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			Quaternion rot = Quaternion.Euler(uprightDirection);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime);
		}
		if (Physics.Raycast(transform.position, transform.right.normalized, out hit, hitLength, layermask))
		{
			if (Physics.Raycast(transform.position, transform.right.normalized, out hit, 0.5f, layermask))
			{
				Debug.DrawRay(transform.position, rigidbody.transform.right * 0.5f, Color.magenta, 1.0f);
				moveSpeed = 0.0f;
				SetTransition(Transition.SawObstacle);
			}
			else if (Physics.Raycast(transform.position, transform.right.normalized, out hit, 1.5f, layermask))
			{
				Debug.DrawRay(transform.position, rigidbody.transform.right * 1.5f, Color.green, 1.5f);
				moveSpeed = 0.04f;
			}
			else
			{
				Debug.DrawRay(transform.position, rigidbody.transform.right.normalized * hitLength, Color.yellow, hitLength);
				moveSpeed = speedSlider.value; //0.1f;
			}
			//Get the normal of the hit point to calculate the new direction
			//var hitNormal : Vector3 = hit.normal;
			Vector3 hitNormal = hit.normal;
			//Debug.DrawRay(hit.point, hitNormal, Color.red, hitLength);
			
			direction = direction + hitNormal;
			// keep the fish from listing (banking too much) as it turns, switching x and z
			//Vector3 uprightDirection = new Vector3(direction.z, direction.y, direction.x);
			//direction = uprightDirection;
			
			//Rotate the fish to its target directional vector
			//var rot = Quaternion.LookRotation(direction);
			direction.Normalize();
			Quaternion rot = Quaternion.LookRotation(direction);
			
			//Vector3 uprightDirection = new Vector3(0.0f, rot.eulerAngles.y, rot.eulerAngles.z);
			//rot = Quaternion.Euler(uprightDirection);
			//Debug.Log("Eulers: " + rot.eulerAngles);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime);
		}
		// Push the fish down if gets too close to the surface
		// split into separate state, swimming near surface??
		else if (rigidbody.transform.position.y > 14.4f) // replace with waterlevel variable
		{
			//Debug.Log ("near the surface");
			Vector3 atSurfaceDirection = new Vector3(direction.x, 0.0f, direction.z);
			direction = atSurfaceDirection;
			direction.Normalize();
			Quaternion rot = Quaternion.LookRotation(direction);
			//Debug.Log(" At srf Eulers: " + rot.eulerAngles);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime);
		}
		
		// repeated from above ???????????
		if ( Mathf.Abs(transform.rotation.eulerAngles.x) > 10)
		{
			Vector3 uprightDirection = new Vector3(0.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			Quaternion rot = Quaternion.Euler(uprightDirection);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime);
		}
	}*/

	// helper function to get the forward direction from the right direction
	public Vector3 rightToForward(Vector3 dir)
	{
		GameObject tranny = new GameObject();
		tranny.transform.position = transform.position;
		tranny.transform.LookAt(dir);
		dir = Vector3.Cross(dir, tranny.transform.up); // because fish is moving in direction right rather than forward
		Destroy(tranny);
		return dir;
	}

	public Vector3 findClearDirection() // may not need a return value
	{
		// only run this code if a clear direction has NOT been found. IE, the first time entering a SawObstacleState. Otherwise, just turn towards the clear direction already found
		rotCount = 0;
		//Vector3 direction = new Vector3();
		//direction = transform.forward;
		RaycastHit hit = new RaycastHit();
		float hitLength = 100.0f;
		float maxDistance = 0.0f;
		float minDistance = 12.0f;
		int layermask = (1<<11) | (1<<4);
		// send out rays in several directions
		// store the values in an array
		// adjust for the amount it goes thru the fish's body (more going stratight to front for instance)
		// find the longest value (over the minumum), and turn to that direction and increase speed.
		// if no direction is over the minimum, then check behind, and if that is clear, go backwards a bit and check again.
		// turn around and go at full speed when a direction is clear
		Vector3[] directions;
		directions = new Vector3[11];
		directions[0] = transform.forward;
		directions[1] = (transform.forward + transform.right).normalized;
		directions[2] = transform.right;
		directions[3] = (transform.right + (transform.forward * -1)).normalized;
		directions[4] = transform.forward * -1;
		directions[5] = (directions[1] + transform.up).normalized;
		directions[6] = (transform.right + transform.up).normalized;
		directions[7] = (directions[3] + transform.up).normalized;
		directions[8] = (directions[1] + (transform.up * -1)).normalized;
		directions[9] = (transform.right + (transform.up * -1)).normalized;
		directions[10] = (directions[3] + (transform.up * -1)).normalized;
		// defaulting hit distance to 50 -- 50 or greater should be plenty of room for fish to move
		float[] distances = new float[]{ hitLength, hitLength, hitLength, hitLength, hitLength, hitLength, hitLength, hitLength, hitLength, hitLength, hitLength };
		for (int i=0; i<directions.Length; i++)
		{
			if (Physics.Raycast(GetComponent<Rigidbody>().position, directions[i], out hit, hitLength, layermask))
			{
				distances[i] = hit.distance;
			}
		}
		for (int i=0; i<distances.Length; i++)
		{
			if (distances[i] > maxDistance)
			{
				maxDistance = distances[i];
				newDirection = directions[i];
				Debug.DrawRay(transform.position, directions[i] * distances[i], Color.cyan, 2.0f);
			}
		}

		// because fish is moving in direction right rather than forward
		// using "tranny" to get the up direction from the direction
		GameObject tranny = new GameObject();
		tranny.transform.position = transform.position;
		tranny.transform.LookAt(newDirection);
		newDirection = Vector3.Cross(newDirection, tranny.transform.up); // because fish is moving in direction right rather than forward
		Destroy(tranny);
		if (maxDistance < minDistance) // no clear direction towards forward, so turn around 180 degrees
		{
			//Quaternion behind = Quaternion.Inverse(transform.rotation);
			//newDirection = behind.eulerAngles.normalized;
			newDirection = -1 * transform.forward;
			Debug.Log ("DO A 180");
		}
		foundClearDirection = true;
		return newDirection;

		if (maxDistance > minDistance)
		{
			Debug.Log("MAX > MIN");
			//direction = Vector3.Cross(direction, transform.up);

			// using "tranny" to get the up direction from the direction
			//GameObject tranny = new GameObject();
			tranny.transform.position = transform.position;
			tranny.transform.LookAt(newDirection);
			newDirection = Vector3.Cross(newDirection, tranny.transform.up); // because fish is moving in direction rught rather than forward
			Destroy(tranny);

			Quaternion rot = Quaternion.LookRotation(newDirection);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2.5f * Time.fixedDeltaTime);
			// logic to check that transform.direction is at (or close) to the new direction. Then allow the fish to swim again.
			float angle = Vector3.Angle(transform.forward, newDirection);

			if (angle < 1.0f)
			{
				//Debug.Log("waiting on the angle");
				foundClearDirection = true;
				moveSpeed = speedSlider.value;
			}
			else
			{
				Debug.Log(angle);
			}
		}
		else
		{
			// need to back out?
			Debug.Log("MIN");
			// turn 180 degrees, and look in the other direction
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Inverse(transform.rotation), 2.5f * Time.fixedDeltaTime);
			// make helper function out of the following for angle because repeat from above
			float angle = Vector3.Angle(transform.forward, newDirection);
			if (angle < 4.0f)
			{
				foundClearDirection = true;
				moveSpeed = speedSlider.value;
			}
		}
	}

	public void turning(Vector3 dir)
	{
		float rotationSpeed = 25.0f * speedSlider.value;
		Quaternion rot = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.fixedDeltaTime);
		// logic to check that transform.direction is at (or close) to the new direction. Then allow the fish to swim again.
		float angle = Vector3.Angle(transform.forward, dir);
		
		if (angle < 5.0f)
		{
			turnIsComplete = true;
			moveSpeed = speedSlider.value;
		}
		else
		{
			Debug.Log(angle + " and rotCount is: " + rotCount);
			rotCount ++;
		}
		// safeguard in case it gets stuck without turning all the way to the angle it's trying to reach
		if (rotCount > 120)
		{
			foundClearDirection = false;
			findClearDirection();
		}
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
		haveTurnedOffPhysics = turnOff;
		// Turn off physics in child colliders when the fish is above water.
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
			//obj.rigidbody.transform.localPosition.y = 0.0f;
			//obj.rigidbody.transform.localPosition.z = 0.0f;
			obj.GetComponent<Rigidbody>().transform.localPosition = new Vector3(obj.GetComponent<Rigidbody>().transform.localPosition.x, 0.0f, 0.0f);
			//Vector3 v3 = rigidbody.velocity;
		//	v3.x = 1.0;
		//	v3.z = 0.0;
		//	rigidbody.velocity = v3;

		}
		
		// recursively check children (bones)
		foreach (Transform trans in obj)  {
			turnOffPhysics(trans, turnOff);  
		}  
	}
	/*
	public void setRagdollState(bool state) {
		// set the parent game object collision detection to opposite of "state"
		// set parent and children isKinematic to opposite of "state"
		
		// define the opposite boolean and set
		bool oppositeBoolean = true;
		if (state) { oppositeBoolean = false; }
		
		rigidbody.detectCollisions = oppositeBoolean;
		turnOffChildCollidersPhysics(oppositeBoolean);
	}*/

}
