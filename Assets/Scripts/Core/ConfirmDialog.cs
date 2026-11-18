using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Yes/No popup used for quitting and buying things.
public class ConfirmDialog : MonoBehaviour
{
    public CanvasGroup group;
    public RectTransform panel;
    public Image accentBar;
    public TMP_Text heading, body;
    public Button ok, cancel;

    Action onOk;
    GameObject previous;
    public bool IsOpen { get; private set; }
    public int ClosedFrame { get; private set; } = -10;

    public void Build()
    {
        group = UIKit.Group(gameObject);
        ((RectTransform)transform).Stretch();
        var dim = UIKit.Img(transform, "Dim", Palette.Dim, null, true);
        dim.RT().Stretch();

        panel = UIKit.Box(transform, "Panel", Palette.Hex("161E2E")).RT().At(.5f, .5f, 0, 0, 760, 380);
        UIKit.Outline(panel, "Border", Palette.Line);
        accentBar = UIKit.Img(panel, "Accent", Palette.Accent);
        accentBar.RT().Band(1, 0, 0, 0, 6);

        heading = UIKit.Heading(panel, "Heading", "ARE YOU SURE?", 46, Palette.Text, TextAlignmentOptions.Left, 3);
        heading.RT().Band(1, 48, 48, -44, 56);
        body = UIKit.Text(panel, "Body", "Body", 28, Palette.TextDim);
        body.RT().Band(1, 48, 48, -112, 110);

        cancel = UIKit.Button(panel, "Cancel", "CANCEL", ButtonStyle.Secondary);
        cancel.RT().At(1, 0, -366, 44, 300, 72);
        ok = UIKit.Button(panel, "Confirm", "CONFIRM", ButtonStyle.Primary);
        ok.RT().At(1, 0, -48, 44, 300, 72);
    }

    public void Init()
    {
        ok.onClick.AddListener(() => Close(true));
        cancel.onClick.AddListener(() => Close(false));
        gameObject.SetActive(false);
    }

    public void Show(string title, string text, string okText, Action confirmed, bool danger)
    {
        onOk = confirmed;
        heading.text = title;
        body.text = text;
        ok.GetComponent<MenuButton>().label.text = okText;
        var mb = ok.GetComponent<MenuButton>();
        mb.style = danger ? ButtonStyle.Danger : ButtonStyle.Primary;
        accentBar.color = danger ? Palette.Red : Palette.Accent;

        previous = EventSystem.current ? EventSystem.current.currentSelectedGameObject : null;
        IsOpen = true;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        mb.Apply(true);
        group.blocksRaycasts = true;
        group.alpha = 0;
        Tween.Fade(group, 1, 0.2f, Ease.OutQuad);
        panel.localScale = Vector3.one * 0.86f;
        Tween.Scale(panel, 1, 0.38f, Ease.OutBack);
        MenuManager.Focus(cancel); // the safe option is focused by default
    }

    public void Close(bool confirmed)
    {
        if (!IsOpen) return;
        IsOpen = false;
        ClosedFrame = Time.frameCount;
        group.blocksRaycasts = false;
        Tween.Scale(panel, 0.94f, 0.15f, Ease.InQuad);
        Tween.Fade(group, 0, 0.15f, Ease.InQuad, () => gameObject.SetActive(false));
        if (previous) MenuManager.Focus(previous.transform);
        if (confirmed) onOk?.Invoke();
    }
}
