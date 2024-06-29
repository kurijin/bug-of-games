using UnityEngine;

public class Time_SceneInitializer : MonoBehaviour
{
    //シーン始まり時にタイマーを動かす
    private void Start()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StartTimer();
        }
    }
}
