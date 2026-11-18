using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 04 - Track select. Selecting a row shows its details on the right.
public class TrackSelectScreen : UIScreen
{
    public Button[] rows;
    public Image[] selectMarks;
    public Image map;
    public TMP_Text trackName, trackArea, lengthValue, turnsValue, bestValue, conditions, modeChip;
    public Image[] diffDots;
    public Button nextButton;
    public RectTransform preview;

    int shown = -1;

    public override void Build()
    {
        BuildHeader("SELECT TRACK", "RACE SETUP  ·  STEP 2 OF 3");
        var tracks = GameData.Tracks;
        rows = new Button[tracks.Length];
        selectMarks = new Image[tracks.Length];
        for (int i = 0; i < tracks.Length; i++)
        {
            var t = tracks[i];
            bool locked = t.unlockLevel > GameData.Level;
            var b = UIKit.Card(transform, "Track_" + i);
            var r = b.RT().At(0, 1, 64, -196 - i * 140, 680, 124);
            // cyan bar on the left of the selected row
            selectMarks[i] = UIKit.Img(r, "Selected", Palette.Cyan);
            selectMarks[i].RT().At(0, .5f, 0, 0, 12, 124);

            var name = UIKit.Heading(r, "Name", t.name, 42, Palette.Text, TextAlignmentOptions.Left, 3);
            name.RT().At(0, 1, 40, -18, 420, 50);
            b.GetComponent<MenuButton>().label = name;
            UIKit.Text(r, "Area", t.area, 24, Palette.TextDim).RT().At(0, 0, 42, 20, 400, 32);

            if (locked)
            {
                var lk = UIKit.Img(r, "Lock", Palette.TextDim, UIKit.T.Icon("lock"));
                lk.RT().At(1, .5f, -150, 0, 36, 36);
                UIKit.Heading(r, "Req", "LV " + t.unlockLevel, 32, Palette.TextDim, TextAlignmentOptions.Right)
                    .RT().At(1, .5f, -36, 0, 100, 40);
            }
            else
            {
                UIKit.Text(r, "Len", t.lengthKm.ToString("0.0") + " KM", 26, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Right, 2)
                    .RT().At(1, .5f, -36, 0, 160, 34);
            }
            rows[i] = b;
        }

        // detail panel
        var p = UIKit.Box(transform, "Preview", Palette.Panel).RT().At(0, 1, 776, -196, 1080, 640);
        preview = p;
        UIKit.Outline(p, "Border", Palette.Line.A(.6f));
        map = UIKit.Img(p, "Map", Palette.Cyan, UIKit.T.Icon("map"));
        map.RT().At(0, .5f, 110, 0, 340, 340);

        trackName = UIKit.Heading(p, "Name", "HARBOR LOOP", 66, Palette.Text, TextAlignmentOptions.Left, 3);
        trackName.RT().At(0, 1, 560, -40, 500, 72);
        trackArea = UIKit.Text(p, "Area", "Port District", 28, Palette.TextDim);
        trackArea.RT().At(0, 1, 562, -112, 480, 36);

        lengthValue = Stat(p, "LENGTH", 560, -186);
        turnsValue = Stat(p, "TURNS", 810, -186);
        bestValue = Stat(p, "BEST LAP", 810, -306);
        UIKit.Text(p, "DiffLabel", "DIFFICULTY", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4)
            .RT().At(0, 1, 562, -306, 220, 28);
        diffDots = new Image[4];
        for (int d = 0; d < 4; d++)
        {
            diffDots[d] = UIKit.Box(p, "Diff" + d, Palette.Accent, 3f);
            diffDots[d].RT().At(0, 1, 562 + d * 46, -350, 36, 24);
        }

        conditions = UIKit.Chip(p, "Conditions", "NIGHT  ·  DRY", Palette.TextDim, 220, 40, 20);
        conditions.transform.parent.RT().At(0, 0, 562, 48, 220, 40);
        modeChip = UIKit.Chip(p, "Mode", "CIRCUIT", Palette.Accent, 200, 40, 20);
        modeChip.transform.parent.RT().At(0, 0, 798, 48, 200, 40);

        nextButton = UIKit.Button(transform, "ChooseCar", "CHOOSE CAR", ButtonStyle.Primary, "car", 34);
        nextButton.RT().At(1, 1, -64, -866, 420, 84);

        BuildFooter(Key("↑ ↓") + "  PREVIEW      " + Key("ENTER") + "  CHOOSE      " + Key("ESC") + "  BACK");
        firstSelected = rows[0];
        Show(0, true);
    }

