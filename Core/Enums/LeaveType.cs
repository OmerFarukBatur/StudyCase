using System.ComponentModel.DataAnnotations;

namespace Core.Enums
{
    public enum LeaveType
    {
        [Display(Name = "Yıllık İzin")]
        Annual = 1,

        [Display(Name = "Hastalık İzni")]
        Sick = 2,

        [Display(Name = "Ücretsiz İzin")]
        Unpaid = 3
    }
}
