using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// CharacterControllerとPlayerInputがつける
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerController : MonoBehaviour 
{
    // 移動とジャンプの速度、ジャンプの音、音量を設定
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 6;
    [SerializeField] private float walkJumpPower = 3;
    [SerializeField] private float runJumpPower = 4.5f; 
    [SerializeField] private AudioClip jumpSound; 
    [SerializeField] private float jumpVolume = 1.0f; 

    //コンポーネントとInputSystemの参照
    private CharacterController _characterController;
    private Transform _transform;
    private Vector3 _moveVelocity;
    private InputAction _move;
    private InputAction _jump;
    private InputAction _run;
    private Animator animator;
    private Transform currentPlatform;
    private Vector3 platformLocalPosition;
    private Rigidbody _rigidbody;
    private AudioSource audioSource; 

    //Playerがギミックにあたりスタン状態かどうかを示すフラグ
    private bool isStunned = false;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _transform = transform;
        animator = GetComponent<Animator>();

        // PlayerInputからアクションマップを取得して有効にする、そのうちMove, Jump, Run のアクションを得る
        var input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();
        _move = input.currentActionMap.FindAction("Move");
        _jump = input.currentActionMap.FindAction("Jump");
        _run = input.currentActionMap.FindAction("Run");
        // ジャンプ音用の AudioSource を設定
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = jumpVolume;
    }

    public void CustomUpdate()
    {
        // スタン状態の場合は処理を中断
        if (isStunned) return;

        // 入力に対して移動している値を取得
        var moveValue = _move.ReadValue<Vector2>();
        bool isRunning = _run.ReadValue<float>() > 0.5f;
        //is Runningがtrueの場合現在のスピードをrunSpeedで、そうじゃない場合walkSpeedを現在の速度とするジャンプパワーも同様
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        float currentJumpPower = isRunning ? runJumpPower : walkJumpPower;

        // 移動ベクトルを計算
        _moveVelocity.x = moveValue.x * currentSpeed;
        _moveVelocity.z = moveValue.y * currentSpeed;

        //現在の位置に対して向いている方向にPlayerを向ける(y=0で固定でx,zに対して)
        _transform.LookAt(_transform.position + new Vector3(_moveVelocity.x, 0, _moveVelocity.z));

        // 地上にいるかどうかでジャンプを変える
        if (_characterController.isGrounded)
        {
            // ジャンプ入力があった場合の処理
            if (_jump.WasPressedThisFrame())
            {
                _moveVelocity.y = currentJumpPower;
                PlayJumpSound(); 
            }
            //アニメーターのパラメータのgroudedをtrueにする（地面にいる判定にする)
            animator.SetBool("Grounded", true);
        }
        else
        {
            // 空中にいる場合の重力計算(Time.deltaTimeで前回のフレームから現在フレームの経過時間をかける)
            _moveVelocity.y += Physics.gravity.y * Time.deltaTime;
            animator.SetBool("Grounded", false);
        }

        // キャラクターの移動を行う
        _characterController.Move(_moveVelocity * Time.deltaTime);

        // アニメーションのパラメーターを設定する。
        animator.SetFloat("MoveSpeed", new Vector3(_moveVelocity.x, 0, _moveVelocity.z).magnitude);
        animator.SetFloat("verticalSpeed", _moveVelocity.y);
        animator.SetFloat("Speed", currentSpeed);
    }

    private void Update()
    {
        //CustomUpdataという関数を定義することで他メソッドからアクセスでき,Update関数の振る舞いを行える
        CustomUpdate(); 
    }

    //ジャンプ音を再生するメソッド
    private void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    //Playerがギミックヒットした場合の処理
    public void HitPlayer(Vector3 force, float stunTime)
    {
        StartCoroutine(Stun(stunTime)); // スタン処理を開始,スタン時間(動き不能)があるためコルーチンで処理をする
        _characterController.Move(force * Time.deltaTime); //ギミックによって飛ばされる
    }

    // スタン状態を管理するコルーチン
    private IEnumerator Stun(float duration)
    {
        isStunned = true; // スタン状態
        yield return new WaitForSeconds(duration); //指定時間待つ
        isStunned = false; // スタン状態を解除
    }
}