    TMP_Text Stat(Transform p, string label, float x, float y)
    {
        UIKit.Text(p, label + "Label", label, 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4)
            .RT().At(0, 1, x + 2, y, 220, 28);
        var v = UIKit.Heading(p, label + "Value", "-", 48, Palette.Text, TextAlignmentOptions.Left, 1);
        v.RT().At(0, 1, x, y - 28, 260, 56);
        return v;
    }

    protected override void Bind()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            int idx = i;
            var et = rows[i].gameObject.AddComponent<EventTrigger>();
            var sel = new EventTrigger.Entry { eventID = EventTriggerType.Select };
            sel.callback.AddListener(_ => Show(idx, false));
            et.triggers.Add(sel);
            rows[i].onClick.AddListener(() => Choose(idx));
        }
        nextButton.onClick.AddListener(() => Choose(shown));
    }

    bool Locked(int i) => GameData.Tracks[i].unlockLevel > GameData.Level;

    void Choose(int i)
    {
        if (Locked(i))
        {
            Tween.Shake(rows[i].RT());
            menu.Toast("Skyline Ring unlocks at level " + GameData.Tracks[i].unlockLevel, Palette.Red);
            return;
        }
        GameData.TrackIndex = i;
        menu.garage.raceSetup = true;
        menu.Push(menu.garage);
    }

    void Show(int i, bool instant)
    {
        if (i == shown && !instant) return;
        shown = i;
        var t = GameData.Tracks[i];
        bool locked = Locked(i);
        for (int k = 0; k < selectMarks.Length; k++)
        {
            var target = k == i ? 1f : 0f;
            foreach (var g in selectMarks[k].GetComponentsInChildren<Graphic>())
            {
                if (instant || !Application.isPlaying) g.canvasRenderer.SetAlpha(target);
                else g.CrossFadeAlpha(target, 0.15f, true);
            }
        }

        void Apply()
        {
            map.color = locked ? Palette.TextMute : Palette.Cyan;
            trackName.text = t.name;
            trackArea.text = t.area + (locked ? "  ·  Locked until level " + t.unlockLevel : "");
            lengthValue.text = t.lengthKm.ToString("0.0") + " KM";
            turnsValue.text = t.turns.ToString();
            bestValue.text = t.bestLap;
            conditions.text = t.conditions;
            for (int d = 0; d < diffDots.Length; d++)
                diffDots[d].color = d < t.difficulty ? Palette.Accent : Palette.Line.A(.7f);
            nextButton.interactable = !locked;
            nextButton.GetComponent<MenuButton>().label.text = locked ? "LOCKED" : "CHOOSE CAR";
        }

        if (instant || !Application.isPlaying) { Apply(); return; }
        // shrink the map icon, update the info, then pop it back
        Tween.Run(map, "swap", 0.1f, Ease.InQuad, k => { map.transform.localScale = Vector3.one * (1 - 0.1f * k); map.canvasRenderer.SetAlpha(1 - k); }, () =>
        {
            Apply();
            Tween.Run(map, "swap", 0.35f, Ease.OutBack, k => { map.transform.localScale = Vector3.one * (0.9f + 0.1f * k); map.canvasRenderer.SetAlpha(Mathf.Clamp01(k)); });
        });
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        kickerLabel.text = "RACE SETUP  ·  STEP 2 OF 3  ·  " + GameData.Modes[GameData.ModeIndex].name;
        modeChip.text = GameData.Modes[GameData.ModeIndex].name;
        shown = -1;
        Show(GameData.TrackIndex, true);
        firstSelected = rows[GameData.TrackIndex];
    }
}
