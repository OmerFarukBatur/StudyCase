using System.ComponentModel.DataAnnotations;

namespace Core.Enums
{
    public enum LeaveStatus
    {
        [Display(Name = "Beklemede")]
        Pending = 10,
        [Display(Name = "Onaylandı")]
        Approved = 20,
        [Display(Name = "Reddedildi")]
        Rejected = 30
    }
}
