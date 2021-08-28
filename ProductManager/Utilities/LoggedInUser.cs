namespace Framework.Utilities
{
    public static class LoggedInUser
    {
        public static DataLayer.Models.User User = null;

        public delegate void LogOutEventArgs();
        public static event LogOutEventArgs OnLogout;

        public static void LogOut()
        {
            User = null;
            if (OnLogout != null)
                OnLogout();
        }
    }
}
