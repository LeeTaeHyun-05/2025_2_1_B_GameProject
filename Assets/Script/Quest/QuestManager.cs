using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("UI��ҵ�")]
    public GameObject questUI;              //����Ʈ �г� UI
    public Text questTitleText;             //����Ʈ Ÿ��Ʋ �ؽ�Ʈ
    public Text questDescriptionText;       //����Ʈ ����
    public Text questProgressText;          //���� ����
    public Button completeButton;           //�Ϸ� ��ư


    [Header("����Ʈ ���")]
    public QuestData[] availableQuests;     //������ �ִ� ����Ʈ ���

    private QuestData currentQuest;         //���� ���� ����Ʈ ������
    private int currentQuestIndex = 0;      //����Ʈ ����߿� ���� ���� ��ȣ

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (availableQuests.Length > 0)
        {
            StartQuest(availableQuests[0]);
        }
        if (completeButton != null)
        {
            completeButton.onClick.AddListener(CompleteCurrentQuest);
        }
    }

    void Update()
    {
        if (currentQuest != null && currentQuest.isActive)
        {
            CheckQuestProgress();
            UpdateQuestUI();
        }
    }

    void UpdateQuestUI()
    {
        if (currentQuest == null) return;

        if (questTitleText != null)
        {
            questTitleText.text = currentQuest.questTitle;
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.text = currentQuest.description;
        }

        if (questProgressText != null)
        {
            questProgressText.text = currentQuest.GetProgressText();
        }
    }

    public void StartQuest(QuestData quest)
    {
        if (quest == null) return;

        currentQuest = quest;               //����Ʈ�� �޾ƿͼ� currentQuest�� �����Ѵ�.
        currentQuest.Initalize();           //���� ����Ʈ�� �ʱ�ȭ�Ѵ�.
        currentQuest.isActive = true;

        Debug.Log("����Ʈ ���� : " + questTitleText);
        UpdateQuestUI();
        if (questUI != null)
        {
            questUI.SetActive(true);
        }
    }

    void CheckDeliveryProgress()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        float distance = Vector3.Distance(player.position, currentQuest.deliveryPosition);

        if (distance <= currentQuest.deliveryRadius)
        {
            if (currentQuest.currentProgresss == 0)
            {
                currentQuest.currentProgresss = 1;
            }

        }
        else
        {
            currentQuest.currentProgresss = 0;
        }
    }

    //���� ����Ʈ ����
    public void AddCollectProgress(string itemTag)
    {
        if (currentQuest == null || !currentQuest.isActive) return;

        if (currentQuest.questType == QuestType.Collect && currentQuest.targetTag == itemTag)
        {
            {
                currentQuest.currentProgresss++;
                Debug.Log("������ ���� : " + itemTag);
            }
        }  
    }


    public void AddInteractProgress(string objectTag)
    {
        if (currentQuest == null || !currentQuest.isActive) return;

        if (currentQuest.questType == QuestType.Interact && currentQuest.targetTag == objectTag)
        {
        
             currentQuest.currentProgresss++;
             Debug.Log("��ȣ�ۿ� �Ϸ� : " + objectTag);
            
        }
    }

    public void CompleteCurrentQuest()
    {
        if (currentQuest == null || !currentQuest.isCompleted) return;

        Debug.Log("����Ʈ �Ϸ� ! " + currentQuest.rewardMessage);

        //�Ϸ� ��ư ��Ȱ��ȭ
        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(false);
        }

        //���� ����Ʈ�� ������ ����
        currentQuestIndex++;
        if (currentQuestIndex < availableQuests.Length)
        {
            StartQuest(availableQuests[currentQuestIndex]);
        }
        else
        {
            currentQuest = null;
            if (questUI != null)
            {
                questUI.gameObject.SetActive(false);
            }
        }
    }

    void CheckQuestProgress()
    {
        if (currentQuest.questType == QuestType.Delivery)
        {
            CheckDeliveryProgress();
        }

        if (currentQuest.IsCompleted() && !currentQuest.isCompleted)
        {
            currentQuest.isCompleted = true;

            if (completeButton != null)
            {
                completeButton.gameObject.SetActive(true);
            }
        }    
    }

}
