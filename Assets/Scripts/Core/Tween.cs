using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Ease { Linear, InQuad, OutQuad, InOutQuad, InCubic, OutCubic, InOutCubic, OutQuart, OutBack, InOutSine, OutElastic }

// Simple tween system (so I don't need DOTween).
// Uses unscaled time so the menus still animate when the game is paused.
public class Tween : MonoBehaviour
{
    class Job
    {
        public string key;
        public UnityEngine.Object target;
        public float delay, duration, time;
        public Ease ease;
        public Action<float> step;
        public Action done;
        public bool loop;
    }

    static Tween runner;
    readonly List<Job> jobs = new List<Job>();

    static Tween Runner
    {
        get
        {
            if (runner == null)
            {
                runner = new GameObject("[Tween]").AddComponent<Tween>();
                if (Application.isPlaying) DontDestroyOnLoad(runner.gameObject);
            }
            return runner;
        }
    }

    public static float Evaluate(Ease e, float t)
    {
        switch (e)
        {
            case Ease.InQuad: return t * t;
            case Ease.OutQuad: return 1 - (1 - t) * (1 - t);
            case Ease.InOutQuad: return t < .5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
            case Ease.InCubic: return t * t * t;
            case Ease.OutCubic: return 1 - Mathf.Pow(1 - t, 3);
            case Ease.InOutCubic: return t < .5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
            case Ease.OutQuart: return 1 - Mathf.Pow(1 - t, 4);
            case Ease.OutBack:
                const float c1 = 1.70158f, c3 = c1 + 1;
                return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
            case Ease.InOutSine: return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
            case Ease.OutElastic:
                if (t <= 0 || t >= 1) return t;
                return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * (2 * Mathf.PI / 3)) + 1;
            default: return t;
        }
    }

    // Core call. A new tween with the same target+channel replaces the old one.
    public static void Run(UnityEngine.Object target, string channel, float duration, Ease ease, Action<float> step,
        Action done = null, float delay = 0f, bool loop = false)
    {
        string key = target.GetInstanceID() + channel;
        Kill(key);
        Runner.jobs.Add(new Job { key = key, target = target, duration = Mathf.Max(0.0001f, duration), ease = ease, step = step, done = done, delay = delay, loop = loop });
    }

    public static void Kill(UnityEngine.Object target, string channel) => Kill(target.GetInstanceID() + channel);

    static void Kill(string key)
    {
        if (runner == null) return;
        runner.jobs.RemoveAll(j => j.key == key);
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        for (int i = jobs.Count - 1; i >= 0; i--)
        {
            if (i >= jobs.Count) continue;
            var j = jobs[i];
            if (j.target == null) { jobs.RemoveAt(i); continue; }
            if (j.delay > 0) { j.delay -= dt; if (j.delay > 0) continue; }
            j.time += dt;
            float p = Mathf.Clamp01(j.time / j.duration);
            j.step(Evaluate(j.ease, p));
            if (p < 1) continue;
            if (j.loop) { j.time = 0; continue; }
            jobs.Remove(j);
            j.done?.Invoke();
        }
    }

    // helpers
    public static void Fade(CanvasGroup g, float to, float dur, Ease e, Action done = null, float delay = 0)
    {
        float from = g.alpha;
        Run(g, "fade", dur, e, t => g.alpha = Mathf.LerpUnclamped(from, to, t), done, delay);
    }

    public static void Scale(Transform tr, float to, float dur, Ease e, Action done = null, float delay = 0)
    {
        Vector3 from = tr.localScale;
        Run(tr, "scale", dur, e, t => tr.localScale = Vector3.LerpUnclamped(from, Vector3.one * to, t), done, delay);
    }

    public static void Move(RectTransform r, Vector2 to, float dur, Ease e, Action done = null, float delay = 0)
    {
        Vector2 from = r.anchoredPosition;
        Run(r, "move", dur, e, t => r.anchoredPosition = Vector2.LerpUnclamped(from, to, t), done, delay);
    }

    public static void Tint(Graphic g, Color to, float dur, Ease e, Action done = null, float delay = 0)
    {
        Color from = g.color;
        Run(g, "tint", dur, e, t => g.color = Color.LerpUnclamped(from, to, t), done, delay);
    }

    public static void FillTo(Image img, float to, float dur, Ease e, float delay = 0)
    {
        float from = img.fillAmount;
        Run(img, "fill", dur, e, t => img.fillAmount = Mathf.LerpUnclamped(from, to, t), null, delay);
    }

    public static void Width(RectTransform r, float to, float dur, Ease e, float delay = 0)
    {
        float from = r.sizeDelta.x;
        Run(r, "width", dur, e, t => r.sizeDelta = new Vector2(Mathf.LerpUnclamped(from, to, t), r.sizeDelta.y), null, delay);
    }

    // Damped horizontal shake, used for "you can't do that" feedback.
    public static void Shake(RectTransform r, float strength = 14f, float dur = 0.4f)
    {
        Vector2 home = r.anchoredPosition;
        Run(r, "move", dur, Ease.Linear, t =>
        {
            float x = Mathf.Sin(t * Mathf.PI * 7) * strength * (1 - t);
            r.anchoredPosition = home + new Vector2(x, 0);
        }, () => r.anchoredPosition = home);
    }
}
