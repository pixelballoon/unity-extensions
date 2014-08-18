public class WaitForNotification : ICoroutineYieldable
{
	
	public bool IsFinished { get; private set; }

	private readonly string _notification;

	public WaitForNotification(string notification)
	{
		_notification = notification;
		Messenger.AddListener(_notification, OnNotification);
	}

	~WaitForNotification()
	{
		Messenger.RemoveListener(_notification, OnNotification);
	}

	private void OnNotification()
	{
		IsFinished = true;
	}

}
