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
            //InsertNinja();
            //InsertMultipleNinjas();
            //InsertNinjaWithEquipment();
            SimpleNinjaQueries();
            //SimpleNinjaGraphQuery();
            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();
            //DeleteNinja;
            Console.ReadKey();
        }

        private static void InsertNinja()
        {
            ////First insert example
            //var ninja = new Ninja()
            //{
            //    Name = "JulieSan",
            //    ServedInOniwaban = false,
            //    DateOfBirth = new DateTime(1980, 1, 1),
            //    ClanId = 1
            //};

            //Second Insert example
            var ninja = new Ninja()
            {
                Name = "SampsonSan",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(2008, 1, 28),
                ClanId = 1
            };

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                //Single Ninja insert used for first and second inserts
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }
        }

        private static void InsertMultipleNinjas()
        {          
            //Third insert example
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

                //Multiple Ninjas insert used for third insert. 
                //EF opens connection, starts a transaction and performs a distinct insert for each Ninja, 
                //before committing the transaction and closing the connection.
                //1 SQL insert is run for each Ninja, will rollback all if even one fails.
                context.Ninjas.AddRange(new List<Ninja> { ninja1, ninja2 });

                context.SaveChanges();
            }
        }

        private static void SimpleNinjaQueries()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                //var ninjas = context.Ninjas.ToList(); //Uses a LINQ Execution Method to make the query execute against the database.

                //var query = context.Ninjas; //This statement doesn't run the query; var is actually a DbSet<Ninja> type.                
                //var someNinjas = query.ToList(); // ToList<> LINQ execution method runs the query as it did above.
                ////Enumerating through a DbSet will also force queries to execute.
                ////Warning the database connection will remain open until we finish enumeration through the DbSet, so don't do lots of work in the enumeration.
                //foreach (var ninja in query)
                //{
                //    Console.WriteLine(ninja.Name);
                //}

                ////var ninjas = context.Ninjas.Where(n => n.Name == "Raphael"); //Where<> is not an executing method, so we will enumerate to execute.
                ////var ninjas = context.Ninjas.Where(n => n.DateOfBirth == new DateTime(1984,1,1));
                //var ninjas = context.Ninjas.Where(n => n.ClanId == 1);
                //foreach (var ninja in ninjas)
                //{
                //    Console.WriteLine(ninja.Name);
                //}

                //var ninja = context.Ninjas.Where(n => n.ClanId == 1).FirstOrDefault(); //FirstOrDefault<> is an execution method.
                //Console.WriteLine(ninja.Name); //Can't enumerate because we only returned a single ninja  

                //int clanId = 1; 
                ////Example of combining an executing method with a predicate...
                //var ninja = context.Ninjas.FirstOrDefault(n => n.ClanId == clanId); //...the Lambda filter is in FirstOrDefault<>; Where<> is not needed...
                ////...but this works only when the execution method is the final piece of the query!
                ////Everything after the execution method is performed in memory, not in the database query. The DB connection closes after the execution method runs.
                ////Don't want to pull down tons of data you don't need, just to filter it once in memory!
                //Console.WriteLine(ninja.Name); //When EF sees a variable in a LINQ query it will always parameterize thar query (in SQL) -- check log.

                //Here we'll go bach to using Where<>. Then I can sort and page. All operations are performed in on Database query.
                var ninja = context.Ninjas
                    .Where(n => n.ClanId == 1)
                    .OrderBy(n => n.Name)
                    .Skip(1).Take(1) //Paging is not an execution method, so we still use FirstOrDefault<>.
                    .FirstOrDefault(); //FirstOrDefault<> will return a single Ninja item, rather than a List<Ninja> containing one Ninja.
                Console.WriteLine(ninja.Name);
            }
        }

    }
}
