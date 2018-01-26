#pragma strict

private var hit : RaycastHit;

function Start () {

}

function Update () {
	if (Physics.Raycast (transform.position, Vector3.down, hit, 200)) {
		Debug.Log("a hit");
		Debug.DrawRay(transform.position, Vector3.down * 5, Color.yellow, 1);
		Debug.DrawRay(transform.position, hit.normal, Color.blue, 3);
	} else {
		Debug.Log("no hit");
	}
}