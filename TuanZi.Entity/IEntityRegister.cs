using System;

using Microsoft.EntityFrameworkCore;


namespace TuanZi.Entity
{
    public interface IEntityRegister
    {
        Type DbContextType { get; }

        Type EntityType { get; }

        void RegistTo(ModelBuilder modelBuilder);
    }
}