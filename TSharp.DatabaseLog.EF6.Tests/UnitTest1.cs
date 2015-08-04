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
    public class UnitTest1
    {
        public class EntityTest
        {
            public EntityTest()
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
        public class TestDb : DbContext
        {
            public TestDb()
                : base("TestDb")
            {

            }

            public DbSet<EntityTest> TestTable { get; set; }


        }

        StringBuilder sb = new StringBuilder();
        [TestInitialize()]
        public void Init()
        {
            DbConfiguration.SetConfiguration(new TSharpDbConfiguration());
            TSharpDatabaseLogger logger = new TSharpDatabaseLogger(new StringWriter(sb));
            logger.StartLogging();

            Database.SetInitializer<TestDb>(new CreateDatabaseIfNotExists<TestDb>());

        }


        [TestMethod]
        public void TestLogWithEntityframeworkExtend()
        {
            sb.Clear();
            
            using (var db = new TestDb())
            {
                db.TestTable.Add(new EntityTest()
                {
                    Name = "Name 1"
                });
                db.TestTable.Add(new EntityTest()
                {
                    Name = "Name 2"
                });
                db.TestTable.Add(new EntityTest()
                {
                    Name = "Name 3"
                });
                db.TestTable.Add(new EntityTest()
                {
                    Name = "Name 3"
                });
                db.TestTable.Add(new EntityTest()
                {
                    Name = "Name 3"
                });
                db.SaveChanges();
            }


            using (var db = new TestDb())
            {
                db.TestTable.AsNoTracking().Where(x => x.Name=="Name 3").Delete();
            }

            using (var db = new TestDb())
            {
                db.TestTable.AsNoTracking().Where(x => x.Name == "Name 3").Delete();
            } 

            using (var db = new TestDb())
            {
                db.TestTable.Where(x => x.Name == "Name 2").Delete();
            }

            using (var db = new TestDb())
            {
                db.TestTable.Where(x => x.Name == "Name 2").Delete();
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
