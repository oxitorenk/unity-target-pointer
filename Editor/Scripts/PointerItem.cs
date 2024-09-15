using UnityEngine;

namespace Oxitorenk.TargetPointer
{
    public class PointerItem : MonoBehaviour
    {
        [SerializeField] private PointerType type;
        [SerializeField] private string key;
        
        public PointerType Type => type;
        public string Key => key;

        public enum PointerType
        {
            OnScreen,
            OffScreen
        }
    }
}
