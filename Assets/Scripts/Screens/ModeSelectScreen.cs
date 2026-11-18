using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 03 - Mode select
public class ModeSelectScreen : UIScreen
{
    public Button[] cards;

    public override void Build()
    {
        BuildHeader("SELECT MODE", "RACE SETUP  ·  STEP 1 OF 3");
        var modes = GameData.Modes;
        cards = new Button[modes.Length];
        for (int i = 0; i < modes.Length; i++)
        {
            var m = modes[i];
            bool locked = m.unlockLevel > GameData.Level;
            var b = UIKit.Card(transform, "Mode_" + i);
            var r = b.RT().At(0, 1, 112 + i * 432, -206, 400, 640);
            Color main = locked ? Palette.TextMute : Palette.Accent;

            var strip = UIKit.Img(r, "Strip", locked ? Palette.Line.A(.4f) : Palette.Accent);
            strip.RT().Band(1, 0, 0, 0, 8);
            var disc = UIKit.Img(r, "Disc", Palette.Hex("0E1420"), UIKit.T.circle);
            disc.RT().At(.5f, 1, 0, -56, 136, 136);
            var ic = UIKit.Img(disc.transform, "Icon", main, UIKit.T.Icon(m.icon));
            ic.RT().At(.5f, .5f, 0, 0, 72, 72);

            var name = UIKit.Heading(r, "Name", m.name, 54, Palette.Text, TextAlignmentOptions.Center, 4);
            name.RT().At(.5f, 1, 0, -214, 380, 60);
            b.GetComponent<MenuButton>().label = name;
            var desc = UIKit.Text(r, "Desc", m.description, 26, locked ? Palette.TextMute : Palette.TextDim, null, TextAlignmentOptions.Top);
            desc.alignment = TextAlignmentOptions.Center;
            desc.RT().At(.5f, 1, 0, -288, 330, 120);

            var chip = UIKit.Chip(r, "Details", m.details, locked ? Palette.TextMute : Palette.Cyan, 290, 40, 20);
            chip.transform.parent.RT().At(.5f, 1, 0, -430, 290, 40);

            var line = UIKit.Img(r, "Divider", Palette.Line.A(.5f));
            line.RT().Band(0, 32, 32, 122, 2);

            if (locked)
            {
                var lk = UIKit.Img(r, "Lock", Palette.TextDim, UIKit.T.Icon("lock"));
                lk.RT().At(.5f, 0, -140, 62, 36, 36);
                var req = UIKit.Heading(r, "Req", "LOCKED  ·  LV " + m.unlockLevel, 30, Palette.TextDim, TextAlignmentOptions.Left, 3);
                req.RT().At(.5f, 0, 26, 60, 260, 40);
                b.interactable = false;
            }
            else
            {
                UIKit.Text(r, "UpTo", "UP TO", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Right, 4)
                    .RT().At(.5f, 0, -114, 64, 80, 30);
                var coin = UIKit.Img(r, "Coin", Palette.Gold, UIKit.T.Icon("coin"));
                coin.RT().At(.5f, 0, -48, 62, 32, 32);
                UIKit.Heading(r, "Reward", GameData.Money(m.reward) + " CR", 36, Palette.Gold, TextAlignmentOptions.Left, 1)
                    .RT().At(.5f, 0, 78, 58, 200, 44);
            }
            b.GetComponent<MenuButton>().Apply(true);
            cards[i] = b;
        }
        BuildFooter(Key("← →") + "  BROWSE      " + Key("ENTER") + "  SELECT      " + Key("ESC") + "  BACK");
        firstSelected = cards[GameData.ModeIndex];
    }

    protected override void Bind()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int idx = i;
            cards[i].onClick.AddListener(() =>
            {
                GameData.ModeIndex = idx;
                menu.Push(menu.trackSelect);
            });
            if (!cards[i].interactable)
            {
                // clicking a locked card shakes it and shows why
                var et = cards[i].gameObject.AddComponent<EventTrigger>();
                var e = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                e.callback.AddListener(_ =>
                {
                    Tween.Shake(cards[idx].RT());
                    menu.Toast("Reach level " + GameData.Modes[idx].unlockLevel + " to unlock " + GameData.Modes[idx].name, Palette.Red);
                });
                et.triggers.Add(e);
            }
        }
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        firstSelected = cards[GameData.ModeIndex];
    }
}
