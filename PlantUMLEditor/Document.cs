using PlantUml.Net;

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
                    plantUmlRenderer = new RendererFactory()
                        .CreateRenderer(new PlantUmlSettings
                        {
                            JavaPath = AdvancedOptions.Instance.Path
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
                        ParsedPlantUML = Encoding.UTF8.GetString(bytes);
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
