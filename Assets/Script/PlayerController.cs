using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("점프 설정")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;              //중력 속도 추가
    public float landingduration = 0.3f;

    [Header("공격 설정")]
    public float attackDuration = 0.8f;             //공격 지속 시간
    public bool canMoveWithAttacking = false;       //공격 중 이동 가능 여부 판단 bool

    [Header("컴포넌트")]                
    public Animator animator;                       //컴포넌트 하위에 animator가 존재하기 때문

    private CharacterController controller;
    private Camera playerCamera;

    //현재 상태 값들
    private float currentSpeed;
    private bool isAttacking = false;
    private bool isLanding = false;         //착지 중인지 확인
    private float landingTimer;             //착지 타이머

    private Vector3 velocity;
    private bool isGrounded;            //땅에 있는지 판별
    private bool wasGrounded;           //직전 프레임에 땅에 있었는지 판단
    private float attackTimer;

    private bool isUIMode = false;

    // Start is called before the first frame update
    void Start()
    {
        controller= GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCursorlock();
        }

        if (!isUIMode)
        {
            CheckGrounded();
            HandleLanding();
            HandleMovement();
            UpdateAnimator();
            HandleAttack();
            HandleJump();
        }
        
    }

    void HandleMovement()       //이동함수 제작
    {

        //공격 중이거나 착지 중일 때 움직임 제한
        if ((isAttacking && !canMoveWithAttacking) || isLanding)
        {
            currentSpeed = 0;
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)           //둘중에 하나라도 압력이 있을 때
        {
            //카메라가 보는 방향의 앞쪽으로 설정
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;        //이동 방향 설정

            if (Input.GetKey(KeyCode.LeftShift))                 //왼쪽 Shioft를 눌러서 Run모드로 변경
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);      //캐릭터 컨트롤러의 이동입력

            //이동 진행을 바라보며 이동
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation , targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0;
        }
    }

    void UpdateAnimator()
    {
        //전체 최대속도 (runSpeed) 기준으로 0 ~ 1 계산
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        animator.SetFloat("speed", animatorSpeed);
        animator.SetBool("isGrounded", isGrounded);

        bool isFaliing = !isGrounded && velocity.y < -0.1f;
        animator.SetBool("isFalling", isFaliing); 
        animator.SetBool("isLanding", isLanding);
        
    }

    void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;                     //캐릭터 컨트롤러에서 상태 값을 받아옴
        

        if(!isGrounded && wasGrounded)                          //지금 프레임은 땅이 아니고, 이전 프레임은 땅
        {
            Debug.Log("떨어지기 시작");
        }

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;

            if(!wasGrounded && animator != null)                //착지를 진행
            {
                isLanding = true;
                landingTimer = landingduration;
            }
        }
    }

    void HandleJump()
    {

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("jumpTrigger");
            }
        }

        if (!isGrounded)                                        //땅위에 있지 않을경우 중력 작용
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLanding()
    {
        if (isLanding)
        {
            landingTimer -= Time.deltaTime;

            if (landingTimer < 0)
            {
                isLanding = false;              //착지 완료 처리
            }
        }
    }

    void HandleAttack()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
            isAttacking = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)  && !isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;

            if (animator != null)
            {
                animator.SetTrigger("attackTrigger");
            }
        }
        
    }

    public void SetCursorLock(bool loskCursor)
    {
        if (loskCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isUIMode = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isUIMode = true;
        }
    }

    public void ToggleCursorlock()
    {
        bool ShouldLock = Cursor.lockState != CursorLockMode.Locked;
        SetCursorLock(ShouldLock);
    }

    public void SetUIMode(bool uiMode)
    {
        SetCursorLock(!uiMode);
    }
}
