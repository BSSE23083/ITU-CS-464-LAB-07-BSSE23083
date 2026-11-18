using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 08 - Race HUD and pause menu. The race is not playable, it's just a fake 15 second race
// so the pause menu and results screen can be tested.
public class RaceScreen : UIScreen
{
    public CanvasGroup hud, pause;
    public RectTransform pausePanel;
    public TMP_Text posValue, lapValue, timeValue, speedValue, gearValue, countdown, pauseInfo;
    public Image speedFill, nitroFill;
    public Button pauseButton, resumeButton, restartButton, settingsButton, quitButton;

    public bool InRace { get; private set; }
    bool paused, running, finished;
    float clock, countdownLeft, finishWait;
    int lastCount;

    const float LapSeconds = 5f;      // real seconds per lap in the demo
    const float TimeScaleShown = 20f; // shown race clock runs 20x faster
    const int Laps = 3;

    public override void Build()
    {
        var bg = UIKit.Img(transform, "RaceBackground", Palette.Hex("070A10"));
        bg.RT().Stretch();

        hud = UIKit.Group(UIKit.New("HUD", transform).Stretch().gameObject);
        var h = hud.transform;

        var posBox = Panel(h, "Position", 0, 1, 48, -40, 250, 156);
        UIKit.Text(posBox, "Label", "POSITION", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4).RT().At(0, 1, 24, -16, 200, 28);
        posValue = UIKit.Heading(posBox, "Value", "6<size=50%><color=#A6B0C4>/8</color></size>", 100, Palette.Text, TextAlignmentOptions.Left);
        posValue.RT().At(0, 0, 22, 4, 220, 110);

        var lapBox = Panel(h, "Lap", 0, 1, 318, -40, 330, 156);
        UIKit.Text(lapBox, "Label", "LAP", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4).RT().At(0, 1, 24, -16, 200, 28);
        lapValue = UIKit.Heading(lapBox, "Value", "1/3", 64, Palette.Text, TextAlignmentOptions.Left);
        lapValue.RT().At(0, 1, 22, -40, 280, 66);
        timeValue = UIKit.Heading(lapBox, "Time", "0:00.000", 34, Palette.Cyan, TextAlignmentOptions.Left, 2);
        timeValue.RT().At(0, 0, 24, 14, 280, 40);

        pauseButton = UIKit.Button(h, "PauseButton", "", ButtonStyle.Secondary);
        pauseButton.RT().At(.5f, 1, 0, -40, 80, 80);
        var pmb = pauseButton.GetComponent<MenuButton>();
        pmb.hotScale = 1.1f;
        pmb.icon = UIKit.Img(pauseButton.transform, "Icon", Palette.Text, UIKit.T.Icon("pause"));
        pmb.icon.RT().At(.5f, .5f, 0, 0, 48, 48);
        pmb.Apply(true);
        UIKit.Text(h, "PauseHint", Key("ESC") + "  PAUSE", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Center, 3).RT().At(.5f, 1, 0, -132, 300, 30);

        // speed
        var spd = Panel(h, "Speed", 1, 0, -48, 40, 360, 190);
        speedValue = UIKit.Heading(spd, "Value", "0", 96, Palette.Text, TextAlignmentOptions.Left);
        speedValue.RT().At(0, 1, 24, -10, 200, 100);
        UIKit.Text(spd, "Units", "KM/H", 22, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4).RT().At(0, 1, 200, -50, 100, 30);
        UIKit.Text(spd, "GearLabel", "GEAR", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Right, 4).RT().At(1, 1, -24, -20, 100, 28);
        gearValue = UIKit.Heading(spd, "Gear", "3", 48, Palette.Accent, TextAlignmentOptions.Right);
        gearValue.RT().At(1, 1, -24, -46, 100, 56);
        var st = UIKit.Box(spd, "Track", Palette.Raised).RT().At(0, 0, 24, 28, 312, 16);
        speedFill = UIKit.Img(st, "Fill", Palette.Accent, UIKit.T.round);
        speedFill.type = Image.Type.Filled; speedFill.fillMethod = Image.FillMethod.Horizontal; speedFill.fillAmount = .6f;
        speedFill.RT().Stretch();

        var nitro = Panel(h, "Nitro", 0, 0, 48, 40, 400, 86);
        UIKit.Text(nitro, "Label", "NITRO", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4).RT().At(0, 1, 24, -14, 200, 28);
        var nt = UIKit.Box(nitro, "Track", Palette.Raised, 4f).RT().At(0, 0, 24, 20, 352, 16);
        nitroFill = UIKit.Img(nt, "Fill", Palette.Cyan, UIKit.T.round);
        nitroFill.type = Image.Type.Filled; nitroFill.fillMethod = Image.FillMethod.Horizontal; nitroFill.fillAmount = .7f;
        nitroFill.RT().Stretch();

        countdown = UIKit.Heading(h, "Countdown", "", 280, Palette.Text, TextAlignmentOptions.Center, 6);
        countdown.RT().At(.5f, .5f, 0, 80, 1400, 300);

        // pause overlay
        pause = UIKit.Group(UIKit.New("PauseOverlay", transform).Stretch().gameObject);
        UIKit.Img(pause.transform, "Dim", Palette.Dim, null, true).RT().Stretch();
        pausePanel = UIKit.Box(pause.transform, "Panel", Palette.Hex("141B29")).RT().At(.5f, .5f, 0, 0, 640, 720);
        UIKit.Outline(pausePanel, "Border", Palette.Line);
        UIKit.Img(pausePanel, "Accent", Palette.Accent).RT().Band(1, 0, 0, 0, 6);
        UIKit.Heading(pausePanel, "Title", "PAUSED", 84, Palette.Text, TextAlignmentOptions.Center, 8).RT().At(.5f, 1, 0, -40, 440, 96);
        pauseInfo = UIKit.Text(pausePanel, "Info", "CIRCUIT  ·  HARBOR LOOP  ·  LAP 1/3", 24, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Center, 3);
        pauseInfo.RT().At(.5f, 1, 0, -140, 580, 32);

        resumeButton = UIKit.Button(pausePanel, "Resume", "RESUME", ButtonStyle.Primary, "flag", 36);
        resumeButton.RT().At(.5f, 1, 0, -212, 520, 88);
        restartButton = UIKit.Button(pausePanel, "Restart", "RESTART RACE", ButtonStyle.Secondary, "loop", 32);
        restartButton.RT().At(.5f, 1, 0, -324, 520, 80);
        settingsButton = UIKit.Button(pausePanel, "Settings", "SETTINGS", ButtonStyle.Secondary, "gear", 32);
        settingsButton.RT().At(.5f, 1, 0, -420, 520, 80);
        quitButton = UIKit.Button(pausePanel, "Quit", "QUIT TO MENU", ButtonStyle.Danger, "power", 32);
        quitButton.RT().At(.5f, 1, 0, -548, 520, 80);
        UIKit.Img(pausePanel, "Sep", Palette.Line.A(.5f)).RT().At(.5f, 1, 0, -522, 520, 2);
        UIKit.Text(pausePanel, "Hint", Key("ESC") + "  RESUME", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Center, 3)
            .RT().At(.5f, 0, 0, 28, 400, 30);
        pause.gameObject.SetActive(false);
    }

