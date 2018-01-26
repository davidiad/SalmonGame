using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger1 : MonoBehaviour {

    private NPFish npf;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPFish"))
        {
            npf = other.transform.parent.GetComponent<NPFish>();
            npf.jumpReady = true;
        }
    }

}
