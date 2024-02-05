using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    [SerializeField] private float ballMass;
    [SerializeField] private float dragRate;
    [SerializeField] private float angularDragRate;
    [SerializeField] private float defaultMass;
    [SerializeField] private float defaultDrag;
    [SerializeField] private float defaultAngDrag;
    private float tempMass, tempDrag, tempAngDrag;
    public static PhysicsController instance;
    public delegate void OnPhysicsChanged();
    public static OnPhysicsChanged physicsDelegate;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        physicsDelegate.Invoke();
    }
    public void SetDefaultPhysics()
    {
        ballMass = defaultMass;
        tempMass = defaultMass;
        dragRate = defaultDrag;
        tempDrag = defaultDrag;
        angularDragRate = defaultAngDrag;
        tempAngDrag = defaultAngDrag;
    }
    public void ApplyPhysicsChanges()
    {
        ballMass = tempMass;
        dragRate = tempDrag;
        angularDragRate = tempAngDrag;
    }
    public float getBallMass()
    {
        return ballMass;
    }
    public float getDefaultBallMass()
    {
        return defaultMass;
    }
    public float getDrag()
    {
        return dragRate;
    }
    public float getDefaultDrag()
    {
        return defaultDrag;
    }
    public float getAngularDrag()
    {
        return angularDragRate;
    }
    public float getDefaultAngularDrag()
    {
        return defaultAngDrag;
    }
    public void setBallMass(float ballMass)
    {
        this.ballMass = ballMass;
    }
    public void setTempMass(float ballMass)
    {
        tempMass = ballMass;
    }
    public void setDrag(float dragRate)
    {
        this.dragRate = dragRate;
    }
    public void setTempDrag(float dragRate)
    {
        tempDrag = dragRate;
    }
    public void setAngularDrag(float angularDragRate)
    {
        this.angularDragRate = angularDragRate;
    }
    public void setTempAngularDrag(float angularDragRate)
    {
        tempAngDrag = angularDragRate;
    }
}
