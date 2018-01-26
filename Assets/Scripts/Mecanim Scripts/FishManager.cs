using UnityEngine;
using System.Collections;

public class FishManager : MonoBehaviour
{
    public Vector3 chdir;
    public float waterlevel = 14.7f;
    public float cumulativeDragAmount;
    public float moveSpeed;
    public float turningMoveSpeed; // speed of forward motion while turning under user control
    public float nonKinematicTime = 2.0f;
    public int defaultTurnDirection;
    public float additionalJumpforce = 400.0f;

    private Rigidbody rb;
    private Animator animator;
    private int tapHash = Animator.StringToHash("tap");
    private float kinematicTimer;
    private bool goneAboveWater;
    private SphereCollider triggerCollider;
    private GameObject[] downwaters;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        chdir = new Vector3(0.0f, 0.0f, 0.0f);
        kinematicTimer = 0.0f;
        goneAboveWater = false;
        GameObject bumpTrigger = GameObject.FindGameObjectWithTag("fishtrig3");
        triggerCollider = bumpTrigger.GetComponent<SphereCollider>();
        downwaters = GameObject.FindGameObjectsWithTag("WaterDown");
        defaultTurnDirection = 1;
    }

    private void FixedUpdate()
    {

        // Need to know if fisg has been above water - will affect later behavior
        if (transform.position.y > (waterlevel + 0.8f))
        {
            goneAboveWater = true;
            if (rb.isKinematic)
            {
                // set to ragdoll state. Fish is above water and should fall
                rb.isKinematic = false;
                MinimizeTrigger();
                setRagdollState(true);
                animator.enabled = false;
                foreach (GameObject downwater in downwaters)
                {
                    downwater.GetComponent<Collider>().enabled = false;
                }
            }
        }


        if (!rb.isKinematic)
        {
            // mimic the force of the fish falling into the water. Should vary with velocity.
            if (goneAboveWater && (transform.position.y < (waterlevel + 0.8f)) && (transform.position.y > (waterlevel + 0.5f)))
            {
                float speed = 25.0f * rb.velocity.y;
                rb.AddForce(Vector3.up * speed);
            }

            MinimizeTrigger();
            // start timer for returning to Kinematic state for various cases
            if (transform.position.y < (waterlevel + 0.3f))
            {
                // Reduce the gravity when in water
                Physics.gravity = new Vector3(0f, 0f, 0f);
                rb.drag = 4.0f;

                // allows a user to add more jump force, but only if still underwater
                // (because a fish can't add force when out of water -- only air to push against)
                // Note: for this additional force, can tap anywhere, doesn't need to be on
                // fish like the initial tap
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 forceVector = additionalJumpforce * transform.forward;
                    rb.AddForce(forceVector);
                }
                if (kinematicTimer < 0.5f)
                {
                    kinematicTimer += Time.deltaTime;
                    // if > .5s, and hasn't gone above water, return to kinematic
                }
                else if (!goneAboveWater)
                {
                    returnToKinematic();
                    // if has gone above water, return to kin. in 2 or 3 s
                }
                else if (kinematicTimer < nonKinematicTime)
                {
                    kinematicTimer += Time.deltaTime;
                }
                else
                {
                    returnToKinematic();
                }
            }
            else // above water level
            {
                Physics.gravity = new Vector3(0f, -9.81f, 0f);
                rb.drag = 0.02f;
            }

        }
        else // not kinematic. Tap on fish to jump.
        {
            if (Input.GetMouseButtonDown(0))
            {
                Tap();
            }
        }
    }

    private void Tap()
    {
        // *Should* be handled now by Gesture Responses -- but isn't as of now
        // trigger to jump state
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (gameObject == GameObject.Find(hit.collider.gameObject.name))
            {
                animator.SetTrigger(tapHash);
            }
        }
    }

    private void MinimizeTrigger()
    {
        if (triggerCollider != null)
        {
            triggerCollider.radius = 0.01f;
            triggerCollider.center = new Vector3(0, 0, 0);
        }
    }

    private void MaximizeTrigger()
    {
        if (triggerCollider != null)
        {
            triggerCollider.radius = 0.15f;
            triggerCollider.center = new Vector3(0f, 0.1f, 1.0f);
        }
    }

    // Each time the animator is enabled, choose a direction to auto-turn (left or right)
    private void SetDefaultTurnDirection()
    {
        if (Random.Range(0, 2) == 1)
        {
            defaultTurnDirection = 1;
        }
        else
        {
            defaultTurnDirection = -1;
        }
    }

    public void returnToKinematic()
    {
        kinematicTimer = 0.0f;
        goneAboveWater = false;
        rb.isKinematic = true;
        SetDefaultTurnDirection(); // In avoid, -1 or 1 will turn left or right
        // Turn the animator state machine back on, now that we are done with using physics engine
        animator.enabled = true;
        // set the state to swim, otherwise it picks up where it left off with in jumping state
        animator.CrossFade("swim", 0.0f);

        foreach (GameObject downwater in downwaters)
        {
            downwater.GetComponent<Collider>().enabled = true;
        }
        // need to turn kinematic back on
        setRagdollState(false);
        MaximizeTrigger();
    }

    public void setRagdollState(bool state)
    {
        // set the parent game object collision detection to opposite of "state"
        // set parent and children isKinematic to opposite of "state"

        // define the opposite boolean and set
        bool oppositeBoolean = true;
        if (state) { oppositeBoolean = false; }

        rb.detectCollisions = oppositeBoolean;
        turnOffChildCollidersPhysics(oppositeBoolean);
    }


    public void turnOffChildCollidersPhysics(bool turnOff)
    {
        //haveTurnedOffPhysics = turnOff;
        foreach (Transform child in transform)
        {
            turnOffPhysics(child, turnOff);
        }
    }

    public void turnOffPhysics(Transform obj, bool turnOff)
    {
        Animation ani = obj.GetComponent<Animation>();
        Rigidbody rbObj = obj.GetComponent<Rigidbody>();

        if (ani)
        {
            ani.Stop();
        }

        if (rbObj)
        {
            rbObj.isKinematic = turnOff;
            rbObj.transform.localPosition = new Vector3(rbObj.transform.localPosition.x, 0.0f, 0.0f);
        }

        // recursively check children (bones)
        foreach (Transform trans in obj)
        {
            turnOffPhysics(trans, turnOff);
        }
    }
}
