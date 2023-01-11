using UnityEditor.IMGUI.Controls;

namespace BreathLib.SerializableInterface
{
    public class NoneDropdownItem : AdvancedDropdownItem, IDropdownItem
    {
        public NoneDropdownItem()
            : base("None")
        {
        }

        ReferenceMode IDropdownItem.Mode => ReferenceMode.Raw;

        public object GetValue()
        {
            return null;
        }
    }
}
