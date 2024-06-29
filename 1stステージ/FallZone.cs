using UnityEngine;

//死ぬための場所設定
public class FallZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Playerをリスポーンさせるためのスクリプトを読み込む
            PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();
            if (respawner != null)
            {
                respawner.Respawn();
            }
        }
    }
}
