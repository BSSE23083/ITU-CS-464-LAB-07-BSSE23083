using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 07 - Settings. Opened from the main menu or from the pause menu; Back returns to whichever one opened it.
public class SettingsScreen : UIScreen
{
    public Button[] tabs;
    public TMP_Text[] tabLabels;
    public RectTransform tabIndicator;
    public RectTransform[] pages;
    public TMP_Text infoTitle, infoBody, dirtyLabel;
    public Button applyButton, resetButton;
    public SettingControl[] controls;

    int tab;
    bool dirty;
    static readonly string[] TabNames = { "VIDEO", "AUDIO", "CONTROLS" };

    public override void Build()
    {
        BuildHeader("SETTINGS", "OPTIONS");
        tabs = new Button[3];
        tabLabels = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
        {
            tabs[i] = UIKit.Button(transform, "Tab_" + TabNames[i], TabNames[i], ButtonStyle.Secondary, null, 30);
            tabs[i].RT().At(0, 1, 64 + i * 236, -184, 220, 64);
            var mb = tabs[i].GetComponent<MenuButton>();
            tabLabels[i] = mb.label;
            mb.label = null; // tab text colour shows active/inactive, so the screen controls it
        }
        tabIndicator = UIKit.Img(transform, "TabIndicator", Palette.Accent).RT().At(0, 1, 64, -254, 220, 5);

        var list = new List<SettingControl>();
        pages = new RectTransform[3];
        for (int p = 0; p < 3; p++)
        {
            pages[p] = UIKit.New("Page_" + TabNames[p], transform).At(0, 1, 64, -284, 1140, 600);
            UIKit.Group(pages[p].gameObject);
        }

        int row = 0;
        SettingControl Add(int page, SettingControl.Kind kind, string key, string title, string desc, float def, params string[] opts)
        {
            var c = Row(pages[page], row++, kind, key, title, desc, def, opts);
            list.Add(c);
            return c;
        }

        row = 0;
        Add(0, SettingControl.Kind.Choice, "res", "RESOLUTION", "Size of the game image. Higher looks sharper but costs performance.", 2, "1280 × 720", "1600 × 900", "1920 × 1080", "2560 × 1440");
        Add(0, SettingControl.Kind.Choice, "display", "DISPLAY MODE", "Windowed keeps your desktop visible. Fullscreen gives the lowest input lag.", 2, "WINDOWED", "BORDERLESS", "FULLSCREEN");
        Add(0, SettingControl.Kind.Choice, "quality", "GRAPHICS QUALITY", "Preset for shadows, reflections and draw distance.", 2, "LOW", "MEDIUM", "HIGH", "ULTRA");
        Add(0, SettingControl.Kind.Toggle, "vsync", "V-SYNC", "Locks the frame rate to your monitor to stop screen tearing.", 1);
        Add(0, SettingControl.Kind.Toggle, "blur", "MOTION BLUR", "Blurs the edges of the screen at high speed. Some players find it uncomfortable.", 0);
        Add(0, SettingControl.Kind.Slider, "bright", "BRIGHTNESS", "Raise this if night tracks look too dark on your screen.", 50);
        row = 0;
        Add(1, SettingControl.Kind.Slider, "master", "MASTER VOLUME", "Overall loudness of the game.", 80);
        Add(1, SettingControl.Kind.Slider, "music", "MUSIC", "Menu music and in-race radio.", 60);
        Add(1, SettingControl.Kind.Slider, "sfx", "EFFECTS", "Menu clicks, collisions and crowd.", 75);
        Add(1, SettingControl.Kind.Slider, "engine", "ENGINE", "Engine, turbo and tyre sounds.", 85);
        Add(1, SettingControl.Kind.Toggle, "bgmute", "MUTE IN BACKGROUND", "Silence the game when you alt-tab out.", 1);
        row = 0;
        Add(2, SettingControl.Kind.Slider, "steer", "STEERING SENSITIVITY", "How quickly the car turns with keyboard or stick input.", 50);
        Add(2, SettingControl.Kind.Choice, "gearbox", "TRANSMISSION", "Automatic shifts for you. Manual is faster if you can keep up.", 0, "AUTOMATIC", "MANUAL");
        Add(2, SettingControl.Kind.Choice, "camera", "DEFAULT CAMERA", "The view used when a race starts. You can still switch in-race.", 0, "CHASE", "HOOD", "BUMPER", "COCKPIT");
        Add(2, SettingControl.Kind.Choice, "units", "SPEED UNITS", "Shown on the speedometer and in car stats.", 0, "KM/H", "MPH");
        Add(2, SettingControl.Kind.Toggle, "vibe", "CONTROLLER VIBRATION", "Rumble on kerbs, collisions and gear shifts.", 1);
        controls = list.ToArray();

        // help panel explains whatever row has focus
        var info = UIKit.Box(transform, "Info", Palette.Panel).RT().At(1, 1, -64, -284, 590, 572);
        UIKit.Outline(info, "Border", Palette.Line.A(.5f));
        var ic = UIKit.Img(info, "Icon", Palette.Cyan, UIKit.T.Icon("gear"));
        ic.RT().At(0, 1, 36, -36, 48, 48);
        infoTitle = UIKit.Heading(info, "Title", "RESOLUTION", 40, Palette.Text, TextAlignmentOptions.Left, 3);
        infoTitle.RT().At(0, 1, 36, -104, 520, 48);
        infoBody = UIKit.Text(info, "Body", controls[0].description, 28, Palette.TextDim, null, TextAlignmentOptions.TopLeft);
        infoBody.RT().At(0, 1, 36, -164, 520, 240);
        dirtyLabel = UIKit.Text(info, "Dirty", "● UNSAVED CHANGES", 22, Palette.Gold, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4);
        dirtyLabel.RT().At(0, 0, 36, 32, 500, 30);
        dirtyLabel.gameObject.SetActive(false);

        resetButton = UIKit.Button(transform, "Reset", "RESET DEFAULTS", ButtonStyle.Secondary, null, 30);
        resetButton.RT().At(1, 1, -384, -880, 330, 84);
        applyButton = UIKit.Button(transform, "Apply", "APPLY", ButtonStyle.Primary, "check", 34);
        applyButton.RT().At(1, 1, -64, -880, 300, 84);

        BuildFooter(Key("Q / E") + "  SWITCH TAB      " + Key("← →") + "  CHANGE VALUE      " + Key("ENTER") + "  TOGGLE      " + Key("ESC") + "  BACK");
        ShowTab(0, true);
    }

