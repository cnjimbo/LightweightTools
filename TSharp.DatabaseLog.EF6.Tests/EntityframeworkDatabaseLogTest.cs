using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using EntityFramework;
using System.Linq;
using EntityFramework.Extensions;
using System.Data.Entity.Infrastructure;
namespace TSharp.DatabaseLog.EF6.Tests
{
    [TestClass]
    public class EntityframeworkDatabaseLogTest
    {
        public class Person
        {
            public Person()
            {
                Id = Guid.NewGuid();
            }
            [Key]
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Age
            {
                get;
                set;
            }
        }
        public class HumanResource : DbContext
        {
            public HumanResource()
                : base("TestDb")
            {

            }

            public DbSet<Person> TestTable { get; set; }


        }

        StringBuilder sb = new StringBuilder();
        [TestInitialize()]
        public void Init()
        {
            DbConfiguration.SetConfiguration(new MSSqlDbConfiguration());
            TSharpDatabaseLogger logger = new TSharpDatabaseLogger(new StringWriter(sb));
            logger.StartLogging();

            Database.SetInitializer<HumanResource>(new CreateDatabaseIfNotExists<HumanResource>());

        }


        [TestMethod]
        public void TestLogWithEntityframeworkExtend()
        {
            sb.Clear();

            using (var db = new HumanResource())
            {
                db.TestTable.Add(new Person()
                {
                    Name = "Name 1"
                });
                db.TestTable.Add(new Person()
                {
                    Name = "Name 2"
                });
                db.TestTable.Add(new Person()
                {
                    Name = "Name 3"
                });
                db.TestTable.Add(new Person()
                {
                    Name = "Name 3"
                });
                db.TestTable.Add(new Person()
                {
                    Name = "Name 3"
                });
                db.SaveChanges();
            }


            //according batch operation (update or delete), databaselog can't log any sql statement.

            using (var db = new HumanResource())
            {
                db.TestTable.AsNoTracking().Where(x => x.Name == "Name 3").Delete();
            }

            using (var db = new HumanResource())
            {
                db.TestTable.AsNoTracking().Where(x => x.Name == "Name 3").Delete();
            }

            using (var db = new HumanResource())
            {
                db.TestTable.Where(x => x.Name == "Name 2").Delete();
            }

            using (var db = new HumanResource())
            {
                db.TestTable.Where(x => x.Name == "Name 2").Delete();
            }

            Console.WriteLine(sb.ToString());
        }


    }
}
