using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//動かし方は1,2stageと同様それに攻撃を加えている
public class Player_onBoss : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private Vector2 movementInput;
    private Vector3 velocity;

    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float gravity = -9.81f;
    public float attackDuration = 0.5f;

    private bool isRunning = false;
    private bool isAttacking = false;
    private bool isDialogueActive = false;


    // 剣と剣のコライダーを得る（攻撃時のみコライダーを有効）
    public GameObject sword;
    private Collider swordCollider;

    public DialogueManager dialogueManager;

    //ゲームオーバーに関するもの
    public GameObject gameOverUI; 
    private bool isGameOver = false; 

    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    private InputAction moveAction;
    private InputAction runAction;
    private InputAction attackAction;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (sword != null)
        {
            swordCollider = sword.GetComponent<Collider>();
            swordCollider.enabled = false;
        }

        audioSource = gameObject.AddComponent<AudioSource>();

        var input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();
        moveAction = input.currentActionMap.FindAction("Move");
        runAction = input.currentActionMap.FindAction("Run");
        attackAction = input.currentActionMap.FindAction("Attack");

        // ダイアログ表示
        StartCoroutine(StartDialogue());
    }

    private void Update()
    {
        CustomUpdate();
    }

    //攻撃中などは動きを無効にする
    private void CustomUpdate()
    {
        if (isDialogueActive) return;

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        isRunning = runAction.ReadValue<float>() > 0.5f;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;


        if (!isAttacking)
        {
            Vector3 move = new Vector3(moveValue.x, 0, moveValue.y);
            controller.Move(move * currentSpeed * Time.deltaTime);

            if (move != Vector3.zero)
            {
                transform.forward = move;
            }

            float speed = move.magnitude * currentSpeed;
            animator.SetFloat("Speed", speed);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (attackAction.triggered)
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetTrigger("Attack");
        //攻撃時は剣のコライダーを有効にする.          
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
        StartCoroutine(EnableCollider());
        StartCoroutine(CompleteAttack());
        //攻撃中は攻撃を止める
        movementInput = Vector2.zero;
        isRunning = false;
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForEndOfFrame();
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
    }

    private IEnumerator CompleteAttack()
    {
        yield return new WaitForSeconds(attackDuration);
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
        isAttacking = false;
    }

    //シーン開始時の会話UIの表示
    private IEnumerator StartDialogue()
    {
        yield return null;
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue();
            isDialogueActive = true;
            while (dialogueManager.isDialogueActive)
            {
                yield return null;
            }
            isDialogueActive = false;
        }
    }

    //PlayerのHPが0になった時PlayerHealthから呼び出される.
    public void Die()
    {
        isGameOver = true;

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        //死亡アニメーションのためのトリガー
        animator.SetTrigger("Die");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    //GameOverUIのリトライ画面でリトライを押すと復帰する
    public void OnRetry()
    {
        if (isGameOver)
        {
            isGameOver = false;
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }
            StartCoroutine(ReloadScene());
        }
    }
    
    private IEnumerator ReloadScene()
    {
        // 現在のシーン名を取得
        string currentSceneName = SceneManager.GetActiveScene().name;
        // タイマーのテキストオブジェクトを一時的に保持
        Text timerText = TimerManager.Instance?.timerText;
        //非同期ロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 新しいシーンがロードされた後にTimerManagerのテキストオブジェクトを再設定

        
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.SetTimerTextObject(timerText);
        }
        
    }
}
