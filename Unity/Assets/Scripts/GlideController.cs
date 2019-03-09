 using UnityEngine;
 
 
 /*
  *
  * !!! CREDIT !!!
  * User mmeeks1 · Oct 26, 2017 at 11:03 AM for this script controlling the gliding objects
  * https://answers.unity.com/questions/635535/how-to-move-a-gameobject-with-an-animation.html
  * The following code was taken directly from this page and used unmodified for movable objects
  * 
  */
 
 
 public class GlideController : MonoBehaviour {
     public float speed;
 
     private Vector3 destination;
 
     void Start () {
         // Set the destination to be the object's position so it will not start off moving
         SetDestination (gameObject.transform.position);
     }
     
     void Update () {
         // If the object is not at the target destination
         if (destination != gameObject.transform.position) {
             // Move towards the destination each frame until the object reaches it
             IncrementPosition ();
         }
     }
 
     void IncrementPosition ()
     {
         // Calculate the next position
         float delta = speed * Time.deltaTime;
         Vector3 currentPosition = gameObject.transform.position;
         Vector3 nextPosition = Vector3.MoveTowards (currentPosition, destination, delta);
 
         // Move the object to the next position
         gameObject.transform.position = nextPosition;
     }
 
     // Set the destination to cause the object to smoothly glide to the specified location
     public void SetDestination (Vector3 value) {
         destination = value;
     }
 }