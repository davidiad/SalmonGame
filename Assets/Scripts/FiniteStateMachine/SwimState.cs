using UnityEngine;
using System.Collections;

public class SwimState : FSMState
{
	//The fsmStates are not changing directly but updated by using transitions
	//public bool tappedFish = false;
	//public bool bumpedFish = false;

    public SwimState() 
    {
        stateID = FSMStateID.Swimming;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

    }

    //public override void Reason(Transform player, Transform npc)
	public override void Reason(Transform fish)
    {
        //Check the distance with player tank
        //When the distance is near, transition to chase state
        //if (Vector3.Distance(npc.position, player.position) <= 300.0f)
        //{
        //Debug.Log("Switch to Swim State");
		//GameObject theFish = GameObject.FindWithTag("Fishy");
		FSMFishController fishController  = fish.GetComponent<FSMFishController>();
		//tappedFish = fishController.fishWasTapped;
		//bumpedFish = fishController.bumped;
		if (fishController.fishWasTapped) // || theFish.transform.position.y > 14.7f) // jumps either if tapped, or if fish is "driven" above water
		{
			fishController.jumpForce = 1100.0f;
           // fish.GetComponent<FSMFishController>().SetTransition(Transition.Tapped);
			fishController.SetTransition(Transition.Tapped);
		} 
		else if (fish.transform.position.y > 14.8f) // fish has been "driven" above water
		{
			fishController.jumpForce = 0.0f;
			fishController.SetTransition(Transition.Tapped);
		}
		else if (fishController.bumped)
		{
			fishController.SetTransition(Transition.BumpedIntoSomething);
			Debug.Log (fishController.bumped + ": BUMPED while Swimming!!!!");
			fishController.bumped = false;
			Debug.Log ("Bumped is now: " + fishController.bumped);
		}
		else if (fishController.foundClearDirection)
		{
			fishController.foundClearDirection = false;
			fishController.SetTransition(Transition.SwimmingToTurning);
		}

		if (fish.transform.position.y < 0.0f)
		{
			fishController.SetTransition(Transition.GoneBelow);
		}
		// fishController.OnTriggerStay();
        //}
    }

   // public override void Act(Transform player, Transform npc)
	public override void Act(Transform fish)
    {
		Debug.Log("SWIMSTATE");
        //Find another random patrol point if the current point is reached
		/*
        if (Vector3.Distance(fish.position, destPos) <= 100.0f)
        {
            Debug.Log("Reached to the destination point\ncalculating the next point");
            //FindNextPoint();
        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - fish.position);
        fish.rotation = Quaternion.Slerp(fish.rotation, targetRotation, Time.deltaTime * curRotSpeed);
*/
		float maxForwardSpeed = 12.0f;
		float maxBackwardSpeed = -8.0f;
		float targetSpeed = 0.0f;
		float rotSpeed = 120.0f;


			if (Input.GetKey(KeyCode.W))
			{
				targetSpeed = maxForwardSpeed;
			}
			else if (Input.GetKey(KeyCode.S))
			{
				targetSpeed = maxBackwardSpeed;
			}
			else
			{
				targetSpeed = 0;
			}
			
			if (Input.GetKey(KeyCode.A))
			{
				fish.transform.Rotate(0, -rotSpeed * Time.deltaTime, 0.0f);
			}
			else if (Input.GetKey(KeyCode.D))
			{
				fish.transform.Rotate(0, rotSpeed * Time.deltaTime, 0.0f);
			}
			
			//Determine current speed
			curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
			//transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);    
		//Go Forward
		//fish.Translate(Vector3.forward * Time.deltaTime * curSpeed);
		fish.GetComponent<FSMFishController>().avoidObstacles();
		fish.GetComponent<FSMFishController>().move();
		//fish.GetComponent<FSMFishController>().avoidObstacles();
		fish.GetComponent<Animation>().Play();
		

	}

}