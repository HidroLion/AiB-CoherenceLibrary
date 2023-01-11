using UnityEngine;

namespace BreathLib.SerializableInterface
{
    internal interface IReferenceDrawer
    {
        float GetHeight();
        void OnGUI(Rect position);
    }
}
