namespace WorkflowApi.Client.Test.Runner;

public static class MultitenantTestTenants
{
    public const string TenantA = "TenantA";
    public const string TenantALogical = "TenantA.Logical";
    public const string TenantB = "TenantB";
    public const string TenantC = "TenantC";

    public static string[] TenantAIds => [TenantA, TenantALogical];
    public static string[] TenantBIds => [TenantB];
    public static string[] TenantCIds => [TenantC];
    public static string[] AllTenantIds => [TenantA, TenantALogical, TenantB, TenantC];
}
