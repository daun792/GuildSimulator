using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GuildReceptionFlow : MonoBehaviour
{
    [Header("Visitor portraits (uses Assets/03. Images/Customer directly)")]
    [SerializeField] private Sprite[] visitorPortraits;

    private sealed class Profile
    {
        public VisitorData data;
        public int actualCombat;
        public string resumeClaim;
        public string personality;
        public string preference;
        public string avoidance;
        public string secret;
        public string answerCareer;
        public string answerPreference;
    }

    private readonly List<Profile> queue = new();
    private readonly List<Profile> party = new();
    private readonly List<Profile> waiting = new();

    private TMP_FontAsset font;
    private RectTransform root;
    private Image portrait;
    private TextMeshProUGUI statusText;
    private TextMeshProUGUI visitorText;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI partyText;
    private TextMeshProUGUI questionCountText;
    private Button dispatchButton;
    private GameObject documentPopup;
    private Profile current;
    private int queueIndex;
    private int questionsLeft;
    private int turn = 1;

    private readonly Color ink = new(.20f, .11f, .065f, 1f);
    private readonly Color parchment = new(.91f, .79f, .59f, .98f);
    private readonly Color wood = new(.28f, .14f, .07f, .96f);
    private readonly Color green = new(.43f, .56f, .22f, 1f);
    private readonly Color red = new(.62f, .25f, .19f, 1f);

    private IEnumerator Start()
    {
        yield return null;
        font = Resources.Load<TMP_FontAsset>("UI/Pretendard-Bold SDF");
        BuildProfiles(App.Get<TitleData>());
        var savedRoot = transform.Find("ReceptionGameRoot") as RectTransform;
        if (savedRoot == null)
            BuildScreen();
        else
            BindSavedScreen(savedRoot);
        ShowNextVisitor();
    }

    public void BuildEditorLayout()
    {
        font = Resources.Load<TMP_FontAsset>("UI/Pretendard-Bold SDF");
        BuildScreen();
    }

    private void BuildProfiles(TitleData titleData)
    {
        var ordered = new List<VisitorData>();
        if (titleData.Visitor.TryGetValue("adv_rio", out var rio)) ordered.Add(rio);
        ordered.AddRange(titleData.Visitor.Values.Where(v => v.id != "adv_rio").Take(8));

        foreach (var visitor in ordered)
        {
            var profile = new Profile
            {
                data = visitor,
                actualCombat = visitor.combat,
                resumeClaim = string.IsNullOrWhiteSpace(visitor.combatRecords) ? "초급 의뢰 수행 경험" : visitor.combatRecords,
                personality = "신중함",
                preference = "소형 마물 토벌",
                avoidance = "없음",
                secret = "특이 사항 없음",
                answerCareer = "이력서에 적힌 전적은 사실입니다.",
                answerPreference = "안전한 전열과 함께라면 임무를 수행할 수 있습니다."
            };

            switch (visitor.id)
            {
                case "adv_rio":
                    profile.personality = "성실함 / 긴장하면 말이 빨라짐";
                    profile.preference = "슬라임과 소형 마물";
                    profile.avoidance = "독성 가스가 심한 장소";
                    profile.answerCareer = "마을 외곽에서 슬라임을 직접 토벌했습니다. 불은 쓰지 않았어요.";
                    profile.answerPreference = "슬라임은 익숙하지만 독성 환경에는 지원이 필요합니다.";
                    break;
                case "adv_elena":
                    profile.actualCombat = 35;
                    profile.resumeClaim = "베테랑 전사 / 고블린 부대 격퇴";
                    profile.personality = "허세가 강함";
                    profile.secret = "대규모 전투 전적은 동료의 공적을 과장한 것";
                    profile.answerCareer = "제가 선두였다고 쓰긴 했지만… 실제로는 후방 경계였습니다.";
                    break;
                case "adv_finn":
                    profile.personality = "관찰력이 좋고 말수가 적음";
                    profile.preference = "추적 / 독성 생물";
                    profile.answerPreference = "슬라임 점액과 독성 가스를 구분할 수 있습니다.";
                    break;
                case "adv_mira":
                    profile.personality = "타인을 우선함";
                    profile.preference = "구조 / 치료";
                    profile.answerPreference = "민간인이 있다면 전투보다 구조를 먼저 하겠습니다.";
                    break;
                case "adv_iris":
                    profile.preference = "화염 마법을 이용한 섬멸";
                    profile.avoidance = "밀폐 공간";
                    profile.secret = "당황하면 화염 주문을 사용함";
                    profile.answerPreference = "빠른 섬멸이 특기지만 좁은 곳에서는 위험할 수 있습니다.";
                    break;
            }
            queue.Add(profile);
        }
    }

    private void BuildScreen()
    {
        var old = transform.Find("ReceptionGameRoot");
        if (old != null)
        {
            if (Application.isPlaying) Destroy(old.gameObject);
            else DestroyImmediate(old.gameObject);
        }

        root = Rect("ReceptionGameRoot", transform);
        Stretch(root);
        root.SetAsLastSibling();

        var background = Image("Background", root, Color.white, Resources.Load<Sprite>("UI/background_1"));
        Stretch(background.rectTransform);
        background.raycastTarget = false;

        var top = Image("TopStatus", root, parchment);
        Set(top.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -48), new Vector2(-30, 82));
        statusText = Text("Status", top.transform, "", 25, ink, TextAlignmentOptions.Center);
        Stretch(statusText.rectTransform, 18);

        var questCard = Image("QuestCard", root, wood);
        Set(questCard.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(245, 75), new Vector2(410, 610));
        var questTitle = Text("QuestSummary", questCard.transform,
            "오늘의 의뢰\n\n하수도 슬라임 제거\n\n공개 조건\n· 슬라임 10마리 제거\n· 권장 3~4명\n· 예상 위험도: 보통",
            25, new Color(1f, .89f, .68f), TextAlignmentOptions.TopLeft);
        Stretch(questTitle.rectTransform, 34);
        Button("QuestDocumentButton", questCard.transform, "의뢰서 자세히 보기", new Vector2(0, -245), new Vector2(330, 64), parchment, ink, OpenQuestDocument);

        portrait = Image("VisitorPortrait", root, Color.white, null);
        Set(portrait.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, 100), new Vector2(460, 460));
        portrait.preserveAspect = true;
        portrait.raycastTarget = false;

        var speech = Image("DialogueCard", root, parchment);
        Set(speech.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(305, 210), new Vector2(360, 190));
        dialogueText = Text("Dialogue", speech.transform, "", 21, ink, TextAlignmentOptions.TopLeft);
        Stretch(dialogueText.rectTransform, 24);

        var visitorPlate = Image("VisitorPlate", root, parchment);
        Set(visitorPlate.rectTransform, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 335), new Vector2(460, 70));
        visitorText = Text("VisitorName", visitorPlate.transform, "", 25, ink, TextAlignmentOptions.Center);
        Stretch(visitorText.rectTransform, 14);

        Button("ResumeButton", root, "두루마리 이력서 열기", new Vector2(0, 245), new Vector2(300, 78), parchment, ink, OpenResumeDocument);

        var action = Image("ActionPanel", root, wood);
        Set(action.rectTransform, new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(-245, 40), new Vector2(410, 680));
        questionCountText = Text("QuestionCount", action.transform, "", 23, new Color(1f, .89f, .68f), TextAlignmentOptions.Center);
        Set(questionCountText.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -45), new Vector2(340, 55));
        Button("CareerQuestion", action.transform, "전적을 질문한다", new Vector2(0, 230), new Vector2(330, 62), parchment, ink, () => Ask(true));
        Button("PreferenceQuestion", action.transform, "선호 임무를 질문한다", new Vector2(0, 155), new Vector2(330, 62), parchment, ink, () => Ask(false));
        Button("Approve", action.transform, "승인 · 파티 후보", new Vector2(0, 45), new Vector2(330, 68), green, Color.white, Approve);
        Button("Wait", action.transform, "대기", new Vector2(0, -35), new Vector2(330, 62), new Color(.65f, .48f, .24f), Color.white, WaitVisitor);
        Button("Reject", action.transform, "거절", new Vector2(0, -110), new Vector2(330, 62), red, Color.white, Reject);
        Button("UndoParty", action.transform, "최근 승인 취소", new Vector2(0, -205), new Vector2(330, 56), parchment, ink, UndoParty);
        dispatchButton = Button("DispatchParty", action.transform, "파티 파견", new Vector2(0, -275), new Vector2(330, 72), green, Color.white, Dispatch);

        var roster = Image("PartyRoster", root, new Color(.22f, .11f, .055f, .96f));
        Set(roster.rectTransform, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 90), new Vector2(940, 140));
        partyText = Text("PartyText", roster.transform, "", 22, new Color(1f, .88f, .66f), TextAlignmentOptions.Center);
        Stretch(partyText.rectTransform, 18);

        RefreshParty();
    }

    private void BindSavedScreen(RectTransform savedRoot)
    {
        root = savedRoot;
        root.SetAsLastSibling();
        portrait = FindDeep("VisitorPortrait").GetComponent<Image>();
        statusText = FindDeep("Status").GetComponent<TextMeshProUGUI>();
        visitorText = FindDeep("VisitorName").GetComponent<TextMeshProUGUI>();
        dialogueText = FindDeep("Dialogue").GetComponent<TextMeshProUGUI>();
        partyText = FindDeep("PartyText").GetComponent<TextMeshProUGUI>();
        questionCountText = FindDeep("QuestionCount").GetComponent<TextMeshProUGUI>();
        dispatchButton = FindDeep("DispatchParty").GetComponent<Button>();

        BindButton("QuestDocumentButton", OpenQuestDocument);
        BindButton("ResumeButton", OpenResumeDocument);
        BindButton("CareerQuestion", () => Ask(true));
        BindButton("PreferenceQuestion", () => Ask(false));
        BindButton("Approve", Approve);
        BindButton("Wait", WaitVisitor);
        BindButton("Reject", Reject);
        BindButton("UndoParty", UndoParty);
        BindButton("DispatchParty", Dispatch);
        RefreshParty();
    }

    private Transform FindDeep(string objectName)
    {
        foreach (var child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == objectName) return child;
        return null;
    }

    private void BindButton(string objectName, UnityEngine.Events.UnityAction action)
    {
        var button = FindDeep(objectName).GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    private void ShowNextVisitor()
    {
        CloseDocument();
        if (queue.Count == 0)
        {
            current = null;
            visitorText.text = "오늘의 방문자가 모두 처리되었습니다";
            dialogueText.text = "승인한 모험가로 파티를 구성해 의뢰에 파견하세요.";
            portrait.enabled = false;
            return;
        }

        current = queue[queueIndex % queue.Count];
        queueIndex++;
        questionsLeft = 2;
        portrait.enabled = true;
        if (visitorPortraits != null && visitorPortraits.Length > 0)
            portrait.sprite = visitorPortraits[(queueIndex - 1) % visitorPortraits.Length];
        visitorText.text = $"{current.data.displayName} · Lv.{current.data.level} · {JobName(current.data.jobId)}";
        dialogueText.text = "안녕하세요. 길드 등록과 의뢰 참가를 신청하러 왔습니다.";
        RefreshStatus();
    }

    private void Ask(bool career)
    {
        if (current == null || questionsLeft <= 0) return;
        questionsLeft--;
        dialogueText.text = career ? current.answerCareer : current.answerPreference;
        RefreshStatus();
    }

    private void Approve()
    {
        if (current == null) return;
        if (party.Count >= 4)
        {
            dialogueText.text = "파티 후보는 최대 4명입니다. 한 명을 취소한 뒤 승인하세요.";
            return;
        }
        if (!party.Contains(current)) party.Add(current);
        RemoveCurrentAndContinue();
    }

    private void Reject()
    {
        if (current == null) return;
        RemoveCurrentAndContinue();
    }

    private void WaitVisitor()
    {
        if (current == null) return;
        waiting.Add(current);
        queue.Remove(current);
        queue.Add(current);
        queueIndex = 0;
        ShowNextVisitor();
    }

    private void RemoveCurrentAndContinue()
    {
        queue.Remove(current);
        queueIndex = 0;
        RefreshParty();
        ShowNextVisitor();
    }

    private void UndoParty()
    {
        if (party.Count == 0) return;
        var returned = party[^1];
        party.RemoveAt(party.Count - 1);
        queue.Add(returned);
        RefreshParty();
        dialogueText.text = $"{returned.data.displayName}의 승인을 취소하고 대기열로 돌려보냈습니다.";
    }

    private void RefreshParty()
    {
        partyText.text = party.Count == 0
            ? "파티 후보 0/4 · 이력과 답변을 검토해 모험가를 승인하세요"
            : $"파티 후보 {party.Count}/4\n" + string.Join("  |  ", party.Select(p => $"{p.data.displayName}({JobName(p.data.jobId)})"));
        if (dispatchButton != null) dispatchButton.interactable = party.Count >= 3;
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        if (statusText != null) statusText.text = $"제 1막 · 접수 턴 {turn}   |   질문 {questionsLeft}/2   |   파티 {party.Count}/4   |   길드 평판 50";
        if (questionCountText != null) questionCountText.text = $"추가 질문 {questionsLeft}/2";
    }

    private void OpenResumeDocument()
    {
        if (current == null) return;
        var d = current.data;
        var injury = string.IsNullOrWhiteSpace(d.injuryHistory) ? "없음" : d.injuryHistory;
        OpenDocument("모험가 등록 이력서",
            $"이름  {d.displayName}\n종족  인간\n직업  {JobName(d.jobId)}\n등급  Lv.{d.level}\n\n" +
            $"공개 능력\n전투 {d.combat}  민첩 {d.agility}  생존 {d.survival}  지식 {d.knowledge}\n\n" +
            $"제출 장비\n{d.submittedEquipment}\n\n부상 이력\n{injury}\n\n기재 경력\n{current.resumeClaim}\n\n" +
            "※ 서류 내용은 본인의 주장으로, 사실과 다를 수 있습니다.");
    }

    private void OpenQuestDocument()
    {
        OpenDocument("의뢰서 · 하수도 슬라임 제거",
            "의뢰인  서쪽 구역 주민 대표\n목표  하수도 슬라임 10마리 제거\n권장 인원  3~4명\n예상 위험도  보통\n환경  좁고 습한 지하 수로\n\n" +
            "의뢰인의 말\n\"냄새가 좀 심하지만 슬라임만 치우면 됩니다.\n너무 요란하게 처리하지는 말아 주세요.\"\n\n" +
            "※ 의뢰인이 모든 위험과 조건을 정확히 설명했다고 보장할 수 없습니다.");
    }

    private void Dispatch()
    {
        if (party.Count < 3) return;
        CloseDocument();
        var fireRisk = party.Any(p => p.data.id is "adv_iris" or "adv_sena");
        var poisonReady = party.Any(p => p.data.jobId is "job_ranger" or "job_cleric" || p.data.knowledge >= 50);
        var rescueReady = party.Any(p => p.data.jobId == "job_cleric" || p.data.survival >= 50);
        var combat = party.Sum(p => p.actualCombat);
        var lies = party.Where(p => p.actualCombat + 10 < p.data.combat).Select(p => p.data.displayName).ToArray();
        var score = combat + (poisonReady ? 35 : -35) + (rescueReady ? 25 : -25) + (fireRisk ? -45 : 0);
        var grade = score >= 210 ? "완전 성공" : score >= 155 ? "조건부 성공" : "목표 달성 후 추가 피해";
        var report =
            $"판정  {grade}\n\n" +
            $"파견 파티  {string.Join(", ", party.Select(p => p.data.displayName))}\n\n" +
            "현장에서 확인된 숨은 조건\n" +
            $"· 화염 사용 금지: {(fireRisk ? "위반 · 시설 피해 발생" : "준수")}\n" +
            $"· 독성 가스: {(poisonReady ? "대응 성공" : "대응 실패 · 전원 경상")}\n" +
            $"· 민간인 구조: {(rescueReady ? "구조 성공" : "발견 지연 · 의뢰인 불만")}\n\n" +
            (lies.Length > 0 ? $"서류 검증 결과\n{string.Join(", ", lies)}의 공개 전투 경력이 과장되어 있었습니다.\n\n" : "") +
            "놓친 단서는 다음 접수에서 질문과 서류 비교에 활용할 수 있습니다.";
        OpenDocument("제 1막 퀘스트 결과 보고서", report, true);
    }

    private void OpenDocument(string title, string body, bool result = false)
    {
        CloseDocument();
        documentPopup = new GameObject("DocumentPopup", typeof(RectTransform), typeof(Image), typeof(DraggableResumePopup));
        documentPopup.layer = 5;
        var rect = documentPopup.GetComponent<RectTransform>();
        rect.SetParent(root, false);
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.sizeDelta = new Vector2(760, 790);
        rect.anchoredPosition = new Vector2(40, 0);
        documentPopup.GetComponent<Image>().color = parchment;
        var titleText = Text("DocumentTitle", rect, title, 31, ink, TextAlignmentOptions.Center);
        Set(titleText.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -48), new Vector2(-140, 70));
        var bodyText = Text("DocumentBody", rect, body, 22, ink, TextAlignmentOptions.TopLeft);
        bodyText.textWrappingMode = TextWrappingModes.Normal;
        bodyText.overflowMode = TextOverflowModes.Truncate;
        bodyText.rectTransform.anchorMin = Vector2.zero;
        bodyText.rectTransform.anchorMax = Vector2.one;
        bodyText.rectTransform.offsetMin = new Vector2(60, result ? 60 : 70);
        bodyText.rectTransform.offsetMax = new Vector2(-60, -100);
        Button("CloseDocument", rect, "닫기 ×", new Vector2(300, 350), new Vector2(120, 50), wood, Color.white, CloseDocument);
    }

    private void CloseDocument()
    {
        if (documentPopup != null)
        {
            documentPopup.SetActive(false);
            Destroy(documentPopup);
        }
        documentPopup = null;
    }

    private RectTransform Rect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    private Image Image(string name, Transform parent, Color color, Sprite sprite = null)
    {
        var rect = Rect(name, parent);
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.sprite = sprite;
        return image;
    }

    private TextMeshProUGUI Text(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        var rect = Rect(name, parent);
        var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        if (font != null) text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private Button Button(string name, Transform parent, string label, Vector2 position, Vector2 size, Color color, Color textColor, UnityEngine.Events.UnityAction action)
    {
        var image = Image(name, parent, color);
        Set(image.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), position, size);
        var button = image.gameObject.AddComponent<Button>();
        button.onClick.AddListener(action);
        var text = Text("Label", image.transform, label, 22, textColor, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, 10);
        return button;
    }

    private static void Set(RectTransform rect, Vector2 min, Vector2 max, Vector2 position, Vector2 size)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    private static void Stretch(RectTransform rect, float margin = 0)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(margin, margin);
        rect.offsetMax = new Vector2(-margin, -margin);
        rect.localScale = Vector3.one;
    }

    private static string JobName(string id) => id switch
    {
        "job_warrior" => "전사", "job_ranger" => "레인저", "job_cleric" => "성직자",
        "job_rogue" => "도적", "job_mage" => "마법사", "job_paladin" => "성기사", _ => "모험가"
    };
}
