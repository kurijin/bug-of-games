using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//2ndステージの動かし方と同じ
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class SecondStagePlayer : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private Transform cameraTransform;

    private CharacterController _characterController;
    private Vector3 _moveVelocity;
    private InputAction _move;
    private InputAction _look;

    private float _cameraPitch = 0.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float maxLookAngle = 90.0f;
    private Animator animator;

    [SerializeField] private AudioClip footstepSound; 
    private AudioSource audioSource; 

    private bool isWalking = false;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        var input = GetComponent<PlayerInput>();
        _move = input.actions["Move"];
        _look = input.actions["Look"];

        audioSource = gameObject.AddComponent<AudioSource>(); 
        audioSource.clip = footstepSound;
        audioSource.loop = true; 
        audioSource.playOnAwake = false;
    }

    private void CustomUpdate()
    {
        if (!_characterController.enabled) return;

        var moveValue = _move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveValue.x, 0, moveValue.y).normalized;

        if (_characterController.isGrounded)
        {
            _moveVelocity = transform.TransformDirection(moveDirection) * walkSpeed;
        }
        
        animator.SetFloat("MoveSpeed", _moveVelocity.magnitude / walkSpeed);

        var lookValue = _look.ReadValue<Vector2>();
        _cameraPitch -= lookValue.y * lookSpeed;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * lookValue.x * lookSpeed);

        if (!_characterController.isGrounded)
        {
            _moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        _characterController.Move(_moveVelocity * Time.deltaTime);

        if (_moveVelocity.magnitude > 0.1f && !isWalking)
        {
            PlayFootstepSound();
        }
        else if (_moveVelocity.magnitude <= 0.1f && isWalking)
        {
            StopFootstepSound();
        }
    }
    
    private void Update()
    {
        CustomUpdate();
    }

    private void PlayFootstepSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            isWalking = true;
        }
    }

    public void StopFootstepSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            isWalking = false;
        }
    }
}
