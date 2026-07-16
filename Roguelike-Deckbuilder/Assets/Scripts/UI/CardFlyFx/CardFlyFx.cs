#define UNITASK_DOTWEEN_SUPPORT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.ObjectPool;
using UnityEngine;

public class CardFlyFx : MonoBehaviour
{
    private Tween _flyTween;
    private Tween _scaleTween;
    private Tween _fadeTween;

    /// <summary>
    /// 飞向目标（带弧线、自动回收）
    /// </summary> 
    public async UniTask FlyToTarget(
        Vector3 startWorldPos,
        Vector3 targetPos,
        Transform flightContainer,  // TopLayerCanvas
     CancellationToken token
       , float duration = 0.4f)
    {
        transform.SetParent(flightContainer, worldPositionStays: true);
        transform.position = startWorldPos;
        await transform.DOMove(targetPos, duration).ToUniTask(cancellationToken: token);
        transform.gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        _flyTween?.Kill();
        _scaleTween?.Kill();
        _fadeTween?.Kill();
    }
}
