using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudPanel : UIBase
{
    public override bool IsDefaultPanel => true;

    [SerializeField] private TextMeshProUGUI _dayTxt;
    [SerializeField] private TextMeshProUGUI _visitorTxt;
    [SerializeField] private TextMeshProUGUI _assetTxt;
    [SerializeField] private TextMeshProUGUI _reputationTxt;
    [SerializeField] private TextMeshProUGUI _crisisTxt;

    private int _day;
    private int _visitor;
    private int _visitorsPerDay;
    private int _asset;
    private int _reputation;
    private int _crisis;

    private const string DayString = "DAY {0}";
    private const string VisitorString = "{0}/{1}명";
    private const string AssetString = "자산 {0}";
    private const string ReputationString = "평판 {0}";
    private const string CrisisString = "위기 {0}";
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _day = 1;
        _visitor = 0;
        
        var commonData = App.Get<TitleData>().Common;
        _visitorsPerDay = commonData["visitorsPerDay"];
        _asset = commonData["guildFunds"];
        _reputation = commonData["reputation"];
        _crisis = commonData["kingdomCrisis"];
        
        SetHudInfos();
    }

    private void SetHudInfos()
    {
        _dayTxt.text = string.Format(DayString, _day);
        _visitorTxt.text = string.Format(VisitorString, _visitor, _visitorsPerDay);
        _assetTxt.text = string.Format(AssetString, _asset);
        _reputationTxt.text = string.Format(ReputationString, _reputation);
        _crisisTxt.text = string.Format(CrisisString, _crisis);
    }
}
