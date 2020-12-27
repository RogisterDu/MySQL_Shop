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
    public partial class AddToCart : Skin_DevExpress
    {
        public double good_price;                     //商品单价
        public int goods_num;                      //商品数量
        public bool IsAddtoCart = false;           //是否加入购物车
        public AddToCart()
        {
            InitializeComponent();
        }

        private void AddToCart_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = 1;
            SumPrice();
        }

        private void SumPrice()
        {
            skinLabel2.Text = "价格为"+ (Convert.ToDouble(numericUpDown1.Value) * good_price).ToString();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SumPrice();
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            goods_num = (int)numericUpDown1.Value;
            IsAddtoCart = true;
            this.Close();
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            SumPrice();
        }
    }
}
