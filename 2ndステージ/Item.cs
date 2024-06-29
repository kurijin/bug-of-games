using UnityEngine;


//itemの設定
public class Item : MonoBehaviour
{
    //item名とアイテムの画像,説明文,拾える範囲
    public string itemName;
    public string itemDescription;
    public Sprite itemSprite; // アイテムの画像
    public float pickupRange = 2.0f;

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            //コライダーがないならBoxColliderをデフォルトで追加しトリガーをonにする
            collider = gameObject.AddComponent<BoxCollider>();
        }
        collider.isTrigger = true;
    }
}
