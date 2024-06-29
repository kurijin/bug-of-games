using UnityEngine;
using UnityEngine.UI;

//1stと同様
public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueUI;
    public Text dialogueText;
    public GameObject tutorialUI;
    public Image characterImage1;
    public Image characterImage2;
    public Sprite[] characterSprites; 
    public string[] dialogues;
    public int[] dialogueCharacterIndices; 
    private int currentDialogueIndex = 0;
    public bool isDialogueActive { get; private set; } = false; 
    private bool isTutorialActive = false;

    void Start()
    {
        dialogueUI.SetActive(false);
        tutorialUI.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))) // ゲームパッドの右ボタン、リターンキー、またはマウスの左クリック
        {
            AdvanceDialogue();
        }
        else if (isTutorialActive && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))) 
        {
            CloseTutorial();
        }
    }

    public void StartDialogue()
    {
        isDialogueActive = true;
        dialogueUI.SetActive(true);
        Time.timeScale = 0f; // ゲームを停止
        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
        ShowCharacterImage(); // 立ち絵を表示
    }

    private void ShowCharacterImage()
    {
        int characterIndex = dialogueCharacterIndices[currentDialogueIndex];
        if (characterIndex == 0)
        {
            characterImage1.sprite = characterSprites[characterIndex];
            characterImage1.gameObject.SetActive(true);
            characterImage2.gameObject.SetActive(false);
        }
        else if (characterIndex == 1)
        {
            characterImage2.sprite = characterSprites[characterIndex];
            characterImage2.gameObject.SetActive(true);
            characterImage1.gameObject.SetActive(false);
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
            ShowCharacterImage(); // 次の立ち絵を表示
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.SetActive(false);
        tutorialUI.SetActive(true); 
        isTutorialActive = true; 
    }

    private void CloseTutorial()
    {
        isTutorialActive = false;
        tutorialUI.SetActive(false); 
        Time.timeScale = 1f; 
    }
}
