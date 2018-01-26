using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger3 : MonoBehaviour
{

    private NPFish npf;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPFish"))
        {
            npf = other.transform.parent.GetComponent<NPFish>(); // assumes collider on the child, and script on the parent
            npf.ReturnToPool(); // return to the Object Pool, since npf is a pooled object
            npf.ResetNPF();
        }
    }

}
