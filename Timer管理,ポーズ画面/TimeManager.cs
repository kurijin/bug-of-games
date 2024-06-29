using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    // シングルトンでシーンに一つのみで保持する
    public static TimerManager Instance { get; private set; }

    // タイマーのテキストUIを参照するためのフィールド
    public Text timerText;
    private float elapsedTime = 0f; // 経過時間を保持するフィールド
    private bool isRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // このオブジェクトをシーン間で破棄しないように設定
        }
        else if (Instance != this)
        {
            Destroy(gameObject); //重複してたら削除
            return;
        }
    }

    // シーンがロードされたときにイベント登録
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // シーンが終わる時にイベント登録解除
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime; //毎フレーム経過時間をふさせていく
            UpdateTimerText(); // タイマーのテキストを更新
        }
    }

    // タイマーを開始するメソッド
    public void StartTimer()
    {
        isRunning = true;
    }

    // タイマーを停止するメソッド
    public void StopTimer()
    {
        isRunning = false;
    }

    // タイマーをリセットするメソッド
    public void ResetTimer()
    {
        isRunning = false;
        elapsedTime = 0f; // 経過時間をリセット
        UpdateTimerText(); // タイマーのテキストを更新
    }

    // タイマーのテキストを更新するメソッド
    private void UpdateTimerText()
    {
        if (timerText != null) 
        {
            // 経過時間を60で割ったものの商が分で,余りが秒それをMM:SSで表す
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    //シーンでテキストオブジェクトを設定
    public void SetTimerTextObject(Text newTimerText)
    {
        timerText = newTimerText;
        UpdateTimerText(); 
    }

    // シーンがロードされたときのもの
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetupTimerText());
    }

    // シーンが完全にロードされるのを待ってからタイマーのテキストを設定する
    private IEnumerator SetupTimerText()
    {
        yield return new WaitForSeconds(0.5f); //てシーンが完全にロードされるのを待つ遅延

        // シーン内のTimerTextオブジェクトを設定してUIとして表示
        Text newTimerText = GameObject.Find("TimerText")?.GetComponent<Text>();
        if (newTimerText != null)
        {
            SetTimerTextObject(newTimerText);
        }
    }

    // 経過時間を取得するメソッド
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
