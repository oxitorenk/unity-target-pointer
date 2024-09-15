using System.Collections.Generic;
using UnityEngine;

namespace Oxitorenk.TargetPointer.Editor.Scripts
{
    [DefaultExecutionOrder(0)]
    public class PointerTarget : MonoBehaviour
    {
        [Tooltip("List of available pointers for this target.")]
        [SerializeField] private List<PointerItem> pointerItems;
        
        public PointerItem CurrentPointer { get; private set; }
        
        private void OnEnable()
        {
            PointerDrawer.TargetStateChanged?.Invoke(this, true);
        }
        
        private void OnDisable()
        {
            PointerDrawer.TargetStateChanged?.Invoke(this, false);
        }

        /// <summary>
        /// Attaches a new pointer to the target.
        /// </summary>
        /// <param name="pointer">Pointer item to attach.</param>
        public void AttachPointer(PointerItem pointer)
        {
            if (CurrentPointer == pointer) return;
            
            if (CurrentPointer != null)
                CurrentPointer.gameObject.SetActive(false);
            
            CurrentPointer = pointer;
            if (CurrentPointer != null)
                CurrentPointer.gameObject.SetActive(true);
        }

        /// <summary>
        /// Removes the current pointer from the target.
        /// </summary>
        public void RemovePointer()
        {
            if (CurrentPointer == null) return;
            
            CurrentPointer.gameObject.SetActive(false);
            CurrentPointer = null;
        }

        /// <summary>
        /// Checks if a pointer of the specified type is available and returns it.
        /// </summary>
        /// <param name="pointerType">Pointer type to check for (on-screen or off-screen).</param>
        /// <param name="pointer">Pointer item to return if found.</param>
        /// <returns>True if a pointer of the specified type is found, false otherwise.</returns>
        public bool TryGetPointer(PointerItem.PointerType pointerType, out PointerItem pointer)
        {
            pointer = pointerItems.Find(item => item.Type == pointerType);
            return pointer != null;
        }
    }
}
