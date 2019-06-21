namespace Business.Core.Common.Helpers
{
    public interface IQueryCommand<out TResult>
    {
        TResult Execute();
    }
}