using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLotDAL.EF;
using AutoLotDAL.Models;
using System.Data.Entity;
using AutoLotDAL.Repos;
using System.Data.Entity.Infrastructure;

namespace AutoLotTestDrive
{
    class Program
    {
        static void Main(string[] args)
        {
            //Database.SetInitializer(new MyDataInitialiizer());
            Console.WriteLine("Fun with ADO.NET EF Code First \n");
            using (var context = new AutoLotEntities())
            {
                foreach (Inventory inventory in context.Inventory)
                {
                    Console.WriteLine(inventory);
                }
            }

            Console.WriteLine("Using a Repository \n");
            using (var repo = new InventoryRepo())
            {
                foreach (Inventory inventory in repo.GetAll())
                {
                    Console.WriteLine(inventory);
                }
            }

            TestConcurrency();
            Console.ReadLine();
        }

        //Добавление инвентаризационных записей
        private static void AddNewRecord(Inventory car)
        {
            //Добавить запись в таблицу Inventory базы данных AutoLot
            using (var repo = new InventoryRepo())
            {
                repo.Add(car);
            }
        }

        //Редактирование записей
        private static void UpdateRecord(int carId)
        {
            using (var repo = new InventoryRepo())
            {
                //Извлечь запись об автомобиле, изменить ее и сохранить
                var carToUpdate = repo.GetOne(carId);
                if (carToUpdate == null) return;
                carToUpdate.Color = "Blue";
                repo.Save(carToUpdate);

            }
        }

        //Удаление записей
        private static void RemoveRecordByCar(Inventory carToDelete)
        {
            using (var repo = new InventoryRepo())
            {
                repo.Delete(carToDelete);
            }
        }

        private static void RemoveRecordById(int carId, byte[] timeStamp)
        {
            using (var repo = new InventoryRepo())
            {
                repo.Delete(carId, timeStamp);
            }
        }

        private static void TestConcurrency()
        {
            var repo1 = new InventoryRepo();
            //Использовать хранилище, чтобы гаранитировать применение отличающегося контекста
            var repo2 = new InventoryRepo();
            var car1 = repo1.GetOne(1);
            var car2 = repo2.GetOne(1);
            car1.PetName = "NewName";
            repo1.Save(car1);
            car2.PetName = "OtherName";
            try
            {
                repo2.Save(car2);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var currentValues = entry.CurrentValues;
                var originalValues = entry.OriginalValues;
                var dbValues = entry.GetDatabaseValues();
                Console.WriteLine("Cuncurency");
                Console.WriteLine("Type\tPetName");
                Console.WriteLine($"Current:\t{currentValues[nameof(Inventory.PetName)]}");
                Console.WriteLine($"Orig:\t{originalValues[nameof(Inventory.PetName)]}");
                Console.WriteLine($"db:\t{dbValues[nameof(Inventory.PetName)]}");
            }
        }
    }
}
