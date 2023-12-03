using UnityEngine;
using System.Collections;

public interface ICollisionHandler
{
    void HandleCollision(GameObject obj, Collision c);
}

/// <summary>
/// This script simply allows forwarding collision events for the objects that collide with something. This
/// allows you to have a generic collision handler and attach a collision forwarder to your child objects.
/// In addition, you also get access to the game object that is colliding, along with the object being
/// collided into, which is helpful.
/// </summary>
public class FireCollisionForward : MonoBehaviour
{
    public ICollisionHandler CollisionHandler;

    public void OnCollisionEnter(Collision col)
    {
        Debug.Log("Particle col");
        CollisionHandler.HandleCollision(gameObject, col);
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Particle trigger");
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle collision!");
    }
}
