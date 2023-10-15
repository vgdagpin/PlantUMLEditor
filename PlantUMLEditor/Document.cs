using PlantUml.Net;

using System.IO;
using System.Reflection;
using System.Text;

namespace PlantUMLEditor
{
    public class Document : IDisposable
    {
        private IPlantUmlRenderer plantUmlRenderer;

        protected virtual IPlantUmlRenderer PlantUmlRenderer
        {
            get
            {
                if (plantUmlRenderer == null)
                {
                    var factory = new RendererFactory();
                    var vsixPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var plantUml = Path.Combine(vsixPath, "PlantUML", "plantuml.jar");

                    plantUmlRenderer = factory.CreateRenderer(new PlantUmlSettings
                    {
                        JavaPath = plantUml
                    });
                }

                return plantUmlRenderer;
            }
        }

        public Func<string> GetText { get; set; }

        public bool IsParsing { get; private set; }

        public string ParsedPlantUML { get; private set; }

        public event Action<Document> Parsed;

        private string currentText;

        public async Task ParseAsync()
        {
            IsParsing = true;
            bool success = false;

            try
            {
                var text = GetText?.Invoke()?.Trim();

                if (currentText != text)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var bytes = await PlantUmlRenderer.RenderAsync(text, OutputFormat.Svg);
                        ParsedPlantUML = ASCIIEncoding.UTF8.GetString(bytes);
                    }
                    else
                    {
                        ParsedPlantUML = "Empty";
                    }

                    currentText = text;
                    success = true;
                }
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
            }
            finally
            {
                IsParsing = false;

                if (success)
                {
                    Parsed?.Invoke(this);
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
