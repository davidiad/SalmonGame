using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitFollowCamera : MonoBehaviour {

    public Transform target;
    public Vector3 targetOffset = new Vector3(0.6f, 0.3f, -0.8f);
    public float distance = 4.0f;

    public LayerMask lineOfSightMask = 0;
    public float closerRadius = 0.2f;
    public float closerSnapLag = 0.2f;

    public float xSpeed = 200.0f;
    public float ySpeed = 80.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float currentDistance = 7.0f;

    private GameObject fish;
    private GameObject camParent;
    private float x = 0.0f;
    private float y = 0.0f;
    private float distanceVelocity = 0.0f;

    void Start()
    {
        fish = GameObject.FindGameObjectWithTag("Fishy");
        camParent = GameObject.FindGameObjectWithTag("CamParent");

        //var angles = transform.eulerAngles;
        x = transform.eulerAngles.y;
        y = transform.eulerAngles.x;
        currentDistance = distance;
    }

    void Update()
    {
        camParent.transform.position = fish.transform.position;
    }

    private void LateUpdate()
    {
        slerpCamera();
    }

    private void slerpCamera()
    {
        //yield WaitForSeconds(0.2);
        //camParent.transform.rotation = Quaternion.Slerp(camParent.transform.rotation, fish.gameObject.transform.rotation, Time.deltaTime);
        //transform.LookAt(fish.transform);
        //yield;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) { angle += 360f; }
        if (angle > 360f) { angle -= 360f; }
        return Mathf.Clamp(angle, min, max);
    }
}
