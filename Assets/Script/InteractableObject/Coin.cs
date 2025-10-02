using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : InteractableObject
{

    [Header("µ¿Àü ¼³Á¤")]
    public int coinValue = 10;
    public string questTag = "Coin";

    protected override void Start()
    {
        base.Start();
        objectName = "µ¿Àü";
        interactionText = "[E] µ¿Àü È¹µæ";
        interactionType = InteractionType.Item;
    }

    protected override void CollectItem()
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.AddCollectProgress(questTag);
        }
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
