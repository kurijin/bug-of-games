using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


//Playerは一人称で動く
//必要要素を確認し取得
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]


public class FirstPersonController : MonoBehaviour
{
    //Playerの設定や,UIなどを参照
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private GameObject itemUIPanel;
    [SerializeField] private Image itemUIImage;
    [SerializeField] private Text itemUIText;
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Inventory inventory;//Playerの取得したアイテム
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Text messageText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite characterSprite;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float maxLookAngle = 90.0f;
    [SerializeField] private AudioClip itemPickupSound; 
    [SerializeField] private AudioClip walkSound; 
    [SerializeField] private AudioClip addItemSound; 

    private CharacterController _characterController;
    private Vector3 _moveVelocity;
    private InputAction _move;
    private InputAction _look;
    private InputAction _pickup;
    private float _cameraPitch = 0.0f;
    private bool isItemUIPanelActive = false;
    private bool isHowToPlayPanelActive = false;
    private Animator animator;
    private Item currentItem;
    private AudioSource audioSource; 
    private bool isWalking = false;

    private void Start()
    {
        //playerInputsystemからmove,look,pickupを取得
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        var input = GetComponent<PlayerInput>();
        _move = input.actions["Move"];
        _look = input.actions["Look"];
        _pickup = input.actions["Pickup"];


        audioSource = gameObject.AddComponent<AudioSource>();
        InitializeUIElements();
        StartCoroutine(DelayedShowInitialMessage());

        //カーソルの表示
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //1stageと同様
    private void Update()
    {
        CustomUpdate();
    }

    private void CustomUpdate()
    {
        //アイテム取得,遊び方UIが有効である時pickupと同様の操作で非表示
        if (isItemUIPanelActive && _pickup.WasPressedThisFrame())
        {
            CloseItemUI();
            return;
        }

        if (isHowToPlayPanelActive && _pickup.WasPressedThisFrame())
        {
            CloseHowToPlayPanel();
            return;
        }

        if (!_characterController.enabled) return;

        //キャラクターコントローラーが有効な時動きと拾う動き
        HandleMovement();
        HandlePickup();
    }

    //初期メッセージの表示読み込みのため少し遅延
    private IEnumerator DelayedShowInitialMessage()
    {
        yield return new WaitForSeconds(2); 
        StartCoroutine(ShowInitialMessage());
    }

    //動かし方は基本的に1stageと同じ、カメラを上下左右で動かせるものとする
    private void HandleMovement()
    {
        var moveValue = _move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveValue.x, 0, moveValue.y).normalized;
        _moveVelocity = transform.TransformDirection(moveDirection) * walkSpeed;

        animator.SetFloat("MoveSpeed", _moveVelocity.magnitude / walkSpeed);

        //lookアクションを取得し,移動範囲内で上下に動かす
        var lookValue = _look.ReadValue<Vector2>();
        _cameraPitch -= lookValue.y * lookSpeed;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);
        //左右にカメラを動かす
        cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * lookValue.x * lookSpeed);

        //重力
        if (_characterController.isGrounded)
        {
            _moveVelocity.y = 0f;
        }
        else
        {
            _moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        _characterController.Move(_moveVelocity * Time.deltaTime);

        //止まっているときは歩行音を止める
        if (_moveVelocity.magnitude > 0.1f && !isWalking)
        {
            PlayWalkSound();
        }
        else if (_moveVelocity.magnitude <= 0.1f && isWalking)
        {
            StopWalkSound();
        }

        //マウスやゲームパッドの入力があればカーソルを表示する
        if (Input.GetMouseButtonDown(0) || Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //pickupボタンが押されればtrypickupを読み込む
    private void HandlePickup()
    {
        if (_pickup.WasPressedThisFrame())
        {
            TryPickupItem();
        }
    }

    //
    private void TryPickupItem()
    {
        //カメラの位置からレイを発射する
        Vector3 rayOrigin = cameraTransform.position;
        Vector3 rayDirection = cameraTransform.forward;
        float rayDistance = 5.0f;

        //レイが当たった中でItemレイヤーのものであるならばそのアイテムを取得するためにitem変数に入れる
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance, itemLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                currentItem = item;
                //取得音再生
                if (itemPickupSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(itemPickupSound);
                }
                //アイテムの説明文のUI
                DisplayItemUI(item);
            }
        }
    }

    private void DisplayItemUI(Item item)
    {
        Time.timeScale = 0f;
        isItemUIPanelActive = true;
        itemUIPanel.SetActive(true);
         // 効果音を再生
        if (itemPickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(itemPickupSound);
        }
        //アイテムの画像と説明文を取得し表示
        itemUIImage.sprite = item.itemSprite;
        itemUIText.text = item.itemDescription;
    }

    private void CloseItemUI()
    {
        Time.timeScale = 1f;
        isItemUIPanelActive = false;
        itemUIPanel.SetActive(false);

        //itemUiを閉じるときに現在item変数に入っているitemをインベントリに格納し、実際のアイテムは削除する
        if (currentItem != null)
        {
            AddItemToInventory(currentItem);
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }

    public void AddItemToInventory(Item item)
    {
        //inventoryオブジェクトのadditemを参照
        inventory.AddItem(item);

        // 効果音を再生
        if (addItemSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(addItemSound);
        }

        //inventory UIに取得済みのアイテムを表示するため,itemにアタッチされているImageを取得し貼り付ける.
        GameObject newItem = Instantiate(inventoryItemPrefab, inventoryPanel);
        Image newItemImage = newItem.GetComponent<Image>();
        if (newItemImage != null)
        {
            newItemImage.sprite = item.itemSprite;
        }

    }

    //メッセージの表示 と非表示
    public void ShowMessage(string message)
    {
        messagePanel.SetActive(true);
        messageText.text = message.Replace("\\n", "\n");

        if (characterImage != null && characterSprite != null)
        {
            characterImage.sprite = characterSprite;
            characterImage.gameObject.SetActive(true);
        }

        StartCoroutine(HideMessageAfterDelay(3));
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        if (characterImage != null)
        {
            characterImage.gameObject.SetActive(false);
        }
    }

    //何かのアイテムがあった時に限りitemを得れる場合の取得処理
    public void DisplayRewardItemUI(Item item)
    {
        Time.timeScale = 0f;
        isItemUIPanelActive = true;
        itemUIPanel.SetActive(true);

        // 効果音を再生
        if (itemPickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(itemPickupSound);
        }

        itemUIImage.sprite = item.itemSprite;
        itemUIText.text = item.itemDescription;
        StartCoroutine(HideRewardItemUIAfterDelay(3));
    }

    private IEnumerator HideRewardItemUIAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        isItemUIPanelActive = false;
        itemUIPanel.SetActive(false);
    }

    //シーン再生時のメッセージと遊び方パネルの表示
    private IEnumerator ShowInitialMessage()
    {
        Time.timeScale = 0f;
        messagePanel.SetActive(true);
        messageText.text = "ここは僕の部屋だ...閉じ込められているどうやって出よう";
        if (characterImage != null && characterSprite != null)
        {
            characterImage.sprite = characterSprite;
            characterImage.gameObject.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(3);

        messagePanel.SetActive(false);
        if (characterImage != null)
        {
            characterImage.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
        howToPlayPanel.SetActive(true);
        isHowToPlayPanelActive = true;
    }

    private void CloseHowToPlayPanel()
    {
        howToPlayPanel.SetActive(false);
        isHowToPlayPanelActive = false;
    }

    //シーン再生時のUI表示処理
    private void InitializeUIElements()
    {
        itemUIPanel.SetActive(false);
        messagePanel.SetActive(false);
        characterImage.gameObject.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    private void PlayWalkSound()
    {
        if (walkSound != null && audioSource != null)
        {
            audioSource.loop = true;
            audioSource.clip = walkSound;
            audioSource.Play();
            isWalking = true;
        }
    }

    private void StopWalkSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            isWalking = false;
        }
    }
}
