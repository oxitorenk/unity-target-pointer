using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oxitorenk.TargetPointer.Editor.Scripts
{
    /// <summary>
    /// Manages and updates the pointers for targets.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class PointerDrawer : MonoBehaviour
    {
        public static Action<PointerTarget, bool> TargetStateChanged;
        
        // Offset to ensure pointers stay slightly within the screen bounds.
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
        /// Iterates through all active targets and updates their pointers.
        /// </summary>
        private void UpdatePointers()
        {
            foreach (var target in _targets)
            {
                var pointerCurrentPosition = PointerHelper.GetScreenPosition(_mainCamera, target.transform.position);
                var isTargetVisible = PointerHelper.IsTargetVisible(pointerCurrentPosition);

                if (isTargetVisible && target.TryGetPointer(PointerItem.PointerType.OnScreen, out var pointerItem))
                {
                    // Target is visible: position the on-screen pointer.
                    var pointer = GetPointer(target, PointerItem.PointerType.OnScreen, pointerItem);
                    pointerCurrentPosition.z = 0;
                    pointer.transform.position = pointerCurrentPosition;
                    
                    target.AttachPointer(pointer);
                }
                else if (!isTargetVisible && target.TryGetPointer(PointerItem.PointerType.OffScreen, out pointerItem))
                {
                    // Target is off-screen: calculate angle and position for the off-screen indicator.
                    var pointerPositionInfo = PointerHelper.GetOnScreenPointerPositionAndAngle(pointerCurrentPosition, _screenCenter, _screenBounds);
                    var pointer = GetPointer(target, PointerItem.PointerType.OffScreen, pointerItem);
                    pointer.transform.rotation = Quaternion.Euler(0, 0, pointerPositionInfo.angle * Mathf.Rad2Deg);
                    pointer.transform.position = pointerPositionInfo.screenPosition;
                    
                    target.AttachPointer(pointer);
                }
                else
                {
                    target.RemovePointer();
                }
            }
        }

        /// <summary>
        /// Handles changes in the target state.
        /// </summary>
        /// <param name="target">The target whose state has changed.</param>
        /// <param name="isActive">Indicates if the target is active or inactive.</param>
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
        /// Retrieves an available pointer from the pool or instantiates a new one if none are available.
        /// </summary>
        /// <param name="pointerItem">Pointer item to retrieve or instantiate.</param>
        /// <returns>An available or newly instantiated pointer item.</returns>
        private PointerItem GetPointer(PointerTarget target, PointerItem.PointerType type, PointerItem item)
        {
            // Current pointer is already the same one with the requested one.
            if (target.CurrentPointer != null && target.CurrentPointer.Type == type)
            {
                return target.CurrentPointer;
            }
            
            // Check if there is a list for this pointer type in the pool.
            if (_pointers.ContainsKey(item.Key) == false)
                _pointers[item.Key] = new List<PointerItem>();

            // Look for an inactive pointer in the pool.
            var availablePointer = _pointers[item.Key].Find(item => !item.gameObject.activeSelf);
            if (availablePointer != null)
                return availablePointer;

            // If no pointer is available, instantiate a new one and add it to the pool.
            var newPointer = Instantiate(item, transform);
            _pointers[item.Key].Add(newPointer);
            
            return newPointer;
        }
    }
}