    SettingControl Row(RectTransform page, int i, SettingControl.Kind kind, string key, string title, string desc, float def, string[] opts)
    {
        var bg = UIKit.Box(page, "Row_" + key, Palette.Raised.A(.55f));
        var r = bg.RT().At(0, 1, 0, -i * 98, 1140, 86);
        var c = bg.gameObject.AddComponent<SettingControl>();
        c.kind = kind; c.key = key; c.title = title; c.description = desc; c.options = opts; c.defaultValue = def;
        UIKit.Heading(r, "Label", title, 32, Palette.Text, TextAlignmentOptions.Left, 3).RT().At(0, .5f, 32, 0, 520, 40);

        switch (kind)
        {
            case SettingControl.Kind.Choice:
                c.prev = Small(r, "Prev", "chev_left", -404);
                c.next = Small(r, "Next", "chev_right", -20);
                c.valueText = UIKit.Heading(r, "Value", opts[0], 32, Palette.Text, TextAlignmentOptions.Center, 2);
                c.valueText.RT().At(1, .5f, -84, 0, 312, 44);
                break;
            case SettingControl.Kind.Toggle:
                var trackImg = UIKit.Box(r, "Switch", Palette.Line, 0.5f);
                trackImg.raycastTarget = true;
                trackImg.RT().At(1, .5f, -20, 0, 124, 56);
                c.track = trackImg;
                c.toggle = trackImg.gameObject.AddComponent<Button>();
                c.toggle.transition = Selectable.Transition.None;
                var mb = trackImg.gameObject.AddComponent<MenuButton>();
                mb.style = ButtonStyle.Secondary;
                mb.border = UIKit.Outline(trackImg.transform, "Border", Palette.Line);
                var knob = UIKit.Img(trackImg.transform, "Knob", Palette.Text, UIKit.T.circle);
                c.knob = knob.RT().At(.5f, .5f, 30, 0, 42, 42);
                c.valueText = UIKit.Heading(r, "State", "ON", 30, Palette.Text, TextAlignmentOptions.Right, 3);
                c.valueText.RT().At(1, .5f, -164, 0, 100, 40);
                break;
            case SettingControl.Kind.Slider:
                var sr = UIKit.New("Slider", r).At(1, .5f, -110, 0, 340, 40);
                var back = UIKit.Box(sr, "Track", Palette.Line, 4f);
                back.RT().At(.5f, .5f, 0, 0, 340, 12);
                back.RT().anchorMin = new Vector2(0, .5f); back.RT().anchorMax = new Vector2(1, .5f); back.RT().sizeDelta = new Vector2(0, 12);
                var fillArea = UIKit.New("FillArea", sr).Stretch(0, 14, 0, 14);
                var fill = UIKit.Box(fillArea, "Fill", Palette.Accent, 4f);
                fill.RT().Stretch(); fill.RT().sizeDelta = Vector2.zero;
                var handleArea = UIKit.New("HandleArea", sr).Stretch(14, 0, 14, 0);
                var handle = UIKit.Img(handleArea, "Handle", Palette.Text, UIKit.T.circle, true);
                handle.RT().sizeDelta = new Vector2(34, 0);
                handle.RT().anchorMin = new Vector2(0, 0); handle.RT().anchorMax = new Vector2(0, 1);
                var s = sr.gameObject.AddComponent<Slider>();
                s.fillRect = fill.RT();
                s.handleRect = handle.RT();
                s.targetGraphic = handle;
                s.minValue = 0; s.maxValue = 100; s.wholeNumbers = true;
                var cb = s.colors;
                cb.normalColor = Palette.Text; cb.highlightedColor = Palette.AccentHot; cb.selectedColor = Palette.AccentHot; cb.pressedColor = Palette.Accent;
                s.colors = cb;
                c.slider = s;
                c.valueText = UIKit.Heading(r, "Value", "50", 32, Palette.Text, TextAlignmentOptions.Right, 1);
                c.valueText.RT().At(1, .5f, -24, 0, 70, 40);
                break;
        }
        c.Set(def, false, false);
        return c;
    }

