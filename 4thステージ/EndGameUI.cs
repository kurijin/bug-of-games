using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    public float displayDuration = 4.0f; 
    public int timeThreshold; // 閾値
    public string sceneA; // 閾値以下の場合のシーン名(happyend)
    public string sceneB; // 閾値以上の場合のシーン名(badend)
    public Text timerText; 
    public string bestTimeKey = "BestTime"; 

    private void OnEnable()
    {
        StartCoroutine(DisplayUIAndCheckTimer());
    }

    private IEnumerator DisplayUIAndCheckTimer()
    {
        // タイマーの数字を取得
        float elapsedTime = TimerManager.Instance.GetElapsedTime();
        int timerValue = Mathf.FloorToInt(elapsedTime);

        // ベストタイムを取得 (タイトル画面でのキーと一致している必要性)
        string bestTimeString = PlayerPrefs.GetString(bestTimeKey, "99:99");
        int bestTime = ConvertToSeconds(bestTimeString);
        Debug.Log(bestTime);
        Debug.Log(timerValue);

        // ベストタイムを更新する
        if (timerValue < bestTime)
        {
            string newBestTimeString = ConvertToTimeString(timerValue);
            Debug.Log(newBestTimeString);
            PlayerPrefs.SetString(bestTimeKey, newBestTimeString);
            PlayerPrefs.Save();
        }

        gameObject.SetActive(true);

        // タイマーのUIを非表示にする
        TimerManager.Instance.timerText.gameObject.SetActive(false);

        // 指定された時間待機
        yield return StartCoroutine(WaitForRealSeconds(displayDuration));

        gameObject.SetActive(false);



        // タイマーの値によってシーンを変更
        if (timerValue <= timeThreshold)
        {
            // タイマーのリセット
            TimerManager.Instance.ResetTimer();
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneA);
        }
        else
        {
            // タイマーのリセット
            TimerManager.Instance.ResetTimer();
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneB);
        }
    }

    private int ConvertToSeconds(string timeString)
    {
        int totalSeconds = 0;
        string[] timeParts = timeString.Split(':');
        if (timeParts.Length == 2)
        {
            int minutes;
            int seconds;
            if (int.TryParse(timeParts[0], out minutes) && int.TryParse(timeParts[1], out seconds))
            {
                totalSeconds = minutes * 60 + seconds; // 総秒数に変換
            }
        }
        return totalSeconds;
    }

    private string ConvertToTimeString(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    private void ResetDontDestroyOnLoad()
    {
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == null) 
            {
                Destroy(obj);
            }
        }
    }

    private void OnDisable()
    {
        ResetDontDestroyOnLoad();
    }
}
