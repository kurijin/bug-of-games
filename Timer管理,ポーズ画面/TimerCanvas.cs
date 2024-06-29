using UnityEngine;

//TimerUIをDontdestoryにする
public class TimerCanvas : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
