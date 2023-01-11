﻿using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using DevExpress.Persistent.Base;
using Microsoft.Extensions.DependencyInjection;
using EFCoreCustomLogonAll.Module.BusinessObjects;

namespace EFCustomLogon.Module.BusinessObjects;

[DomainComponent, Serializable]
[DisplayName("Log In")]
public class CustomLogonParameters : INotifyPropertyChanged, ISerializable, IDisposable, IServiceProviderConsumer {
    private Company company;
    private ApplicationUser applicationUser;
    private string password;
    IServiceProvider? serviceProvider;
    readonly List<IDisposable> objToDispose = new List<IDisposable>();
    IReadOnlyList<Company>? _companies = null;

    [JsonIgnore]
    [ImmediatePostData]
    [DataSourceProperty("Companies", DataSourcePropertyIsNullMode.SelectAll)]
    public Company Company {
        get { return company; }
        set {
            if(value == company) return;
            company = value;
            if(ApplicationUser?.Company != company) {
                ApplicationUser = null;
            }
            OnPropertyChanged(nameof(Company));
        }
    }

    [Browsable(false)]
    [JsonIgnore]
    public IReadOnlyList<Company>? Companies {
        get {
            if(_companies == null) {
                _companies = LoadData();
            }
            return _companies;
        }
    }
    private IReadOnlyList<Company> LoadData() {
        List<Company> companies = new List<Company>();
        INonSecuredObjectSpaceFactory nonSecuredObjectSpaceFactory = serviceProvider!.GetRequiredService<INonSecuredObjectSpaceFactory>();
        var os = nonSecuredObjectSpaceFactory.CreateNonSecuredObjectSpace<Company>();

        objToDispose.Add(os);
        companies.AddRange(os.GetObjects<Company>());

        return companies.AsReadOnly();
    }
    void IDisposable.Dispose() {
        foreach(IDisposable disposable in objToDispose) {
            disposable.Dispose();
        }
        serviceProvider = null;
    }
    [DataSourceProperty("Company.ApplicationUsers"), ImmediatePostData]
    public ApplicationUser ApplicationUser {
        get { return applicationUser; }
        set {
            if(value == applicationUser) return;
            applicationUser = value;
            Company = applicationUser?.Company;
            UserName = applicationUser?.UserName;
            OnPropertyChanged(nameof(ApplicationUser));
        }
    }
    [Browsable(false)]
    public string UserName { get; set; }
    [PasswordPropertyText(true)]
    public string Password {
        get { return password; }
        set {
            if(password == value) return;
            password = value;
        }
    }
    public CustomLogonParameters() { }
    // ISerializable 
    public CustomLogonParameters(SerializationInfo info, StreamingContext context) {
        if(info.MemberCount > 0) {
            UserName = info.GetString("UserName");
            Password = info.GetString("Password");
        }
    }
    private void OnPropertyChanged(string propertyName) {
        if(PropertyChanged != null) {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    [System.Security.SecurityCritical]
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("UserName", UserName);
        info.AddValue("Password", Password);
    }
    public void RefreshPersistentObjects(IObjectSpace objectSpace) {
        ApplicationUser = UserName == null ? null : objectSpace.FirstOrDefault<ApplicationUser>(e => e.UserName == UserName);
    }
    void IServiceProviderConsumer.SetServiceProvider(IServiceProvider serviceProvider) {
        this.serviceProvider = serviceProvider;
    }
}