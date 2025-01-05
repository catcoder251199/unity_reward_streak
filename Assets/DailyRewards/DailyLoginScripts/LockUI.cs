namespace DailyRewards
{
    public class LockUI
    {
        private bool isLocked = false;
        public bool IsLocked() => isLocked;
        public void Lock() => isLocked = true;
        public void UnLock() => isLocked = false;
        public void ForceUnlock() => isLocked = false;
    }
}
