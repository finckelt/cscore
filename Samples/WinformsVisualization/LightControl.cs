using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MagicLedTests;

namespace WinformsVisualization
{
    public partial class LightControl : UserControl
    {

        public Light Light { get; set; }

        public LightControl(Light light)
        {
            Light = light;
            InitializeComponent();
            lightBindingSource.DataSource = Light;
            lightBindingSource.ResetBindings(false);

        }

        private void btnCurrentColor_Click(object sender, EventArgs e)
        {
            colorCurrent.Color = Light.CurrentColor;
            this.colorCurrent.ShowDialog();
            this.Light.CurrentColor = colorCurrent.Color;
        }

        private void btnLowFrequencyColor_Click(object sender, EventArgs e)
        {
            colorCurrent.Color = Light.LowColor;
            this.colorCurrent.ShowDialog();
            Light.LowColor = colorCurrent.Color;
        }

        private void btnMidFrequencyColor_Click(object sender, EventArgs e)
        {
            colorCurrent.Color = Light.MidColor;
            this.colorCurrent.ShowDialog();
            Light.MidColor = colorCurrent.Color;
        }

        private void btnHighFrequencyColor_Click(object sender, EventArgs e)
        {
            colorCurrent.Color = Light.HighColor;
            this.colorCurrent.ShowDialog();
            Light.HighColor = colorCurrent.Color;
        }

        private void btnToggleOnOff_Click(object sender, EventArgs e)
        {
            Light.PowerState = !Light.PowerState;

            this.btnToggleOnOff.Text = Light.PowerState ? "Turn Off" : "Turn On";
        }

        private void LightControl_Load(object sender, EventArgs e)
        {
            this.btnToggleOnOff.Text = Light.PowerState ? "Turn Off" : "Turn On";
        }

        private void lightBindingSource_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            this.btnToggleOnOff.Text = Light.PowerState ? "Turn Off" : "Turn On";
        }
    }
}
