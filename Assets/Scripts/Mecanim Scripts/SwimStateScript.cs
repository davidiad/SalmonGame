using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwimStateScript : StateMachineBehaviour {

	private GameObject fish;
    private FishManager fishManager;
	private float moveSpeed = 0.1f;
	//public Slider speedSlider; // Will let user adjust the fish's speed

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		fish = animator.gameObject;
        fishManager = fish.GetComponent<FishManager>();
	}


 	//OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
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

    public void updateMoveSpeed() // TODO:
	{
		//Slider moveSpeedSlider = GetComponent<Slider>();
		//moveSpeedSlider = GameObject.Find("MoveSpeedSlider");
		//moveSpeed = speedSlider.value;
	}
}
