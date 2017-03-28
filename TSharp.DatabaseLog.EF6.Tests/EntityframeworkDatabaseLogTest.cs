namespace TSharp.DatabaseLog.EF6.Tests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Text;

    using EntityFramework.Extensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EntityframeworkDatabaseLogTest
    {
        private readonly StringBuilder sb = new StringBuilder();

        [TestInitialize]
        public void Init()
        {
            MSSqlDbConfiguration.IsLogCommand = true;
            DbConfiguration.SetConfiguration(new MSSqlDbConfiguration());
            var logger = new TSharpDatabaseLogger(new StringWriter(sb));
            logger.StartLogging();

            Database.SetInitializer(new CreateDatabaseIfNotExists<HumanResource>());
        }

        [TestMethod]
        public void TestLogWithEntityframeworkExtend()
        {
            sb.Clear();

            using (var db = new HumanResource())
            {
                db.TestTable.Add(new Person { Name = "Name 1" });
                db.TestTable.Add(new Person { Name = "Name 2" });
                db.TestTable.Add(new Person { Name = "Name 3" });
                db.TestTable.Add(new Person { Name = "Name 3" });
                db.TestTable.Add(new Person { Name = "Name 3" });
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
            using (var db = new HumanResource())
            {
                var q = db.TestTable.Where(x => x.Name != "Name 2");
                var q1 = q.FutureCount();
                var q2 = q.OrderBy(x => x.Age).Skip(5).Take(1).Future();

                var v = q1.Value;
            }

            Console.WriteLine(sb.ToString());
        }

    }


    public class Person
    {
        public Person()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class HumanResource : DbContext
    {
        public HumanResource()
            : base("TestDb")
        {
        }

        public DbSet<Person> TestTable { get; set; }
    }
}