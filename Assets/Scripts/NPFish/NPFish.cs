using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPFish : PooledObject {
    public Transform startWaypoint;
    public Transform midWaypoint;
    public Vector3 start;
    public Vector3 jumpDirection = Vector3.up;
    public float defaultJumpThrust;
    public float jumpThrust;
    public bool jumpReady;
    // settings for vertical stabilization
    public float stability = 0.3f;
    public float speed = 2.0f;
    public float thrust = 20f;

    private float waterlevel = 15.5f;
    public Vector3 target;
	private Rigidbody rb;
    private Vector3 direction;
    private ConstantForce gravity;
    public bool jumping;
    public bool goneAboveWater; // public just to see it for testing
    public float maxJumpTime = 0.5f; // analogous to nonKinematicTime for player fish
    public float jumpTimer;

    private float sqrMagnitude;

	void Start () {
        // init target
        target = new Vector3(0f, 0f, 0f);
        gravity = gameObject.GetComponent<ConstantForce>();
        jumpTimer = 0.0f;
        jumping = false;
        goneAboveWater = false;
        if (!midWaypoint)  { // for unknown reason, this field not getting saved wwith prefab
            midWaypoint = GameObject.FindWithTag("MidWayPoint").transform;
        }
        if (!startWaypoint)
        { // for unknown reason, this field not getting saved wwith prefab
            startWaypoint = GameObject.FindWithTag("StartWayPoint").transform;
        }

        start = RandomizePosition(startWaypoint);
        jumpReady = false;
		rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 0.5f;

        ResetNPF();

	}

    public void ResetNPF()
    {
        target = RandomizePosition(midWaypoint); // reset target to the first target
        // reset velocities to 0
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // reset to new position and rotation
        transform.position = start;
        // reset rotation to 0?

        // add initial forces
        direction = this.transform.forward;
        rb.AddForce(direction * thrust, ForceMode.Impulse);
    }

    // Each time, vary the strength of the jump a bit, roughly around 10%
    private void RandomizeJumpThrust()
    {
        jumpThrust = (0.95f + (0.1f * Random.value)) * defaultJumpThrust;
    }

    public Vector3 RandomizePosition(Transform waypt)
    {
        return new Vector3(waypt.position.x - 15f + (Random.value * 30f), waypt.position.y - 4f + (Random.value * 8f), waypt.position.z - 1f + (Random.value * 2f));
    }

    void FixedUpdate()
    {
        // If stopped, get unstuck
        sqrMagnitude = rb.velocity.sqrMagnitude;
        if (sqrMagnitude < 0.2f)
        {
            rb.AddRelativeTorque((transform.up + transform.right) * thrust * thrust * 5f);
            rb.AddForce(((-Vector3.forward * thrust * 4f) + Vector3.up), ForceMode.Impulse);
        } else if (sqrMagnitude < 1f) 
        {
            rb.AddTorque(Vector3.up * thrust * thrust);
            rb.AddForce(transform.forward * thrust, ForceMode.Impulse);
            // NOTE: I think there is a tendency to get stuck here in an oscillation. If velocity never
            // goes below the lower threshold (e.g. 0.2), the more extreme unsticking movement above
            // will never get called. Or may get called after a long while of jump attempts.
        }

        if (jumping && !goneAboveWater)
        {
            // set a timer to set jumping to false, in the case
            // that the fish never goes above the water when jumping.
            if (jumpTimer < maxJumpTime)
            {
                jumpTimer += Time.fixedDeltaTime;//Time.deltaTime;
            }
            else
            {
                jumpTimer = 0.0f;
                // jumping = false; // jump is continuing in a way
                jumpReady = true; // ok to put here? since jump was unsuccessful, allow another try
                                  // (won't cover sit. when jump above water but hits log and falls back in. 
                                  // But in that case, would likely re-enter the trigger 
                                  // and set jumpReady to true that way
            }
        }

        // Add gravity when above the water, remove when below
        if (transform.position.y > waterlevel)
        {
            gravity.enabled = true;
            rb.drag = 0f;
            goneAboveWater = true;

        } else // below water
        {
            gravity.enabled = false;
            rb.drag = 4f;
            if (goneAboveWater) // was above, now below
            { 
                jumping = false; 
            }
            // TODO: also need to set a timer to set jumping to false, in the case
            // that the fish never goes above the water when jumping.
            // ALSO, need to account for the case where the fish DOES go above water, but gets stuck on top
            // of a log or something
        }

            
        // Jump when in position in front of log

        //allow only 1 jump at a time, not a continuous up force. How to check?
        if (jumpReady)//(transform.position.z > 35f && transform.position.z < 45f)
        {
            JumpLog();
            jumpReady = false;
            jumping = true;


        }
        else if (!jumping)
        {
            
            AlignToVector(direction);

            sqrMagnitude = rb.velocity.sqrMagnitude; // need to set again here, can it change within the cycle?

            // not moving, so try to rotate by adding a torque
            //if (sqrMagnitude < 0.1f)
            //{
            //    rb.AddRelativeTorque(this.transform.up * thrust * thrust);
            //}

            // Slowing down, so add thrust
            if (sqrMagnitude < 8.0f)
            {

                direction = GetDirection();
                //TODO: rotate the fish towards the direction
                rb.AddForce(direction * thrust, ForceMode.Impulse);
            }
        }

	}

    private Vector3 GetDirection () {
        direction = (target - transform.position).normalized;
        return direction;
    }

    //TODO: Try breaking this into 2 parts -- calculating the torque, and adding the torque
    //Adding the torque happens every frame
    // Calculating only periodically
    // Alternatively, use ForceMode.impulse, and apply torque only periodically as well.
    private void Stabilize()
    {
        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed, rb.angularVelocity) * transform.up;
        float dot = Vector3.Dot(predictedUp, Vector3.up);
        if (dot < 0.99f) 
        { 
            Debug.Log(predictedUp);
            Debug.DrawRay(transform.position, predictedUp * 10f, Color.red, 1.0f);
            Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
            rb.AddTorque(torqueVector * speed * speed * (1 - Mathf.Abs(dot)) );
        }
    }

    private void AlignToVector(Vector3 dir)
    {
        Vector3 torqueVectorForward = new Vector3(0f, 0f, 0f);
        Vector3 torqueVectorUp = new Vector3(0f, 0f, 0f);

        float dotForward = Vector3.Dot(transform.forward, dir);
        if (dotForward < 0.99f)
        {
            Debug.DrawRay(transform.position, dir * 10f, Color.blue, 1.0f);
            torqueVectorForward = Vector3.Cross(transform.forward, dir);
            //rb.AddTorque(torqueVectorForward * speed * speed * (1 - Mathf.Abs(dotForward)));
        }

        float dotUp = Vector3.Dot(transform.up, Vector3.up);
        if (dotUp < 0.99f)
        {
            Debug.DrawRay(transform.position, transform.up * 10f, Color.red, 1.0f);
            torqueVectorUp = Vector3.Cross(transform.up, Vector3.up);
            //rb.AddTorque(torqueVectorUp * speed * speed * (1 - Mathf.Abs(dotUp)));
        }

        Vector3 torqueVector = ( (torqueVectorForward * (1 - Mathf.Abs(dotForward))) + (torqueVectorUp * (1 - Mathf.Abs(dotUp))) );
        rb.AddTorque(torqueVector * speed * speed);

    }

    private void JumpLog()
    {
        // If close enough to the log
        // Align to point up at some angle
        AlignToVector(jumpDirection);
        // Add an impulse force to jump over logs (force can have random variations in amount)
        RandomizeJumpThrust();
        rb.AddForce(jumpDirection * jumpThrust, ForceMode.Impulse);
        // If failure, try again
        // if success, move on to next waypoint
        // Need a way to measure success or failure to reach waypoint
    }

}
