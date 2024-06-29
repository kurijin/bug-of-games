using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//Bossの体力管理のスクリプト
public class BossHealth : MonoBehaviour
{
    //BossのHpが0になった時クリアUIを表示
    public int maxHP = 10;
    public float invincibilityDuration = 2.0f;
    public float flashDuration = 0.1f;
    public GameObject clearUI; // クリアUI
    public float clearUIDelay = 2.0f; // クリアUIが表示されるまでの遅延時間

    private int currentHP;
    private bool isInvincible = false;
    private Renderer[] renderers;
    private Animator animator;

    private void Start()
    {
        currentHP = maxHP;
        renderers = GetComponentsInChildren<Renderer>();

        animator = GetComponent<Animator>();
        if (clearUI != null)
        {
            clearUI.SetActive(false); // 初期状態でクリアUIを非表示に
        }
    }

    //攻撃を受けたらダメージを受ける。引いた数が0出ないなら無敵時間を発動、0なら死亡
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHP -= damage;

            if (currentHP <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvincibilityFlash());
            }
        }
    }

    //志望アニメーションの表示
    private void Die()
    {
        Debug.Log("Boss died");
        animator.SetTrigger("Die"); // Dieアニメーションを再生
        StartCoroutine(HandleBossDeath());
    }

    private IEnumerator HandleBossDeath()
    {
        // アニメーションの再生が終わるまで待つ
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

         Time.timeScale = 0f;
        // クリアUIを表示
        if (clearUI != null)
        {
            clearUI.SetActive(true);
        }
        Time.timeScale = 1f;
        // 一定時間待つ
        yield return new WaitForSeconds(clearUIDelay);
        //最後のシーンに移動
        SceneManager.LoadScene("End Stage"); 
    }

    //無敵時間の設定。
    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        //現在の時間に無敵時間を足して無敵時間が終わるまでフラッシュする
        float invincibilityEndTime = Time.time + invincibilityDuration;
        while (Time.time < invincibilityEndTime)
        {
            //レンダーでキャラクターを無敵時間終わるまで表示、非表示を繰り返す.
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }
            yield return new WaitForSeconds(flashDuration);
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }
            yield return new WaitForSeconds(flashDuration);
        }

        isInvincible = false;
    }
}
