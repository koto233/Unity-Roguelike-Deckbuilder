using UnityEngine;
using TMPro;
using DG.Tweening;

public static class NumberAnimator
{
    /// <summary>
    /// 播放数字动画，并自动处理旧动画的Kill
    /// </summary>
    public static Tween Play(TextMeshProUGUI text, float from, float to, float duration,
                             string format = "N0", Ease ease = Ease.OutQuad)
    {
        // 强制停止当前动画（防止多个动画同时修改同一个Text）
        text.DOKill();

        // 记录当前值（用于闭包）
        float value = from;

        Tween tween = DOTween.To(() => value, v =>
        {
            value = v;
            // 可以根据数值大小自动切换格式，例如大于10000时显示"1.0万"
            if (format == "Auto")
            {
                if (v >= 10000) text.text = (v / 10000f).ToString("F1") + "万";
                else text.text = Mathf.Floor(v).ToString("N0");
            }
            else
            {
                text.text = v.ToString(format);
            }
        }, to, duration)
        .SetEase(ease);

        return tween;
    }
}