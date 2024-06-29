using UnityEngine;

public class first_UIManager : MonoBehaviour
{
    //クリア後のUiを表示させたり消したりするもの
    [SerializeField] private GameObject clearUIPanel;

    public void ShowClearUI()
    {
        clearUIPanel.SetActive(true);
    }

    public void HideClearUI()
    {
        clearUIPanel.SetActive(false);
    }
}
