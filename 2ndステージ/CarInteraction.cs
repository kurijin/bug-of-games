using UnityEngine;
using UnityEngine.InputSystem;

public class CarInteraction : MonoBehaviour
{
    //車に乗るための必要なものを参照
    [SerializeField] private string requiredItemName = "鍵2"; // 必要なアイテム名
    [SerializeField] private Inventory playerInventory; // プレイヤーのインベントリ参照
    [SerializeField] private CarController carController; // 車のコントローラー参照
    [SerializeField] private PlayerInput playerInput; // プレイヤーの PlayerInput コンポーネント
    private bool playerInRange = false;
    private InputAction pickupAction;
    private InputAction walkAction;
    private InputAction lookAction;

    private void Start()
    {
        //PickUpのアクションによりplayerとcarを繋ぐ
        pickupAction = playerInput.actions["PickUp"];
        walkAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
    }

    //以下の3つのメソッドはPlayerがCarの範囲内かを確認し、そこでpickupのアクションを行ったとき,特定のアイテムがあれば車に乗れるというもの.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            pickupAction.Enable();
            pickupAction.performed += OnPickup;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            pickupAction.performed -= OnPickup;
            pickupAction.Disable();
        }
    }

    private void OnPickup(InputAction.CallbackContext context)
    {
        if (playerInRange && playerInventory.HasItem(requiredItemName))
        {
            EnterCarAction();
        }
    }

    //車が車に乗った時Playerinputのwalk,lookアクション以外を無効にする(Pick upを無効)
    private void EnterCarAction()
    {
        carController.EnterCar();

        foreach (var action in playerInput.actions)
        {
            if (action != walkAction && action != lookAction)
            {
                action.Disable();
            }
            else
            {
                action.Enable();
            }
        }
    }
}
