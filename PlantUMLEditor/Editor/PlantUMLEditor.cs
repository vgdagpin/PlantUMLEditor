using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

using System.ComponentModel.Composition;

namespace PlantUMLEditor.Editor;

[Export(typeof(IWpfTextViewCreationListener))]
[ContentType(Constants.LanguageName)]
[TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
public class PlantUMLEditor : WpfTextViewCreationListener
{
    private DocumentView docView;
    private Document document;

    [Import] internal IBufferTagAggregatorFactoryService _bufferTagAggregator = null;

    protected override void Created(DocumentView docView)
    {
        this.docView = docView;
        this.docView.Document.FileActionOccurred += Document_FileActionOccurred;

        this.docView.TextView.Options.SetOptionValue(DefaultTextViewHostOptions.GlyphMarginName, false);
        this.docView.TextView.Options.SetOptionValue(DefaultTextViewHostOptions.SelectionMarginName, true);
        this.docView.TextView.Options.SetOptionValue(DefaultTextViewHostOptions.ShowEnhancedScrollBarOptionName, false);

        this.document = docView.TextBuffer.GetDocument();
    }

    private void Document_FileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
        {
            document.ParseAsync().FireAndForget();
        }
    }

    protected override void Closed(IWpfTextView textView)
    {
        docView.Document.FileActionOccurred -= Document_FileActionOccurred;
    }
}    