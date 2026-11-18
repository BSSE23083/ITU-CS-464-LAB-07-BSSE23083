using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 02 - Main menu
public class MainMenuScreen : UIScreen
{
    public Button raceTile, garageTile, shopTile, settingsTile, creditsTile, quitButton;
    public Image raceCar;
    public TMP_Text raceCarLabel, profileLevel, profileXp, garageSub;
    public Image xpFill;

    public override void Build()
    {
        BuildHeader("NIGHTLINE <color=#FF6A1A>GT</color>", "WELCOME BACK, " + GameData.PlayerName, false);

        // hero tile
        raceTile = UIKit.Card(transform, "RaceTile");
        var rt = raceTile.RT().At(0, 1, 64, -190, 880, 560);
        raceTile.GetComponent<MenuButton>().hotScale = 1.015f;
        var bar = UIKit.Img(rt, "AccentBar", Palette.Accent);
        bar.RT().At(0, .5f, 0, 0, 10, 560);
        raceCar = UIKit.Img(rt, "Car", GameData.CarColor(1), UIKit.T.Icon("car"));
        raceCar.RT().At(1, 0, -60, 60, 300, 300);
        var label = UIKit.Heading(rt, "Title", "RACE", 130, Palette.Text, TextAlignmentOptions.Left, 6);
        label.RT().At(0, 1, 48, -30, 600, 140);
        raceTile.GetComponent<MenuButton>().label = label;
        var sub = UIKit.Text(rt, "Sub", "Circuit  ·  Sprint  ·  Drift Trial", 30, Palette.TextDim);
        sub.RT().At(0, 1, 52, -168, 700, 40);
        raceCarLabel = UIKit.Chip(rt, "ActiveCar", "ACTIVE CAR: VANTA S7", Palette.Cyan, 360, 40, 20);
        raceCarLabel.transform.parent.RT().At(0, 0, 52, 40, 360, 40);
        var start = UIKit.Heading(rt, "Go", "START  <size=26><color=#A6B0C4>›</color></size>", 30, Palette.Accent, TextAlignmentOptions.Right, 6);
        start.RT().At(1, 1, -40, -46, 300, 40);

        // supporting tiles
        garageTile = Tile("GarageTile", "car", "GARAGE", "4 cars  ·  2 owned", 976, -190, out garageSub);
        shopTile = Tile("ShopTile", "tag", "DEALERSHIP", "New arrival: Spectre X", 1428, -190, out _);
        settingsTile = Tile("SettingsTile", "gear", "SETTINGS", "Video, audio, controls", 976, -482, out _);
        creditsTile = Tile("CreditsTile", "star", "CREDITS", "Who made this", 1428, -482, out _);

        // profile
        var prof = UIKit.Box(transform, "Profile", Palette.Panel).RT().At(0, 1, 64, -774, 880, 150);
        UIKit.Outline(prof, "Border", Palette.Line.A(.6f));
        var av = UIKit.Img(prof, "Avatar", UIKit.ChipFill(Palette.Cyan), UIKit.T.circle);
        av.RT().At(0, .5f, 32, 0, 96, 96);
        UIKit.Outline(av.transform, "Ring", Palette.Cyan, true);
        var ini = UIKit.Heading(av.transform, "Initials", "NR", 40, Palette.Cyan, TextAlignmentOptions.Center);
        ini.RT().Stretch();
        var name = UIKit.Heading(prof, "Name", GameData.PlayerName, 42, Palette.Text, TextAlignmentOptions.Left, 3);
        name.RT().At(0, 1, 156, -22, 400, 50);
        profileLevel = UIKit.Text(prof, "Level", "LEVEL 9  ·  STREET RACER", 22, Palette.Cyan, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4);
        profileLevel.RT().At(0, 1, 158, -76, 500, 30);
        var track = UIKit.Box(prof, "XpTrack", Palette.Raised, 4f).RT().At(0, 0, 158, 26, 520, 14);
        xpFill = UIKit.Img(track, "Fill", Palette.Cyan, UIKit.T.round);
        xpFill.type = Image.Type.Filled;
        xpFill.fillMethod = Image.FillMethod.Horizontal;
        xpFill.fillAmount = GameData.LevelProgress;
        xpFill.RT().Stretch();
        profileXp = UIKit.Text(prof, "Xp", "4,400 / 8,000 XP", 22, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Right);
        profileXp.RT().At(1, 0, -32, 18, 240, 30);

        // next unlock (information only, not clickable)
        var next = UIKit.Box(transform, "NextUnlock", Palette.Panel).RT().At(0, 1, 976, -774, 428, 150);
        UIKit.Outline(next, "Border", Palette.Line.A(.35f));
        var lockI = UIKit.Img(next, "Lock", Palette.TextDim, UIKit.T.Icon("lock"));
        lockI.RT().At(0, .5f, 28, 0, 44, 44);
        UIKit.Text(next, "Kicker", "NEXT UNLOCK", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4)
            .RT().At(0, 1, 94, -26, 300, 28);
        UIKit.Heading(next, "Name", "TIME ATTACK", 38, Palette.Text).RT().At(0, 1, 94, -52, 320, 44);
        UIKit.Text(next, "Req", "Reach level 12", 22, Palette.TextDim).RT().At(0, 1, 94, -98, 300, 30);

        quitButton = UIKit.Button(transform, "QuitButton", "QUIT GAME", ButtonStyle.Danger, "power", 32);
        quitButton.RT().At(0, 1, 1428, -774, 428, 150);

        BuildFooter(Key("↑ ↓ ← →") + "  BROWSE      " + Key("ENTER") + "  SELECT      " + Key("ESC") + "  TITLE SCREEN");

        firstSelected = raceTile;
    }

