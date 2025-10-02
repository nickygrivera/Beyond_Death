using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ITargeteable
{
    [SerializeField] private bool simpleMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideSlowdownTime;
    [SerializeField] private float slideRampUp;
    [SerializeField] private float slideRampDown;
    [SerializeField] private float velocityFactorTimeScale;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float stickToGroundVelocity = -1f;
    
    [SerializeField] private AnimationCurve slideSlowdownCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private float _verticalVelocity;
    private Vector3 _slideVelocity;
    private float _slideTime = 0;
    private float _slideVelocityFactor = 1;
    private bool _isJumping = false;
    private bool _isSliding = false;
    private bool _isSwitchingAnimation;
    
    private CharacterController _characterController;
    private Animator _animator;
    private Camera _camera;
    private Vector3 _playerVelocity;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int ZSpeed = Animator.StringToHash("zSpeed");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Movement = Animator.StringToHash("Movement");
    private static readonly int Slide = Animator.StringToHash("Sliding");

    private void Start()
    {
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InputManager.Instance.JumpPerformed += JumpOnPerformed;
    }

    private void Update()
    {
        UpdatePlayerMovement();
        UpdateVerticalVelocity();
        UpdateSlideVelocity();
        ApplyTotalVelocity();

        UpdateRotation();
    }

    private void UpdateSlideVelocity()
    {
        var maxSlideVelocity = Vector3.zero;
        RaycastHit hitInfo;
        if (_characterController.isGrounded &&
            Physics.SphereCast(
                transform.position + _characterController.center,
                _characterController.radius,
                Vector3.down, out hitInfo)
           )
        {
            var angle = Vector3.Angle(Vector3.up, hitInfo.normal);
            if (angle > _characterController.slopeLimit)
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Movement && !_isSwitchingAnimation)
                {
                    _isSwitchingAnimation = true;
                    _animator.CrossFadeInFixedTime(Slide, 0.1f);
                }
                else if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Slide && _isSwitchingAnimation)
                {
                    _isSwitchingAnimation = false;
                }
                
                _isSliding = true;
                
                var slideDirection = Vector3.ProjectOnPlane(Vector3.down, hitInfo.normal).normalized;
                maxSlideVelocity = slideDirection * slideSpeed;
                
                Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.blue, 3);
                Debug.DrawRay(hitInfo.point, slideDirection, Color.red, 3);
            }
            else
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Slide && !_isSwitchingAnimation)
                {
                    _isSwitchingAnimation = true;
                    _animator.CrossFadeInFixedTime(Movement, 0.2f);
                }
                else if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Movement && _isSwitchingAnimation)
                {
                    _isSwitchingAnimation = false;
                }
                _isSliding = false;
                _slideTime = 0;
            }
            
            
            
            if(_isSliding) _slideTime += Time.deltaTime;
            
            _slideVelocity = _isSliding ? 
                Vector3.Lerp(_slideVelocity, maxSlideVelocity, Time.deltaTime * slideRampUp) :
                Vector3.Lerp(_slideVelocity, Vector3.zero, Time.deltaTime * slideRampDown);

            _slideVelocityFactor = _isSliding ? 
                slideSlowdownCurve.Evaluate(Mathf.Clamp01(_slideTime / slideSlowdownTime)) :
                Mathf.Lerp(_slideVelocityFactor, 1, Time.deltaTime * velocityFactorTimeScale);

            _playerVelocity += _slideVelocity;
        }
    }

    private void UpdateRotation()
    {
        var rotationVector = InputManager.Instance.GetLook();
        transform.Rotate(new Vector3(0,rotationVector.x, 0) * (rotationSpeed * Time.deltaTime), Space.Self);
    }

    private void ApplyTotalVelocity()
    {
        if (!simpleMove)
        {
            _characterController.Move(_playerVelocity * (_slideVelocityFactor * moveSpeed * Time.deltaTime));
        }
        else
        {
            _characterController.SimpleMove(_playerVelocity * (_slideVelocityFactor * moveSpeed));
        }
    }

    private void UpdateVerticalVelocity()
    {
        if (_characterController.isGrounded && !_isJumping)
        {
            _verticalVelocity = stickToGroundVelocity;
            if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Jump && !_isSwitchingAnimation)
            {
                _isSwitchingAnimation = true;
                _animator.CrossFadeInFixedTime(Movement, 0.2f);
            }
            else if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Movement && _isSwitchingAnimation)
            {
                _isSwitchingAnimation = false;
            }
        }
        else if (!_characterController.isGrounded)
        {
            _isJumping = false;
            _verticalVelocity -= gravity * Time.deltaTime;
        }
        _playerVelocity += Vector3.up * _verticalVelocity;
    }

    private void UpdatePlayerMovement()
    {
        var inputVector = InputManager.Instance.GetMovement();
        _playerVelocity = _camera.transform.right * inputVector.x + 
                             _camera.transform.forward * inputVector.y;
        _playerVelocity.y = 0;
        
        _animator.SetFloat(XSpeed, inputVector.x);
        _animator.SetFloat(ZSpeed, inputVector.y);
    }
    
    private void JumpOnPerformed()
    {
        if (_characterController.isGrounded && !_isSliding)
        {
            _verticalVelocity = jumpForce;
            _isJumping = true;
            _animator.CrossFadeInFixedTime(Jump, 0.1f);
        }
    }

    public Transform GetTransform(out bool playerOnSight)
    {
        playerOnSight = true;   
        return transform;
    }
}
