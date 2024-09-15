using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxitorenk.TargetPointer
{
    [DefaultExecutionOrder(-1)]
    public class PointerDrawer : MonoBehaviour
    {
        public static Action<PointerTarget, bool> TargetStateChanged;
        
        private const float ScreenBoundOffset = 0.9f;
        
        private readonly List<PointerTarget> _targets = new();
        private readonly Dictionary<string, List<PointerItem>> _indicators = new();
        
        private Camera _mainCamera;
        private Vector3 _screenCentre;
        private Vector3 _screenBounds;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
            _screenBounds = _screenCentre * ScreenBoundOffset;
            
            TargetStateChanged += HandleTargetStateChanged;
        }
        
        private void OnDestroy()
        {
            TargetStateChanged -= HandleTargetStateChanged;
        }

        private void LateUpdate()
        {
            DrawIndicators();
        }

        private void DrawIndicators()
        {
            foreach(var target in _targets)
            {
                var screenPosition = PointerHelper.GetScreenPosition(_mainCamera, target.transform.position);
                var isTargetVisible = PointerHelper.IsTargetVisible(screenPosition);

                if(isTargetVisible && target.HasIndicatorType(PointerItem.IndicatorType.OnScreen, out var indicatorItem))
                {
                    screenPosition.z = 0;
                    
                    var indicator = target.CurrentPointer != null ? target.CurrentPointer : GetIndicator(indicatorItem);
                    indicator.transform.position = screenPosition;
                    
                    target.AttachIndicator(indicator);
                }
                else if (!isTargetVisible && target.HasIndicatorType(PointerItem.IndicatorType.OffScreen, out indicatorItem))
                {
                    var angle = float.MinValue;
                    PointerHelper.GetOnScreenIndicatorPositionAndAngle(ref screenPosition, ref angle, _screenCentre, _screenBounds);

                    var indicator = target.CurrentPointer != null ? target.CurrentPointer : GetIndicator(indicatorItem);
                    indicator.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg); // Sets the rotation for the arrow indicator.
                    indicator.transform.position = screenPosition;
                    
                    target.AttachIndicator(indicator);
                }
                else
                {
                    target.RemoveIndicator();
                    return;
                }
            }
        }
        
        private void HandleTargetStateChanged(PointerTarget target, bool active)
        {
            if (active)
            {
                _targets.Add(target);
            }
            else
            {
                target.RemoveIndicator();
                _targets.Remove(target);
            }
        }
        
        private PointerItem GetIndicator(PointerItem pointerItem)
        {
            if (_indicators.ContainsKey(pointerItem.Key) == false)
                _indicators.Add(pointerItem.Key, new List<PointerItem>());

            var isAnyPointerExist = _indicators[pointerItem.Key].Any(item => !item.gameObject.activeSelf);
            if (isAnyPointerExist)
            {
                return _indicators[pointerItem.Key].First(item => !item.gameObject.activeSelf);
            }

            // Create new pointer
            var indicator = Instantiate(pointerItem, transform);
            _indicators[pointerItem.Key].Add(indicator);

            return indicator;
        }
    }
}
