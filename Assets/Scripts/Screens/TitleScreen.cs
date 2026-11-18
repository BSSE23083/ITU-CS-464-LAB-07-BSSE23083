using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 01 - Title screen
public class TitleScreen : UIScreen
{
    public TMP_Text logo, prompt;
    public RectTransform logoBar, car;

    public override void Build()
    {
        logo = UIKit.Heading(transform, "Logo", "NIGHTLINE <color=#FF6A1A>GT</color>", 210, Palette.Text, TextAlignmentOptions.Center, 10);
        logo.RT().At(.5f, .5f, 0, 270, 1700, 230);
        logoBar = UIKit.Img(transform, "LogoBar", Palette.Accent).RT().At(.5f, .5f, 0, 160, 560, 8);
        var tag = UIKit.Text(transform, "Tagline", "STREET CIRCUIT CHAMPIONSHIP", 32, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Center, 26);
        tag.RT().At(.5f, .5f, 0, 112, 1400, 44);

        var carImg = UIKit.Img(transform, "Car", GameData.CarColor(1), UIKit.T.Icon("car"));
        car = carImg.RT().At(.5f, .5f, 0, -150, 300, 300);

        prompt = UIKit.Heading(transform, "Prompt", "PRESS ANY KEY TO START", 44, Palette.Text, TextAlignmentOptions.Center, 14);
        prompt.RT().At(.5f, 0, 0, 150, 1200, 56);
        var sub = UIKit.Text(transform, "PromptSub", "or click anywhere", 24, Palette.TextDim, null, TextAlignmentOptions.Center);
        sub.RT().At(.5f, 0, 0, 112, 600, 32);

        var ver = UIKit.Text(transform, "Version", "v1.0  ·  LAB 07 BUILD", 20, Palette.TextMute, UIKit.T.bodyBold, TextAlignmentOptions.Left, 4);
        ver.RT().At(0, 0, 64, 36, 600, 30);
        var quit = UIKit.Text(transform, "QuitHint", Key("ESC") + "  QUIT", 20, Palette.TextDim, UIKit.T.bodyBold, TextAlignmentOptions.Right, 4);
        quit.RT().At(1, 0, -64, 36, 400, 30);
    }

    public override void OnShow(bool returning)
    {
        base.OnShow(returning);
        // logo scales down into place
        logo.transform.localScale = Vector3.one * 1.15f;
        Tween.Scale(logo.transform, 1, 0.7f, Ease.OutCubic);
        logoBar.sizeDelta = new Vector2(0, 8);
        Tween.Width(logoBar, 560, 0.55f, Ease.OutCubic, 0.3f);
        // pulse the "press any key" text
        Tween.Run(prompt, "pulse", 1.6f, Ease.Linear, t => prompt.alpha = 0.35f + 0.65f * (0.5f + 0.5f * Mathf.Cos(t * Mathf.PI * 2)), null, 0, true);
    }

    public override void OnHide() => Tween.Kill(prompt, "pulse");

    void Update()
    {
        if (menu == null || menu.Busy || menu.Current != this) return;
        var kb = Keyboard.current;
        bool key = kb != null && kb.anyKey.wasPressedThisFrame && !kb.escapeKey.wasPressedThisFrame;
        bool click = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        if (key || click) menu.Push(menu.main);
    }

    public override void OnBack() =>
        menu.Confirm("QUIT GAME?", "You'll be returned to the desktop.", "QUIT", menu.QuitGame, true);
}
