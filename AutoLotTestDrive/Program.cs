using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLotDAL.EF;
using AutoLotDAL.Models;
using System.Data.Entity;
using AutoLotDAL.Repos;

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
    }
}
