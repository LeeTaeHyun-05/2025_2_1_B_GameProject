using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievement/Achievement Data")]
public class AchievementData : ScriptableObject
{
    public string achievementName;
    public string description;
    public AchievementType achievementType;
    public int requiredAmount;
    public int rewardCoins;
    public bool isUnlocked;
    public Sprite icon;
}
