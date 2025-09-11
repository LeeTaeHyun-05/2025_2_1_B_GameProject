using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("���� ����")]
    public float attackDuration = 0.8f;             //���� ���� �ð�
    public bool canMoveWithAttacking = false;       //���� �� �̵� ���� ���� �Ǵ� bool

    [Header("������Ʈ")]                
    public Animator animator;                       //������Ʈ ������ animator�� �����ϱ� ����

    private CharacterController contoller;
    private Camera playerCamera;

    //���� ���� ����
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

    void HandleMovement()       //�̵��Լ� ����
    {
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

            contoller.Move(moveDirection * currentSpeed * Time.deltaTime);      //ĳ���� ��Ʈ�ѷ��� �̵��Է�

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
    }
}
