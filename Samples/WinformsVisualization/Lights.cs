using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MagicLedTests;

namespace WinformsVisualization
{
    public partial class Lights : Form
    {
        public LightsController LightsController { get; set; }
        private List<LightControl> _lightControls = new List<LightControl>();
        public Lights(LightsController controller)

        {
            LightsController = controller;
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearControls();
            LightsController.Refresh();
            LoadControls();
        }

        private void Lights_Load(object sender, EventArgs e)
        {
            ClearControls();

            LoadControls();
        }

        private void LoadControls()
        {
            LightsController.Lights.ForEach(l =>
            {
                var lc = new LightControl(l);
                _lightControls.Add(lc);
                tblLayout.Controls.Add(lc);
            });
        }

        private void ClearControls()
        {
            _lightControls.ForEach(lc => { tblLayout.Controls.Remove(lc); }
            );
            _lightControls.Clear();
        }
    }
}