    RectTransform Panel(Transform p, string name, float ax, float ay, float x, float y, float w, float hgt)
    {
        var b = UIKit.Box(p, name, Palette.Panel.A(.94f)).RT().At(ax, ay, x, y, w, hgt);
        UIKit.Outline(b, "Border", Palette.Line.A(.5f));
        return b;
    }

    protected override void Bind()
    {
        pauseButton.onClick.AddListener(() => SetPaused(true));
        resumeButton.onClick.AddListener(() => SetPaused(false));
        restartButton.onClick.AddListener(() => { SetPaused(false); StartRace(); });
        settingsButton.onClick.AddListener(() => menu.Push(menu.settings));
        quitButton.onClick.AddListener(() => menu.Confirm("QUIT RACE?", "You'll lose your progress in this race and won't earn any credits.", "QUIT RACE", () =>
        {
            InRace = false;
            Time.timeScale = 1;
            menu.ResetTo(menu.main);
        }, true));
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        if (returning && InRace) { firstSelected = resumeButton; return; } // back from settings, still paused
        StartRace();
    }

    void StartRace()
    {
        InRace = true;
        running = finished = false;
        clock = 0;
        countdownLeft = 0f; // no countdown, the race just starts
        running = true;
        countdown.text = "";
        paused = false;
        Time.timeScale = 1;
        pause.gameObject.SetActive(false);
        firstSelected = pauseButton;
        UpdateHud();
    }

