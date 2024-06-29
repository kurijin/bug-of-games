using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//キャラクター同士の会話設定
public class first_DialogueManager : MonoBehaviour
{
    //会話を表示するパネルとテキストの場所を参照
    public GameObject dialoguePanel;  
    public Text dialogueText;

    //キャラクター1,2の立ち絵を表示する場所と画像
    public Image character1Image;
    public Image character2Image;

    public Sprite character1Sprite;
    public Sprite character2Sprite;

    //スタート時の会話内容と、会話しているキャラクター0:猫1:Player
    public string[] startDialogueLines;
    public bool[] isCharacter1SpeakingStart;
    public GameObject tutorialPanel;   //遊び方のパネル

    //ステージクリア時のパネル(End Stageでは使わない)
    public string[] endDialogueLines;
    public bool[] isCharacter1SpeakingEnd;

    //猫のキャラが喋るための音声
    public AudioClip character2SpeakingSound;
    public float soundVolume = 1.0f;

    //現在表示している会話を参照するもの
    private string[] currentDialogueLines;
    private bool[] currentIsCharacter1Speaking;
    private int currentLineIndex = 0;
    private bool isShowingTutorial = false;

    private AudioSource audioSource;

    private void Start()
    {
        StartDialogue(true);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;
    }
    
    //会話を表示→遊び方と表示する
    private void Update()
    {
        //会話パネルが表示されてるか,遊び方が表示されているか
        if (dialoguePanel != null && (dialoguePanel.activeSelf || isShowingTutorial))
        {
            //エンターorクリックorゲームパッドの右ボタン
            if ((Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame) || 
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) || 
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                //遊び方が表示されているときは遊び方を閉じる,ダイアログの場合は会話を進める
                if (isShowingTutorial)
                {
                    HideTutorial();
                }
                else
                {
                    AdvanceDialogue();
                }
            }
        }
    }

    private void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        isShowingTutorial = true;
        Time.timeScale = 0f;
    }

    private void HideTutorial()
    {
        tutorialPanel.SetActive(false);
        isShowingTutorial = false;
        Time.timeScale = 1f;
    }

    public void StartDialogue(bool isStartDialogue)
    {
        //ダイアログの会話数とキャラクター情報をを取得する
        if (isStartDialogue)
        {
            currentDialogueLines = startDialogueLines;
            currentIsCharacter1Speaking = isCharacter1SpeakingStart;
        }
        else
        {
            currentDialogueLines = endDialogueLines;
            currentIsCharacter1Speaking = isCharacter1SpeakingEnd;
        }

        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;
        UpdateDialogue();
    }

    private void AdvanceDialogue()
    {
        //入力された会話の表示以上になるまでは会話を進めて,会話の長さに達したら会話を終える。
        currentLineIndex++;
        if (currentLineIndex < currentDialogueLines.Length)
        {
            UpdateDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void UpdateDialogue()
    {
        dialogueText.text = currentDialogueLines[currentLineIndex];
        //会話を進める。Playerと猫の立ち絵を入れ替える。猫の場合は声を再生する。
        if (currentIsCharacter1Speaking[currentLineIndex])
        {
            character1Image.sprite = character1Sprite;
            character1Image.gameObject.SetActive(true);
            character2Image.gameObject.SetActive(false);
        }
        else
        {
            character2Image.sprite = character2Sprite;
            character2Image.gameObject.SetActive(true);
            character1Image.gameObject.SetActive(false);

            PlayCharacter2Sound();
        }
    }

    private void PlayCharacter2Sound()
    {
        if (character2SpeakingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(character2SpeakingSound);
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        //もしシーン再生直後のダイアログが終われば遊び方を表示して,ステージクリア後であれば2ndステージに移動
        if (currentDialogueLines == startDialogueLines)
        {
            ShowTutorial();
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("2 stage");
        }
    }
}
