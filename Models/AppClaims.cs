namespace Library_API.Models
{
    public static class AppClaims
    {
        public const string Permission = "permission";
        public const string MemberId = "member_id";

        public static readonly IReadOnlySet<string> RoleClaimTypes =
              new HashSet<string>(StringComparer.Ordinal)
              {
                  //Add a validation to the ClaimValidationService each time you include a new claim.
                    Permission
              };

        public static readonly IReadOnlySet<string> UserClaimTypes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                  //Add a validation to the ClaimValidationService each time you include a new claim.
                Permission,
            };
    }
}
