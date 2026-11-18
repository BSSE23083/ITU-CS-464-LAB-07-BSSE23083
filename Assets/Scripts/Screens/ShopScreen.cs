using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 06 - Dealership (shop)
public class ShopScreen : UIScreen
{
    public RectTransform[] cards;
    public Button[] buyButtons;
    public TMP_Text[] priceLabels, needLabels;
    public RectTransform[] stamps;

    public override void Build()
    {
        BuildHeader("DEALERSHIP", "SPEND YOUR CREDITS");
        var items = GameData.Shop;
        int n = items.Length;
        cards = new RectTransform[n];
        buyButtons = new Button[n];
        priceLabels = new TMP_Text[n];
        needLabels = new TMP_Text[n];
        stamps = new RectTransform[n];

        for (int i = 0; i < n; i++)
        {
            var it = items[i];
            var r = UIKit.Box(transform, "Item_" + i, Palette.Panel).RT().At(0, 1, 96 + (i % 3) * 584, -196 - (i / 3) * 364, 560, 340);
            UIKit.Outline(r, "Border", Palette.Line.A(.55f));
            cards[i] = r;

            var art = UIKit.Box(r, "ArtBg", Palette.Raised.A(.6f)).RT();
            art.Band(1, 12, 12, -12, 150);
            if (it.carIndex >= 0)
            {
                var img = UIKit.Img(art, "Car", GameData.CarColor(it.carIndex), UIKit.T.Icon("car"));
                img.RT().At(.5f, .5f, 0, 0, 120, 120);
                UIKit.Chip(art, "Type", "CAR", Palette.Cyan, 80, 32, 18).transform.parent.RT().At(0, 1, 14, -14, 80, 32);
            }
            else
            {
                var img = UIKit.Img(art, "Icon", Palette.Accent, UIKit.T.Icon(it.sprite));
                img.RT().At(.5f, .5f, 0, 0, 84, 84);
                UIKit.Chip(art, "Type", "UPGRADE", Palette.Accent, 130, 32, 18).transform.parent.RT().At(0, 1, 14, -14, 130, 32);
            }

            UIKit.Heading(r, "Name", it.name, 42, Palette.Text, TextAlignmentOptions.Left, 3).RT().At(0, 1, 28, -172, 500, 48);
            var d = UIKit.Text(r, "Desc", it.description, 24, Palette.TextDim);
            d.RT().At(0, 1, 28, -214, 510, 34);

            var coin = UIKit.Img(r, "Coin", Palette.Gold, UIKit.T.Icon("coin"));
            coin.RT().At(0, 0, 28, 46, 32, 32);
            priceLabels[i] = UIKit.Heading(r, "Price", GameData.Money(it.price), 40, Palette.Gold, TextAlignmentOptions.Left, 1);
            priceLabels[i].RT().At(0, 0, 70, 38, 220, 48);
            needLabels[i] = UIKit.Text(r, "Need", "", 20, Palette.Red, UIKit.T.bodyBold, TextAlignmentOptions.Left, 2);
            needLabels[i].RT().At(0, 0, 30, 14, 280, 26);

            buyButtons[i] = UIKit.Button(r, "Buy", "BUY", ButtonStyle.Primary, null, 32);
            buyButtons[i].RT().At(1, 0, -24, 24, 200, 68);

            var stamp = UIKit.Box(r, "OwnedStamp", UIKit.ChipFill(Palette.Green), 2f).RT().At(1, 1, -24, -24, 150, 42);
            UIKit.Outline(stamp, "Border", Palette.Green);
            var chk = UIKit.Img(stamp, "Check", Palette.Green, UIKit.T.Icon("check"));
            chk.RT().At(0, .5f, 12, 0, 24, 24);
            UIKit.Heading(stamp, "Text", "OWNED", 26, Palette.Green, TextAlignmentOptions.Center, 3).RT().Stretch(30, 0, 0, 0);
            stamps[i] = stamp;
        }

        BuildFooter(Key("ARROWS") + "  BROWSE      " + Key("ENTER") + "  BUY      " + Key("ESC") + "  BACK");
        RefreshCards();
        firstSelected = buyButtons[2];
    }

    protected override void Bind()
    {
        for (int i = 0; i < buyButtons.Length; i++)
        {
            int idx = i;
            buyButtons[i].onClick.AddListener(() => AskBuy(idx));
        }
    }

    void AskBuy(int i)
    {
        var it = GameData.Shop[i];
        menu.Confirm("BUY " + it.name + "?",
            GameData.Money(it.price) + " CR will be taken from your balance of " + GameData.Money(GameData.Credits) + " CR.",
            "BUY", () => Buy(i));
    }

    void Buy(int i)
    {
        var it = GameData.Shop[i];
        if (it.owned || GameData.Credits < it.price) return;
        int before = GameData.Credits;
        GameData.Credits -= it.price;
        it.owned = true;
        if (it.carIndex >= 0) GameData.Cars[it.carIndex].owned = true;

        // count the credits down and show the OWNED stamp
        int after = GameData.Credits;
        Tween.Run(creditsLabel, "count", 0.8f, Ease.OutCubic, t =>
            creditsLabel.text = GameData.Money(Mathf.RoundToInt(Mathf.Lerp(before, after, t))) + " CR");
        RefreshCards();
        var s = stamps[i];
        s.gameObject.SetActive(true);
        s.localScale = Vector3.one * 1.8f;
        Tween.Scale(s, 1, 0.45f, Ease.OutBack);
        menu.Toast(it.name + " purchased", Palette.Green);
        MenuManager.Focus(buyButtons[(i + 1) % buyButtons.Length]);
    }

    void RefreshCards()
    {
        for (int i = 0; i < GameData.Shop.Length; i++)
        {
            var it = GameData.Shop[i];
            bool afford = GameData.Credits >= it.price;
            var mb = buyButtons[i].GetComponent<MenuButton>();
            stamps[i].gameObject.SetActive(it.owned);
            buyButtons[i].interactable = !it.owned && afford;
            mb.label.text = it.owned ? "OWNED" : "BUY";
            priceLabels[i].color = it.owned ? Palette.TextMute : afford ? Palette.Gold : Palette.Red;
            needLabels[i].text = it.owned || afford ? "" : "NEED " + GameData.Money(it.price - GameData.Credits) + " MORE";
            mb.Apply(!Application.isPlaying);
        }
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        RefreshCards();
        firstSelected = backButton;
        foreach (var b in buyButtons) if (b.interactable) { firstSelected = b; break; }
    }
}
