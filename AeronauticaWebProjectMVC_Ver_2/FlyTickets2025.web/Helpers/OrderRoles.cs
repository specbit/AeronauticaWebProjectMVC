namespace FlyTickets2025.web.Helpers
{
    public static class OrderRoles
    {
        /// <summary>
        /// Returns the sort order for a given role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static int GetRoleSortOrder(string role)
        {
            switch (role)
            {
                case "Administrador":
                    return 1;
                case "Funcionário":
                    return 2;
                case "Cliente":
                    return 3;
                default:
                    return 4; // Other roles or empty roles go last
            }

        }
    }
}
