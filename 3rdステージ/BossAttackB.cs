using UnityEngine;
using System.Collections;

//左手を振る攻撃
public class BossAttackB : MonoBehaviour
{
    private Animator animator;
    private BossMovement bossMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        bossMovement = GetComponent<BossMovement>();
    }

    public void Attack()
    {
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        //左手を振るアニメーションのトリガー設定
        animator.SetTrigger("AttackB");
        //アニメーション終了まで待機
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
