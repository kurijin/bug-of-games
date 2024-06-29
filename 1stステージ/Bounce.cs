using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float force = 10f; //　playerを押す力
    public float stunTime = 0.5f; //playerの硬直時間
    private Vector3 hitDir;

    
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //ColiisionのtagがPlayerであった場合の処理
            if (collision.gameObject.tag == "Player")
            {
                hitDir = contact.normal;
                var playerController = collision.gameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                   //Playerのスクリプトに引き渡すもの.ヒット距離に力をかけて,stan時間も変数に渡す。
                    playerController.HitPlayer(-hitDir * force, stunTime);
                }
                return;
            }
        }
    }
}
