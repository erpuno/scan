
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TWAIN32;

namespace INFOTECH
{
    public partial class FormSelect : Form
    {
        public FormSelect(List<string> a_lszIdentity, string a_szDefault)
        {
            string[] aszIdentity;
            string[] aszDefault;
            InitializeComponent();
            aszDefault = CSV.Parse(a_szDefault);
            m_listboxSelect.BeginUpdate();
            foreach (string sz in a_lszIdentity)
            {
                aszIdentity = CSV.Parse(sz);
                m_listboxSelect.Items.Add(aszIdentity[11].ToString());
            }
            m_listboxSelect.SelectedIndex = m_listboxSelect.FindStringExact(aszDefault[11]);
            m_listboxSelect.EndUpdate();
        }

        private string m_szSelected;
        public string GetSelectedDriver() { return (m_szSelected); }
        private void m_buttonOpen_Click(object sender, EventArgs e) { m_szSelected = (string)m_listboxSelect.SelectedItem; }
        private void m_listboxSelect_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_szSelected = (string)m_listboxSelect.SelectedItem;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }
}
