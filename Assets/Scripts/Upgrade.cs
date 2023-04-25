using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Upgrade
{
    public string id;
    public string prereqId;
    public double followerCost;
    public double haterCost;
    public double viewRequirement;
    public string buttonText;

    [TextArea]
    public string header;
    [TextArea]
    public string description;
    [TextArea] 
    public string optionalCostText;

    public double clickMultiplier;
    public float attentionFloor;
    public float attentionLossMultiplier;
    public float attentionLossDelay;
    public float maxAttention;
}