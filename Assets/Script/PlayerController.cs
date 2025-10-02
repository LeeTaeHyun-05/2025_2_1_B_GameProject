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
    [Header("�̵� ����")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("���� ����")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;              //�߷� �ӵ� �߰�
    public float landingduration = 0.3f;

    [Header("���� ����")]
    public float attackDuration = 0.8f;             //���� ���� �ð�
    public bool canMoveWithAttacking = false;       //���� �� �̵� ���� ���� �Ǵ� bool

    [Header("������Ʈ")]                
    public Animator animator;                       //������Ʈ ������ animator�� �����ϱ� ����

    private CharacterController controller;
    private Camera playerCamera;

    //���� ���� ����
    private float currentSpeed;
    private bool isAttacking = false;
    private bool isLanding = false;         //���� ������ Ȯ��
    private float landingTimer;             //���� Ÿ�̸�

    private Vector3 velocity;
    private bool isGrounded;            //���� �ִ��� �Ǻ�
    private bool wasGrounded;           //���� �����ӿ� ���� �־����� �Ǵ�
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

    void HandleMovement()       //�̵��Լ� ����
    {

        //���� ���̰ų� ���� ���� �� ������ ����
        if ((isAttacking && !canMoveWithAttacking) || isLanding)
        {
            currentSpeed = 0;
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)           //���߿� �ϳ��� �з��� ���� ��
        {
            //ī�޶� ���� ������ �������� ����
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;        //�̵� ���� ����

            if (Input.GetKey(KeyCode.LeftShift))                 //���� Shioft�� ������ Run���� ����
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);      //ĳ���� ��Ʈ�ѷ��� �̵��Է�

            //�̵� ������ �ٶ󺸸� �̵�
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
        //��ü �ִ�ӵ� (runSpeed) �������� 0 ~ 1 ���
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
        isGrounded = controller.isGrounded;                     //ĳ���� ��Ʈ�ѷ����� ���� ���� �޾ƿ�
        

        if(!isGrounded && wasGrounded)                          //���� �������� ���� �ƴϰ�, ���� �������� ��
        {
            Debug.Log("�������� ����");
        }

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;

            if(!wasGrounded && animator != null)                //������ ����
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

        if (!isGrounded)                                        //������ ���� ������� �߷� �ۿ�
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
                isLanding = false;              //���� �Ϸ� ó��
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
