using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger2 : MonoBehaviour
{
    public Transform nextTarget;
    private NPFish npf;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPFish")) 
        {
            // Set the next waypoint
            npf = other.transform.parent.GetComponent<NPFish>();
            npf.target = npf.RandomizePosition(nextTarget); // set the colliding fish's  new target
        }
    }

}