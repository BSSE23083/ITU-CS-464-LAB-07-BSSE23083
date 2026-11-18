using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Switches between screens using a stack, so Back always returns to the previous screen.
// New screens slide in from the right, going back slides from the left.
public class MenuManager : MonoBehaviour
{
    public UITheme theme;
    public TitleScreen title;
    public MainMenuScreen main;
    public ModeSelectScreen modeSelect;
    public TrackSelectScreen trackSelect;
    public GarageScreen garage;
    public ShopScreen shop;
    public SettingsScreen settings;
    public RaceScreen race;
    public ResultsScreen results;
    public CreditsScreen credits;
    public ConfirmDialog dialog;
    public Toast toast;

    readonly List<UIScreen> stack = new List<UIScreen>();
    bool busy;

    public UIScreen Current => stack.Count > 0 ? stack[stack.Count - 1] : null;
    // also "busy" for a frame after a dialog closes, so the same click/key isn't handled twice
    public bool Busy => busy || dialog.IsOpen || Time.frameCount - dialog.ClosedFrame < 2;

    UIScreen[] All => new UIScreen[] { title, main, modeSelect, trackSelect, garage, shop, settings, race, results, credits };

    void Start()
    {
        UIKit.T = theme;
        foreach (var s in All)
        {
            s.Init(this);
            s.gameObject.SetActive(false);
        }
        dialog.Init();
        toast.gameObject.SetActive(false);
        Push(title);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.escapeKey.wasPressedThisFrame)
        {
            if (dialog.IsOpen) dialog.Close(false);
            else if (!Busy && Current != null) Current.OnBack();
        }
    }

    public void Push(UIScreen next)
    {
        if (busy || next == Current) return;
        var from = Current;
        stack.Add(next);
        Transition(from, next, 1, false);
    }

    public void Back()
    {
        if (busy || stack.Count <= 1) return;
        var from = Current;
        stack.RemoveAt(stack.Count - 1);
        Transition(from, Current, -1, true);
    }

    // Replace the whole history, e.g. after a race the "back" path is Main Menu, not the race.
    public void ResetTo(params UIScreen[] chain)
    {
        if (busy) return;
        var from = Current;
        stack.Clear();
        stack.AddRange(chain);
        Transition(from, Current, 1, false);
    }

    void Transition(UIScreen from, UIScreen to, int dir, bool returning)
    {
        busy = true;
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);

        if (from != null && from != to)
        {
            from.OnHide();
            var fg = from.Group;
            var fr = (RectTransform)from.transform;
            fg.interactable = false;
            fg.blocksRaycasts = false;
            Tween.Move(fr, new Vector2(-dir * 90, 0), 0.2f, Ease.InCubic);
            Tween.Fade(fg, 0, 0.2f, Ease.InCubic, () =>
            {
                from.gameObject.SetActive(false);
                fr.anchoredPosition = Vector2.zero;
            });
        }

        to.gameObject.SetActive(true);
        var tg = to.Group;
        var tr = (RectTransform)to.transform;
        tg.alpha = 0;
        tg.interactable = false;
        tg.blocksRaycasts = false;
        tr.anchoredPosition = new Vector2(dir * 140, 0);
        to.OnShow(returning);

        float delay = from != null ? 0.12f : 0f;
        Tween.Move(tr, Vector2.zero, 0.45f, Ease.OutCubic, null, delay);
        Tween.Fade(tg, 1, 0.32f, Ease.OutCubic, () =>
        {
            tg.interactable = true;
            tg.blocksRaycasts = true;
            busy = false;
            Focus(to.firstSelected);
        }, delay);
    }

    public static void Focus(Component c)
    {
        if (EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(c != null ? c.gameObject : null);
    }

    public void Toast(string message, Color color) => toast.Show(message, color);

    public void Confirm(string heading, string body, string okText, System.Action onOk, bool danger = false)
        => dialog.Show(heading, body, okText, onOk, danger);

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
