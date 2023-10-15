using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace PlantUMLEditor
{
    public class Browser : IDisposable
    {
        private readonly Document document;

        public WebView2 WebView2 { get; private set; }

        public string HtmlTemplate => 
@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=Edge"" />
    <meta charset=""utf-8"" />

    <title>PlantUML Editor</title>

    <meta http-equiv=""cache-control"" content=""no-cache"">
</head>
<body>
    <div id=""___plantuml___"">
        [content]
    </div>
</body>
</html>
";

        public Browser(Document document)
        {
            this.document = document;

            WebView2 = new WebView2() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(0), Visibility = Visibility.Hidden };
            WebView2.Initialized += BrowserInitialized;

            WebView2.SetResourceReference(Control.BackgroundProperty, VsBrushes.ToolWindowBackgroundKey);
        }        

        private void BrowserInitialized(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var tempDir = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name);
                var webView2Environment = await CoreWebView2Environment.CreateAsync(browserExecutableFolder: null, userDataFolder: tempDir, options: null);

                await WebView2.EnsureCoreWebView2Async(webView2Environment);

                WebView2.Visibility = Visibility.Visible;

                await UpdateBrowserAsync();
            }).FireAndForget();
        }

        private async Task<bool> IsHtmlTemplateLoadedAsync()
        {
            var hasContentResult = await WebView2.ExecuteScriptAsync($@"document.getElementById(""___plantuml___"") !== null;");

            return hasContentResult == "true";
        }

        public async Task UpdateBrowserAsync()
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var html = document.ParsedPlantUML;

                if (await IsHtmlTemplateLoadedAsync())
                {
                    html = html.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"");
                    await WebView2.ExecuteScriptAsync($@"document.getElementById(""___plantuml___"").innerHTML=""{html}"";");
                }
                else
                {
                    var htmlTemplate = HtmlTemplate;

                    html = string.Format(CultureInfo.InvariantCulture, "{0}", html);
                    html = htmlTemplate.Replace("[content]", html);

                    WebView2.NavigateToString(html);
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            WebView2.Initialized -= BrowserInitialized;
            WebView2.Dispose();
        }
    }
}
