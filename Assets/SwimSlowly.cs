using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwimSlowly : StateMachineBehaviour
{
    public float moveSpeed = 0.02f;

    private GameObject fish;
    private FishManager fishManager;
    //private int layermask;

    //public float shortHitLength = 1.0f;
    //public float longHitLength = 10.0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fish = animator.gameObject;
        fishManager = fish.GetComponent<FishManager>();
        //layermask = (1 << 15) | (1 << 4);
    }

    // TODO: refactor this func
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 moveDirection = new Vector3();
        moveDirection = fish.transform.forward + (0.06f * fishManager.chdir);
        moveDirection.Normalize();

        Quaternion rot = Quaternion.LookRotation(moveDirection);

        fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, rot, 2.5f * Time.deltaTime);
        Vector3 movement = new Vector3();
        movement = fish.transform.forward * moveSpeed;
        fish.transform.localPosition += movement;
        // TODO:-should be using movePosition() so it's not "teleporting", and the physics engine is aware of it.
    }

    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    RaycastHit hit = new RaycastHit();

    //    // int layermask = (1 << 15) | (1 << 4); // layer 13 is the fish trigger, don't want the ray to detect that

    //    if (Physics.Raycast(fish.transform.position, fish.transform.forward, out hit, shortHitLength, layermask))
    //    {
    //        // very close to hitting something, so stop (go to idle state)
    //        animator.SetBool("idle", true);

    //    }
    //    else if (Physics.Raycast(fish.transform.position, fish.transform.forward, out hit, longHitLength, layermask))
    //    {
    //        // There's some space, so return to a state where swimming is faster
    //        animator.SetBool("foundClearDirection", true);
    //        animator.SetBool("obstacleIsClose", false);
    //    }
    //}
}
