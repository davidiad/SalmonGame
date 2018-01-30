using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    //public Transform target;
    public Vector3 targetOffset;// = new Vector3(0.6f, 0.3f, -0.8f); // used in GestureResponses.cs
    //public float distance = 4.0f;

    //public LayerMask lineOfSightMask = 0;
    //public float closerRadius = 0.2f;
    //public float closerSnapLag = 0.2f;

    //public float xSpeed = 200.0f;
    //public float ySpeed = 80.0f;

    //public float yMinLimit = -20f;
    //public float yMaxLimit = 80f;

    //public float currentDistance = 7.0f;

    private GameObject fish;
    private GameObject camParent;
    //private float x = 0.0f;
    //private float y = 0.0f;
    //private float distanceVelocity = 0.0f;

    void Start()
    {
        fish = GameObject.FindGameObjectWithTag("Fishy");
        camParent = GameObject.FindGameObjectWithTag("CamParent");
        targetOffset = transform.localPosition;
        //x = transform.eulerAngles.y;
        //y = transform.eulerAngles.x;
        //currentDistance = distance;
    }

    void Update()
    {
        camParent.transform.position = fish.transform.position;
    }

    private void LateUpdate()
    {
        SlerpCamera();
    }

    public void UpdateTargetOffset()
    {
        transform.localPosition = targetOffset;
    }

    // Rotate camera slowly to follow fish as it moves
    private void SlerpCamera()
    {
        // slight delay before starting the rotations has a better feel
        StartCoroutine("CamFollowPause"); 
        // camParent is at same position as fish. So when it rotates, the camera (being a child of camParent) rotates in a circle around the fish
        // keeping the camera in the same relative position to the fish
        camParent.transform.rotation = Quaternion.Slerp(camParent.transform.rotation, fish.gameObject.transform.rotation, Time.deltaTime);
        // After rotating around the camParent's axis, now rotate the camera around its own axis to look at the fish
        transform.LookAt(fish.transform);
        //yield;
    }

    private IEnumerator CamFollowPause()
    {
        yield return new WaitForSeconds(0.2f);
    }

    //private float ClampAngle(float angle, float min, float max)
    //{
    //    if (angle < -360f) { angle += 360f; }
    //    if (angle > 360f) { angle -= 360f; }
    //    return Mathf.Clamp(angle, min, max);
    //}

}
