namespace Library_API.Auditing
{
    public static class AuditEvents
    {

        public const string LoginSuccess = "auth.login.success";
        public const string LoginFailed = "auth.login.failed";
        public const string LoginLockedOut = "auth.login.locked_out";

        public const string Registered = "auth.register";

        public const string TokenRefreshed = "auth.token.refreshed";
        public const string RefreshTokenExpired = "auth.token.expired";
        public const string RefreshTokenDenied = "auth.token.denied";
        public const string RefreshReuseDetected = "auth.token.reuse_detected";

        public const string Logout = "auth.logout";
        public const string LogoutAll = "auth.logout_all";


        public const string RoleAssigned = "admin.role.assigned";
        public const string RoleRemoved = "admin.role.removed";
        public const string RolePermissionsChanged = "admin.role.permissions_changed";

        public const string RoleClaimAdded = "admin.role_claim.added";
        public const string RoleClaimRemoved = "admin.role_claim.removed";

        public const string AccessDenied = "authz.denied";

        public const string AuditLogsRead = "admin.audit_logs.read";
    }
}
