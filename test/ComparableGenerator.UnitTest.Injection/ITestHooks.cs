namespace ComparableGenerator.UnitTest.Injection
{
    public interface ITestHooks
    {
        bool EqualsHook(
            object? left,
            object? right);

        int CompareHook(
            object? left,
            object? right);
    }
}
