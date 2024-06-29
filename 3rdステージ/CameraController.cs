using UnityEngine;

//Player中心のカメラ設定
public class CameraController : MonoBehaviour
{
    public Transform player; // プレイヤーの位置を参照する
    public float cameraHeight = 10f; // カメラの高さ
    public float cameraDistance = 10f; // カメラの距離
    public float angle = 45f; // カメラの角度

    private void LateUpdate()
    {
        if (player != null)
        {
            //カメラの位置を設定した高さと距離に設定する(Player基準)
            Vector3 direction = new Vector3(0, cameraHeight, -cameraDistance);
            Quaternion rotation = Quaternion.Euler(angle, 0, 0);//見る角度
            transform.position = player.position + rotation * direction;
            transform.LookAt(player.position);
        }
    }
}
