using UnityEngine;

namespace Oxitorenk.TargetPointer
{
    public class PointerItem : MonoBehaviour
    {
        [SerializeField] private IndicatorType type;
        [SerializeField] private string key;
        
        public IndicatorType Type => type;
        public string Key => key;

        public enum IndicatorType
        {
            OnScreen,
            OffScreen
        }
    }
}
