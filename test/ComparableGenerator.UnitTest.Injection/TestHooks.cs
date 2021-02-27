using System;

namespace ComparableGenerator.UnitTest.Injection
{
    public class TestHooks :
        ITestHooks
    {
        private readonly ITestHooks _internalHook;

        public TestHooks(
            ITestHooks internalHook)
        {
            if (internalHook is null)
            {
                throw new ArgumentNullException(nameof(internalHook));
            }

            this._internalHook = internalHook;
        }

        public virtual bool EqualsHook(
            object? left,
            object? right)
        {
            return this._internalHook.EqualsHook(left, right);
        }

        public int CompareHook(
            object? left,
            object? right)
        {
            return this._internalHook.CompareHook(left, right);
        }
    }
}
