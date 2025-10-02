using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("UI요소들")]
    public GameObject questUI;              //퀘스트 패널 UI
    public Text questTitleText;             //퀘스트 타이틀 텍스트
    public Text questDescriptionText;       //퀘스트 내용
    public Text questProgressText;          //진행 상태
    public Button completeButton;           //완료 버튼


    [Header("퀘스트 목록")]
    public QuestData[] availableQuests;     //가지고 있는 퀘스트 목록

    private QuestData currentQuest;         //진행 주인 퀘스트 데이터
    private int currentQuestIndex = 0;      //퀘스트 목록중에 진행 중인 번호

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

        currentQuest = quest;               //퀘스트를 받아와서 currentQuest에 셋팅한다.
        currentQuest.Initalize();           //지금 퀘스트를 초기화한다.
        currentQuest.isActive = true;

        Debug.Log("퀘스트 시작 : " + questTitleText);
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

    //수집 퀘스트 진행
    public void AddCollectProgress(string itemTag)
    {
        if (currentQuest == null || !currentQuest.isActive) return;

        if (currentQuest.questType == QuestType.Collect && currentQuest.targetTag == itemTag)
        {
            {
                currentQuest.currentProgresss++;
                Debug.Log("아이템 수집 : " + itemTag);
            }
        }  
    }


    public void AddInteractProgress(string objectTag)
    {
        if (currentQuest == null || !currentQuest.isActive) return;

        if (currentQuest.questType == QuestType.Interact && currentQuest.targetTag == objectTag)
        {
        
             currentQuest.currentProgresss++;
             Debug.Log("상호작용 완료 : " + objectTag);
            
        }
    }

    public void CompleteCurrentQuest()
    {
        if (currentQuest == null || !currentQuest.isCompleted) return;

        Debug.Log("퀘스트 완료 ! " + currentQuest.rewardMessage);

        //완료 버튼 비활성화
        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(false);
        }

        //다음 퀘스트가 있으면 시작
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
