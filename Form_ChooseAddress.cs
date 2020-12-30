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
    public partial class Form_ChooseAddress : Skin_DevExpress
    {
        MySQLHelper DB = GlobalVariable.helper;
        public string UserID =string.Empty;
        public string AddressID;
        public Form_ChooseAddress()
        {
            InitializeComponent();
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems[0].Tag.ToString() == "" || listView2.SelectedItems.Count<=0)
            {
                return;
            }
            AddressID = listView2.SelectedItems[0].Tag.ToString();
            this.Close();
        }

        private void Form_ChooseAddress_Load(object sender, EventArgs e)
        {
            LoadAddress();
        }

        private void LoadAddress()
        {
            string sql = @"select sh_user_address.id, consignee,IF(is_default > 0,'是','') as is_default, province, city, district, address, zip, phone
                           from sh_user_address
                           where user_id = '" + UserID + "'";

            DataSet CartSet = DB.GetDataSet(sql);
            this.listView2.BeginUpdate();
            listView2.Items.Clear();
            for (int i = 0; i < CartSet.Tables[0].DefaultView.Count; i++)
            {
                DataRow row = CartSet.Tables[0].DefaultView[i].Row;
                ListViewItem item = new ListViewItem(row["consignee"].ToString());
                item.Tag = row["id"].ToString();
                item.SubItems.Add(row["phone"].ToString());
                item.SubItems.Add(row["province"].ToString());
                item.SubItems.Add(row["city"].ToString());
                item.SubItems.Add(row["district"].ToString());
                item.SubItems.Add(row["address"].ToString());
                item.SubItems.Add(row["zip"].ToString());
                item.SubItems.Add(row["is_default"].ToString());

                this.listView2.Items.Add(item);
            }
            this.listView2.EndUpdate();
        }
    }
}