    Button Small(RectTransform parent, string name, string icon, float x)
    {
        var b = UIKit.Button(parent, name, "", ButtonStyle.Secondary, null);
        b.RT().At(1, .5f, x, 0, 60, 60);
        var mb = b.GetComponent<MenuButton>();
        mb.hotScale = 1.1f;
        mb.icon = UIKit.Img(b.transform, "Icon", Palette.Text, UIKit.T.Icon(icon));
        mb.icon.RT().At(.5f, .5f, 0, 0, 26, 26);
        mb.Apply(true);
        return b;
    }

    protected override void Bind()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int idx = i;
            tabs[i].onClick.AddListener(() => ShowTab(idx, false));
        }
        foreach (var c in controls)
        {
            c.Init();
            c.onFocus = ShowInfo;
            c.onChanged = _ => SetDirty(true);
        }
        applyButton.onClick.AddListener(Apply);
        resetButton.onClick.AddListener(() =>
        {
            foreach (var c in controls) c.Set(c.defaultValue, true, false);
            SetDirty(true);
            menu.Toast("Defaults restored. Press APPLY to keep them.", Palette.Cyan);
        });
    }

    void Update()
    {
        if (menu == null || menu.Busy || menu.Current != this) return;
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.qKey.wasPressedThisFrame) ShowTab((tab + 2) % 3, false);
        if (kb.eKey.wasPressedThisFrame) ShowTab((tab + 1) % 3, false);
    }

    void ShowTab(int i, bool instant)
    {
        tab = i;
        for (int k = 0; k < tabs.Length; k++)
        {
            tabLabels[k].color = k == i ? Palette.Text : Palette.TextDim;
            pages[k].gameObject.SetActive(k == i);
        }
        var target = new Vector2(64 + i * 236, -254);
        if (instant || !Application.isPlaying) { tabIndicator.anchoredPosition = target; return; }
        Tween.Move(tabIndicator, target, 0.28f, Ease.OutCubic);
        SlideIn(pages[i], new Vector2(64, -284), new Vector2(0, -24), 0f, 0.3f);
        var first = FirstControl(i);
        if (first) MenuManager.Focus(first.First);
    }

    SettingControl FirstControl(int page)
    {
        foreach (var c in controls) if (c.transform.parent == pages[page]) return c;
        return null;
    }

    void ShowInfo(SettingControl c)
    {
        infoTitle.text = c.title;
        infoBody.text = c.description;
    }

    void SetDirty(bool d)
    {
        dirty = d;
        if (dirtyLabel.gameObject.activeSelf == d) return;
        dirtyLabel.gameObject.SetActive(d);
        if (d) PopIn(dirtyLabel, 0, 0.8f);
    }

    void Apply()
    {
        foreach (var c in controls) GameData.Settings[c.key] = c.Value;
        QualitySettings.vSyncCount = Get("vsync") > 0.5f ? 1 : 0;
        AudioListener.volume = Get("master") / 100f;
        SetDirty(false);
        menu.Toast("Settings applied", Palette.Green);
    }

    static float Get(string key) => GameData.Settings.TryGetValue(key, out var v) ? v : 0;

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        // load the saved values (or defaults) every time the screen opens
        foreach (var c in controls)
            c.Set(GameData.Settings.TryGetValue(c.key, out var v) ? v : c.defaultValue, false, false);
        SetDirty(false);
        ShowTab(0, true);
        ShowInfo(controls[0]);
        firstSelected = controls[0].First;
        kickerLabel.text = menu.race.InRace ? "OPTIONS  ·  RACE PAUSED" : "OPTIONS";
    }

    public override void OnBack()
    {
        if (!dirty) { menu.Back(); return; }
        menu.Confirm("DISCARD CHANGES?", "You changed some settings but didn't apply them.", "DISCARD", () => menu.Back(), true);
    }
}
