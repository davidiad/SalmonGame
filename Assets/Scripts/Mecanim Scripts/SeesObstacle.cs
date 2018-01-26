using UnityEngine;
using System.Collections;

public class SeesObstacle : StateMachineBehaviour
{

    private GameObject fish;
    private FishManager fishManager;
    private Vector3 direction;
    private float moveSpeed = 0.1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fish = animator.gameObject;
        fishManager = fish.GetComponent<FishManager>();
        direction = fish.transform.forward;
        animator.SetBool("obstacleIsClose", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RaycastHit hit = new RaycastHit();
        float hitLength = 19.0f;
        int layermask = (1 << 15) | (1 << 4); // layer 13 is the fish trigger, don't want the ray to detect that

        if (Physics.Raycast(fish.transform.position, fish.transform.forward, out hit, 3.5f, layermask))
        {
            animator.SetBool("foundClearDirection", false);
            animator.SetBool("obstacleIsClose", true);
        }
        else if (Physics.Raycast(fish.transform.position, fish.transform.forward, out hit, hitLength, layermask))
        {
            //if (Physics.Raycast (fish.transform.position, fish.transform.forward, out hit, 0.5f, layermask)) {
            //Debug.DrawRay(fish.transform.position, fish.GetComponent<Rigidbody>().transform.forward * 3.5f, Color.magenta, 1.0f);
            //}
        }
        else
        {
            if (animator.GetBool("canAutoTurn"))
            {
                animator.SetBool("foundClearDirection", true);
            }
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //animator.SetBool("canAutoTurn", true);
    }
}
