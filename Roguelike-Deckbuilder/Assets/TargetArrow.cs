using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI.Extensions;


public class TargetArrow : UIBase
{
    [SerializeField] private UILineRenderer _lineRenderer;
    [SerializeField] private RectTransform _arrowHead; // 箭头头部（子物体）
    [SerializeField] private int _curveSegments = 30;
    private Vector2 _currentStart;
    private Vector2 _currentControlOffset;
    private Vector2[] _points;

    // 首次显示：创建数组并设置曲线
    public void Init(Vector2 start, Vector2 end, Vector2 controlOffset)
    {
        _currentStart = start;
        _currentControlOffset = controlOffset;

        if (_points == null || _points.Length != _curveSegments)
        {
            _points = new Vector2[_curveSegments];
        }

        gameObject.SetActive(true);
        UpdateCurve(end);
    }

    // ★ 新增：只更新终点，复用数组
    public void UpdateArrow(Vector2 end)
    {
        if (!gameObject.activeSelf) return;
        UpdateCurve(end);
    }

    private void UpdateCurve(Vector2 end)
    {
        Vector2 mid = (_currentStart + end) * 0.5f;
        Vector2 control = mid + _currentControlOffset;

        for (int i = 0; i < _curveSegments; i++)
        {
            float t = i / (float)(_curveSegments - 1);
            _points[i] = CalculateBezierPoint(_currentStart, control, end, t);
        }

        _lineRenderer.Points = _points;
        _lineRenderer.SetAllDirty();

        // 更新箭头头部
        if (_arrowHead != null)
        {
            _arrowHead.anchoredPosition = end;
            Vector2 dir = (end - _points[_points.Length - 2]).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
               angle -= 90f; 
            _arrowHead.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}