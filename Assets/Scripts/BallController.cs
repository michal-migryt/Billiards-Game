using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BallType { NULL, WHITE, HALF, FULL, BLACK };
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{

    private Rigidbody _rb;
    [SerializeField] private BallType ballType;
    [SerializeField] private int ballNumber;
    // Start is called before the first frame update
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        PhysicsController.physicsDelegate += ApplyPhysics;
        //StartCoroutine(ManageVelocityEnum());
    }
    private void Update()
    {
    }
    private void LateUpdate()
    {
        if (Mathf.Sqrt(Mathf.Pow(_rb.velocity.x, 2) + Mathf.Pow(_rb.velocity.y, 2) + Mathf.Pow(_rb.velocity.z, 2)) <= Mathf.Sqrt(0.0008f))
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Band")
        {
            Vector3 objectDir = transform.forward;
            Vector3 otherNormal = other.GetContact(0).normal;

            _rb.velocity = Vector3.Reflect(_rb.velocity, otherNormal);

            _rb.angularVelocity = -_rb.angularVelocity;
        }
    }
    public void ManageVelocity()
    {
        _rb.velocity = _rb.velocity * 0.9985f;
        _rb.angularVelocity = _rb.angularVelocity * 0.9985f;
        if (ballType == BallType.WHITE) { Debug.Log(_rb.velocity); }
        if (Mathf.Sqrt(Mathf.Pow(_rb.velocity.x, 2) + Mathf.Pow(_rb.velocity.y, 2) + Mathf.Pow(_rb.velocity.z, 2)) <= Mathf.Sqrt(0.0001f))
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
    public IEnumerator ManageVelocityEnum()
    {
        while (true)
        {
            _rb.velocity = _rb.velocity * 0.991f;
            _rb.angularVelocity = _rb.angularVelocity * 0.991f;
            if (Mathf.Sqrt(Mathf.Pow(_rb.velocity.x, 2) + Mathf.Pow(_rb.velocity.y, 2) + Mathf.Pow(_rb.velocity.z, 2)) <= Mathf.Sqrt(0.0001f))
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            //yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        }
    }
    public void ApplyPhysics()
    {
        if (_rb != null)
        {
            _rb.angularDrag = PhysicsController.instance.getAngularDrag();
            _rb.mass = PhysicsController.instance.getBallMass();
            _rb.drag = PhysicsController.instance.getDrag();
        }
    }
    public void GotPocketed(PocketType pocketType)
    {
        GameController.instance.CheckPocketedBall(this, pocketType);
        if (ballType == BallType.HALF || ballType == BallType.FULL)
        {
            PhysicsController.physicsDelegate -= ApplyPhysics;
            Destroy(gameObject, 2f);
        }
    }
    // BUG after foul ball falls through table
    public void SetEndGameState()
    {
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
        StopCoroutine(ManageVelocityEnum());
    }
    public void TakeToWaitingPoint()
    {
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
        transform.position = GameController.instance.GetWaitingPoint();
    }
    public BallType getBallType()
    {
        return ballType;
    }
    public int getBallNumber()
    {
        return ballNumber;
    }
    public bool isMoving()
    {
        return _rb.velocity != Vector3.zero;
    }
}