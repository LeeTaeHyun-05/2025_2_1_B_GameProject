using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("��ȣ �ۿ� ����")]
    public float interactionRange = 2.0f;           //��ȣ �ۿ� ����
    public LayerMask interactionLayerMask = 1;      //��ȣ�ۿ��� ���̾�
    public KeyCode interactionKey = KeyCode.E;      //��ȣ �ۿ� Ű (E Ű)

    [Header("UI ����")]
    public Text interactionText;           //��ȣ�ۿ� UI �ؽ�Ʈ
    public GameObject interactionUI;

    private Transform playerTransform;         
    private InteractableObject currentInteractable;       //������ ������Ʈ ��� Ŭ����

    private void Start()
    {
        playerTransform = transform;    
        HideInteractionUI();
    }
    private void Update()
    {
        CheckForInteractables();
        HandleInteractionInput();
    }
    void HandleInteractionInput()                                                   //���ͷ��� �Է� �Լ�
    {
        if (currentInteractable != null && Input.GetKeyDown(interactionKey))        //���ͷ��� Ű ���� ������ ��
        {
            currentInteractable.Interact();                                         //�ൿ�� ��
        }
    }

    void ShowInteractionUI(string text)                                             //���ͷ��� UIâ�� ����.
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.text = text;
        }
    }

    void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    void CheckForInteractables()
    {
        Vector3 checkPosition = playerTransform.position + playerTransform.forward * (interactionRange * 0.5f);

        Collider[]hitColliders = Physics.OverlapSphere(checkPosition, interactionRange, interactionLayerMask);          //��ü�� �浹�� ��� �ݶ��̴� �迭

        InteractableObject closestInteractable = null;         //���� ����� ��ü ����
        float closestDistance = float.MaxValue;         //�Ÿ� ����'

        foreach (Collider collider in hitColliders)
        {
            InteractableObject interactable = collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(playerTransform.position, collider.transform.position);
                //�÷��̾ �ٶ󺸴� ���⿡ �ִ��� Ȯ�� (���� üũ)
                Vector3 directionToObject = (collider.transform.position - playerTransform.position).normalized;
                float angle = Vector3.Angle(playerTransform.forward, directionToObject);

                if (angle < 90f && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        if (closestInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.onPlayerExit();                         //���� ������Ʈ���� ����
            }

            currentInteractable = closestInteractable;

            if (currentInteractable != null)
            {
                currentInteractable.onPlayerExit();                         //�� ������Ʈ ����
                ShowInteractionUI(currentInteractable.GetINteractuonText());
            }
            else
            {
                HideInteractionUI();
            }
        }
    }
}