    Button Tile(string name, string icon, string title, string desc, float x, float y, out TMP_Text subLabel)
    {
        var b = UIKit.Card(transform, name);
        var r = b.RT().At(0, 1, x, y, 428, 268);
        var ic = UIKit.Img(r, "Icon", Palette.Accent, UIKit.T.Icon(icon));
        ic.RT().At(0, 1, 36, -36, 64, 64);
        var t = UIKit.Heading(r, "Title", title, 46, Palette.Text, TextAlignmentOptions.Left, 4);
        t.RT().At(0, 0, 36, 76, 360, 54);
        b.GetComponent<MenuButton>().label = t;
        subLabel = UIKit.Text(r, "Sub", desc, 24, Palette.TextDim);
        subLabel.RT().At(0, 0, 38, 36, 360, 34);
        var chev = UIKit.Img(r, "Chevron", Palette.TextMute, UIKit.T.Icon("chev_right"));
        chev.RT().At(1, 1, -32, -44, 32, 32);
        return b;
    }

    protected override void Bind()
    {
        raceTile.onClick.AddListener(() => menu.Push(menu.modeSelect));
        garageTile.onClick.AddListener(() => { menu.garage.raceSetup = false; menu.Push(menu.garage); });
        shopTile.onClick.AddListener(() => menu.Push(menu.shop));
        settingsTile.onClick.AddListener(() => menu.Push(menu.settings));
        creditsTile.onClick.AddListener(() => menu.Push(menu.credits));
        quitButton.onClick.AddListener(() =>
            menu.Confirm("QUIT GAME?", "Your progress is saved. See you on the streets.", "QUIT", menu.QuitGame, true));
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        var car = GameData.Cars[GameData.CarIndex];
        raceCar.color = GameData.CarColor(GameData.CarIndex);
        raceCarLabel.text = "ACTIVE CAR: " + car.name;
        int owned = 0;
        foreach (var c in GameData.Cars) if (c.owned) owned++;
        garageSub.text = GameData.Cars.Length + " cars  ·  " + owned + " owned";
        xpFill.fillAmount = GameData.LevelProgress;
    }
}
