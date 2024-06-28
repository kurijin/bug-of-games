using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartScript : MonoBehaviour
{
    public GameObject targetObject; // ボタンを押した時にStart画面を消すためにそのスクリプトがアタッチされているオブジェクトを参照

    void Start()
    {
        // シーン開始時にゲーム内時間を停止します
        Time.timeScale = 0f;
    }
    // ボタンがクリックされたときに呼び出されるメソッド
    public void OnStartButtonClicked()
    {
        // StartMovieスクリプトを取得してメソッドを呼び出します
        StartMovie targetScript = targetObject.GetComponent<StartMovie>();
        if (targetScript != null)
        {
            targetScript.Movie();
        }
    }
}
