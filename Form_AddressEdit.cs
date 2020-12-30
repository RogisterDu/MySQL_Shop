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
    public partial class Form_AddressEdit : Skin_DevExpress
    {
        MySQLHelper DB = GlobalVariable.helper;

        public string ID = string.Empty;
        public string UserID = string.Empty;
        public string consignee = string.Empty;
        public string phone = string.Empty;
        public string zip = string.Empty;
        public string province = string.Empty;
        public string city = string.Empty;
        public string district = string.Empty;
        public string address = string.Empty;

        public int LoadingAddressmode = 0; //0新增新地址 1编辑地址
        public Form_AddressEdit()
        {
            InitializeComponent();
        }

        private void Form_AddressEdit_Load(object sender, EventArgs e)
        {
            if (LoadingAddressmode == 0)
            {
                skinButton1.Text = "新增地址";
            }
            else
            {
                skinButton1.Text = "保存地址";
                LoadAdressInfo();
            }
        }



        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (LoadingAddressmode == 0)
            {
                AddNewAdress();
                this.Close();
            }
            else
            {
                SaveAddressInfo();
                this.Close();
            }
        }
        /// <summary>
        /// 读取数据到变量
        /// </summary>
        private void ReadInfolist()
        {
            consignee = consigneeTextBox.Text;
            phone = phoneTextBox.Text;
            zip = ZipTextBox.Text;
            province = ProvinceTextBox.Text;
            city = CityTextBox.Text;
            district = DistrictTextBox.Text;
            address = AddressTextBox.Text;
        }

        private void LoadAdressInfo()
        {
            consigneeTextBox.Text = consignee;
            phoneTextBox.Text = phone;
            ZipTextBox.Text = zip;
            ProvinceTextBox.Text = province;
            CityTextBox.Text = city;
            DistrictTextBox.Text = district;
            AddressTextBox.Text = address;
        }


        private void AddNewAdress()
        {
            ReadInfolist();
            string sql = string.Format(@"insert into sh_user_address(consignee,phone,zip,province,city,district,address,is_default,user_id)values(
                                                            '{0}',{1},{2},{3},{4},{5},{6},'{7}',{8})",
                                                            consignee,
                                                            phone,
                                                            zip,
                                                            province,
                                                            city,
                                                            district, 
                                                            address,
                                                            0,
                                                            UserID   
                                                            );
            DB.Execute(sql);
            
        }
        private void SaveAddressInfo()
        {
            ReadInfolist();
            string sql = string.Format(@"update  sh_user_address set consignee='{0}', phone ='{1}', zip ='{2}', province='{3}', city='{4}', district='{5}', address= '{6}' where id ='{7}'",
                                                            consignee,
                                                            phone,
                                                            zip,
                                                            province,
                                                            city,
                                                            district,
                                                            address,
                                                            ID
                                                            );
            DB.Execute(sql);

        }
    }
}
