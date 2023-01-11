using System.Collections.ObjectModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace EFCustomLogon.Module.BusinessObjects;

[DefaultClassOptions]
public class Company : BaseObject {
    public virtual string Name { get; set; }
    public virtual IList<ApplicationUser> ApplicationUsers { get; set; } = new ObservableCollection<ApplicationUser>();
}
