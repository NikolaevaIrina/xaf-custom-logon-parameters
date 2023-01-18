using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using System.Runtime.Serialization;
using CustomLogonXPOWin.Module.BusinessObjects;

namespace CustomLogonXPOWin.Module.Security {
    [DomainComponent, Serializable]
    [DisplayName("Log In")]
    public class CustomLogonParameters : INotifyPropertyChanged, ISerializable {
        private Company company;
        private ApplicationUser applicationUser;
        private string password;

        [ImmediatePostData]
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
    }
}
