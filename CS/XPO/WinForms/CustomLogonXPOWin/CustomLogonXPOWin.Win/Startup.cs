using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Win.ApplicationBuilder;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraEditors;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Design;
using CustomLogonXPOWin.Module.BusinessObjects;
using CustomLogonXPOWin.Module.Security;
using DevExpress.ExpressApp.Core;
using CustomLogonXPOWin.Module.DatabaseUpdate;
using DevExpress.Mvvm.Native;

namespace CustomLogonXPOWin.Win;

public class ApplicationBuilder : IDesignTimeApplicationFactory {
    public static WinApplication BuildApplication(string connectionString) {
        var builder = WinApplication.CreateBuilder();
        builder.UseApplication<CustomLogonXPOWinWindowsFormsApplication>();
        builder.Modules
            .AddConditionalAppearance()
            .AddValidation(options => {
                options.AllowValidationDetailsAccess = false;
            })
            .Add<CustomLogonXPOWin.Module.CustomLogonXPOWinModule>()
        	.Add<CustomLogonXPOWinWinModule>();
        builder.ObjectSpaceProviders
            .AddSecuredXpo((application, options) => {
                options.ConnectionString = connectionString;
            })
            .AddNonPersistent();
        builder.Security
            .UseIntegratedMode(options => {
                options.RoleType = typeof(PermissionPolicyRole);
                options.UserType = typeof(CustomLogonXPOWin.Module.BusinessObjects.ApplicationUser);
                options.UserLoginInfoType = typeof(CustomLogonXPOWin.Module.BusinessObjects.ApplicationUserLoginInfo);
                options.UseXpoPermissionsCaching();
                options.Events.OnSecurityStrategyCreated = securityStrategyBase => {
                    // ...
                    var securityStrategy = (SecurityStrategy)securityStrategyBase;
                    securityStrategy.Authentication = new CustomAuthentication();
                    securityStrategy.AnonymousAllowedTypes.Add(typeof(Company));
                    securityStrategy.AnonymousAllowedTypes.Add(typeof(ApplicationUser));
                };
            });
           // .UsePasswordAuthentication();
        builder.AddBuildStep(application => {
            application.ConnectionString = connectionString;
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached && application.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                application.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
#endif
        });
        var winApplication = builder.Build();


        return winApplication;
    }

    XafApplication IDesignTimeApplicationFactory.Create()
        => BuildApplication(XafApplication.DesignTimeConnectionString);
}


