using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxitorenk.TargetPointer
{
    [DefaultExecutionOrder(0)]
    public class PointerTarget : MonoBehaviour
    {
        [SerializeField] private List<PointerItem> indicators;

        public PointerItem CurrentPointer { get; private set; }
        
        private void OnEnable()
        {
            PointerDrawer.TargetStateChanged?.Invoke(this, true);
        }
        
        private void OnDisable()
        {
            PointerDrawer.TargetStateChanged?.Invoke(this, false);
        }
        
        public void AttachIndicator(PointerItem pointer)
        {
            if (CurrentPointer == pointer) return;
            
            // Hide the current indicator if it's exist
            if (CurrentPointer != null)
                CurrentPointer.gameObject.SetActive(false);
            
            // Change current indicator and show it
            CurrentPointer = pointer;
            CurrentPointer.gameObject.SetActive(true);
        }
        
        public void RemoveIndicator()
        {
            if (CurrentPointer == null) return;
            
            CurrentPointer.gameObject.SetActive(false);
            CurrentPointer = null;
        }
        
        public bool HasIndicatorType(PointerItem.IndicatorType indicatorType, out PointerItem pointer)
        {
            if (indicators.Count(indicator => indicator.Type == indicatorType && indicator != null) == 0)
            {
                pointer = null;
                return false;
            }
            
            pointer = indicators.First(item => item.Type == indicatorType);
            return pointer != null;
        }
    }
}
