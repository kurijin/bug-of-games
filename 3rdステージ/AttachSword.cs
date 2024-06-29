using UnityEngine;

public class AttachSword : MonoBehaviour
{
    //剣を手につけるスクリプト,剣のprefabと手のひらを参照
    public GameObject swordPrefab;
    public Transform rightHand;

    void Start()
    {
        // 剣が存在するか確認し,ついていないなら剣を手に生成させる。
        if (rightHand.childCount > 0)
        {
            foreach (Transform child in rightHand)
            {
                if (child.CompareTag("Sword"))
                {
                    return; 
                }
            }
        }
        else{
        // 剣のプレハブを生成
            GameObject sword = Instantiate(swordPrefab);

        // 生成した剣の親を手のボーンに設定
            sword.transform.SetParent(rightHand);

        // 剣の位置と回転をリセット
            sword.transform.localPosition = Vector3.zero;
            sword.transform.localRotation = Quaternion.identity;

        // 剣のスケールをリセット
            sword.transform.localScale = Vector3.one;

        // 剣にタグを設定
            sword.tag = "Sword";
        }
    }
}
