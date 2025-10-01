namespace MagiXSquad.Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {


        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            if (IsAuthenticated(principal))
                return null;

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetRoleId(this ClaimsPrincipal principal)
        {
            if (IsAuthenticated(principal))
                return null;

            return principal.FindFirst("roleId")?.Value;
        }

        public static string? GetEmail(this ClaimsPrincipal principal)
        {
            if (IsAuthenticated(principal))
                return null;

            return principal.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetUserName(this ClaimsPrincipal principal)
        {
            if (IsAuthenticated(principal))
                return null;

            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static bool HasRole(this ClaimsPrincipal principal, string role)
        {
            if (IsAuthenticated(principal))
                return false;

            return principal.IsInRole(role);
        }

        private static bool IsAuthenticated(this ClaimsPrincipal principal)
        {
            return principal?.Identity?.IsAuthenticated != true;
        }
    }
}
