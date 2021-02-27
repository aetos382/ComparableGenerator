namespace ComparableGenerator.Tests.Injection
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
