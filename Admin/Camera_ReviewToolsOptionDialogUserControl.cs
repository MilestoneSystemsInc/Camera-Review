using VideoOS.Platform.Admin;

namespace Camera_Review.Admin
{
    public partial class Camera_ReviewToolsOptionDialogUserControl : ToolsOptionsDialogUserControl
    {
        public Camera_ReviewToolsOptionDialogUserControl()
        {
            InitializeComponent();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }

        public string MyPropValue
        {
            set { textBoxPropValue.Text = value ?? ""; }
            get { return textBoxPropValue.Text; }
        }
    }
}
