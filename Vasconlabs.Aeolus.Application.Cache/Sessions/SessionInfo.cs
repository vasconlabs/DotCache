namespace Vasconlabs.Aeolus.Application.Cache.Sessions
{
    internal class SessionInfo
    {
        public AeolusSession Session { get; }
        public DateTime LastActivity { get; private set; }

        public SessionInfo(AeolusSession session)
        {
            Session = session;
            LastActivity = DateTime.UtcNow;
        }

        public void UpdateActivity()
        {
            LastActivity = DateTime.UtcNow;
        }
    }
}
