using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public enum BallType { BLACK, WHITE };
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{
    private Rigidbody _rb;
    private AudioSource _audioSourse;
    private float speed, maxSpeed = 0.008f, volume;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private BallType ballType;
    [SerializeField] private int ballNumber;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSourse = gameObject.GetComponent<AudioSource>();
        PhysicsController.physicsDelegate += ApplyPhysics;
    }

    private void Update()
    {
        RoundSpeed();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (_rb.velocity.sqrMagnitude > 0.004f)
            {
                speed = _rb.velocity.magnitude;
                volume = Mathf.Clamp01(speed);
                PlayCollisionSound(speed / 2);
            }
        }
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
    }

    public IEnumerator ManageVelocityEnum()
    {
        while (true)
        {
            _rb.velocity = _rb.velocity * 0.991f;
            _rb.angularVelocity = _rb.angularVelocity * 0.991f;

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

    public void GotPocketed()
    {
        GameController.instance.CheckPocketedBall(this);
        PhysicsController.physicsDelegate -= ApplyPhysics;
        Destroy(gameObject, 1f);
    }

    private void RoundSpeed()
    {
        if (_rb.velocity.sqrMagnitude <= 0.002f)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void PlayCollisionSound(float volume)
    {
        _audioSourse.PlayOneShot(_audioSourse.clip, volume);
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
        return _rb.velocity.magnitude != 0;
    }
}