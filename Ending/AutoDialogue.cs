using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoDialogue : MonoBehaviour
{
    public Canvas dialogueCanvas; // ダイアログ用のCanvas
    public Text dialogueText; // ダイアログ用のText
    public string[] dialogues; // セリフの配列
    public float dialogueDelay = 2.0f; // セリフを表示する間隔

    public Transform targetObject; // カメラが向かうターゲットオブジェクト
    public float moveSpeed = 1.0f; // カメラの移動速度
    public float stopDistance = 2.0f; // ターゲットの前で停止する距離

    public AudioClip soundEffect; 
    private AudioSource audioSource; 

    private int currentDialogueIndex = 0;
    private float timer = 0f;
    private bool dialogueFinished = false;
    private bool soundPlayed = false; 

    void Start()
    {
        dialogueCanvas.gameObject.SetActive(true); 
        ShowNextDialogue();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        //ダイアログが自動で進む,表示時間まで行ったら次に行って,再び0フレームからupdate関数で時間経過を数える
        if (!dialogueFinished)
        {
            timer += Time.deltaTime;
            if (timer >= dialogueDelay)
            {
                ShowNextDialogue();
                timer = 0f;
            }
        }
        else
        {
            StartCoroutine(MoveCameraToTarget());
        }
    }

    void ShowNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else
        {
            dialogueCanvas.gameObject.SetActive(false); // 全てのセリフが表示されたらダイアログを非表示にする
            dialogueFinished = true; 
        }
    }

    //mainカメラを取得
    private IEnumerator MoveCameraToTarget()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null && targetObject != null)
        {
            //ターゲット(猫)とメインカメラのポジションの差を正規化して方向ベクトルを
            Vector3 direction = (targetObject.position - mainCamera.transform.position).normalized;
            //ターゲットのやや前で止まる
            Vector3 targetPosition = targetObject.position - direction * stopDistance;

            // カメラをターゲットオブジェクトに向けて近づける
            while (Vector3.Distance(mainCamera.transform.position, targetPosition) >= 0.1f)
            {
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                mainCamera.transform.LookAt(targetObject);
                yield return null;
            }

            if (!soundPlayed && soundEffect != null && audioSource != null)
            {
                audioSource.PlayOneShot(soundEffect);
                soundPlayed = true;
            }

            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            //badendの場合カメラも動かさずクリア
            if (!soundPlayed && soundEffect != null && audioSource != null)
            {
                audioSource.PlayOneShot(soundEffect);
                soundPlayed = true;
            }
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("StartScene");
        }
    }
}
