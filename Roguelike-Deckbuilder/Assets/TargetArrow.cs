using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;


public class TargetArrow : MonoBehaviour
{
    [SerializeField] private UILineRenderer _lineRenderer;
    [SerializeField] private RectTransform _arrowHead; // 箭头头部（子物体）
    [SerializeField] private int _curveSegments = 30;

    private Vector2[] _points;

    public void SetCurve(Vector2 start, Vector2 end, Vector2 controlOffset)
    {
        // 计算控制点（偏移方向可根据需要调整）
        Vector2 mid = (start + end) * 0.5f;
        Vector2 control = mid + controlOffset;

        // 生成曲线点
        _points = new Vector2[_curveSegments];
        for (int i = 0; i < _curveSegments; i++)
        {
            float t = i / (float)(_curveSegments - 1);
            _points[i] = CalculateBezierPoint(start, control, end, t);
        }

        // 赋值给 LineRenderer
        _lineRenderer.Points = _points;
        _lineRenderer.SetAllDirty(); // 刷新绘制

        // 更新箭头头部位置和旋转
        if (_arrowHead != null)
        {
            _arrowHead.anchoredPosition = end;
            Vector2 dir = (end - _points[_points.Length - 2]).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _arrowHead.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}