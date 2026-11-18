using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Base class for all screens.
// Build() makes the UI (from the editor), Bind() adds button listeners, OnShow() runs when the screen opens.
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    [HideInInspector] public MenuManager menu;
    public Button backButton;
    public TMP_Text titleLabel, kickerLabel, creditsLabel, levelLabel;
    public Selectable firstSelected;

    CanvasGroup group;
    public CanvasGroup Group => group != null ? group : (group = GetComponent<CanvasGroup>());

    protected UITheme Theme => menu != null ? menu.theme : UIKit.T;

    public void Init(MenuManager m)
    {
        menu = m;
        if (backButton) backButton.onClick.AddListener(OnBack);
        Bind();
    }

    public abstract void Build();
    protected virtual void Bind() { }

    public virtual void OnShow(bool returning) => RefreshWallet();
    public virtual void OnHide() { }
    public virtual void OnBack() => menu.Back();

    public void RefreshWallet()
    {
        if (creditsLabel) creditsLabel.text = GameData.Money(GameData.Credits) + " CR";
        if (levelLabel) levelLabel.text = "LV " + GameData.Level;
    }

    // fade + scale an item in (used for staggered animations)
    protected static void PopIn(Component c, float delay, float from = 0.9f)
    {
        var g = UIKit.Group(c.gameObject);
        g.alpha = 0;
        c.transform.localScale = Vector3.one * from;
        Tween.Fade(g, 1, 0.25f, Ease.OutQuad, null, delay);
        Tween.Scale(c.transform, 1, 0.4f, Ease.OutBack, null, delay);
    }

    protected static void SlideIn(RectTransform r, Vector2 home, Vector2 offset, float delay, float dur = 0.4f)
    {
        var g = UIKit.Group(r.gameObject);
        g.alpha = 0;
        r.anchoredPosition = home + offset;
        Tween.Fade(g, 1, dur * 0.7f, Ease.OutQuad, null, delay);
        Tween.Move(r, home, dur, Ease.OutCubic, null, delay);
    }

    // Same header on every inner screen: back button, step kicker, title, wallet.
    protected void BuildHeader(string title, string kicker, bool back = true, bool wallet = true)
    {
        float x = 64;
        if (back)
        {
            backButton = UIKit.Button(transform, "BackButton", "BACK", ButtonStyle.Secondary, "chev_left", 28);
            backButton.RT().At(0, 1, 64, -50, 176, 64);
            x = 272;
        }
        kickerLabel = UIKit.Text(transform, "Kicker", kicker, 22, Palette.Accent, UIKit.T.bodyBold, TextAlignmentOptions.Left, 8);
        kickerLabel.RT().At(0, 1, x, -42, 1000, 28);
        titleLabel = UIKit.Heading(transform, "Title", title, 60, Palette.Text, TextAlignmentOptions.Left, 4);
        titleLabel.RT().At(0, 1, x, -66, 1000, 66);

        if (wallet)
        {
            var pill = UIKit.Box(transform, "Wallet", Palette.Panel);
            pill.RT().At(1, 1, -64, -50, 290, 64);
            UIKit.Outline(pill.transform, "Border", Palette.Gold.A(.45f));
            var coin = UIKit.Img(pill.transform, "Coin", Palette.Gold, UIKit.T.Icon("coin"));
            coin.RT().At(0, .5f, 22, 0, 34, 34);
            creditsLabel = UIKit.Heading(pill.transform, "Credits", "0 CR", 32, Palette.Gold, TextAlignmentOptions.Right, 1);
            creditsLabel.RT().Stretch(64, 0, 22, 0);

            var lv = UIKit.Box(transform, "Level", Palette.Panel);
            lv.RT().At(1, 1, -370, -50, 120, 64);
            UIKit.Outline(lv.transform, "Border", Palette.Cyan.A(.45f));
            levelLabel = UIKit.Heading(lv.transform, "Text", "LV 0", 30, Palette.Cyan, TextAlignmentOptions.Center, 2);
            levelLabel.RT().Stretch();
        }

        var line = UIKit.Img(transform, "HeaderLine", Palette.Line.A(.55f));
        line.RT().Band(1, 64, 64, -146, 2);
        RefreshWallet();
    }

    // keyboard hints at the bottom of the screen
    protected void BuildFooter(string hints)
    {
        var line = UIKit.Img(transform, "FooterLine", Palette.Line.A(.35f));
        line.RT().Band(0, 64, 64, 78, 1);
        var t = UIKit.Text(transform, "Hints", hints, 22, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Left, 3);
        t.RT().Band(0, 64, 64, 26, 40);
        t.richText = true;
    }

    protected static string Key(string k) => "<color=#F3F5FA><mark=#3E4C6A55 padding=\"8,8,4,4\">" + k + "</mark></color>";
}
