using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    /* ==== References ==== */
    [SerializeField] Resources resources;
    [SerializeField] UpgradeMenu upgradeMenu;
    [SerializeField] List<Upgrade> upgradeList;
    
    /* ==== Game Objects ==== */
    
    /* ==== Local Variables ==== */
    List<string> purchasedUpgrades = new();
    List<Upgrade> upgrades;

    [HideInInspector] public double maxAttention = 2.0d;
    [HideInInspector] public double clickMultiplier = 1.0d;
    [HideInInspector] public float attLossMultiplier = 1f;
    [HideInInspector] public float attFloor = 0;
    [HideInInspector] public float attLossDelay = 5f; // Idle time in seconds before attention starts to drop off
    [HideInInspector] public int maxOfflineTime = 5;
    [HideInInspector] public TimeSpan maxOfflineUpgrade = TimeSpan.FromMinutes(5);

    /* ==== Default Stats ==== */
    public double d_maxAttention = 2.0d;
    public double d_clickMultiplier = 1.0d;
    public float d_attLossMultiplier = 1f;
    public float d_attFloor = 0;
    public float d_attLossDelay = 5f;

    // Start is called before the first frame update
    void Start()
    {
        upgrades = new List<Upgrade>(upgradeList);
        InitializeStats();
        StartCoroutine(UpdateUpgradeMenu());
    }

    IEnumerator UpdateUpgradeMenu()
    {
        while (true)
        {
            for(int i = 0; i < upgrades.Count; ++i)
            {
                if (resources.views >= upgrades[i].viewRequirement &&
                    resources.followers >= upgrades[i].followerCost/2 &&
                    resources.haters >= upgrades[i].haterCost/2)
                {
                    upgradeMenu.SpawnUpgrade(upgrades[i]);
                    upgrades.RemoveAt(i);
                    --i;
                }

                yield return null;
            }

            yield return null;
        }
    }

    public void InitializeStats()
    {
        maxAttention = d_maxAttention;
        clickMultiplier = d_clickMultiplier;
        attLossMultiplier = d_attLossMultiplier;
        attFloor = d_attFloor;
        attLossDelay = d_attLossDelay;
    }

    public void PurchaseUpgrade(Upgrade upgrade)
    {
        LoadUpgrade(upgrade);

        resources.followers -= upgrade.followerCost;
        resources.haters -= upgrade.haterCost;
    }

    public List<string> GetPurchasedUpgrades()
    {
        return purchasedUpgrades;
    }

    private void LoadUpgrade(Upgrade upgrade)
    {
        maxAttention += upgrade.maxAttention;
        clickMultiplier += upgrade.clickMultiplier;
        attLossMultiplier -= upgrade.attentionLossMultiplier;
        attLossDelay -= upgrade.attentionLossDelay;
        attFloor += upgrade.attentionFloor;
        maxOfflineTime += upgrade.maxOfflineTime;

        if (upgrade.maxOfflineTime > 0)
            maxOfflineUpgrade = TimeSpan.FromMinutes(maxOfflineTime);

        purchasedUpgrades.Add(upgrade.id);
    }

    public void LoadPurchasedUpgrades(List<string> pUpgrades)
    {
        upgradeMenu.ResetMenu();

        foreach(string id in pUpgrades)
        {
            for(int i = 0; i < upgrades.Count; ++i)
            {
                if(id == upgrades[i].id)
                {
                    LoadUpgrade(upgrades[i]);
                    upgrades.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void RemoveUpgrades()
    {
        purchasedUpgrades = new();
        upgrades = new List<Upgrade>(upgradeList);
        upgradeMenu.ResetMenu();
        InitializeStats();
    }
}
