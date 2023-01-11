﻿using UnityEditor.IMGUI.Controls;

namespace BreathLib.SerializableInterface
{
    internal sealed class AdvancedDropdownItemWrapper : AdvancedDropdownItem
    {
        /// <inheritdoc />
        public AdvancedDropdownItemWrapper(string name)
            : base(name)
        {
        }

        public new AdvancedDropdownItemWrapper AddChild(AdvancedDropdownItem child)
        {
            base.AddChild(child);
            return this;
        }

        public new AdvancedDropdownItemWrapper AddSeparator()
        {
            base.AddSeparator();
            return this;
        }
    }
}
