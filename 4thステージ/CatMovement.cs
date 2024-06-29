using System.Collections;
using UnityEngine;


//猫のオブジェクトが自動で動くもの
//このボスをplayerの剣で切るとクリアできる。
public class CatMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public GameObject moveArea; // 移動範囲であるオブジェクト
    public Canvas endingUI;
    //定期的に猫の鳴き声
    public AudioClip meowSound; 
    public float meowInterval = 5f; 

    //EndStageで流しているBGMを猫のボス戦の時には曲を切り替える
    public AudioSource bgmSource; 
    public AudioClip bgmClipB; 

    private Vector3 targetPosition;
    private Bounds moveBounds;
    private Animator animator;
    private AudioSource audioSource; 

    private void OnEnable()
    {
        SwitchBGM();
    }

    void Start()
    {
        if (GetComponent<CapsuleCollider>() == null)
        {
            gameObject.AddComponent<CapsuleCollider>();
        }
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; 
        }

        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>(); 

        if (moveArea != null)
        {
            moveBounds = moveArea.GetComponent<Collider>().bounds;
        }
        //移動位置をランダムに決める。
        SetRandomTargetPosition();
        //定期的な間隔で鳴くもの
        StartCoroutine(MeowAtIntervals());
    }

    void Update()
    {
        MoveToTarget();
    }

    void SetRandomTargetPosition()
    {
        if (moveArea != null)
        {
            //移動エリア内でランダムで選択する
            targetPosition = new Vector3(
                Random.Range(moveBounds.min.x, moveBounds.max.x),
                transform.position.y,
                Random.Range(moveBounds.min.z, moveBounds.max.z)
            );
        }
    }

    //目標地点に向かってキャラクターを動かす
    void MoveToTarget()
    {
        animator.SetBool("isWalking", true);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 移動方向を向く
        if (targetPosition != transform.position)
        {
            //目標地点と現在の地点との地点との差を正規化して方向ベクトルを求める
            Vector3 direction = (targetPosition - transform.position).normalized;
            //目標地点への向きを計算
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            //現在向きを目標地点に合わせる.5fはその速さの重み(大きいほど速く向く)
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); 
        }

        // 新しい目標地点を設定
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
        }
    }

    //剣で切られた時の処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }

            if (endingUI != null)
            {
                endingUI.gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    private IEnumerator MeowAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(meowInterval);
            PlayMeowSound();
        }
    }

    private void PlayMeowSound()
    {
        if (audioSource != null && meowSound != null)
        {
            audioSource.PlayOneShot(meowSound);
        }
    }

    //元のbgmと新しいbgmを切り替えるもの
    private void SwitchBGM()
    {
        if (bgmSource != null && bgmClipB != null)
        {
            bgmSource.clip = bgmClipB;
            bgmSource.Play();
        }
    }
}
