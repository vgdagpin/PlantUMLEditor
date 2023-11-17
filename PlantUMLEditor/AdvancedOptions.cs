using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PlantUMLEditor;

internal class OptionsProvider
{
    [ComVisible(true)]
    public class AdvancedOptions : BaseOptionPage<PlantUMLEditor.AdvancedOptions> { }
}

public class AdvancedOptions : BaseOptionModel<AdvancedOptions>, IRatingConfig
{
    private string path;

    [Category("Preview Window")]
    [DisplayName("Enable preview window")]
    [Description("Determines if the preview window should be shown.")]
    [DefaultValue(true)]
    public bool EnablePreviewWindow { get; set; } = true;

    [Category("Preview Window")]
    [DisplayName("Render Type")]
    [Description("Determines if PUML will render locally or thru remote. Requires re-opening document to take effect.")]
    [DefaultValue(RenderType.Remote)]
    [TypeConverter(typeof(EnumConverter))]
    public RenderType RenderType { get; set; } = RenderType.Remote;

    [Category("Preview Window")]
    [DisplayName(".Jar Path")]
    [Description("Path to .jar if local, download the latest from here: https://plantuml.com/download")]
    public string Path
    {
        get
        {
            if (path == null)
            {
                var vsixPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = System.IO.Path.Combine(vsixPath, "PlantUML", "plantuml.jar");
            }

            return path;
        }
        set
        {
            path = value;
        }
    }

    [Category("Preview Window")]
    [DisplayName("Remote Url")]
    [Description("Remote Url (or create your own container app: https://hub.docker.com/r/vgdagpin/plantumlserver)")]
    [DefaultValue("https://puml-server-ca.wittygrass-142105d5.southeastasia.azurecontainerapps.io/")]
    public string RemoteUrl { get; set; } = "https://puml-server-ca.wittygrass-142105d5.southeastasia.azurecontainerapps.io/";

    [Category("Preview Window")]
    [DisplayName("Preview window width")]
    [Description("The width in pixels of the preview window.")]
    [DefaultValue(500)]
    [Browsable(false)] // hidden
    public int PreviewWindowWidth { get; set; } = 500;

    [Browsable(false)]
    public int RatingRequests { get; set; }
}

public enum RenderType
{
    Local,
    Remote,
}