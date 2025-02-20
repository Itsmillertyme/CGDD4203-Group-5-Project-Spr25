using System.Collections;
using UnityEngine;

public class PlayerProjectileController : MonoBehaviour {
    //**FIELDS**
    float speed = 0.0f;
    public Vector3 velocity;
    //
    bool inTrigger = false;

    public float Speed
    {
        get => speed; set
        {
            if (Mathf.Abs(velocity.sqrMagnitude) > 0)
            {
                Debug.LogWarning("Setting speed after init will add to the velocity");
            }
            speed = value;
            velocity += transform.forward * speed;
        }
    }


    //**UNITY METHODS**
    void FixedUpdate()
    {
        //Move forward

        transform.position += velocity * Time.fixedDeltaTime;
    }
    //
    private void OnTriggerEnter(Collider other) {
        //Test if colliding with wall trigger and not already in it
        if (other.gameObject.tag == "Wall" && !inTrigger) {
            inTrigger = true;

            //Test wall orientation
            if (other.transform.localScale.x > other.transform.localScale.z) {
                //Flip z coord
                transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
            }
            else {
                //Flip x coord
                transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
            }

            StartCoroutine(TriggerReset());
        }
    }

    //**COROUTINES**
    IEnumerator TriggerReset() {//Allows the projectile to clear the trigger before reseting
        //This is terrible code...smh, refactor later in a smarter way
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        inTrigger = false;
    }
}
