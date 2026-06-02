namespace frontendnet.Security;

public static class AppRoles
{
    public const string Administrator = "Administrador";
    public const string User = "Usuario";

    public const string AuthenticatedRoles = Administrator + "," + User;
}