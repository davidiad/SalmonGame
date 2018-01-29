#pragma strict


var fog : GameObject;
var target : Transform;
static var targetOffset = Vector3(0.6, 0.3, -0.8);
var distance = 4.0;

var lineOfSightMask : LayerMask = 0;
var closerRadius : float = 0.2;
var closerSnapLag : float = 0.2;

var xSpeed = 200.0;
var ySpeed = 80.0;

var yMinLimit = -20;
var yMaxLimit = 80;

var currentDistance = 7.0;
private var x = 0.0;
private var y = 0.0;
private var distanceVelocity = 0.0;
private var fish : GameObject;
private var camParent: GameObject;

/*
// Underwater vars
private var defaultFog = RenderSettings.fog;

private var defaultFogColor = RenderSettings.fogColor;

private var defaultFogDensity = RenderSettings.fogDensity;

private var defaultSkybox = RenderSettings.skybox;

var noSkybox : Material;

var FogDensity = RenderSettings.fogDensity;
*/
/*
function Awake () { // added from underwater script

GetComponent("GlowEffect").enabled = false;
}
*/
function Start () {
	fish = GameObject.FindGameObjectWithTag("Fishy");
	camParent = GameObject.FindGameObjectWithTag("CamParent");

    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;
	currentDistance = distance;

	
	// Make the rigid body not change rotation
   //	if (rigidbody)
		//rigidbody.freezeRotation = true;
}
/*
function LateUpdate () {
	//Underwater();
	Debug.Log("AM I GETTING CALLED FIRST OR SECOND?");
    if (target) {
    	
    	//if (Input.GetAxis("Horizontal")) {
    		//x += Input.GetAxisRaw("Horizontal") * xSpeed * 0.1; 
    		//GameObject.FindWithTag("Fish").GetComponent(KinematicWithJumping).KinematicUpdate();
    	//}
    	
    	
    	// turn off camera orbitin for now:
    	//else if (!Input.GetAxis("Horizontal")) {
        	//x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
        //}
        y -= 0; //Input.GetAxis("Mouse Y") * ySpeed * 0.02;  
 		
 		y = ClampAngle(y, yMinLimit, yMaxLimit);
        var rotation = Quaternion.Euler(y, x, 0);
        var targetPos = target.position + targetOffset;
        var direction = rotation * -Vector3.forward;
		
        var targetDistance = AdjustLineOfSight(targetPos, direction);
		currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, distanceVelocity, closerSnapLag * .3);
        
        transform.rotation = rotation;
        transform.position = targetPos + direction * currentDistance;
    }
}
*/

// change function to look at target
function Update () {	
//    if (target) {
//        var targetPos = target.position + targetOffset;
//        var direction = transform.rotation * -Vector3.forward;
//
//        transform.position = targetPos + direction * currentDistance;
//    }
	camParent.transform.position = fish.transform.position;
}

function moveCameraToMatch() {
//	targetOffset.rotateAround(target.up);
// Get the opposite of target.forward
// add in the offset
// move cam to the new position
}

function AdjustLineOfSight (target : Vector3, direction : Vector3) : float
{
	var hit : RaycastHit;
	if (Physics.Raycast (target, direction, hit, distance, lineOfSightMask.value))
		return hit.distance - closerRadius;
	else
		return distance;
}

static function ClampAngle (angle : float, min : float, max : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return Mathf.Clamp (angle, min, max);
}

function LateUpdate () {
Debug.Log("AM I GETTING CALLED?");
	if (transform.position.y > 15.0) {
		fog.SetActive (false);
	} else {
		fog.SetActive (true);
	}
   //if (Input.GetKeyDown ("space")) {
	slerpCamera();
    }

function slerpCamera() {
    yield WaitForSeconds(0.2);
	camParent.transform.rotation = Quaternion.Slerp(camParent.transform.rotation, fish.gameObject.transform.rotation, Time.deltaTime);
	transform.LookAt(fish.transform);
    yield;
 }


/*
function Underwater () {

var defaultFog = RenderSettings.fog;

var defaultFogColor = RenderSettings.fogColor;

var defaultFogDensity = RenderSettings.fogDensity;

var defaultSkybox = RenderSettings.skybox;

var noSkybox : Material;

var FogDensity = RenderSettings.fogDensity;

if (transform.position.y < 30) {
    RenderSettings.fog = false;
    //RenderSettings.fogColor = Color (0.3, 0.32, 0.22, 0.9);
    //RenderSettings.fogDensity = 0.25;//FogDensity;
    //RenderSettings.skybox = noSkybox;
    //GetComponent("MotionBlur").enabled = true;
    //GetComponent("AudioSource").enabled = true;
    GetComponent("DepthOfField34").enabled = true;
    GetComponent("GlobalFog").enabled = true;
}

else {
    RenderSettings.fog = defaultFog;
    RenderSettings.fogColor = defaultFogColor;
    RenderSettings.fogDensity = defaultFogDensity;
    //RenderSettings.skybox = defaultSkybox;
    //GetComponent("MotionBlur").enabled = false;
    //GetComponent("AudioSource").enabled = false;
    GetComponent("DepthOfField34").enabled = false;
    GetComponent("GlobalFog").enabled = false;
}
}
@script AddComponentMenu("Third Person Camera/Mouse Orbit")
*/