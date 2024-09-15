using UnityEngine;

namespace Oxitorenk.TargetPointer.Editor.Scripts
{
    public class PointerItem : MonoBehaviour
    {
        [Tooltip("Defines whether the pointer is on-screen or off-screen.")]
        [SerializeField] private PointerType pointerType;
        
        [Tooltip("Unique key to identify the pointer for pooling and retrieval.")]
        [SerializeField] private string pointerKey;
        
        public PointerType Type => pointerType;
        public string Key => pointerKey;
        
        public enum PointerType
        {
            OnScreen,
            OffScreen
        }
    }
}