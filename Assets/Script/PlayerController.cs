using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("공격 설정")]
    public float attackDuration = 0.8f;             //공격 지속 시간
    public bool canMoveWithAttacking = false;       //공격 중 이동 가능 여부 판단 bool

    [Header("컴포넌트")]                
    public Animator animator;                       //컴포넌트 하위에 animator가 존재하기 때문

    private CharacterController contoller;
    private Camera playerCamera;

    //현재 상태 값들
    private float currentSpeed;
    private bool isAttacking = false;


    // Start is called before the first frame update
    void Start()
    {
        contoller= GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimator();
        HandleMovement();
    }

    void HandleMovement()       //이동함수 제작
    {
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

            contoller.Move(moveDirection * currentSpeed * Time.deltaTime);      //캐릭터 컨트롤러의 이동입력

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
    }
}
