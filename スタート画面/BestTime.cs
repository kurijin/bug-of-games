using UnityEngine;
using UnityEngine.UI;

public class BestTime : MonoBehaviour
{
    public Text bestTimeText; 
    public string bestTimeKey = "BestTime"; 

    private void Start()
    {
        // PlayerPrefsにキーが存在しない場合、またはベストタイムが「00:00」の場合デフォルト99:99にする
        string bestTimeString = PlayerPrefs.GetString(bestTimeKey, "99:99");
        if (!PlayerPrefs.HasKey(bestTimeKey) || bestTimeString == "00:00")
        {
            PlayerPrefs.SetString(bestTimeKey, "99:99");
            PlayerPrefs.Save();
            bestTimeText.text = "99:99";
        }
        else
        {
            // PlayerPrefsからベストタイムを取得
            bestTimeString = PlayerPrefs.GetString(bestTimeKey, "99:99");

            // ベストタイムが保存されていれば表示し、なければデフォルト値を表示
            bestTimeText.text = bestTimeString;
        }
    }
}
