using MedicalLaboratoryNumber20App.Models.Entities;
using System.Collections.Generic;

namespace MedicalLaboratoryNumber20App.Services
{
    public class BloodServiceEqualityComparer : IEqualityComparer<BloodServiceOfUser>
    {
        public bool Equals(BloodServiceOfUser x, BloodServiceOfUser y)
        {
            return (x.UserId == y.UserId) && (x.ServiceCode == y.ServiceCode);
        }

        public int GetHashCode(BloodServiceOfUser obj)
        {
            return obj.ServiceCode * obj.UserId;
        }
    }
}
