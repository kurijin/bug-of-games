using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ゲームのポーズメニュー
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // ポーズメニューのUI
    public GameObject howToPlayUI; // 遊び方のUI
    public Button resumeButton; // 再開ボタン
    public Button howToPlayButton; // 遊び方ボタン
    public Button quitButton; // 終了ボタン

    private bool isPaused = false;
    private bool isHowToPlayActive = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        howToPlayUI.SetActive(false);

        // 初期選択を再開ボタンに設定
        resumeButton.Select();
    }

    // yキーorゲームパッドのプラスを押せばポーズ
    void Update()
    {
        if ((Keyboard.current != null && Keyboard.current.yKey.wasPressedThisFrame) || 
            (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // 遊び方UIを閉じるためのエンターキー、マウスクリック、またはゲームパッドの右ボタンの入力
        if (isHowToPlayActive && ((Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame) || 
                                  (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) || 
                                  (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)))
        {
            CloseHowToPlay();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        howToPlayUI.SetActive(false); // 遊び方のUIも非表示にする
        Time.timeScale = 1f;
        isPaused = false;
        isHowToPlayActive = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // ポーズ時に再開ボタンを選択状態にする
        resumeButton.Select();
    }

    // 遊び方のUIを表示する
    public void ShowHowToPlay()
    {
        howToPlayUI.SetActive(true);
        isHowToPlayActive = true;
    }

    // 遊び方のUIを閉じる
    public void CloseHowToPlay()
    {
        howToPlayUI.SetActive(false);
        isHowToPlayActive = false;
    }

    // ゲームをやめる処理。タイマーのオブジェクトはリセット
    public void QuitGame()
    {
        // DontDestroyOnLoadで保持されているオブジェクトをリセットする
        ResetDontDestroyOnLoadObjects();

        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    void ResetDontDestroyOnLoadObjects()
    {
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            Destroy(timerManager.gameObject);
        }
    }
}
