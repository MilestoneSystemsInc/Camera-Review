using System;
using VideoOS.Platform;
using VideoOS.Platform.Admin;

namespace Camera_Review.Admin
{
    public partial class Camera_ReviewTabUserControl : VideoOS.Platform.Admin.TabUserControl
    {
        private Item _associatedItem;
        private bool _ignoreChanged = false;
        private AssociatedProperties _associatedProperties;

        public Camera_ReviewTabUserControl(Item item)
        {
            InitializeComponent();

            _associatedItem = item;
            labelItemName.Text = item.Name;
        }

        public override void Init()
        {
            base.Init();
            _ignoreChanged = true;
            textBox1.Text = "";
            textBox2.Text = "";

            _associatedProperties = Configuration.Instance.GetAssociatedProperties(_associatedItem, Camera_ReviewDefinition.Camera_ReviewTabPluginId);

            if (_associatedProperties.Properties.ContainsKey("Property1"))
                textBox1.Text = _associatedProperties.Properties["Property1"];

            if (_associatedProperties.Properties.ContainsKey("Property2"))
                textBox2.Text = _associatedProperties.Properties["Property2"];
            _ignoreChanged = false;
        }

        public override void Close()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            base.Close();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!_ignoreChanged)
            {
                _associatedProperties.Properties["Property1"] = textBox1.Text;
                _associatedProperties.Properties["Property2"] = textBox2.Text;
                FireConfigurationChanged();
            }
        }
    }
}
