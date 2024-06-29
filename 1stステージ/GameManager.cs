using UnityEngine;

public class GameManager : MonoBehaviour
{
    private first_DialogueManager dialogueManager;

    private void Start()
    {
        //シーン再生時ダイアログを流す
        dialogueManager = FindObjectOfType<first_DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(true);
        }
    }

    public void PlayerReachedGoal()
    {
        //ステージクリア後のダイアログを流すnagasu
        Time.timeScale = 0f;
        first_UIManager uiManager = FindObjectOfType<first_UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowClearUI();
        }

        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(false);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
