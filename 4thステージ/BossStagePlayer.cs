using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


//boss stageでのplayerの動き,RetryHp要素の削除したもの
public class BossStagePlayer : MonoBehaviour
{
    public float attackDuration = 0.5f;
    private bool isAttacking = false;
    public GameObject sword;
    private Collider swordCollider;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;
    private CharacterController _characterController;
    private Transform _transform;
    private Vector3 _moveVelocity;
    private InputAction _move;
    private InputAction _run;
    private InputAction _attack;
    private Animator animator;

    private float verticalVelocity = 0f; 

    [SerializeField] private AudioClip swordSwingSound; 
    private AudioSource audioSource; 

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _transform = transform;
        animator = GetComponent<Animator>();
        var input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();
        _move = input.currentActionMap.FindAction("Move");
        _run = input.currentActionMap.FindAction("Run");
        _attack = input.currentActionMap.FindAction("Attack");

        if (sword != null)
        {
            swordCollider = sword.GetComponent<Collider>();
            swordCollider.enabled = false;
        }

        audioSource = gameObject.AddComponent<AudioSource>(); 
    }

    private void Update()
    {
        CustomUpdate();
    }

    private void CustomUpdate()
    {
        if (!isAttacking)
        {
            var moveValue = _move.ReadValue<Vector2>();
            bool isRunning = _run.ReadValue<float>() > 0.5f;
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            _moveVelocity.x = moveValue.x * currentSpeed;
            _moveVelocity.z = moveValue.y * currentSpeed;

            _transform.LookAt(_transform.position + new Vector3(_moveVelocity.x, 0, _moveVelocity.z));

            if (_attack.triggered)
            {
                Attack();
            }


            if (_characterController.isGrounded)
            {
                verticalVelocity = 0f;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }


            _moveVelocity.y = verticalVelocity;

            _characterController.Move(_moveVelocity * Time.deltaTime);

            float speed = _characterController.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
        else
        {
            _characterController.Move(Vector3.zero);
            animator.SetFloat("Speed", 0);
        }
    }

    public void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetTrigger("Attack");
        PlaySwordSwingSound(); 
        StartCoroutine(EnableCollider());
        StartCoroutine(CompleteAttack());
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForEndOfFrame();
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
    }

    private IEnumerator CompleteAttack()
    {
        yield return new WaitForSeconds(attackDuration);
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
        isAttacking = false;
    }

    private void PlaySwordSwingSound()
    {
        if (audioSource != null && swordSwingSound != null)
        {
            audioSource.PlayOneShot(swordSwingSound);
        }
    }
}
