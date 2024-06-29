using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    public Transform startPoint;  // リスポーン地点を参照する
    private Rigidbody rb;
    private CharacterController characterController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    public void Respawn()
    {
        if (startPoint != null)
        {
            if (characterController != null)
            {
                characterController.enabled = false;  // CharacterControllerを無効にする(移動をするため制御がかかるため)
            }

            if (rb != null)
            {
                // Rigidbodyを一時的にキネマティックに設定(直接移動するためにはキネマティックをONにする必要がある)
                rb.isKinematic = true;
                
                // 位置をリセット
                rb.position = startPoint.position;
                
                rb.isKinematic = false; //戻す

                //キネマティックを切ってから速度をリセット
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                transform.position = startPoint.position;
            }

            if (characterController != null)
            {
                characterController.enabled = true;  // CharacterControllerを再度有効にする
            }
        }
        else
        {
            Debug.LogError("Start Nothing");
        }
    }
}
