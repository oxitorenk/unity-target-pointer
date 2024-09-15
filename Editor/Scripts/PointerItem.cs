using UnityEngine;

namespace Oxitorenk.TargetPointer.Editor.Scripts
{
    public class PointerItem : MonoBehaviour
    {
        [SerializeField] private PointerType pointerType;
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