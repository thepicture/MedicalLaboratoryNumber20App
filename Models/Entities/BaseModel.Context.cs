﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MedicalLaboratoryNumber20App.Models.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MedicalLaboratoryNumber20Entities : DbContext
    {
        public MedicalLaboratoryNumber20Entities()
            : base("name=MedicalLaboratoryNumber20Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Analyzer> Analyzer { get; set; }
        public virtual DbSet<Blood> Blood { get; set; }
        public virtual DbSet<BloodServiceOfUser> BloodServiceOfUser { get; set; }
        public virtual DbSet<BloodStatus> BloodStatus { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<InsuranceAddress> InsuranceAddress { get; set; }
        public virtual DbSet<InsuranceCompany> InsuranceCompany { get; set; }
        public virtual DbSet<LoginHistory> LoginHistory { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientSocialType> PatientSocialType { get; set; }
        public virtual DbSet<ResultType> ResultType { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserType> UserType { get; set; }
    }
}
