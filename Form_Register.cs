using System;
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
    public partial class Form_Register : Skin_DevExpress
    {
        MySQLHelper DB = GlobalVariable.helper;
        public bool IsRegister = false;
        public string LoginName = string.Empty;
        public Form_Register()
        {
            InitializeComponent();
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinTextBox6.Text.Length != 11)
            {
                MessageBox.Show("请输入正确的手机号");
                return;
            }
            if (skinTextBox1.Text != string.Empty && skinTextBox2.Text != string.Empty && skinTextBox3.Text != string.Empty && skinTextBox4.Text != string.Empty && skinTextBox5.Text != string.Empty)
            {
                if (skinTextBox2.Text == skinTextBox3.Text)
                {
                    DialogResult r = MessageBox.Show("确认注册么" + skinTextBox1.Text + "用户", "注册", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r==DialogResult.Yes)
                    {
                        
                        string sql = string.Format(@"insert into sh_user(name,password,salt,email,mobile,level,money,gender,qq,is_active,reg_time,create_time,update_time)values(
                                                            '{0}','{1}',{2},'{3}','{4}',{5},{6},{7},'{8}',{9},'{10}','{11}','{12}')",
                                                               skinTextBox1.Text,
                                                               skinTextBox2.Text,
                                                               1,
                                                               skinTextBox4.Text,
                                                               skinTextBox6.Text,
                                                               0,
                                                               0,
                                                               0,
                                                               skinTextBox5.Text,
                                                               0,
                                                               0,
                                                               0,
                                                               DateTime.Now.ToString("yyyymmdd"),
                                                               DateTime.Now.ToString("yyyymmdd"),
                                                               DateTime.Now.ToString("yyyymmdd")
                                                               );
                        DB.Execute(sql);
                        MessageBox.Show("注册成功");
                        IsRegister = true;
                        LoginName = skinTextBox1.Text;
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("有输入信息为空");
            }
        }
    }
}
