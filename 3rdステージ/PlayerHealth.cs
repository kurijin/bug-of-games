using UnityEngine;
using System.Collections;

//playerの体力管理のスクリプト
public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public float invincibilityDuration = 2.0f;
    public float flashDuration = 0.1f;
    private int currentHP;
    private bool isInvincible = false;
    private Renderer[] renderers;

    public Player_onBoss player;


    private void Start()
    {
        currentHP = maxHP;
        //無敵時間点滅のためのもの
        renderers = GetComponentsInChildren<Renderer>();
        //HPのハートUI
        HeartUIManager.instance.UpdateHealthUI(currentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHP -= damage;
            HeartUIManager.instance.UpdateHealthUI(currentHP, maxHP);

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

    private void Die()
    {
        if (player != null)
        {
        player.Die();
        }
    }

    //ボスの点滅処理と同様renderで表示非表示繰り返し、無敵時間に達するまで繰り返す
    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float invincibilityEndTime = Time.time + invincibilityDuration;

        while (Time.time < invincibilityEndTime)
        {
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
