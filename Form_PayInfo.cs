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
    public partial class Form_PayInfo : Skin_DevExpress
    {
        public int Loadingmode; //0 结算 1为未付款 2为查看订单详情
        public string PayuserID;
        public double totalPay;
        public Form_PayInfo()
        {
            InitializeComponent();
        }

        private void PayInfo_Load(object sender, EventArgs e)
        {
           
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            string sql = @"Insert";
        }
    }
}
