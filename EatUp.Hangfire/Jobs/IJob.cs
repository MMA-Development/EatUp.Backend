namespace EatUp.Hangfire.Jobs
{
    public interface IJob
    {
        public Task ExecuteAsync();
    }
}
