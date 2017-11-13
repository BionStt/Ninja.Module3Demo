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
            //SimpleNinjaQueries();
            SimpleNinjaGraphQuery();
            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();
            //RetrieveDataWithFind();
            //RetrieveDataWithStoredProc();
            //DeleteNinja();
            //DeleteNinjaWithKeyValue();
            //DeleteNinjaWithStoredProcedure();
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

        private static void QueryAndUpdateNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.FirstOrDefault();
                ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);
                context.SaveChanges(); //SQL only updates the field that was changed.
            }
        }

        private static void QueryAndUpdateNinjaDisconnected()
        {
            Ninja ninja;

            //Code block represents API returning a Ninja to the Client.
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
            }

            //Statement represents client modifying the Ninja.
            ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);

            //Code block uses a new context instance to update the Ninja.
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                //context.Ninjas.Add(ninja); //Nope: this would insert a new record, ignoring and overwriting the Id.
                context.Ninjas.Attach(ninja); //Gets EF to WATCH ninja, but sees it as a new Ninja by default.
                context.Entry(ninja).State = EntityState.Modified; //Makes the Context realize that ninja should ne Updated.
                context.SaveChanges(); //SQL updates all fields because changes were mad outside of the Context. While EF was told ninja was modified, it doesn't know what was modified.
                //This behavior when working with disconnected objects is not elegant, and there are workarounds.
            }

        }

        private static void RetrieveDataWithFind()
        {
            var keyVal = 4;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninja = context.Ninjas.Find(keyVal); //Find() retrieves an object with a SQL query using a Key Value. Fails if there are multiple matches.
                Console.WriteLine($"After Find#1: {ninja.Name}");

                var someNinja = context.Ninjas.Find(keyVal); //Find() won't requery the database if the object already exists in the Context. Uses the object that is already retieved to memory.
                Console.WriteLine($"After Find#2: {someNinja.Name}");
                ninja = null;
            }
        }

        private static void RetrieveDataWithStoredProc()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjas = context.Ninjas.SqlQuery("exec GetOldNinjas"); //Proc won't actually execute until iteration.
                foreach (var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
            }
        }

        private static void DeleteNinja()
        {
            ////All in one block for a simple detete.
            //using (var context = new NinjaContext())
            //{
            //    context.Database.Log = Console.WriteLine;
            //    var ninja = context.Ninjas.FirstOrDefault();
            //    context.Ninjas.Remove(ninja);
            //    context.SaveChanges(); //SQL deletes using key value of ninja.
            //}

            ////For more complex apps, you'll want to close the context in between retrieving and deleting.
            //Ninja ninja;
            //using (var context = new NinjaContext()) //First context is used for retieval only.
            //{
            //    context.Database.Log = Console.WriteLine;
            //    ninja = context.Ninjas.FirstOrDefault();                
            //}
            ////Perform additional operations here.
            //using (var context = new NinjaContext()) //Second context is used to delete only.
            //{
            //    context.Database.Log = Console.WriteLine;
            //    context.Ninjas.Attach(ninja); //Need to attach ninja to the context because this is a new context.
            //    context.Ninjas.Remove(ninja);
            //    context.SaveChanges(); //SQL deletes using key value of ninja.
            //}

            //Attaching to remove is wierd, so the folowing way is preferred over the previous.
            Ninja ninja;
            using (var context = new NinjaContext()) //First context is used for retieval only.
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
            }
            //Perform additional operations here.
            using (var context = new NinjaContext()) //Second context is used to delete only.
            {
                context.Database.Log = Console.WriteLine;
                context.Entry(ninja).State = EntityState.Deleted; //Mark ninja record for deletion in the new context.
                context.SaveChanges(); //SQL deletes using key value of ninja.
            }
        }

        private static void DeleteNinjaWithKeyValue() //If you have the object key you can use it directly, but this way takes 2 round trips to the DB.
        {
            var keyVal = 1;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.Find(keyVal); //round trip 1
                context.Ninjas.Remove(ninja);
                context.SaveChanges(); //round trip 2
            }
        }

        private static void DeleteNinjaWithStoredProcedure() //Using the object key and a stored proc only needs one trip to the db.
        {
            var keyVal = 3;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Database.ExecuteSqlCommand($"exec DeleteNinjaViaId {keyVal}"); //can pass multiple parameters by puttin spaces in between them.
            }
        }

        private static void InsertNinjaWithEquipment() //3 sql inserts all take place in the same connection and transaction.
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninja = new Ninja
                {
                    Name = "Kacy Catanzaro",
                    ServedInOniwaban = false,
                    DateOfBirth = new DateTime(1990, 1, 14),
                    ClanId = 1
                };
                var muscles = new NinjaEquipment
                {
                    Name = "Muscles",
                    Type = EquipmentType.Tool,
                };
                var spunk = new NinjaEquipment
                {
                    Name = "Spunk",
                    Type = EquipmentType.Weapon,
                };
                context.Ninjas.Add(ninja);
                ninja.EquipmentOwned.Add(muscles);
                ninja.EquipmentOwned.Add(spunk);
                context.SaveChanges();
            }
        }

        private static void SimpleNinjaGraphQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                //Eager Loading example using Include method. Gets all the equipment right away.
                var ninja = context.Ninjas.Include(n => n.EquipmentOwned).FirstOrDefault(n => n.Name.StartsWith("Kacy"));

                ////Explicit Loading example using the collection method.
                //var ninja = context.Ninjas.FirstOrDefault(n => n.Name.StartsWith("Kacy")); //round trip 1
                //Console.WriteLine($"Ninja retrieved: {ninja.Name}");
                //context.Entry(ninja).Collection(n => n.EquipmentOwned).Load(); //round trip 2

                ////Lazy Loading example. Only works for properties that are virtual. This returns a count of 0 if EquipmentOwned is not virtual.
                //var ninja = context.Ninjas.FirstOrDefault(n => n.Name.StartsWith("Kacy")); //round trip 1
                //Console.WriteLine($"Ninja retrieved: {ninja.Name}");
                //Console.WriteLine($"Ninja equipment count: {ninja.EquipmentOwned.Count()}"); //round trip 2
                ////Warning: while lazy loading fetches property data only as needed, it will need to perform a seperate query for each parent object record. 
                ////This can be very slow for a data grids, etc.
            }
        }

    }
}
