using UnityEngine;
using UnityEngine.Events;

public class CollisionPointEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent<Vector3> onCollision;

    private void OnCollisionEnter(Collision collision)
    {
        onCollision?.Invoke(collision.contacts[0].point);
    }
}
