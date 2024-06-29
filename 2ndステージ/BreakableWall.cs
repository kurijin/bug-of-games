using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

//あるアイテムがあるかどうかで壁が壊れる仕組みのスクリプト,壁以外にも適用可能
public class BreakableWall : MonoBehaviour
{
    //壁壊れ時やヒント,必要アイテムの参照
    [SerializeField] private string requiredItemName;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private Text hintText;
    [SerializeField] private string hintMessage;
    [SerializeField] private string destroyMessage;
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite characterSprite;
    [SerializeField] private AudioClip destroySound; 

    private bool playerInRange = false;
    private FirstPersonController playerController;
    private AudioSource audioSource;

    private void Start()
    {
        playerController = FindObjectOfType<FirstPersonController>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetMouseButtonDown(0) || (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                //Playerが壊れるかべの一定範囲内でクリックorゲームパッドのボタンを押しているかを検知,推されていたら壁への参照
                OnDestroyWall();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Playerが壁の一定範囲内にいたらフラグをON
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

    private void OnDestroyWall()
    {
        //範囲内であるか再確認
        if (playerInRange)
        {
            //Playerのインベントリ内に特定のアイテムがあるかどうかを確認
            if (playerInventory.HasItem(requiredItemName))
            {
                StartCoroutine(DestroyWallWithMessage());
            }
            else
            {
                //ないならヒント
                ShowHint();
            }
        }
    }

    private IEnumerator DestroyWallWithMessage()
    {
        // 効果音を再生
        if (destroySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(destroySound);
        }

        //壊れた時のメッセージを表示,表示後数秒待機
        if (!string.IsNullOrEmpty(destroyMessage))
        {
            playerController.ShowMessage(destroyMessage);
            yield return new WaitForSecondsRealtime(3);
        }
        //壁を破壊
        Destroy(gameObject);
    }

    private void ShowHint()
    {
        //ヒントメッセージの改行処理
        hintText.text = hintMessage.Replace("\\n", "\n");
        hintPanel.SetActive(true);

        //ヒントメッセージを表示
        if (characterImage != null && characterSprite != null)
        {
            characterImage.sprite = characterSprite;
            characterImage.gameObject.SetActive(true);
        }
        //コルーチンにより数秒待機後にメッセージ非表示
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
