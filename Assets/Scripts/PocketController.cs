using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PocketType { BOTTOMLEFT, BOTTOM, BOTTOMRIGHT, UPPERLEFT, UPPER, UPPERRIGHT };
[RequireComponent(typeof(BoxCollider))]
public class PocketController : MonoBehaviour
{
    [SerializeField] private PocketType pType = PocketType.BOTTOMLEFT;
    private void OnTriggerEnter(Collider other)
    {
        BallController bcontr = other.GetComponent<BallController>();
        if (bcontr)
        {
            bcontr.GotPocketed(pType);
        }
    }
}