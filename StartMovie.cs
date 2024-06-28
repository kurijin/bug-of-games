using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartMovie : MonoBehaviour
{
    public string nextSceneName; //次シーン名
    public Canvas targetCanvas; // 任意のキャンバス
    public float delayBeforeNextScene = 5f; // 次のシーンに進むまでの秒数
    public Image fadeImage; // フェード用のImage(主に黒)
    public float fadeDuration = 1f; // フェードアウトの時間
    public AudioClip effectSound; // 効果音のオーディオクリップ
    public float volume = 1.0f; // 効果音のボリューム
    public AudioClip clickSound; // ボタンクリック音のオーディオクリップ
    public float clickVolume = 1.0f; // ボタンクリック音のボリューム

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
    }

    // ボタンがクリックされたときに呼び出されるメソッド
    public void Movie()
    {
        //音を再生します
        PlayClickSound();

        //スタート画面非表示
        targetCanvas.gameObject.SetActive(false);

        // ゲーム内時間を再開します
        Time.timeScale = 1f;

        // 一定時間後に次のシーンをロードします
        StartCoroutine(LoadNextSceneAfterDelay(delayBeforeNextScene));
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            //スタート画面用のクリック音
            audioSource.PlayOneShot(clickSound, clickVolume);
        }
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // 現実の時間で遅延を待ちます

        // フェード用のImageを表示します
        fadeImage.gameObject.SetActive(true);

        // 効果音を再生します
        PlayEffectSound();

        // 画面を暗転させます
        yield return StartCoroutine(FadeOut());

        // 次のシーンをロードします
        SceneManager.LoadScene(nextSceneName);
    }

    private void PlayEffectSound()
    {
        if (effectSound != null && audioSource != null)
        {
            //OPの車のブレーキ音
            audioSource.PlayOneShot(effectSound, volume);
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        //今回は黒
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            //設定したfadeDurationに達成するまでだんだん時間をあげてその割合でimageの透過率を上げていく
            elapsedTime += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 最後に完全に暗転させる
        color.a = 1f;
        fadeImage.color = color;
    }
}
