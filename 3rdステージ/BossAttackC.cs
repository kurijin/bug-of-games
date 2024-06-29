using UnityEngine;
using System.Collections;

//右手を振って,左手を振る.アニメーターは攻撃両方のアニメーションをそのまま繋げる
public class BossAttackC : MonoBehaviour
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
        animator.SetTrigger("AttackC");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
