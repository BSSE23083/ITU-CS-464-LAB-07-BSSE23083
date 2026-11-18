using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 09 - Race results
public class ResultsScreen : UIScreen
{
    public TMP_Text place, totalTime, bestLap, carChip, rewardTotal, xpLabel, levelLabelSmall;
    public RectTransform podium, board, rewards;
    public RectTransform[] rows;
    public TMP_Text[] rowNames, rowCars, rowTimes, rowPos;
    public Image playerHighlight, xpFill;
    public Button continueButton, againButton, carButton;

    static readonly string[] Rivals = { "VEX", "KAITO_R", "MARLOWE", "D. SANTOS", "LUNA", "OKAFOR", "BRAKE_LATE" };
    static readonly string[] RivalCars = { "SPECTRE X", "BRUTUS V8", "VANTA S7", "BRUTUS V8", "KESTREL HR", "VANTA S7", "KESTREL HR" };
    const int Reward = 9600;

    public override void Build()
    {
        BuildHeader("RACE RESULTS", "CIRCUIT  ·  HARBOR LOOP", false);

        // podium
        podium = UIKit.Box(transform, "Podium", Palette.Panel).RT().At(0, 1, 64, -190, 500, 650);
        UIKit.Outline(podium, "Border", Palette.Line.A(.55f));
        var trophy = UIKit.Img(podium, "Trophy", Palette.Gold, UIKit.T.Icon("trophy"));
        trophy.RT().At(.5f, 1, 0, -40, 72, 72);
        UIKit.Text(podium, "Label", "YOU FINISHED", 22, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Center, 6).RT().At(.5f, 1, 0, -126, 400, 30);
        place = UIKit.Heading(podium, "Place", "2<size=40%>ND</size>", 230, Palette.Gold, TextAlignmentOptions.Center, 2);
        place.RT().At(.5f, 1, 0, -150, 460, 230);
        totalTime = Stat(podium, "TOTAL TIME", "5:00.000", -400);
        bestLap = Stat(podium, "BEST LAP", "1:38.204", -486);
        carChip = UIKit.Chip(podium, "Car", "VANTA S7", Palette.Cyan, 260, 42, 22);
        carChip.transform.parent.RT().At(.5f, 0, 0, 36, 260, 42);

        // leaderboard
        board = UIKit.Box(transform, "Leaderboard", Palette.Panel).RT().At(0, 1, 592, -190, 780, 650);
        UIKit.Outline(board, "Border", Palette.Line.A(.55f));
        string[] heads = { "POS", "DRIVER", "CAR", "TIME" };
        float[] cols = { 28, 110, 370, 600 };
        for (int c = 0; c < 4; c++)
            UIKit.Text(board, "Head" + c, heads[c], 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4).RT().At(0, 1, cols[c], -26, 200, 28);
        rows = new RectTransform[8];
        rowNames = new TMP_Text[8]; rowCars = new TMP_Text[8]; rowTimes = new TMP_Text[8]; rowPos = new TMP_Text[8];
        playerHighlight = UIKit.Box(board, "PlayerRow", UIKit.ChipFill(Palette.Cyan));
        for (int i = 0; i < 8; i++)
        {
            var r = UIKit.Img(board, "Row" + i, i % 2 == 0 ? Palette.Raised.A(.35f) : new Color(0, 0, 0, 0)).RT();
            r.Band(1, 12, 12, -66 - i * 70, 64);
            rows[i] = r;
            rowPos[i] = UIKit.Heading(r, "Pos", (i + 1).ToString(), 36, Palette.Text, TextAlignmentOptions.Left);
            rowPos[i].RT().At(0, .5f, 18, 0, 60, 44);
            rowNames[i] = UIKit.Heading(r, "Name", "-", 32, Palette.Text, TextAlignmentOptions.Left, 2);
            rowNames[i].RT().At(0, .5f, 98, 0, 250, 40);
            rowCars[i] = UIKit.Text(r, "Car", "-", 24, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 2);
            rowCars[i].RT().At(0, .5f, 358, 0, 220, 34);
            rowTimes[i] = UIKit.Heading(r, "Time", "-", 32, Palette.Text, TextAlignmentOptions.Left, 1);
            rowTimes[i].RT().At(0, .5f, 588, 0, 170, 40);
        }
        UIKit.Outline(playerHighlight.transform, "Border", Palette.Cyan, true);
        var you = UIKit.Chip(playerHighlight.transform, "You", "YOU", Palette.Cyan, 64, 30, 18);
        you.transform.parent.RT().At(0, .5f, 270, 0, 64, 30);

        // rewards
        rewards = UIKit.Box(transform, "Rewards", Palette.Panel).RT().At(1, 1, -64, -190, 456, 650);
        UIKit.Outline(rewards, "Border", Palette.Gold.A(.4f));
        UIKit.Text(rewards, "Label", "REWARDS", 22, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 6).RT().At(0, 1, 32, -30, 300, 30);
        var coin = UIKit.Img(rewards, "Coin", Palette.Gold, UIKit.T.Icon("coin"));
        coin.RT().At(0, 1, 32, -80, 52, 52);
        rewardTotal = UIKit.Heading(rewards, "Total", "+0 CR", 64, Palette.Gold, TextAlignmentOptions.Left, 1);
        rewardTotal.RT().At(0, 1, 98, -70, 340, 70);
        Line(rewards, "Finished 2nd", "+8,000", -180);
        Line(rewards, "Clean race bonus", "+1,000", -228);
        Line(rewards, "Fastest lap bonus", "+600", -276);
        UIKit.Img(rewards, "Sep", Palette.Line.A(.5f)).RT().Band(1, 32, 32, -334, 2);
        xpLabel = UIKit.Heading(rewards, "Xp", "+1,250 XP", 44, Palette.Cyan, TextAlignmentOptions.Left, 1);
        xpLabel.RT().At(0, 1, 32, -360, 380, 50);
        levelLabelSmall = UIKit.Text(rewards, "Level", "LEVEL 9", 22, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4);
        levelLabelSmall.RT().At(0, 1, 32, -424, 380, 30);
        var xt = UIKit.Box(rewards, "XpTrack", Palette.Raised, 4f).RT().At(0, 1, 32, -464, 392, 16);
        xpFill = UIKit.Img(xt, "Fill", Palette.Cyan, UIKit.T.round);
        xpFill.pixelsPerUnitMultiplier = 4f;
        xpFill.type = Image.Type.Filled; xpFill.fillMethod = Image.FillMethod.Horizontal; xpFill.fillAmount = .7f;
        xpFill.RT().Stretch();
        UIKit.Text(rewards, "Next", "Time Attack unlocks at level 12", 22, Palette.TextDim).RT().At(0, 1, 32, -500, 392, 30);

        carButton = UIKit.Button(transform, "ChangeCar", "CHANGE CAR", ButtonStyle.Secondary, "car", 30);
        carButton.RT().At(1, 1, -768, -880, 320, 84);
        againButton = UIKit.Button(transform, "RaceAgain", "RACE AGAIN", ButtonStyle.Secondary, "loop", 30);
        againButton.RT().At(1, 1, -432, -880, 320, 84);
        continueButton = UIKit.Button(transform, "Continue", "CONTINUE", ButtonStyle.Primary, "chev_right", 34);
        continueButton.RT().At(1, 1, -64, -880, 352, 84);

        BuildFooter(Key("ENTER") + "  SELECT      " + Key("ESC") + "  CONTINUE TO MENU");
        firstSelected = continueButton;
        Fill();
    }

