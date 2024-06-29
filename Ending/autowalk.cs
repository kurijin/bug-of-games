using UnityEngine;

//playerを自動で歩かせるもの
public class autowalk : MonoBehaviour
{
    public float MoveSpeed = 2.0f; 
    private CharacterController characterController;
    private Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 move = transform.forward * MoveSpeed * Time.deltaTime;
        characterController.Move(move);

        animator.SetFloat("MoveSpeed", MoveSpeed);

        // Groundedを常にtrue
        animator.SetBool("Grounded", true);
    }
}
