namespace TestProject.WebHost.Services.Internal;

public class RequestCounterService
{
    private long _requestCounter = 0L;

    public long RequestCounter => Interlocked.Read(ref _requestCounter);

    public void AddNewRequest()
    {
        Interlocked.Increment(ref _requestCounter);
    }
}
