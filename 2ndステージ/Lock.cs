using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


//lockされているものを特定のアイテムで開けてあるアイテムを取得する.
public class Lock : MonoBehaviour
{

    [SerializeField] private string requiredItemName; // 必要なアイテム名
    [SerializeField] private Inventory playerInventory; // プレイヤーのインベントリ参照
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private Text hintText; 
    [SerializeField] private string hintMessage; 
    [SerializeField] private Item rewardItem; // 報酬アイテム
    [SerializeField] private string unlockMessage; 
    [SerializeField] private Image characterImage; 
    [SerializeField] private Sprite characterSprite; 

    private bool playerInRange = false;
    private bool isUnlocked = false; 
    private FirstPersonController playerController;

    //UI表示のためplayerのスクリプトを取得
    private void Start()
    {
        playerController = FindObjectOfType<FirstPersonController>();
    }

    private void Update()
    {
        //範囲内であるか,クリックorゲームパッドボタンを押されたかを確認し範囲内であれば解除メソッドを参照
        if (playerInRange && !isUnlocked)
        {
            if (Input.GetMouseButtonDown(0) || (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                OnUnlock();
            }
        }
    }

    //Playerがlockのトリガー範囲内かを検出
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    //範囲内かつアイテムを持っていたらunlock処理,そうでなければヒントメッセージ
    private void OnUnlock()
    {
        if (playerInRange && !isUnlocked)
        {
            if (playerInventory.HasItem(requiredItemName))
            {
                StartCoroutine(UnlockAndHandleReward());
            }
            else
            {
                ShowHint();
            }
        }
    }

    private IEnumerator UnlockAndHandleReward()
    {
        isUnlocked = true; // ロックが解除されたことを記録
        Time.timeScale = 0f; // 時間を停止

        // ロック解除メッセージの表示
        if (!string.IsNullOrEmpty(unlockMessage))
        {
            playerController.ShowMessage(unlockMessage); // ロック解除時のメッセージを表示
            yield return new WaitForSecondsRealtime(3); 
        }

        // 報酬アイテムを取得
        if (rewardItem != null)
        {
            playerController.DisplayRewardItemUI(rewardItem); // 報酬アイテムのUIを表示
            yield return new WaitForSecondsRealtime(3); 
            playerController.AddItemToInventory(rewardItem); // 報酬アイテムをインベントリに追加
        }

        Destroy(gameObject); // ロックオブジェクトを削除する
        Time.timeScale = 1f; 
    }

    //ヒントメッセージの処理
    private void ShowHint()
    {
        hintText.text = hintMessage.Replace("\\n", "\n"); 
        hintPanel.SetActive(true);

        if (characterImage != null && characterSprite != null)
        {
            characterImage.sprite = characterSprite; 
            characterImage.gameObject.SetActive(true);
        }

        StartCoroutine(HideHintAfterDelay(3)); 
    }

    private IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hintPanel.SetActive(false);
        if (characterImage != null)
        {
            characterImage.gameObject.SetActive(false); 
        }
    }
}
