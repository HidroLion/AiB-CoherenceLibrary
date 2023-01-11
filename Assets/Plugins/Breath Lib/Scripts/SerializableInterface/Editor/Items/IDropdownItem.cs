namespace BreathLib.SerializableInterface
{
    internal interface IDropdownItem
    {
        internal ReferenceMode Mode { get; }
        object GetValue();
    }
}
