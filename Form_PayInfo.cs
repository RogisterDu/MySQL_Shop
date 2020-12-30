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
        MySQLHelper DB = GlobalVariable.helper;
        public int Loadingmode; //0 结算 1为未付款 2为查看订单详情
        public string PayuserID;
        public double totalPay;
        public int isPay = 0;// 3为取消订单 0为支付 1稍后支付

        private string consignee;
        private string phone;
        private string zip;
        private string province;
        private string city;
        private string district;
        private string address;
        public Form_PayInfo()
        {
            InitializeComponent();
        }

        private void PayInfo_Load(object sender, EventArgs e)
        {
            LoadToBuyList();    //加载要购买的商品
            LoadAddressInfo(0); //加载默认地址
        }

        private void LoadAddressInfo(int AddressID)
        {
            string sql = @"select * from sh_user_address where user_id ='" + PayuserID + "'";
            if (AddressID == 0)
            {
                sql += " and is_default = 1";
            }
            else
            {
                sql += "and id =" + AddressID;
            }
            DataRow dr = DB.GetDataSet(sql).Tables[0].DefaultView[0].Row;
            consignee = dr["consignee"].ToString();
            phone = dr["phone"].ToString();
            zip  = dr["zip"].ToString();
            province = dr["province"].ToString();
            city = dr["city"].ToString();
            district = dr["district"].ToString();
            address = dr["address"].ToString();
            skinLabel1.Text = "收货人： " + consignee;
            skinLabel2.Text = "收货电话 ：" + phone;
            skinLabel3.Text = "邮编：" + zip;
            skinLabel4.Text = "收货地址：" + province+"省"+city+"市"+district+"区"+address;
        }

        private void LoadToBuyList()
        {

            string sql = @"select sh_user_shopcart.id, sh_goods.name, goods_price, goods_num, (goods_num *goods_price) as totoalsingleprice
                           from sh_user_shopcart,sh_goods 
                           where user_id = '" + PayuserID + "' and sh_user_shopcart.goods_id = sh_goods.id and sh_user_shopcart.is_select = 0  ";
            DataSet CartSet = DB.GetDataSet(sql);
            this.listView1.BeginUpdate();
            listView1.Items.Clear();
            for (int i = 0; i < CartSet.Tables[0].DefaultView.Count; i++)
            {
                DataRow row = CartSet.Tables[0].DefaultView[i].Row;
                ListViewItem item = new ListViewItem(row["name"].ToString());
                item.Tag = row["id"].ToString();
                item.SubItems.Add(row["goods_price"].ToString());
                item.SubItems.Add(row["goods_num"].ToString());
                item.SubItems.Add(row["totoalsingleprice"].ToString());

                this.listView1.Items.Add(item);
            }
            this.listView1.EndUpdate();
        }
        /// <summary>
        /// 立即付款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (Loadingmode == 0)//新订单
            {
                isPay = 0;
                AddToOrder();//添加到订单
                this.Close();
            }
            else if (Loadingmode == 1)// 重新支付
            {
                isPay = 0;
                RepayOrder();
            }
            else//查看订单详情
            {

            }
        }

        private void RepayOrder()
        {

        }

        /// <summary>
        /// 添加到Order
        /// </summary>
        private void AddToOrder()
        {
            string sql = string.Format(@"insert into sh_order(user_id,total_price,order_price,province,city,district,address,zip,consignee,phone,is_valid,is_cancel,is_pay,status)values(
                                                            '{0}',{1},{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},{11},{12},{13})",
                                                               PayuserID,
                                                               totalPay,
                                                               totalPay,
                                                               province,
                                                               city,
                                                               district,
                                                               address,
                                                               zip,
                                                               consignee,
                                                               phone,
                                                               0,
                                                               0,
                                                               isPay,
                                                               0
                                                               );
            DB.Execute(sql);
            string OrderID = DB.GetDataSet("select id from sh_order where user_id='" + PayuserID + "' order by id DESC").Tables[0].Rows[0][0].ToString();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                ListViewItem row = listView1.Items[i];
                string sql2 = string.Format(@"insert into sh_order_goods(order_id,goods_id,goods_name,goods_num,goods_price)values(
                                                                            '{0}','{1}','{2}','{3}','{4}')",
                                                                            OrderID,
                                                                            row.Tag.ToString(),
                                                                            row.SubItems[0].Text,
                                                                            row.SubItems[2].Text,
                                                                            row.SubItems[1].Text
                                                                            );
                DB.Execute(sql2);
            }
            MessageBox.Show("结算成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
        private void skinButton3_Click(object sender, EventArgs e)
        {
            Form_ChooseAddress form_ChooseAddress = new Form_ChooseAddress();
            form_ChooseAddress.UserID = PayuserID;
            form_ChooseAddress.ShowDialog();
            LoadAddressInfo(Convert.ToInt32(form_ChooseAddress.AddressID));
        }
        /// <summary>
        /// 稍后付款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinButton2_Click(object sender, EventArgs e)
        {
            isPay = 1;
            AddToOrder();
            this.Close();
        }
    }
}
