using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//シーンラストで車に乗る演出のためのスクリプト
//動かし方はキャラクターとほぼ同じ
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class CarController : MonoBehaviour
{
    //車の速度など,車を実物より小さくするためのスケール,UIなど
    [SerializeField] private float driveSpeed = 10f;
    [SerializeField] private Transform cameraTransform;//カメラも新しくする
    [SerializeField] private float lookSpeed = 2.0f;//マウスの感度
    [SerializeField] private float maxLookAngle = 90.0f;
    [SerializeField] private Vector3 smallScale = new Vector3(0.1f, 0.1f, 0.1f); // 小さくするスケール
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private GameObject messageUIPanel; 
    [SerializeField] private Text messageUIText; 
    [SerializeField] private Image characterImage; 
    [SerializeField] private string enterCarMessage; 
    [SerializeField] private Sprite characterSprite; 

    [SerializeField] private GameObject clearUIPanel; 
    [SerializeField] private Text clearUIText; 
    [SerializeField] private string clearMessage ; 
    [SerializeField] private Image clearCharacterImage; 
    [SerializeField] private Sprite clearCharacterSprite; 

    [SerializeField] public string nextSceneName; // 次のシーン名

    [SerializeField] private AudioClip drivingSound; 
    [SerializeField] private AudioClip successSound; 

    private CharacterController _characterController;
    private Vector3 _moveVelocity;
    private Vector3 _velocity;
    private InputAction _move;
    private InputAction _look;
    private float _cameraPitch = 0.0f;
    private FirstPersonController playerController;
    private bool isActive = false;
    private AudioSource drivingAudioSource; 
    private AudioSource successAudioSource; 

    private void Start()
    {
        //PlayerInputからMovez ,Lookを参照動きはキャラクターとほぼ同じ
        _characterController = GetComponent<CharacterController>();
        var playerInput = GetComponent<PlayerInput>();
        _move = playerInput.actions["Move"];
        _look = playerInput.actions["Look"];
        playerController = FindObjectOfType<FirstPersonController>();

        drivingAudioSource = gameObject.AddComponent<AudioSource>();
        successAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        //車の操作がactiveでないか,charactercontoroleerが無効ならupdate関数には参照しない
        if (!isActive || !_characterController.enabled) return;

        //z軸の移動を参照する(前後)
        var moveValue = _move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(0, 0, moveValue.y).normalized;

        _moveVelocity = transform.TransformDirection(moveDirection) * driveSpeed;

        //視点は上下,Clampによりカメラを指定範囲内で動かす
        var lookValue = _look.ReadValue<Vector2>();
        _cameraPitch -= lookValue.y * lookSpeed;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);
        //カメラを左右で回転させる
        cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * lookValue.x * lookSpeed);

        //重力の設定
        if (_characterController.isGrounded)
        {
            _velocity.y = 0f;
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
        }

        _characterController.Move((_moveVelocity + _velocity) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //車でclearのトリガー範囲に入ればクリア
        if (other.CompareTag("ClearTrigger"))
        {
            DisplayClearUI(clearMessage);
        }
    }

    public void EnterCar()
    {
        //車に乗った時の処理
        playerController.gameObject.SetActive(false); // プレイヤーを非表示
        isActive = true; // 車の操作を有効化
        transform.localScale = smallScale; // 車を小さくする
        AdjustColliderSize();//小さくするのに伴ってコライダーのサイズも変更

        if (GetComponent<AudioListener>() == null)
        {
            gameObject.AddComponent<AudioListener>();
        }

        // 車に運転音を設定
        drivingAudioSource.clip = drivingSound;
        drivingAudioSource.loop = true;
        drivingAudioSource.playOnAwake = false;
        drivingAudioSource.Play();
        //乗った時のメッセージUI
        StartCoroutine(ShowMessageAndResume(enterCarMessage));
    }

    private void AdjustColliderSize()
    {
        //smallscaleに対応してコライダーの中心半径高さを変える
        _characterController.center = new Vector3(_characterController.center.x * smallScale.x, _characterController.center.y * smallScale.y, _characterController.center.z * smallScale.z);
        _characterController.height *= smallScale.y;
        _characterController.radius *= smallScale.x;
    }

    private IEnumerator ShowMessageAndResume(string message)
    {
        //乗った時のメッセージの表示と非表示
        Time.timeScale = 0f; // 時間を停止
        ShowMessageUI(message);
        yield return new WaitForSecondsRealtime(3); 
        HideMessageUI(); 
        Time.timeScale = 1f; // 時間を再開
    }

    private void ShowMessageUI(string message)
    {
        //メッセージの内容についての設定
        if (messageUIPanel != null && messageUIText != null)
        {
            messageUIText.text = message.Replace("\\n", "\n"); // メッセージの改行を設定
            messageUIPanel.SetActive(true);

            if (characterImage != null && characterSprite != null)
            {
                characterImage.sprite = characterSprite; // キャラクターの立ち絵を設定
                characterImage.gameObject.SetActive(true);
            }
        }
    }

    //メッセージの非表示
    private void HideMessageUI()
    {
        if (messageUIPanel != null)
        {
            messageUIPanel.SetActive(false);
        }

        if (characterImage != null)
        {
            characterImage.gameObject.SetActive(false); 
        }
    }

    //クリアメッセージの表示
    private void DisplayClearUI(string message)
    {
        Time.timeScale = 0f; // 時間を停止
        if (clearUIPanel != null)
        {
            clearUIPanel.SetActive(true);
            if (clearUIText != null)
            {
                clearUIText.text = message; // クリアメッセージを設定
            }
        }

        if (clearCharacterImage != null && clearCharacterSprite != null)
        {
            clearCharacterImage.sprite = clearCharacterSprite; // キャラクターの立ち絵を設定
            clearCharacterImage.gameObject.SetActive(true); // キャラクター画像を表示
        }

        //効果音を再生
        if (successSound != null && successAudioSource != null)
        {
            successAudioSource.clip = successSound;
            successAudioSource.Play();
        }

        // シーン遷移のコルーチンを開始
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            //待機ののち次シーン読み込み
            StartCoroutine(WaitAndLoadScene(nextSceneName, 4f)); 
        }
    }

    private IEnumerator WaitAndLoadScene(string sceneName, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = 1f; // 時間を再開
        SceneManager.LoadScene(sceneName);
    }
}
