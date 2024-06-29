using UnityEngine;

//最後の猫の前の会話,猫を動かすためのトリガーのスクリプト
public class Zone : MonoBehaviour
{
    public Transform catTransform;
    public GameObject catCameraObject; //猫をアップで写すためのカメラ
    public Canvas conversationUI;
    public GameObject cat; // Catオブジェクト

    private bool isConversationActive = false; // 会話がアクティブかどうかを示すフラグ

    //トリガーに入ったらイベントを発生させる
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ゲーム時間を停止
            Time.timeScale = 0f;

            // catCameraオブジェクトを有効にする(猫ドアップで下に会話が出てくるようなもの)
            if (catCameraObject != null)
            {
                catCameraObject.SetActive(true);
            }

            // UIを表示
            if (conversationUI != null)
            {
                conversationUI.gameObject.SetActive(true);
                isConversationActive = true; 
            }

            // 最初はCatMovementスクリプトを無効化
            if (cat != null)
            {
                CatMovement catMovement = cat.GetComponent<CatMovement>();
                if (catMovement != null)
                {
                    catMovement.enabled = false;
                }
            }
        }
    }

    void Update()
    {
        if (isConversationActive && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton1)))
        {
            // UIを非表示
            conversationUI.gameObject.SetActive(false);
            isConversationActive = false; // 会話が終了

            // catCameraオブジェクトを無効にする
            if (catCameraObject != null)
            {
                catCameraObject.SetActive(false);
            }

            // CatMovementスクリプトを有効化(ここで猫が動き出し、BGMが変わる.)
            if (cat != null)
            {
                CatMovement catMovement = cat.GetComponent<CatMovement>();
                if (catMovement != null)
                {
                    catMovement.enabled = true;
                }
            }

            // ゲーム時間を再開
            Time.timeScale = 1f;

            //このトリガーは一回のみ有効.
            Destroy(gameObject);
        }
    }
}
