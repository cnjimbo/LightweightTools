using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSharp.CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO; 
using System.Configuration;
using TSharp.DatabaseLog.EF6.Tests;
using System.Data;

namespace TSharp.CsvHelper.Tests
{
    /// <summary>
    /// Summary description for UnitTests
    /// </summary>
    [TestClass]
    public class CsvReadOrWriteTests
    {

        #region Test Data

        private const string TEST_DATA_1 = @"column one,column two,column three
1,data 2,2010-05-01 11:26:01
";

        private const string TEST_DATA_2 = @"""column, one"",column two,""column, three""
data 1,""data, 2"",data 3
";

        private const string TEST_DATA_3 = @"""column, one"",""column """"two"",""column, three""
""data """"1"",""data, 2"",data 3
";

        private const string TEST_DATA_4 = @"""column, one"",""column """"two"",""column, three""
""data """",1"",""data, 2"",data 3
";

        private const string TEST_DATA_5 = @"""column, one"",""column """"two"",""column, three""
""data """""""",1"",""dat""""""""""""sa, 2"",data 3
";

        private const string TEST_DATA_6 = @" column one ,  column two  ,   column three   
   1   ,  data 2  , 2010-05-01 11:26:01 
";

        #endregion Test Data

        

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private static string FilePath
        {
            get
            {
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (!filePath.EndsWith("\\"))
                    filePath += "\\";

                return filePath + "abc123xyz.csv";
            }
        }

        public string Location
        {
            get
            {
                string location = ConfigurationManager.AppSettings["TestFilesPath"];

                if (!location.EndsWith("\\"))
                    location += "\\";

                return location;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [ClassCleanup()]
        public static void Cleanup()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        [TestMethod]
        public void CsvReader_TestReadingFromFile()
        {
            File.WriteAllText(FilePath, TEST_DATA_1, Encoding.Default);

            using (CsvReader reader = new CsvReader(FilePath, Encoding.Default))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData1(csvFile.Headers, csvFile.Records);
            }

            File.Delete(FilePath);
        }

        [TestMethod]
        public void CsvReader_TestReadingFromStream()
        {
            using (MemoryStream memoryStream = new MemoryStream(TEST_DATA_1.Length))
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(TEST_DATA_1);
                    streamWriter.Flush();

                    using (CsvReader reader = new CsvReader(memoryStream, Encoding.Default))
                    {
                        List<List<string>> records = new List<List<string>>();

                        while (reader.ReadNextRecord())
                            records.Add(reader.Fields);

                        Assert.IsTrue(records.Count == 2);

                        CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                        VerifyTestData1(csvFile.Headers, csvFile.Records);
                    }
                }
            }
        }

        [TestMethod]
        public void CsvReader_TestReadingFromString()
        {
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_1))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData1(csvFile.Headers, csvFile.Records);
            }
        }

        [TestMethod]
        public void CsvReader_TestReadIntoDataTableWithTypes()
        {
            DataTable dataTable = new DataTable();

            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_1) { HasHeaderRow = true })
            {
                dataTable = reader.ReadIntoDataTable(new[] { typeof(int), typeof(string), typeof(DateTime) });
            }

            CsvFile file = CreateCsvFileFromDataTable(dataTable);
            VerifyTestData1(file.Headers, file.Records);
        }

        [TestMethod]
        public void CsvReader_TestReadIntoDataTableWithoutTypes()
        {
            DataTable dataTable = new DataTable();

            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_1) { HasHeaderRow = true })
            {
                dataTable = reader.ReadIntoDataTable();
            }

            CsvFile file = CreateCsvFileFromDataTable(dataTable);
            VerifyTestData1(file.Headers, file.Records);
        }

        [TestMethod]
        public void CsvReader_TestReadingFromStringWithSampleData2()
        {
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_2))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData2(csvFile.Headers, csvFile.Records);
            }
        }

        [TestMethod]
        public void CsvReader_TestReadingFromStringWithSampleData3()
        {
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_3))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData3(csvFile.Headers, csvFile.Records);
            }
        }

        [TestMethod]
        public void CsvReader_TestReadingFromStringWithSampleData4()
        {
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_4))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData4(csvFile.Headers, csvFile.Records);
            }
        }

        [TestMethod]
        public void CsvReader_TestReadingFromStringWithSampleData5()
        {
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_5))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData5(csvFile.Headers, csvFile.Records);
            }
        }

        [TestMethod]
        public void CsvReader_TestReadingFromStringWithSampleData6()
        {
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_6))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData6(csvFile.Headers, csvFile.Records);
            }
        }

        [TestMethod]
        public void CsvReader_TestColumnTrimming()
        {
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_6) { TrimColumns = true })
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                Assert.IsTrue(records.Count == 2);

                CsvFile csvFile = CreateCsvFile(records[0], records[1]);
                VerifyTestData6Trimmed(csvFile.Headers, csvFile.Records);
            }
        }

        [TestMethod]
        public void CsvFile_PopulateFromFileWithHeader()
        {
            CsvFile csvFile1 = new CsvFile();
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_5))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                csvFile1 = CreateCsvFile(records[0], records[1]);
            }

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            using (CsvWriter writer = new CsvWriter())
            {
                writer.WriteCsv(csvFile1, FilePath, Encoding.Default);
            }

            CsvFile file = new CsvFile();
            file.Populate(FilePath, true);
            VerifyTestData5(file.Headers, file.Records);

            File.Delete(FilePath);
        }

        [TestMethod]
        public void CsvFile_PopulateFromFileWithoutHeader()
        {
            CsvFile csvFile1 = new CsvFile();
            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_5))
            {
                List<List<string>> records = new List<List<string>>();

                while (reader.ReadNextRecord())
                    records.Add(reader.Fields);

                csvFile1 = CreateCsvFile(records[0], records[1]);
            }

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            using (CsvWriter writer = new CsvWriter())
            {
                writer.WriteCsv(csvFile1, FilePath, Encoding.Default);
            }

            CsvFile file = new CsvFile();
            file.Populate(FilePath, false);
            VerifyTestData5Alternative(file.Records);

            File.Delete(FilePath);
        }

        [TestMethod]
        public void CsvFile_PopulateFromStream()
        {
            using (MemoryStream memoryStream = new MemoryStream(TEST_DATA_5.Length))
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(TEST_DATA_5);
                    streamWriter.Flush();

                    CsvFile file = new CsvFile();
                    file.Populate(memoryStream, true);
                    VerifyTestData5(file.Headers, file.Records);
                }
            }
        }

        [TestMethod]
        public void CsvFile_PopulateFromString()
        {
            CsvFile file = new CsvFile();
            file.Populate(true, TEST_DATA_5);
            VerifyTestData5(file.Headers, file.Records);
        }

        [TestMethod]
        public void CsvFile_Indexers()
        {
            CsvFile file = new CsvFile();
            file.Populate(true, TEST_DATA_2);

            Assert.IsTrue(file[0] == file.Records[0]);
            Assert.IsTrue(string.Compare(file[0, 1], "data, 2") == 0);
            Assert.IsTrue(string.Compare(file[0, "column two"], "data, 2") == 0);

        }

        [TestMethod]
        public void CsvWriter_WriteCsvFileObjectToFile()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            CsvFile csvFile = new CsvFile();
            csvFile.Populate(true, TEST_DATA_5);

            using (CsvWriter writer = new CsvWriter())
            {
                writer.WriteCsv(csvFile, FilePath);
            }

            csvFile = new CsvFile();
            csvFile.Populate(FilePath, true);

            VerifyTestData5(csvFile.Headers, csvFile.Records);

            File.Delete(FilePath);
        }

        [TestMethod]
        public void CsvWriter_WriteCsvFileObjectToStream()
        {
            string content = string.Empty;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                CsvFile csvFile = new CsvFile();
                csvFile.Populate(true, TEST_DATA_5);

                using (CsvWriter writer = new CsvWriter())
                {
                    writer.WriteCsv(csvFile, memoryStream);
                    using (StreamReader reader = new StreamReader(memoryStream))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }

            Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
        }

        [TestMethod]
        public void CsvWriter_WriteCsvFileObjectToString()
        {
            CsvFile csvFile = new CsvFile();
            csvFile.Populate(true, TEST_DATA_5);
            string content = string.Empty;

            using (CsvWriter writer = new CsvWriter())
            {
                content = writer.WriteCsv(csvFile, Encoding.Default);
            }

            Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
        }

        [TestMethod]
        public void CsvWriter_WriteDataTableToFile()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            DataTable table = new DataTable();

            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_5))
            {
                table = reader.ReadIntoDataTable();
            }

            using (CsvWriter writer = new CsvWriter())
            {
                writer.WriteCsv(table, FilePath);
            }

            CsvFile csvFile = CreateCsvFileFromDataTable(table);
            VerifyTestData5(csvFile.Headers, csvFile.Records);
            File.Delete(FilePath);
        }

        [TestMethod]
        public void CsvWriter_WriteDataTableToStream()
        {
            string content = string.Empty;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataTable table = new DataTable();

                using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_5))
                {
                    table = reader.ReadIntoDataTable();
                }

                using (CsvWriter writer = new CsvWriter())
                {
                    writer.WriteCsv(table, memoryStream);

                    using (StreamReader reader = new StreamReader(memoryStream))
                    {
                        content = reader.ReadToEnd();
                    }
                }

            }

            Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
        }

        [TestMethod]
        public void CsvWriter_WriteDataTableToString()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            DataTable table = new DataTable();

            using (CsvReader reader = new CsvReader(Encoding.Default, TEST_DATA_5))
            {
                table = reader.ReadIntoDataTable();
            }

            string content = string.Empty;

            using (CsvWriter writer = new CsvWriter())
            {
                content = writer.WriteCsv(table, Encoding.Default);
            }

            File.Delete(FilePath);
            Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
        }

        [TestMethod]
        public void CsvWriter_VerifyThatCarriageReturnsAreHandledCorrectlyInFieldValues()
        {
            CsvFile csvFile = new CsvFile();
            csvFile.Headers.Add("header ,1");
            csvFile.Headers.Add("header\r\n2");
            csvFile.Headers.Add("header 3");

            CsvRecord record = new CsvRecord();
            record.Fields.Add("da,ta 1");
            record.Fields.Add("\"data\" 2");
            record.Fields.Add("data\n3");
            csvFile.Records.Add(record);

            string content = string.Empty;

            using (CsvWriter writer = new CsvWriter())
            {
                content = writer.WriteCsv(csvFile, Encoding.Default);
            }

            Assert.IsTrue(string.Compare(content, "\"header ,1\",\"header,2\",header 3\r\n\"da,ta 1\",\"\"\"data\"\" 2\",\"data,3\"\r\n") == 0);

            using (CsvWriter writer = new CsvWriter() { ReplaceCarriageReturnsAndLineFeedsFromFieldValues = false })
            {
                content = writer.WriteCsv(csvFile, Encoding.Default);
            }

            Assert.IsTrue(string.Compare(content, "\"header ,1\",header\r\n2,header 3\r\n\"da,ta 1\",\"\"\"data\"\" 2\",data\n3\r\n") == 0);


        }

        private CsvFile CreateCsvFileFromDataTable(DataTable table)
        {
            CsvFile file = new CsvFile();

            foreach (DataColumn column in table.Columns)
                file.Headers.Add(column.ColumnName);

            foreach (DataRow row in table.Rows)
            {
                CsvRecord record = new CsvRecord();

                foreach (object o in row.ItemArray)
                {
                    if (o is DateTime)
                        record.Fields.Add(((DateTime)o).ToString("yyyy-MM-dd hh:mm:ss"));
                    else
                        record.Fields.Add(o.ToString());
                }

                file.Records.Add(record);
            }

            return file;
        }

        private CsvFile CreateCsvFile(List<string> headers, List<string> fields)
        {
            CsvFile csvFile = new CsvFile();

            headers.ForEach(header => csvFile.Headers.Add(header));
            CsvRecord record = new CsvRecord();
            fields.ForEach(field => record.Fields.Add(field));
            csvFile.Records.Add(record);
            return csvFile;
        }

        #region Verification methods

        private void VerifyTestData1(List<string> headers, CsvRecords records)
        {
            Assert.IsTrue(headers.Count == 3);
            Assert.IsTrue(records.Count == 1);
            Assert.AreEqual("column one", headers[0]);
            Assert.AreEqual("column two", headers[1]);
            Assert.AreEqual("column three", headers[2]);
            Assert.AreEqual("1", records[0].Fields[0]);
            Assert.AreEqual("data 2", records[0].Fields[1]);
            Assert.AreEqual("2010-05-01 11:26:01", records[0].Fields[2]);
        }

        private void VerifyTestData2(List<string> headers, CsvRecords records)
        {
            Assert.IsTrue(headers.Count == 3);
            Assert.IsTrue(records.Count == 1);
            Assert.AreEqual("column, one", headers[0]);
            Assert.AreEqual("column two", headers[1]);
            Assert.AreEqual("column, three", headers[2]);
            Assert.AreEqual("data 1", records[0].Fields[0]);
            Assert.AreEqual("data, 2", records[0].Fields[1]);
            Assert.AreEqual("data 3", records[0].Fields[2]);
        }

        private void VerifyTestData3(List<string> headers, CsvRecords records)
        {
            Assert.IsTrue(headers.Count == 3);
            Assert.IsTrue(records.Count == 1);
            Assert.AreEqual("column, one", headers[0]);
            Assert.AreEqual("column \"two", headers[1]);
            Assert.AreEqual("column, three", headers[2]);
            Assert.AreEqual("data \"1", records[0].Fields[0]);
            Assert.AreEqual("data, 2", records[0].Fields[1]);
            Assert.AreEqual("data 3", records[0].Fields[2]);
        }

        private void VerifyTestData4(List<string> headers, CsvRecords records)
        {
            Assert.IsTrue(headers.Count == 3);
            Assert.IsTrue(records.Count == 1);
            Assert.AreEqual("column, one", headers[0]);
            Assert.AreEqual("column \"two", headers[1]);
            Assert.AreEqual("column, three", headers[2]);
            Assert.AreEqual("data \",1", records[0].Fields[0]);
            Assert.AreEqual("data, 2", records[0].Fields[1]);
            Assert.AreEqual("data 3", records[0].Fields[2]);
        }

        private void VerifyTestData5(List<string> headers, CsvRecords records)
        {
            Assert.IsTrue(headers.Count == 3);
            Assert.IsTrue(records.Count == 1);
            Assert.AreEqual("column, one", headers[0]);
            Assert.AreEqual("column \"two", headers[1]);
            Assert.AreEqual("column, three", headers[2]);
            Assert.AreEqual("data \"\",1", records[0].Fields[0]);
            Assert.AreEqual("dat\"\"\"sa, 2", records[0].Fields[1]);
            Assert.AreEqual("data 3", records[0].Fields[2]);
        }

        private void VerifyTestData5Alternative(CsvRecords records)
        {
            Assert.IsTrue(records.Count == 2);
            Assert.AreEqual("column, one", records[0].Fields[0]);
            Assert.AreEqual("column \"two", records[0].Fields[1]);
            Assert.AreEqual("column, three", records[0].Fields[2]);
            Assert.AreEqual("data \"\",1", records[1].Fields[0]);
            Assert.AreEqual("dat\"\"\"sa, 2", records[1].Fields[1]);
            Assert.AreEqual("data 3", records[1].Fields[2]);
        }

        private void VerifyTestData6(List<string> headers, CsvRecords records)
        {
            Assert.IsTrue(headers.Count == 3);
            Assert.IsTrue(records.Count == 1);
            Assert.AreEqual(" column one ", headers[0]);
            Assert.AreEqual("  column two  ", headers[1]);
            Assert.AreEqual("   column three   ", headers[2]);
            Assert.AreEqual("   1   ", records[0].Fields[0]);
            Assert.AreEqual("  data 2  ", records[0].Fields[1]);
            Assert.AreEqual(" 2010-05-01 11:26:01 ", records[0].Fields[2]);
        }

        private void VerifyTestData6Trimmed(List<string> headers, CsvRecords records)
        {
            Assert.IsTrue(headers.Count == 3);
            Assert.IsTrue(records.Count == 1);
            Assert.AreEqual("column one", headers[0]);
            Assert.AreEqual("column two", headers[1]);
            Assert.AreEqual("column three", headers[2]);
            Assert.AreEqual("1", records[0].Fields[0]);
            Assert.AreEqual("data 2", records[0].Fields[1]);
            Assert.AreEqual("2010-05-01 11:26:01", records[0].Fields[2]);
        }

        [TestMethod]
        public void CSVWirteReadLargeFile()
        {
            var field =   "\"'|\\/?>,<";
            var filePath = RootFileNameAndEnsureTargetFolderExists("App_Data/csv" + DateTimeOffset.UtcNow.ToFileTime() + ".csv");
            var csvFile = new CsvFile();
            
            foreach (var r in Enumerable.Range(1, 10000).Select(i =>
            {
                var rec = new CsvRecord();
                
                foreach (var r in Enumerable.Range(1, 80).Select(s =>field ))
                {
                    rec.Fields.Add(r);
                }

                return rec;
            }))
            {
                csvFile.Records.Add(r);
            }
            using (CsvWriter wr = new CsvWriter())
            {
                wr.WriteCsv(csvFile, filePath);
            }
            string filed1 = "";
            using (var reader = new CsvReader(filePath))
            {
                if (reader.ReadNextRecord())
                    filed1 = reader.Fields[0];
            }
            Assert.AreEqual(filed1, field);
        }

        #endregion Verification methods

        [TestMethod]
        public void MyTestMethod()
        {
            
        }

        

        private static string RootFileNameAndEnsureTargetFolderExists(string fileName)
        {
            string rootedFileName = fileName;
            if (!Path.IsPathRooted(rootedFileName))
            {
                rootedFileName = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, rootedFileName);
            }

            string directory = Path.GetDirectoryName(rootedFileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return rootedFileName;
        }
    }


}
