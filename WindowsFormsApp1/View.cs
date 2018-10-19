//*************************************************************************************************************************************************
//*** David Kenyon's Screenshot Project
//*** Ruush: A streamlined screenshot capturing software
//*** Class: View.cs
//*** Created: April 2 2018
//*** Last Updated: August 7 2018
//*************************************************************************************************************************************************

using System;
using System.Windows.Forms;

namespace RuushApplication
{
    public partial class View : Form  
    {
        public View()
        {
            InitializeComponent();
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            //Mnemonic for screen capture
            ToolTip1.SetToolTip(this.button1, "ALT+S");
        }

        private void ScreenshotButton(object sender, EventArgs e)
        {
            Controller ctrl = new Controller();
            
            //Create new directory if necessary
            ctrl.CreateDir();

            //Make form invisible for screenshot capturing
            this.Opacity = 0;

            //Capture screenshot
            ctrl.TakeScreenshot();

            //Make form visible again
            this.Opacity = 1;
            
            //Move to dialog box stage for painting and uploading
            DialogResult dialogResultPaint = MessageBox.Show("Would you like to edit your screenshot in paint?", "Edit", MessageBoxButtons.YesNo);
            if (dialogResultPaint == DialogResult.Yes)
            {
                ctrl.PaintAndUpload();
            }
            else if (dialogResultPaint == DialogResult.No)
            {
                //do something else
            }
            
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void View_Load(object sender, EventArgs e)
        {

        }
    }
}
