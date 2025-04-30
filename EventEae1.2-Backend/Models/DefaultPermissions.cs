namespace EventEae1._2_Backend.Models
{
    public  static class DefaultPermissions
    {
        // Admin
        public const string CanApproveManagers = "CanApproveManagers";
        public const string CanManageUsers = "CanManageUsers";
        public const string CanManageSettings = "CanManageSettings";

        // Event Manager
        public const string CanCreateEvents = "CanCreateEvents";
        public const string CanViewOwnEvents = "CanViewOwnEvents";
        public const string CanViewTicketSales = "CanViewTicketSales";

        // User
        public const string CanBrowseEvents = "CanBrowseEvents";
        public const string CanViewOwnTickets = "CanViewOwnTickets";

        // Common
        public const string CanManageProfile = "CanManageProfile";
    }
}
