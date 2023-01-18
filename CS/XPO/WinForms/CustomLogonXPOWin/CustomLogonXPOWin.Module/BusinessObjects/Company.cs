using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace CustomLogonXPOWin.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Company : BaseObject {
        public Company(Session session) : base(session) { }
        private string name;
        public string Name {
            get { return name; }
            set { SetPropertyValue(nameof(Name), ref name, value); }
        }
        [Association("Company-ApplicationUsers")]
        public XPCollection<ApplicationUser> ApplicationUsers {
            get { return GetCollection<ApplicationUser>(nameof(ApplicationUsers)); }
        }
    }
}
