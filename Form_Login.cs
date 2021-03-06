﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;

namespace MySQL_Shop
{
    public partial class Form_Login : Skin_DevExpress
    {
        public string LoginID =string.Empty;
        public bool isLogin = false;
        MySQLHelper DB = GlobalVariable.helper;
        public Form_Login()
        {
            InitializeComponent();
        }


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinTextBox1.Text != string.Empty || skinTextBox2.Text !=string.Empty)
            {
                string sql = "select id from sh_user where name =" + "'" + skinTextBox1.Text + "' and password ='" + skinTextBox2.Text + "'";
                DataView LoginView = DB.GetDataSet(sql).Tables[0].DefaultView;
                int result =LoginView.Count;
                if (result ==1)
                {
                    isLogin = true;
                    LoginID = LoginView[0][0].ToString();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("用户名或者密码出错");
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            Form_Register form_Register = new Form_Register();
            form_Register.ShowDialog();
            if (form_Register.IsRegister)
            {
                skinTextBox1.Text = form_Register.LoginName;
            }
        }
    }
}
