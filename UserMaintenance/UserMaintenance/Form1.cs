using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserMaintenance.Entities;

namespace UserMaintenance
{
    public partial class Form1 : Form
    {
        BindingList<User> users = new BindingList<User>();
        public Form1()
        {
            InitializeComponent();
            lblLastName.Text = Resource1.FullName; // label1
            btnAdd.Text = Resource1.Add; // button1
            btnExp.Text = Resource1.Export; //button2
            btnDel.Text = Resource1.Delete; //button3

            listBox1.DataSource = users;
            listBox1.ValueMember = "ID";
            listBox1.DisplayMember = "FullName";


        }

        private void btnExp_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text Files | *.txt";
            sf.ShowDialog();

            StreamWriter sr = new StreamWriter(sf.FileName);

            foreach (var item in users)
            {
                sr.WriteLine(item.FullName + " - " + item.ID);
            }

            sr.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var u = new User()
            {
                FullName = txtLastName.Text,
            };
            users.Add(u);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            var name = Convert.ToString(listBox1.SelectedValue);
            var u = from x in users
                    where x.ID.ToString() == name
                    select x;
            users.Remove(u.FirstOrDefault());


        }
    }
}
