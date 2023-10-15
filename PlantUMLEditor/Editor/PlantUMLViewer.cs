using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

using System.ComponentModel.Composition;

namespace PlantUMLEditor.Editor;

[Export(typeof(IWpfTextViewMarginProvider))]
[Name(nameof(PlantUMLViewer))]
[Order(After = PredefinedMarginNames.RightControl)]
[MarginContainer(PredefinedMarginNames.Right)]
[ContentType(Constants.LanguageName)]
[TextViewRole(PredefinedTextViewRoles.Debuggable)]
public class PlantUMLViewer : IWpfTextViewMarginProvider
{
    public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
    {
        return wpfTextViewHost.TextView.Properties.GetOrCreateSingletonProperty(() => new BrowserRightPanel(wpfTextViewHost.TextView));
    }
}