using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oxitorenk.TargetPointer.Editor.Scripts
{
    [DefaultExecutionOrder(-1)]
    public class PointerDrawer : MonoBehaviour
    {
        public static Action<PointerTarget, bool> TargetStateChanged;
        
        private const float ScreenBoundOffset = 0.9f;
        
        private readonly List<PointerTarget> _targets = new();
        private readonly Dictionary<string, List<PointerItem>> _pointers = new();
        
        private Camera _mainCamera;
        private Vector3 _screenCenter;
        private Vector3 _screenBounds;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
            _screenBounds = _screenCenter * ScreenBoundOffset;
            
            TargetStateChanged += OnTargetStateChanged;
        }
        
        private void OnDestroy()
        {
            TargetStateChanged -= OnTargetStateChanged;
        }

        private void LateUpdate()
        {
            UpdatePointers();
        }

        /// <summary>
        /// Updates the pointers for each target based on visibility
        /// </summary>
        private void UpdatePointers()
        {
            foreach (var target in _targets)
            {
                var screenPosition = PointerHelper.GetScreenPosition(_mainCamera, target.transform.position);
                var isTargetVisible = PointerHelper.IsTargetVisible(screenPosition);

                if (isTargetVisible && target.TryGetPointer(PointerItem.PointerType.OnScreen, out var pointerItem))
                {
                    screenPosition.z = 0;
                    
                    var pointer = target.CurrentPointer != null && target.CurrentPointer.Type == PointerItem.PointerType.OnScreen ? target.CurrentPointer : GetPointer(pointerItem);
                    pointer.transform.position = screenPosition;
                    
                    target.AttachPointer(pointer);
                }
                else if (isTargetVisible == false && target.TryGetPointer(PointerItem.PointerType.OffScreen, out pointerItem))
                {
                    var angle = float.MinValue;
                    PointerHelper.GetOnScreenIndicatorPositionAndAngle(ref screenPosition, ref angle, _screenCenter, _screenBounds);

                    var pointer = target.CurrentPointer != null && target.CurrentPointer.Type == PointerItem.PointerType.OffScreen ? target.CurrentPointer : GetPointer(pointerItem);
                    pointer.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                    pointer.transform.position = screenPosition;
                    
                    target.AttachPointer(pointer);
                }
                else
                {
                    target.RemovePointer();
                }
            }
        }
        
        private void OnTargetStateChanged(PointerTarget target, bool isActive)
        {
            if (isActive)
            {
                _targets.Add(target);
            }
            else
            {
                target.RemovePointer();
                _targets.Remove(target);
            }
        }
        
        /// <summary>
        /// Retrieves an existing pointer or creates a new one if none are available
        /// </summary>
        /// <param name="pointerItem">Pointer item to check if in pool</param>
        /// <returns>Pointer item</returns>
        private PointerItem GetPointer(PointerItem pointerItem)
        {
            if (!_pointers.ContainsKey(pointerItem.Key))
                _pointers[pointerItem.Key] = new List<PointerItem>();

            var availablePointer = _pointers[pointerItem.Key].Find(item => !item.gameObject.activeSelf);
            if (availablePointer != null)
                return availablePointer;

            var newPointer = Instantiate(pointerItem, transform);
            _pointers[pointerItem.Key].Add(newPointer);
            return newPointer;
        }
    }
}
