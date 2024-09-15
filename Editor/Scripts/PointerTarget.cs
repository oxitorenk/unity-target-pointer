using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxitorenk.TargetPointer
{
    [DefaultExecutionOrder(0)]
    public class PointerTarget : MonoBehaviour
    {
        [SerializeField] private List<PointerItem> pointers;

        public PointerItem CurrentPointer { get; private set; }
        
        private void OnEnable()
        {
            PointerDrawer.TargetStateChanged?.Invoke(this, true);
        }
        
        private void OnDisable()
        {
            PointerDrawer.TargetStateChanged?.Invoke(this, false);
        }
        
        public void AttachPointer(PointerItem pointer)
        {
            if (CurrentPointer == pointer) return;
            
            // Hide the current pointer if it's exist
            if (CurrentPointer != null)
                CurrentPointer.gameObject.SetActive(false);
            
            // Change current pointer and show it
            CurrentPointer = pointer;
            CurrentPointer.gameObject.SetActive(true);
        }
        
        public void RemovePointer()
        {
            if (CurrentPointer == null) return;
            
            CurrentPointer.gameObject.SetActive(false);
            CurrentPointer = null;
        }
        
        public bool HasPointerType(PointerItem.PointerType pointerType, out PointerItem pointer)
        {
            var availablePointers = pointers.FindAll(pointer => pointer.Type == pointerType && pointer != null);
            if (availablePointers.Count == 0)
            {
                pointer = null;
                return false;
            }

            pointer = availablePointers.First();
            return true;
        }
    }
}
