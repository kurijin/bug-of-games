using UnityEngine;
using System.Collections;

//走った後に,左手、右手とふる攻撃.
public class BossAttackD : MonoBehaviour
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
        //攻撃の前に走る
        bossMovement.isRunning = true;
        animator.SetTrigger("Run");
        yield return new WaitForSeconds(1.0f); 
        //左手右手とふる。アニメーターを繋げる.
        animator.SetTrigger("AttackD");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        bossMovement.isRunning = false;
    }
}
