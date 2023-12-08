using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    
    private Rigidbody _rb;
    private MeshCollider _collider;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private const float MoveSpeed = 1.5f;
    private readonly Vector3 _raisedPosition = new Vector3(0, 0.85f, 0);

    private void Awake()
    {
        // store rigidbody and collider refs
        _rb = transform.GetComponent<Rigidbody>();
        _collider = transform.GetComponent<MeshCollider>();

        // store original position and rotation
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;

        // initialize pin transform and rotation
        Reset();
    }

    public void Reset()
    {
        // restore original position and rotation
        transform.position = _originalPosition;
        transform.rotation = _originalRotation;

        // set initial raised local position 
        transform.localPosition += _raisedPosition;

        // stop movements
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
    
    public void StartLowerPin()
    {
        EnableCollision(false);

        StartCoroutine(LowerPin());
    }

    IEnumerator LowerPin()
    {
        Debug.Log("Start LowerPin");

        // lower pins by subtracting the raised position to its local position
        yield return StartCoroutine(PinTween(transform.localPosition, transform.localPosition - _raisedPosition, MoveSpeed));

        Debug.Log("End LowerPin");

        EnableCollision(true);
    }

    public void StartRaisePin()
    {
        EnableCollision(false);

        StartCoroutine(RaisePin());
    }

    private IEnumerator RaisePin()
    {
        Debug.Log("Start RaisePin");

        // raise the pins by adding the raised position to its local position
        yield return StartCoroutine(PinTween(transform.localPosition, transform.localPosition + _raisedPosition, MoveSpeed));

        Debug.Log("End RaisePin");
    }

    // tween function to animate local position
    private IEnumerator PinTween(Vector3 from, Vector3 to, float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(from, to, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void EnableCollision(bool value)
    {
        _rb.useGravity = value;
        _collider.enabled = value;
    }
    
    public bool IsPinDown()
    {
        var zAngle = transform.eulerAngles.z;
        Debug.Log($"name: {name} zAngle: {zAngle}");

        // determine if model is down by its z angle
        return zAngle is > 5 and < 359;
    }
}
