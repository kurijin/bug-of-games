using UnityEngine;


//シーンの終わりと同時にタイマーストップ
public class SceneEndManager : MonoBehaviour
{
    public void EndScene()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }
    }
}
