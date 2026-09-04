using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class ToastView : UIWindow
{

    private float _fadeDuration = 0.3f;
    private float _moveDuration = 1.0f;
    private float _displayDuration = 1.5f;
    private float _moveOffset = 100f; // 向上偏移量
    private RectTransform _rectTransform;
    protected override void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        base.Awake();
    }
    public void SetMessage(string message)
    {

        b_Message.SetText(message);
    }

    // 可选：淡入淡出
    public void SetAlpha(float alpha)
    {
        if (b_CanvasGroup != null)
            b_CanvasGroup.alpha = alpha;
    }
    public void ResetState()
    {
        if (b_CanvasGroup != null)
        {
            b_CanvasGroup.alpha = 0f;
            b_CanvasGroup.blocksRaycasts = false;
        }

        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public UniTask PlayShowAnimation()
    {
        var sequence = DOTween.Sequence();

        // 淡入
        sequence.Join(b_CanvasGroup.DOFade(1f, _fadeDuration));
        // 上移
        Vector3 startPos = _rectTransform.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, _moveOffset, 0);
        sequence.Join(_rectTransform.DOAnchorPos(endPos, _moveDuration).SetEase(Ease.OutQuad));

        // 停留一段时间
        sequence.AppendInterval(_displayDuration);
        // 淡出
        sequence.Append(b_CanvasGroup.DOFade(0f, _fadeDuration));

        // 转换为 UniTask
        return sequence.ToUniTask(TweenCancelBehaviour.Complete);
    }

}
