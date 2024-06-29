using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float force = 10f; // Force 10000f
    public float stunTime = 0.5f;
    private Vector3 hitDir;

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
            if (collision.gameObject.tag == "Player")
            {
                hitDir = contact.normal;
                var playerController = collision.gameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.HitPlayer(-hitDir * force, stunTime);
                }
                else
                {
                    Debug.LogError("PlayerController component not found on Player object.");
                }
                return;
            }
        }
    }
}
