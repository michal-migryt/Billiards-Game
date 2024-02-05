using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PocketController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BallController bcontr = other.GetComponent<BallController>();
        if (bcontr)
        {
            bcontr.GotPocketed();
        }
    }
}