namespace System.Diagnostics.CodeAnalysis
{
    [Conditional("COMPILE_TIME_ONLY")]
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class MaybeNullWhenAttribute :
        Attribute
    {
        public bool ReturnValue { get; }

        public MaybeNullWhenAttribute(
            bool returnValue)
        {
            this.ReturnValue = returnValue;
        }
    }
}
