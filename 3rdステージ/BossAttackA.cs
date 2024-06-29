using UnityEngine;
using System.Collections;

//右手を振る攻撃
public class BossAttackA : MonoBehaviour
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
        //アニメーションのトリガーを設定する
        animator.SetTrigger("AttackA");
        //アニメーション終わるまで待機
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
