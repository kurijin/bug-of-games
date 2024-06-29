using UnityEngine;

public class BossSwordCollider : MonoBehaviour
{
    //Bossの剣についているコライダーがplayerに当たった時PlayerのHealthスクリプトに1という数字を渡しダメージを与える
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // プレイヤーにダメージを与える
                playerHealth.TakeDamage(1);
            }
        }
    }
}
