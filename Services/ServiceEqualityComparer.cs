using MedicalLaboratoryNumber20App.Models.Entities;
using System.Collections.Generic;

namespace MedicalLaboratoryNumber20App.Services
{
    public class ServiceEqualityComparer : IEqualityComparer<Service>
    {
        public bool Equals(Service x, Service y)
        {
            return x.Code == y.Code;
        }

        public int GetHashCode(Service obj)
        {
            return obj.Code;
        }
    }
}
