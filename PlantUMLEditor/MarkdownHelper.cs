using Microsoft.VisualStudio.Text;

namespace PlantUMLEditor;
public static class MarkdownHelper
{
    public static Document GetDocument(this ITextBuffer buffer)
    {
        return buffer.Properties.GetOrCreateSingletonProperty(() =>
        {
            var doc = new Document();

            doc.GetText = () => buffer.CurrentSnapshot.GetText();

            doc.ParseAsync().FireAndForget();

            return doc;
        });
    }
}