using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForClearDirection : StateMachineBehaviour {

    private GameObject fish;
    private FishManager fishManager;
    private int layermask;

    public float shortHitLength = 2.5f;
    public float longHitLength = 10.0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fish = animator.gameObject;
        fishManager = fish.GetComponent<FishManager>();
        layermask = (1 << 15);// don't stop for water | (1 << 4);
    }

    // Check to see if there is now a clear direction (which may be the result of user rotating the fish)
    // As oppsed to actively finding a clear direction to move in
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RaycastHit hit = new RaycastHit();

        // int layermask = (1 << 15) | (1 << 4); // layer 13 is the fish trigger, don't want the ray to detect that

        if (Physics.Raycast(fish.transform.position, fish.transform.forward, out hit, shortHitLength, layermask))
        {
            // very close to hitting something, so stop (go to idle state)

                animator.SetBool("idle", true);

        }
        else if (!Physics.Raycast(fish.transform.position, fish.transform.forward, out hit, longHitLength, layermask))
        {
            // There's some space, so return to a state where swimming is faster
            animator.SetBool("foundClearDirection", true);
            animator.SetBool("obstacleIsClose", false);
            animator.SetBool("idle", false);
        }
    }
}
