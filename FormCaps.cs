using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
namespace INFOTECH
{
    public partial class FormCaps : Form
    {
        public FormScan m_formscan;

        public FormCaps(FormScan parent)
        {
            m_formscan = parent;
            InitializeComponent();
        }
    }
}