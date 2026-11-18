using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Short confirmation / warning message that pops up from the bottom and leaves on its own.
public class Toast : MonoBehaviour
{
    public CanvasGroup group;
    public RectTransform box;
    public Image accent;
    public TMP_Text text;

    public void Build()
    {
        ((RectTransform)transform).Stretch();
        box = UIKit.Box(transform, "Box", Palette.Hex("1A2335")).RT().At(.5f, 0, 0, 120, 720, 72);
        group = UIKit.Group(box.gameObject);
        UIKit.Outline(box, "Border", Palette.Line);
        accent = UIKit.Img(box, "Accent", Palette.Green);
        accent.RT().At(0, .5f, 0, 0, 8, 72);
        text = UIKit.Text(box, "Text", "Message", 28, Palette.Text, UIKit.T.bodyBold, TextAlignmentOptions.Center, 1);
        text.RT().Stretch(24, 0, 16, 0);
    }

    public void Show(string message, Color color)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        text.text = message;
        accent.color = color;
        group.alpha = 0;
        box.anchoredPosition = new Vector2(0, 70);
        Tween.Move(box, new Vector2(0, 120), 0.35f, Ease.OutBack);
        Tween.Fade(group, 1, 0.2f, Ease.OutQuad, () =>
            Tween.Fade(group, 0, 0.3f, Ease.InQuad, () => gameObject.SetActive(false), 1.8f));
    }
}
