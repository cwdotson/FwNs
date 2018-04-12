namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    internal static class SecurityExtensions
    {
        public static readonly bool LiveLinqHasNonPublicMembersAccess;

        static SecurityExtensions()
        {
            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
            try
            {
                permission.Demand();
                LiveLinqHasNonPublicMembersAccess = true;
            }
            catch (SecurityException)
            {
                LiveLinqHasNonPublicMembersAccess = false;
            }
        }

        internal static void WithMemberAccessCheck(Action action)
        {
            action.Invoke();
        }
    }
}

