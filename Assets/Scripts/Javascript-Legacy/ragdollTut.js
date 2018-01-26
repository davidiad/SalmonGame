#pragma strict

//Last time I showed how to create rag doll for our characters on http://www.wooglie.com/games/Shooters/Catapult-bug-annihilation and http://www.kongregate.com/games/orktech/catapult-bug-annihihilation I found that on forums many of you are looking for a code to switch from normal animation mode to rag dol. So here it is and its description after

// CODE START
/*
private var deadAlready: boolean;

var hitPoints = 10;

var hitClip: AudioClip;

function ApplyDamage (damage : float) 
{
 if (hitPoints <= 0.0)
  return;


 //gameManager.points += damage; // our own simple logics :)
 
 hitPoints -= damage;
 if (hitPoints <= 0.0) 
 {
   //gameManager.monsters -= 1;    // our own simple logics :)
   KILL();
 }
}


function disablePhisics( trans: Transform ,  turnOff: boolean )
{ 
  if (trans.rigidbody)
  {
   trans.rigidbody.isKinematic = turnOff;
   trans.rigidbody.useGravity = true;
   if (turnOff == false) // TurnOn Phisics? then throw object in air
   {
                              
    trans.rigidbody.AddForce (Vector3.up * 10); // Make our fall a bit more dramatic by accelerating upwards
    /*  // MAKE SURE YOU DO THIS AS WELL
    var ch_AI = GetComponent (AI); // remove ai script
    if (ch_AI)
     Destroy(ch_AI);
                             
                                 // remove CharacterController script
    var ch_controller = GetComponent (CharacterController);
    if (ch_controller)
     Destroy(ch_controller);
                               */
//   }
 // }
//}
/*
function disableLimbPhisics( objectTrans: Transform ,  turnOff: boolean )
{
 if ((objectTrans.animation) && (turnOff ==  true))
  objectTrans.animation.Stop();
 
 disablePhisics( objectTrans,  turnOff);
 
 for (var trans : Transform in objectTrans) 
 {   
  disablePhisics( trans,  turnOff);
  disableLimbPhisics( trans, turnOff );
 }
 
}
function Start () 
{
 //gameManager.monsters += 1;
 disableLimbPhisics( transform,  true);
 //play default animation
 
 animation.Play("idle");
// animation.Play("fly");
 //animation.Play("SpiderIdle");
}


function KILL()
{
  disableLimbPhisics( transform,  false);
  animation.Stop();
  
  if (gameObject.rigidbody)
  {
   gameObject.AddComponent("killBall");
   var comp = gameObject.GetComponent("killBall");
   comp.launch();
  }


 if (audio)
 {
  if (!audio.isPlaying)
   audio.Play();
  audio.loop = false;
 }
} 

*/

// CODE END

/*
Description:
So when you attach this  to your player  or enemy:
 at start it will call   disableLimbPhisics( transform,  true); which in Recursive way will look through your models skeleton bones, find out which bones have physics properties and disable them, and make your ragdoll bones update positions from animation.

disableLimbPhisics( transform,  false);  will do completelly oposite. AND  in MAKE SURE .. section of the script really make sure you delete character controller script and any other unnecessary scripts because some of them might spoil rag dol ( we found it out  the hard way )  Thats it
P.S. All of that stuf was both easy and hard to make so please check out our game on Kongregate.com as appreciation, the link is Upstairs    ( ^_^)   THANKS 
*/