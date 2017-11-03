using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaDomain.Classes;
using NinjaDomain.DataModel;
using System.Data.Entity;

//Run 'install-package EntityFramework' in Package Manager Console so we can reference EF, so we can interract with DBContext.
//If scripting is blocked, parts may fail. I had to manually update App.config.

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());//stop EF from performing DB initialization process.
            InsertNinja();
            //InsertMultipleNinjas();
            //InsertNinjaWithEquipment();
            //SimpleNinjaQuery();
            //SimpleNinjaGraphQuery();
            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();
            //DeleteNinja;
            Console.ReadKey();
        }

        private static void InsertNinja()
        {
            ////First insert
            //var ninja = new Ninja()
            //{
            //    Name = "JulieSan",
            //    ServedInOniwaban = false,
            //    DateOfBirth = new DateTime(1980, 1, 1),
            //    ClanId = 1
            //};

            ////Second Insert
            //var ninja = new Ninja()
            //{
            //    Name = "SampsonSan",
            //    ServedInOniwaban = false,
            //    DateOfBirth = new DateTime(2008, 1, 28),
            //    ClanId = 1
            //};

            //Third insert
            var ninja1 = new Ninja()
            {
                Name = "Leonardo",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1984, 1, 1),
                ClanId = 1
            };
            var ninja2 = new Ninja()
            {
                Name = "Raphael",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1985, 1, 1),
                ClanId = 1
            };

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                ////Single Ninja insert used for first and second inserts
                //context.Ninjas.Add(ninja);

                //Multiple Ninjas insert used for third insert. 
                //EF opens connection and performs a distinct insert for each Ninja, before committing and closing it.
                //1 SQL insert is run for each Ninja, will rollback all if even one fails.
                context.Ninjas.AddRange(new List<Ninja> { ninja1, ninja2 });

                context.SaveChanges();
            }
        }
    }
}
