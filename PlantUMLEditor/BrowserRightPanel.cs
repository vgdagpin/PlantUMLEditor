using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlantUMLEditor
{
    public class BrowserRightPanel : DockPanel, IWpfTextViewMargin
    {
        private readonly Document document;
        private readonly ITextView textView;
        private bool isDisposed;
        private static readonly Debouncer debouncer = new Debouncer();

        public FrameworkElement VisualElement => this;
        public double MarginSize => AdvancedOptions.Instance.PreviewWindowWidth;
        public bool Enabled => true;
        public Browser Browser { get; private set; }

        public BrowserRightPanel(ITextView textview)
        {
            textView = textview;
            document = textview.TextBuffer.GetDocument();

            SetResourceReference(BackgroundProperty, VsBrushes.ToolWindowBackgroundKey);

            Browser = new Browser(document);
            Browser.WebView2.CoreWebView2InitializationCompleted += OnBrowserInitCompleted;

            CreateMarginControls(Browser.WebView2);
        }

        private void OnBrowserInitCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                throw e.InitializationException;
            }

            WebView2 view = sender as WebView2;

            view.SetResourceReference(BackgroundProperty, VsBrushes.ToolWindowBackgroundKey);

            document.Parsed += UpdateBrowser;
        }

        private void CreateMarginControls(WebView2 view)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(MarginSize, GridUnitType.Pixel), MinWidth = 150 });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.SetResourceReference(BackgroundProperty, VsBrushes.ToolWindowBackgroundKey);

            Children.Add(grid);

            grid.Children.Add(view);
            Grid.SetColumn(view, 2);
            Grid.SetRow(view, 0);

            var splitter = new GridSplitter()
            {
                Width = 5,
                ResizeDirection = GridResizeDirection.Columns,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            splitter.SetResourceReference(BackgroundProperty, VsBrushes.ToolWindowBackgroundKey);
            splitter.DragCompleted += SplitterDragCompleted;

            grid.Children.Add(splitter);
            Grid.SetColumn(splitter, 1);
            Grid.SetRow(splitter, 0);

            var fixWidth = new Action(() =>
            {
                // previewWindow maxWidth = current total width - textView minWidth
                double newWidth = (textView.ViewportWidth + grid.ActualWidth) - 150;

                // preveiwWindow maxWidth < previewWindow minWidth
                if (newWidth < 150)
                {
                    // Call 'get before 'set for performance
                    if (grid.ColumnDefinitions[2].MinWidth != 0)
                    {
                        grid.ColumnDefinitions[2].MinWidth = 0;
                        grid.ColumnDefinitions[2].MaxWidth = 0;
                    }
                }
                else
                {
                    grid.ColumnDefinitions[2].MaxWidth = newWidth;
                    // Call 'get before 'set for performance
                    if (grid.ColumnDefinitions[2].MinWidth == 0)
                    {
                        grid.ColumnDefinitions[2].MinWidth = 150;
                    }
                }
            });

            // Listen sizeChanged event of both marginGrid and textView
            grid.SizeChanged += (e, s) => fixWidth();
            textView.ViewportWidthChanged += (e, s) => fixWidth();
        }

        private void UpdateBrowser(Document document)
        {
            if (!document.IsParsing)
            {
                _ = ThreadHelper.JoinableTaskFactory.StartOnIdle(() =>
                {
                    debouncer.Debounce(() => { _ = Browser.UpdateBrowserAsync(); });

                }, VsTaskRunContext.UIThreadIdlePriority);
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return this;
        }

        private void SplitterDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!double.IsNaN(Browser.WebView2.ActualWidth))
            {
                AdvancedOptions.Instance.PreviewWindowWidth = (int)Browser.WebView2.ActualWidth;
                AdvancedOptions.Instance.Save();
            }
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            Browser.WebView2.CoreWebView2InitializationCompleted -= OnBrowserInitCompleted;
            document.Parsed -= UpdateBrowser;

            Browser.Dispose();

            isDisposed = true;
        }
    }
}
