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

            var str = File.ReadAllText(testData);

            var doc = new TestDocument();

            doc.GetText = () => str;

            await doc.ParseAsync();

            Assert.AreEqual(File.ReadAllText(testOutput), doc.ParsedResult);
        }

        public class TestDocument : Document
        {
            protected override async Task<string> RenderPlantUMLAsync(string data)
            {
                var testPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var plantUml = Path.Combine(testPath, "PlantUML", "plantuml.jar");

                var plantUmlRenderer = new RendererFactory()
                        .CreateRenderer(new PlantUmlSettings
                        {
                            JavaPath = plantUml
                        });

                var bytes = await plantUmlRenderer.RenderAsync(data, OutputFormat.Svg);

                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
