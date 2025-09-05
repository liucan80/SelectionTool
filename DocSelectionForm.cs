using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using interop.ICApiIronCAD.SelectionTool;
using System.Runtime.InteropServices;

namespace ICApiAddin.SelectionTool
{
    [ComVisible(false)]
    public partial class DocSelectionForm : Form
    {
        //private IZDoc m_iZDoc;
        public IZDoc m_iZDoc;
        public DocSelectionForm()
        {
            InitializeComponent();
        }

        public IZDoc IronCADDocument
        {
            set { m_iZDoc = value; }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DocSelectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ICAddInUtils.StopSelectionTool(m_iZDoc);
        }

        private void DocSelectionForm_Load(object sender, EventArgs e)
        {
            ICAddInUtils.StartSelectionTool(m_iZDoc);
        }
    }
}
