using System.ComponentModel.DataAnnotations;

namespace Core.Enums
{
    public enum ApprovalAction
    {
        [Display(Name = "Onaylandı")]
        Approve = 100,
        [Display(Name = "Reddedildi")]
        Reject = 200
    }
}
