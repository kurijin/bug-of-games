using UnityEngine;
using Cinemachine;
//ステージ終盤見にくくなるため、このスクリプトでカメラを俯瞰に変更する。
public class CameraTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Vector3 newFollowOffset; //新しいフォローオフセット値
    private CinemachineTransposer transposer;
    private Vector3 originalFollowOffset; // 元のフォローオフセット値

    private void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        // 元のフォローオフセット値を保存
        if (transposer != null)
        {
            originalFollowOffset = transposer.m_FollowOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 該当エリアに入ったときにフォローオフセットを変更
            if (transposer != null)
            {
                transposer.m_FollowOffset = newFollowOffset;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 該当エリアから出たときにフォローオフセットを元に戻す
            if (transposer != null)
            {
                transposer.m_FollowOffset = originalFollowOffset;
            }
        }
    }
}
