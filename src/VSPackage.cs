using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace WebEssentials
{
    [Guid("f168228c-63f6-4db0-b426-43c30e9d1fc7")]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class WebEssentialsPackage : AsyncPackage
    {
    }
}
