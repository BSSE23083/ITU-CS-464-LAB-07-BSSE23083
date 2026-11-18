using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 10 - Credits. Scrolls automatically, can also be scrolled with the mouse wheel.
public class CreditsScreen : UIScreen
{
    public ScrollRect scroll;
    public RectTransform content;
    public Button doneButton;

    float idle, lastPos;
    const float Speed = 60f;

    static readonly string[][] Sections =
    {
        new[] { "GAME & UI DESIGN", "Abdul Samad Sohail", "BSSE23083" },
        new[] { "PROGRAMMING", "Abdul Samad Sohail" },
        new[] { "ICONS", "Material Icons  -  Google  (Apache 2.0)" },
        new[] { "TYPEFACES", "Rajdhani  -  Indian Type Foundry  (SIL OFL 1.1)", "Barlow  -  Jeremy Tribby  (SIL OFL 1.1)" },
        new[] { "ENGINE & TOOLS", "Unity 6000.3  ·  uGUI  ·  TextMesh Pro  ·  Input System" },
        new[] { "INSPIRED BY", "Forza Horizon series  -  Playground Games", "Need for Speed series  -  Criterion Games / EA", "(layout ideas only, no assets used)" },
        new[] { "COURSE", "CS-464 Game Development  ·  Lab 07", "Information Technology University, Lahore" },
        new[] { "SPECIAL THANKS", "Course instructor and lab TA", "Friends who play-tested the menus" },
    };

    public override void Build()
    {
        BuildHeader("CREDITS", "THE PEOPLE BEHIND IT");

        var view = UIKit.Img(transform, "Viewport", new Color(0, 0, 0, 0), null, true);
        var vr = view.RT();
        vr.anchorMin = new Vector2(.5f, 0); vr.anchorMax = new Vector2(.5f, 1); vr.pivot = new Vector2(.5f, .5f);
        vr.sizeDelta = new Vector2(1200, -(170 + 200)); vr.anchoredPosition = new Vector2(0, (200 - 170) / 2f);
        view.gameObject.AddComponent<RectMask2D>().softness = new Vector2Int(0, 80);

        content = UIKit.New("Content", vr);
        content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1); content.pivot = new Vector2(.5f, 1);
        content.anchoredPosition = Vector2.zero;

        float y = -260;
        UIKit.Heading(content, "Logo", "NIGHTLINE <color=#FF6A1A>GT</color>", 110, Palette.Text, TextAlignmentOptions.Center, 8).RT().Band(1, 0, 0, y, 130);
        y -= 130;
        UIKit.Text(content, "Tagline", "A menu system prototype  ·  not a playable game", 28, Palette.TextDim, null, TextAlignmentOptions.Center).RT().Band(1, 0, 0, y, 40);
        y -= 140;
        foreach (var s in Sections)
        {
            UIKit.Text(content, s[0], s[0], 24, Palette.Accent, UIKit.T.bodyBold, TextAlignmentOptions.Center, 10).RT().Band(1, 0, 0, y, 34);
            y -= 44;
            for (int i = 1; i < s.Length; i++)
            {
                bool main = i == 1 || s[0] == "TYPEFACES" || s[0] == "INSPIRED BY" && i < 3;
                var t = main
                    ? UIKit.Heading(content, "Line", s[i], 44, Palette.Text, TextAlignmentOptions.Center, 2)
                    : UIKit.Text(content, "Line", s[i], 28, Palette.TextDim, null, TextAlignmentOptions.Center);
                t.RT().Band(1, 0, 0, y, main ? 52 : 38);
                y -= main ? 54 : 40;
            }
            y -= 70;
        }
        y -= 60;
        var thanks = UIKit.Heading(content, "Thanks", "THANKS FOR PLAYING", 72, Palette.Gold, TextAlignmentOptions.Center, 8);
        thanks.RT().Band(1, 0, 0, y, 90);
        y -= 400;
        content.sizeDelta = new Vector2(0, -y);

        scroll = view.gameObject.AddComponent<ScrollRect>();
        scroll.viewport = vr;
        scroll.content = content;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 40;
        scroll.inertia = true;

        doneButton = UIKit.Button(transform, "Done", "BACK TO MENU", ButtonStyle.Primary, "chev_left", 32);
        doneButton.RT().At(1, 0, -64, 104, 380, 80);
        BuildFooter(Key("MOUSE WHEEL") + "  SCROLL      " + Key("ESC") + "  BACK");
        firstSelected = doneButton;
    }

    protected override void Bind() => doneButton.onClick.AddListener(OnBack);

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        content.anchoredPosition = new Vector2(0, 0);
        lastPos = 0;
        idle = 0.6f; // short beat before the roll begins
    }

    void Update()
    {
        if (menu == null || menu.Current != this) return;
        float y = content.anchoredPosition.y;
        if (Mathf.Abs(y - lastPos) > 0.5f) idle = 3f; // player scrolled, hand them control for a moment
        idle -= Time.unscaledDeltaTime;
        if (idle <= 0)
        {
            float max = content.rect.height - ((RectTransform)scroll.transform).rect.height;
            y += Speed * Time.unscaledDeltaTime;
            if (y >= max) y = 0; // loop
            content.anchoredPosition = new Vector2(0, y);
        }
        lastPos = content.anchoredPosition.y;
    }
}
