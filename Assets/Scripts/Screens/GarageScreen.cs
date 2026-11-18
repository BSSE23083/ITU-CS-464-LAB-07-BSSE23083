using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 05 - Garage / car select. Reached from the main menu (browse) or from track select (race setup).
public class GarageScreen : UIScreen
{
    public bool raceSetup;
    public Image car;
    public TMP_Text carName, maker, classChip, statusChip, priceLabel;
    public Image[] statFills, dots;
    public TMP_Text[] statValues;
    public Button prev, next, mainButton, dealerButton;

    int index;
    static readonly string[] StatNames = { "TOP SPEED", "ACCELERATION", "HANDLING", "BRAKING" };

    public override void Build()
    {
        BuildHeader("GARAGE", "YOUR COLLECTION");

        carName = UIKit.Heading(transform, "CarName", "VANTA S7", 100, Palette.Text, TextAlignmentOptions.Left, 4);
        carName.RT().At(0, 1, 62, -176, 900, 104);
        maker = UIKit.Text(transform, "Maker", "Vanta Automotive", 28, Palette.TextDim);
        maker.RT().At(0, 1, 66, -280, 600, 36);
        classChip = UIKit.Chip(transform, "Class", "CLASS B", Palette.Cyan, 150, 44, 22);
        classChip.transform.parent.RT().At(0, 1, 66, -332, 150, 44);
        statusChip = UIKit.Chip(transform, "Status", "OWNED", Palette.Green, 200, 44, 22);
        statusChip.transform.parent.RT().At(0, 1, 232, -332, 200, 44);

        car = UIKit.Img(transform, "Car", Color.white, UIKit.T.Icon("car"));
        car.RT().At(.5f, .5f, 200, 30, 380, 380);

        prev = Arrow("Prev", "chev_left", -430);
        next = Arrow("Next", "chev_right", 830);

        dots = new Image[GameData.Cars.Length];
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i] = UIKit.Box(transform, "Dot" + i, Palette.Line, 3f);
            dots[i].RT().At(.5f, .5f, 200 + (i - 1.5f) * 44, -200, 30, 10);
        }

        // stats
        var panel = UIKit.Box(transform, "Stats", Palette.Panel).RT().At(0, 0, 64, 118, 860, 250);
        UIKit.Outline(panel, "Border", Palette.Line.A(.5f));
        statFills = new Image[4];
        statValues = new TMP_Text[4];
        for (int i = 0; i < 4; i++)
        {
            float y = -34 - i * 54;
            UIKit.Text(panel, StatNames[i], StatNames[i], 22, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4)
                .RT().At(0, 1, 32, y, 220, 30);
            var track = UIKit.Box(panel, StatNames[i] + "Track", Palette.Raised, 4f).RT().At(0, 1, 262, y - 8, 460, 14);
            statFills[i] = UIKit.Img(track, "Fill", Palette.Accent, UIKit.T.round);
            statFills[i].pixelsPerUnitMultiplier = 4f;
            statFills[i].type = Image.Type.Filled;
            statFills[i].fillMethod = Image.FillMethod.Horizontal;
            statFills[i].RT().Stretch();
            statValues[i] = UIKit.Heading(panel, StatNames[i] + "Value", "0.0", 32, Palette.Text, TextAlignmentOptions.Right);
            statValues[i].RT().At(1, 1, -28, y + 4, 90, 38);
        }

        priceLabel = UIKit.Text(transform, "Price", "", 26, Palette.Gold, UIKit.T.bodyBold, TextAlignmentOptions.Right, 2);
        priceLabel.RT().At(1, 0, -64, 222, 700, 36);
        mainButton = UIKit.Button(transform, "MainAction", "START RACE", ButtonStyle.Primary, "flag", 36);
        mainButton.RT().At(1, 0, -64, 118, 460, 92);
        dealerButton = UIKit.Button(transform, "Dealership", "DEALERSHIP", ButtonStyle.Secondary, "tag", 30);
        dealerButton.RT().At(1, 0, -548, 118, 300, 92);

        BuildFooter(Key("A / D") + "  SWITCH CAR      " + Key("ENTER") + "  CONFIRM      " + Key("ESC") + "  BACK");
        firstSelected = mainButton;
        index = GameData.CarIndex;
        Refresh(true, 0);
    }

    Button Arrow(string name, string icon, float x)
    {
        var b = UIKit.Button(transform, name, "", ButtonStyle.Secondary);
        b.RT().At(.5f, .5f, x, 40, 96, 96);
        var mb = b.GetComponent<MenuButton>();
        mb.hotScale = 1.1f;
        mb.icon = UIKit.Img(b.transform, "Icon", Palette.Text, UIKit.T.Icon(icon));
        mb.icon.RT().At(.5f, .5f, 0, 0, 56, 56);
        mb.Apply(true);
        return b;
    }

    protected override void Bind()
    {
        prev.onClick.AddListener(() => Step(-1));
        next.onClick.AddListener(() => Step(1));
        dealerButton.onClick.AddListener(() => menu.Push(menu.shop));
        mainButton.onClick.AddListener(MainAction);
    }

    void Update()
    {
        if (menu == null || menu.Busy || menu.Current != this) return;
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.aKey.wasPressedThisFrame || kb.qKey.wasPressedThisFrame) Step(-1);
        if (kb.dKey.wasPressedThisFrame || kb.eKey.wasPressedThisFrame) Step(1);
    }

    void Step(int dir)
    {
        index = (index + dir + GameData.Cars.Length) % GameData.Cars.Length;
        var r = car.RT();
        // slide the car out and the next one in
        Tween.Move(r, new Vector2(200 - dir * 260, 30), 0.14f, Ease.InCubic, () =>
        {
            Refresh(false, dir);
            r.anchoredPosition = new Vector2(200 + dir * 320, 30);
            Tween.Move(r, new Vector2(200, 30), 0.42f, Ease.OutCubic);
            Tween.Run(car, "alpha", 0.25f, Ease.OutQuad, k => car.canvasRenderer.SetAlpha(k));
        });
        Tween.Run(car, "alpha", 0.14f, Ease.InQuad, k => car.canvasRenderer.SetAlpha(1 - k));
    }

    void MainAction()
    {
        var c = GameData.Cars[index];
        if (!c.owned) { menu.Push(menu.shop); return; }
        GameData.CarIndex = index;
        if (raceSetup) menu.Push(menu.race);
        else
        {
            menu.Toast(c.name + " is now your active car", Palette.Green);
            Refresh(false, 0);
        }
    }

    void Refresh(bool instant, int dir)
    {
        var c = GameData.Cars[index];
        var theme = UIKit.T;
        car.color = GameData.CarColor(index);
        carName.text = c.name;
        maker.text = c.maker;
        classChip.text = "CLASS " + c.carClass;
        bool active = index == GameData.CarIndex;
        statusChip.text = !c.owned ? "NOT OWNED" : active ? "ACTIVE" : "OWNED";
        statusChip.color = !c.owned ? Palette.Gold : Palette.Green;
        statusChip.transform.parent.GetComponent<Image>().color = UIKit.ChipFill(statusChip.color);
        statusChip.transform.parent.GetComponent<Outline>().effectColor = statusChip.color.A(.8f);
        priceLabel.text = c.owned ? "" : "PRICE  " + GameData.Money(c.price) + " CR";

        float[] values = { c.speed, c.accel, c.handling, c.braking };
        for (int i = 0; i < 4; i++)
        {
            statValues[i].text = (values[i] * 10).ToString("0.0");
            if (instant || !Application.isPlaying) statFills[i].fillAmount = values[i];
            else Tween.FillTo(statFills[i], values[i], 0.5f, Ease.OutCubic, i * 0.05f);
        }
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].color = i == index ? Palette.Accent : Palette.Line;
            dots[i].RT().sizeDelta = new Vector2(i == index ? 44 : 30, 10);
        }

        var mb = mainButton.GetComponent<MenuButton>();
        mainButton.interactable = true;
        if (!c.owned) { mb.label.text = "BUY IN DEALERSHIP"; mb.icon.sprite = theme.Icon("tag"); }
        else if (raceSetup) { mb.label.text = "START RACE"; mb.icon.sprite = theme.Icon("flag"); }
        else if (active) { mb.label.text = "ACTIVE CAR"; mb.icon.sprite = theme.Icon("check"); mainButton.interactable = false; }
        else { mb.label.text = "SET AS ACTIVE"; mb.icon.sprite = theme.Icon("check"); }
        if (!mainButton.interactable && Application.isPlaying) MenuManager.Focus(next);
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        titleLabel.text = raceSetup ? "CHOOSE YOUR CAR" : "GARAGE";
        kickerLabel.text = raceSetup
            ? "RACE SETUP  ·  STEP 3 OF 3  ·  " + GameData.Tracks[GameData.TrackIndex].name
            : "YOUR COLLECTION";
        if (!returning) index = GameData.CarIndex;
        car.canvasRenderer.SetAlpha(1);
        foreach (var f in statFills) f.fillAmount = 0;
        Refresh(false, 0);
        firstSelected = mainButton.interactable ? (Selectable)mainButton : next;
    }
}
