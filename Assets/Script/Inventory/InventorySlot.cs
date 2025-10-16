using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySlot : MonoBehaviour
{
    public ItemData item;
    public int amount;

    [Header("UI Refernece")]
    public Image itemIcon;
    public Text amountText;
    public GameObject emptySlotImage;

    void Start()
    {
        UpdateSlotUI();
    }

    public void SetItem(ItemData newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
        UpdateSlotUI();
    }

    void UpdateSlotUI()
    {
        if (item != null)               //아이템이 있으면
        {
            itemIcon.sprite = item.itemIcon;        //아이콘 표시
            itemIcon.enabled = true;

            amountText.text = amount > 1 ? amount.ToString() : "";      //개수가 1개보다 많으면 숫자 표시
            if (emptySlotImage != null)
            {
                emptySlotImage.SetActive(false);                                //번 슬롯 이미지 숨기기
            }
        }
        else
        {
            itemIcon.enabled = false;           //아이콘 숨기기
            amountText.text = "";               //텍스트 비우기

            if (emptySlotImage != null)
            {
                emptySlotImage.SetActive(true);         //번 슬롯 이미지 표시
            }
        }   
    }

    public void AddAmount(int Value)            //아이템 개수 추가하는 함수
    {
        amount += Value;
        UpdateSlotUI();
    }

    public void RemoveAmount(int value)         //아이템 개수 제거하는 함수
    {
        amount -= value;
        if (amount <= 0)                        //개수가 0이하면 슬롯 비우기
        {
            ClearSlot();
        }
        else
        {
            UpdateSlotUI();
        }
    }

    public void ClearSlot()                     //슬롯을 비우는 함수
    {
        item = null;
        amount = 0;
        UpdateSlotUI();
    }
}
