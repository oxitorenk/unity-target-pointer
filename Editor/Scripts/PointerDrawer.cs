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
        private readonly Dictionary<string, List<PointerItem>> _pointers = new();
        
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
            DrawPointers();
        }

        private void DrawPointers()
        {
            foreach(var target in _targets)
            {
                var screenPosition = PointerHelper.GetScreenPosition(_mainCamera, target.transform.position);
                var isTargetVisible = PointerHelper.IsTargetVisible(screenPosition);

                if(isTargetVisible && target.HasPointerType(PointerItem.PointerType.OnScreen, out var pointerItem))
                {
                    screenPosition.z = 0;
                    
                    var pointer = target.CurrentPointer != null ? target.CurrentPointer : GetPointer(pointerItem);
                    pointer.transform.position = screenPosition;
                    
                    target.AttachPointer(pointer);
                }
                else if (!isTargetVisible && target.HasPointerType(PointerItem.PointerType.OffScreen, out pointerItem))
                {
                    var angle = float.MinValue;
                    PointerHelper.GetOnScreenPointerPositionAndAngle(ref screenPosition, ref angle, _screenCentre, _screenBounds);

                    var pointer = target.CurrentPointer != null ? target.CurrentPointer : GetPointer(pointerItem);
                    pointer.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                    pointer.transform.position = screenPosition;
                    
                    target.AttachPointer(pointer);
                }
                else
                {
                    target.RemovePointer();
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
                target.RemovePointer();
                _targets.Remove(target);
            }
        }
        
        private PointerItem GetPointer(PointerItem pointerItem)
        {
            if (_pointers.ContainsKey(pointerItem.Key) == false)
                _pointers.Add(pointerItem.Key, new List<PointerItem>());

            var isAnyPointerExist = _pointers[pointerItem.Key].Any(item => !item.gameObject.activeSelf);
            if (isAnyPointerExist)
            {
                return _pointers[pointerItem.Key].First(item => !item.gameObject.activeSelf);
            }

            // Create new pointer
            var pointer = Instantiate(pointerItem, transform);
            _pointers[pointerItem.Key].Add(pointer);

            return pointer;
        }
    }
}
