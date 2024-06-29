using UnityEngine;


//playerの持ってる剣でボスに当たったらボスのhpを減らすもの
public class PlayerSwordCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(1); 
            }
        }
    }
}
