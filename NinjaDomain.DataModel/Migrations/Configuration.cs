namespace NinjaDomain.DataModel.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    //Created by running 'enable-migrations' in the Package Manager Console.
    //Follow with 'add-migration migrationName' to create a migration file.
    //Then apply the migration with 'update-database' (default is to apply the latest migration).
    //Note important parameters: 'update-database -script' to generate a SQL script to apply manually (will need to save the file).
    //Note important parameters: 'update-database -verbose' to output progress to PM Console 
    //Also creates a migration history table with a hash of the update in the database.

    internal sealed class Configuration : DbMigrationsConfiguration<NinjaDomain.DataModel.NinjaContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NinjaDomain.DataModel.NinjaContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
