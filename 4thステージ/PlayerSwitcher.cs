using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject virtualCamera; //1stageのカメラ
    public GameObject mainCamera; //bossstageのカメラ
    public GameObject weapon; //剣
    public GameObject cameraObject; //一人称のカメラ
    //上から1,2,bossの動かし方のプレイヤースクリプト
    public RuntimeAnimatorController controllerForPlayer;
    public RuntimeAnimatorController controllerForSecondStagePlayer;
    public RuntimeAnimatorController controllerForBossStagePlayer;

    private MonoBehaviour currentController; // 現在のスクリプトを保持する変数
    private MonoBehaviour currentWeaponController;

    private int currentStage = 1; // 現在のステージを追跡する変数

    private void Start()
    {
        // 最初はvirtualCamera用のスクリプトをアクティブにする
        virtualCamera.SetActive(true);
        weapon.SetActive(false);
        currentController = GetComponent<PlayerController>();
        currentWeaponController = GetComponent<AttachSword>();
    }

    //キーボードでは1,2,3キーでそれぞれ入れ替え,ゲームパッドではLボタン押すたびに1,2,3で切り替える。
    void Update()
    {
        // キーボード入力での切り替え
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToStage(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToStage(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToStage(3);
        }

        // ゲームパッド入力での切り替え
        if (Gamepad.current != null && Gamepad.current.leftShoulder.wasPressedThisFrame)
        {
            currentStage++;
            if (currentStage > 3) currentStage = 1; // 1, 2, 3のループ
            SwitchToStage(currentStage);
        }
    }

    //この関数にUpdate関数で変数を渡す。
    private void SwitchToStage(int stage)
    {
        switch (stage)
        {
            case 1:
                virtualCamera.SetActive(true);
                mainCamera.SetActive(true);
                Animator animator1 = GetComponent<Animator>();
                animator1.runtimeAnimatorController = controllerForPlayer;
                mainCamera.GetComponent<CameraController>().enabled = false;
                weapon.SetActive(false);
                currentController.enabled = false;
                currentController = GetComponent<PlayerController>();
                currentController.enabled = true;
                currentWeaponController.enabled = false;
                cameraObject.SetActive(false);
                break;

            case 2:
            //足音を止める
                if (currentController is SecondStagePlayer)
                {
                    ((SecondStagePlayer)currentController).StopFootstepSound();
                }
                virtualCamera.SetActive(false);
                mainCamera.SetActive(false);
                Animator animator2 = GetComponent<Animator>();
                animator2.runtimeAnimatorController = controllerForSecondStagePlayer;
                weapon.SetActive(false);
                currentController.enabled = false;
                currentController = GetComponent<SecondStagePlayer>();
                currentController.enabled = true;
                currentWeaponController.enabled = false;
                cameraObject.SetActive(true);
                break;

            case 3:
                virtualCamera.SetActive(false);
                mainCamera.SetActive(true);
                Animator animator3 = GetComponent<Animator>();
                animator3.runtimeAnimatorController = controllerForBossStagePlayer;
                mainCamera.GetComponent<CameraController>().enabled = true;
                weapon.SetActive(true);
                currentController.enabled = false;
                currentController = GetComponent<BossStagePlayer>();
                currentController.enabled = true;
                currentWeaponController.enabled = true;
                cameraObject.SetActive(false);
                break;
        }
    }
}
