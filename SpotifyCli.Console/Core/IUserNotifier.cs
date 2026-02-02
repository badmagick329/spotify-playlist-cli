namespace SpotifyCli.Core;

public interface IUserNotifier
{
    public void Notify(NotifyEvent notifyEvent);
}
