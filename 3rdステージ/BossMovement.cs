using UnityEngine;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    //Bossの設定
    public Transform player;
    public float moveSpeed = 2f;
    public float runSpeed = 4f; 
    public float attackInterval = 10f; 
    public float moveDuration = 3f; // ボスが動き続ける時間
    public float idleDuration = 5f; // ボスが停止する時間

    public GameObject rightSword; 
    public GameObject leftSword; 
    public float attackDuration = 1.0f; 

    public AudioClip swordSound; 
    public AudioClip stopSound; 
    private AudioSource audioSource; 

    private Rigidbody rb;
    private Animator animator;
    private BoxCollider rightSwordCollider; 
    private BoxCollider leftSwordCollider; 
    private float nextAttackTime;
    private float nextMoveTime;
    private bool isMoving;
    public bool isRunning;

    private bool isAttacking;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        nextAttackTime = Time.time + attackInterval;
        nextMoveTime = Time.time + moveDuration;
        isMoving = true;
        isRunning = false;
        isAttacking = false;

        audioSource = gameObject.AddComponent<AudioSource>(); 

        //ボスは右手と左手の剣に大きめのBoxColiderをアタッチしていて攻撃時のみそのBoxColiderを有効にする,最初は無効
        if (rightSword != null)
        {
            rightSwordCollider = rightSword.GetComponent<BoxCollider>();
            rightSwordCollider.enabled = false; 
        }

        if (leftSword != null)
        {
            leftSwordCollider = leftSword.GetComponent<BoxCollider>();
            leftSwordCollider.enabled = false; 
        }
    }

    void Update()
    {
        if (!isAttacking)
        {
            //攻撃時じゃない時はキャラクターを追いかける。
            if (isMoving)
            {
                FollowPlayer();
                //移動中現在時間が次の停止時間以上になると止まる
                if (Time.time >= nextMoveTime)
                {
                    StopMoving();
                }
            }
            else
            {
                //動いてない状態で移動開始時間をこしたら動く
                if (Time.time >= nextMoveTime)
                {
                    StartMoving();
                }
            }

            //攻撃開始時間を超えたら攻撃をする
            if (Time.time >= nextAttackTime)
            {
                PerformRandomAttack();
                nextAttackTime = Time.time + attackInterval; // 攻撃後の硬直時間
            }
        }
    }

    void FollowPlayer()
    {
        //現在の位置とプレイヤーの位置との差を正規化して方向を求める。
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (!isRunning)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += direction * runSpeed * Time.deltaTime;
        }

        if (direction.magnitude > 0.1f) // 少しの動きでもWalkアニメーションを再生
        {
            animator.SetBool("isWalking", true);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void PerformRandomAttack()
    {
        isAttacking = true;
        int randomValue = Random.Range(0, 100);
        if (randomValue < 50)
        {
            GetComponent<BossAttackA>().Attack();
        }
        else if (randomValue < 75)
        {
            GetComponent<BossAttackB>().Attack();
        }
        else if (randomValue < 90)
        {
            GetComponent<BossAttackC>().Attack();
        }
        else
        {
            GetComponent<BossAttackD>().Attack();
        }

        // 現在のアニメーションの状態情報を取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        // アニメーションの長さに応じて、攻撃終了処理を呼び出す.
        Invoke("AttackFinished", animationLength);
    }

    //攻撃と同様の処理
    void StopMoving()
    {
        isMoving = false;
        animator.SetBool("isWalking", false);
        nextMoveTime = Time.time + idleDuration;

        //止まっている時Bossの咆哮音声を再生
        if (stopSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(stopSound);
        }
    }

    //動き始める
    void StartMoving()
    {
        isMoving = true;
        animator.SetBool("isWalking", true);
        nextMoveTime = Time.time + moveDuration;
    }

    //以下は両手のアニメーションイベントが開始した時に両手についているBOX Coliderを有効にして,アニメーションが終わればColiderを無効にする
    public void EnableRightSwordCollider()
    {
        if (rightSwordCollider != null)
        {
            rightSwordCollider.enabled = true;
        }
    }

    public void DisableRightSwordCollider()
    {
        if (rightSwordCollider != null)
        {
            rightSwordCollider.enabled = false;
        }
    }

    public void EnableLeftSwordCollider()
    {
        if (leftSwordCollider != null)
        {
            leftSwordCollider.enabled = true;
        }
    }

    public void DisableLeftSwordCollider()
    {
        if (leftSwordCollider != null)
        {
            leftSwordCollider.enabled = false;
        }
    }

    // 剣の効果音
    public void PlaySwordSound()
    {
        if (audioSource != null && swordSound != null)
        {
            audioSource.PlayOneShot(swordSound);
        }
    }

    //攻撃アニメーション終了時に呼ばれるメソッド
    public void AttackFinished()
    {
        isAttacking = false;
        //攻撃終了後に次の移動または停止を設定
        if (isMoving)
        {
            StartMoving();
        }
        else
        {
            StopMoving();
        }
    }
}
