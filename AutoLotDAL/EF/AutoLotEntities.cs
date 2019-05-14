namespace AutoLotDAL.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using AutoLotDAL.Models;
    using System.Data.Entity.Infrastructure.Interception;
    using AutoLotDAL.Interception;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    public partial class AutoLotEntities : DbContext
    {
        static readonly DatabaseLogger logger = new DatabaseLogger("sqllog.txt", true);
        public AutoLotEntities()
            : base("name=AutoLotConnection")
        {
            var context = (this as IObjectContextAdapter).ObjectContext;
            context.ObjectMaterialized += ObjectMaterialized;
            context.SavingChanges += OnSavingChanges;
        }

        private void OnSavingChanges(object sender, EventArgs eventArgs)
        {
            //Параметр sender имеет тип ObjectContext
            //Можно получить текущие и исходные значения
            //а также отменять/модифицировать операцию
            //сохранения любым желаемым образом
            var context = sender as ObjectContext;
            if (context == null)
            {
                return; 
            }
            foreach (ObjectStateEntry objectStateEntry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified | EntityState.Added))
            {
                //Делать что-то важное
                if ((objectStateEntry.Entity as Inventory) != null)
                {
                    var entity = (Inventory)objectStateEntry.Entity;
                    if (entity.Color == "Red")
                    {
                        objectStateEntry.RejectPropertyChanges(nameof(entity.Color));
                    }
                }
            }
        }
        private void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            throw new NotImplementedException();
        }



        public virtual DbSet<CreditRisk> CreditRisks { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void Dispose(bool disposing)
        {
            DbInterception.Remove(logger);
            logger.StopLogging();
            base.Dispose(disposing);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Car)
                .WillCascadeOnDelete(false);
                
        }
    }
}
