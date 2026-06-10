using System;
using UnityEngine;

public class CollisionEventTrigger2D : MonoBehaviour
{
    public event Action<Collision2D> collisionEntered;
    public event Action<Collision2D> collisionExited;

    public event Action<Collider2D> triggerEntered;
    public event Action<Collider2D> triggerExited;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionEntered?.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionExited?.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        triggerEntered?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        triggerExited?.Invoke(other);
    }
}