    TMP_Text Stat(Transform p, string label, string value, float y)
    {
        UIKit.Text(p, label, label, 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Center, 4).RT().At(.5f, 1, 0, y, 400, 28);
        var v = UIKit.Heading(p, label + "Value", value, 46, Palette.Text, TextAlignmentOptions.Center, 1);
        v.RT().At(.5f, 1, 0, y - 28, 400, 52);
        return v;
    }

    void Line(Transform p, string label, string value, float y)
    {
        UIKit.Text(p, "L_" + label, label, 26, Palette.TextDim).RT().At(0, 1, 32, y, 280, 36);
        UIKit.Heading(p, "V_" + label, value, 32, Palette.Text, TextAlignmentOptions.Right).RT().At(1, 1, -32, y, 140, 36);
    }

    protected override void Bind()
    {
        continueButton.onClick.AddListener(OnBack);
        againButton.onClick.AddListener(() => menu.Push(menu.race));
        carButton.onClick.AddListener(() => { menu.garage.raceSetup = true; menu.Push(menu.garage); });
    }

    public override void OnBack() => menu.ResetTo(menu.main);

    void Fill()
    {
        int me = GameData.LastPosition - 1;
        float t = GameData.LastTime;
        int r = 0;
        for (int i = 0; i < 8; i++)
        {
            bool player = i == me;
            float time = t + (i - me) * 1.37f + (i > me ? 0.4f : 0);
            rowNames[i].text = player ? GameData.PlayerName : Rivals[r];
            rowCars[i].text = player ? GameData.Cars[GameData.CarIndex].name : RivalCars[r];
            rowTimes[i].text = i == 0 ? RaceScreen.FormatTime(time) : "+" + (time - (t - me * 1.37f)).ToString("0.000");
            if (!player) r++;
            rowNames[i].color = player ? Palette.Cyan : Palette.Text;
        }
        var hr = playerHighlight.RT();
        hr.anchorMin = rows[me].anchorMin; hr.anchorMax = rows[me].anchorMax; hr.pivot = rows[me].pivot;
        hr.sizeDelta = rows[me].sizeDelta; hr.anchoredPosition = rows[me].anchoredPosition;
        // sits just behind your row so the row's text stays on top
        hr.SetSiblingIndex(rows[me].GetSiblingIndex());

        string[] suffix = { "ST", "ND", "RD", "TH", "TH", "TH", "TH", "TH" };
        place.text = GameData.LastPosition + "<size=40%>" + suffix[me] + "</size>";
        totalTime.text = RaceScreen.FormatTime(t);
        bestLap.text = RaceScreen.FormatTime(t / 3f - 1.8f);
        carChip.text = GameData.Cars[GameData.CarIndex].name;
        kickerLabel.text = GameData.Modes[GameData.ModeIndex].name + "  ·  " + GameData.Tracks[GameData.TrackIndex].name;
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        Fill();
        if (returning) return;

        int before = GameData.Credits;
        GameData.Credits += Reward;
        float xpFrom = GameData.LevelProgress;
        GameData.LevelProgress = Mathf.Min(0.95f, GameData.LevelProgress + 0.15f);
        levelLabelSmall.text = "LEVEL " + GameData.Level + "  ·  " + Mathf.RoundToInt(GameData.LevelProgress * 100) + "%";

        // position pops in, then the credits count up
        place.transform.localScale = Vector3.one * 0.4f;
        Tween.Scale(place.transform, 1, 0.6f, Ease.OutBack, null, 0.2f);
        Tween.Run(rewardTotal, "count", 1.2f, Ease.OutQuart, t =>
        {
            rewardTotal.text = "+" + GameData.Money(Mathf.RoundToInt(Reward * t)) + " CR";
            creditsLabel.text = GameData.Money(Mathf.RoundToInt(Mathf.Lerp(before, GameData.Credits, t))) + " CR";
        }, null, 0.7f);
        xpFill.fillAmount = xpFrom;
        Tween.FillTo(xpFill, GameData.LevelProgress, 0.9f, Ease.OutCubic, 1.0f);
    }
}