    void Update()
    {
        if (menu == null || menu.Current != this || !InRace) return;
        float dt = Time.deltaTime; // scaled: stops while paused

        if (countdownLeft > 0)
        {
            countdownLeft -= dt;
            int n = Mathf.CeilToInt(countdownLeft);
            if (n != lastCount)
            {
                lastCount = n;
                Pop(n > 0 ? n.ToString() : "GO!", n > 0 ? Palette.Text : Palette.Accent);
                if (n <= 0) running = true;
            }
        }
        else if (running)
        {
            clock += dt;
            if (clock >= LapSeconds * Laps) Finish();
        }
        else if (finished)
        {
            finishWait -= Time.unscaledDeltaTime;
            if (finishWait <= 0 && !menu.Busy)
            {
                InRace = false;
                finished = false;
                menu.ResetTo(menu.main, menu.results);
            }
        }
        UpdateHud();
    }

    void Pop(string text, Color c)
    {
        countdown.text = text;
        countdown.color = c;
        countdown.alpha = 1;
    }

    void Finish()
    {
        running = false;
        finished = true;
        finishWait = 1.8f;
        GameData.LastPosition = 2;
        GameData.LastTime = clock * TimeScaleShown;
        Pop("FINISH", Palette.Gold);
    }

    void UpdateHud()
    {
        int lap = Mathf.Clamp(1 + Mathf.FloorToInt(clock / LapSeconds), 1, Laps);
        posValue.text = "2<size=50%><color=#A6B0C4>/8</color></size>";
        lapValue.text = lap + "<size=60%><color=#A6B0C4>/" + Laps + "</color></size>";
        timeValue.text = FormatTime(clock * TimeScaleShown);
        // speed, gear and nitro are just fixed values since the race isn't playable
        speedValue.text = "184";
        speedFill.fillAmount = 0.7f;
        gearValue.text = "5";
        nitroFill.fillAmount = 0.6f;
        var t = GameData.Tracks[GameData.TrackIndex];
        pauseInfo.text = GameData.Modes[GameData.ModeIndex].name + "  ·  " + t.name + "  ·  LAP " + lap + "/" + Laps;
    }

    public static string FormatTime(float s) => Mathf.FloorToInt(s / 60) + ":" + (s % 60).ToString("00.000");

    void SetPaused(bool p)
    {
        if (paused == p || finished) return;
        paused = p;
        Time.timeScale = p ? 0 : 1; // the menu keeps animating because tweens use unscaled time
        if (p)
        {
            pause.gameObject.SetActive(true);
            pause.alpha = 0;
            Tween.Fade(pause, 1, 0.2f, Ease.OutQuad);
            pausePanel.anchoredPosition = new Vector2(0, 60);
            Tween.Move(pausePanel, Vector2.zero, 0.4f, Ease.OutBack);
            firstSelected = resumeButton;
            MenuManager.Focus(resumeButton);
        }
        else
        {
            Tween.Move(pausePanel, new Vector2(0, -40), 0.15f, Ease.InQuad);
            Tween.Fade(pause, 0, 0.15f, Ease.InQuad, () => pause.gameObject.SetActive(false));
            firstSelected = pauseButton;
            MenuManager.Focus(pauseButton);
        }
    }

    public override void OnBack() => SetPaused(!paused);

    public override void OnHide()
    {
        if (!InRace) Time.timeScale = 1;
    }
}
