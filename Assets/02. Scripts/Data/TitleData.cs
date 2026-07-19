using System;
using System.Collections.Generic;

[Serializable]
public class CommonData
{
    public string key;
    public int value;
}

[Serializable]
public class JobData
{
    public string id;
    public string displayName;
    public int combatMod;
    public int agilityMod;
    public int survivalMod;
    public int knowledgeMod;
    public string specialtyRiskTags;
}

[Serializable]
public class ItemData
{
    public string id;
    public string displayName;
    public int quantity;
    public int price;
    public int cost;
    public int maxPerVisitor;
    public bool consumable;
    public string slot;
    public string riskTags;
}

[Serializable]
public class VisitorData
{
    public string id;
    public string displayName;
    public string portraitId;
    public int level;
    public string jobId;
    public int combat;
    public int agility;
    public int survival;
    public int knowledge;
    public int budget;
    public string injuryHistory;
    public string submittedEquipment;
}

[Serializable]
public class QuestData
{
    public string id;
    public string displayName;
    public int riskCombat;
    public int riskTrap;
    public int riskEnvironment;
    public int riskUnknown;
    public int baseReward;
    public int guildFeeRate;
    public int crisisIgnored;
    public int crisisFailed;
    public int crisisSuccess;
    public string riskTags;
}

[Serializable]
public class InsuranceData
{
    public string id;
    public string displayName;
    public int premium;
    public int coverageCap;
    public int deductible;
    public string coveredResults;
    public string coveredEvents;
    public string claimType;
    public int fraudDifficulty;
}

public class TitleData : AppService
{
    public Dictionary<string, int> Common { get; private set; } = new();
    public Dictionary<string, JobData> Job { get; private set; } = new();
    public Dictionary<string, ItemData> Item { get; private set; } = new();
   
    public Dictionary<string, VisitorData> Visitor { get; private set; } = new();
    public Dictionary<string, QuestData> Quest { get; private set; } = new();
    public Dictionary<string, InsuranceData> Insurance { get; private set; } = new();

    #region Data Path
    private const string COMMON_PATH = "Data/CommonData";
    private const string JOB_PATH = "Data/JobData";
    private const string ITEM_PATH = "Data/ItemData";
    private const string VISITOR_PATH = "Data/VisitorData";
    private const string QUEST_PATH = "Data/QuestData";
    private const string INSURANCE_PATH = "Data/InsuranceData";
    #endregion

    protected override void Awake()
    {
        base.Awake();

        LoadData();
    }

    private void LoadData()
    {
        var commonDataRaw = DataLoader.LoadData<CommonData>(COMMON_PATH);
        foreach (var data in commonDataRaw)
        {
            Common.Add(data.key, data.value);
        }
        
        var jobDataRaw = DataLoader.LoadData<JobData>(JOB_PATH);
        foreach (var data in jobDataRaw)
        {
            Job.Add(data.id, data);
        }
        
        var itemDataRaw = DataLoader.LoadData<ItemData>(ITEM_PATH);
        foreach (var data in itemDataRaw)
        {
            Item.Add(data.id, data);
        }
        
        var visitorDataRaw = DataLoader.LoadData<VisitorData>(VISITOR_PATH);
        foreach (var data in visitorDataRaw)
        {
            Visitor.Add(data.id, data);
        }
        
        var questDataRaw = DataLoader.LoadData<QuestData>(QUEST_PATH);
        foreach (var data in questDataRaw)
        {
            Quest.Add(data.id, data);
        }
        
        var insuranceDataRaw = DataLoader.LoadData<InsuranceData>(INSURANCE_PATH);
        foreach (var data in insuranceDataRaw)
        {
            Insurance.Add(data.id, data);
        }
    }
}
