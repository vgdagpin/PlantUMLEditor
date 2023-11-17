using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlantUml.Net;

using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlantUMLEditor.Tests
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public async Task TestMarkdownParseWithPlantUML()
        {
            var testPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var testData = Path.Combine(testPath, "TestData", "Test1.input.txt");
            var testOutput = Path.Combine(testPath, "TestData", "Test1.output.txt");


            var testOutput1 = Path.Combine(testPath, "TestData", "Test2.output.txt");
            var testOutput2 = Path.Combine(testPath, "TestData", "Test3.output.txt");

            var str = File.ReadAllText(testData);

            var doc = new TestDocument();

            doc.GetText = () => str;

            await doc.ParseAsync();

            var expected = File.ReadAllText(testOutput).Trim();
            var result = doc.ParsedResult.Trim();

            File.WriteAllText(testOutput1, expected);
            File.WriteAllText(testOutput2, result);

            Assert.AreEqual(expected, result);
        }

        public class TestDocument : Document
        {
            protected override Task<string> RenderPlantUMLAsync(string data)
            {
                return Task.FromResult(data);
            }
        }
    }
}
