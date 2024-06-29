using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


//playerのhpのUI管理のスクリプト
public class HeartUIManager : MonoBehaviour
{
    public static HeartUIManager instance;

    public GameObject heartPrefab;
    public Transform healthPanel;

    private List<GameObject> hearts = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealthUI(int currentHP, int maxHP)
    {
        //ハートhp分まで合わせる.ハートのprefabをUiのpanelの子としてmaxHp分インスタンス化する
        if (hearts.Count < maxHP)
        {
            for (int i = hearts.Count; i < maxHP; i++)
            {
                GameObject heart = Instantiate(heartPrefab, healthPanel);
                hearts.Add(heart);
            }
        }
        else if (hearts.Count > maxHP)
        {
            for (int i = hearts.Count - 1; i >= maxHP; i--)
            {
                Destroy(hearts[i]);
                hearts.RemoveAt(i);
            }
        }

        //現在のhpにハートの色の変更現在hpは白色で,減った分に関しては黒にする
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].GetComponent<Image>().color = i < currentHP ? Color.white : Color.black;
        }
    }
}
