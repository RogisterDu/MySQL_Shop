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
    public partial class Form_Main : Skin_DevExpress
    {
        MySQLHelper DB = GlobalVariable.helper;
        private string UserID = string.Empty;
        public double sumPay = 0;

        public Form_Main()
        {
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            skinDataGridView1.AllowUserToAddRows = false;
            skinDataGridView1.ReadOnly = true;
            Form_Login form_Login = new Form_Login();
            form_Login.ShowDialog();
            if (!form_Login.isLogin)
            {
                System.Environment.Exit(0);
                return;
            }
            else
            {
                UserID = form_Login.LoginID;
                this.Visible = true;
            }
            LoadCartInfo();
            LoadItem();//加载商品信息
            //CallRecursive(skinTreeView1);
            skinTabControl1.SelectTab(0);
        }
        /// <summary>
        ///  加载购物车
        /// </summary>
        private void LoadCartInfo()
        {
            string sql = @"select sh_user_shopcart.id, sh_goods.name, goods_price, goods_num, is_select, (goods_num *goods_price) as totoalsingleprice
                           from sh_user_shopcart,sh_goods 
                           where user_id = '" + UserID + "' and sh_user_shopcart.goods_id = sh_goods.id";
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
                item.Checked=row["is_select"].ToString()=="0"?true :false;

                this.listView1.Items.Add(item);
            }
            this.listView1.EndUpdate();
            sumFinalPay();
        }


        /// <summary>
        /// 加载列表信息
        /// </summary>
        private void LoadItem()
        {
            LoadProductCategory();
            LoadProductsInfo();
            //LoadProductsInfo(0);
        }
        /// <summary>
        /// 加载所有商品列表
        //private void LoadProductsInfo(int ID)                         ID为加载商品列表 ID=0 为所有商品加载所有
        /// </summary>
        private void LoadProductsInfo()
        {
            //DataView ProjectView = new DataView();
            //string sql = @"select id ,name, price,IF(stock>0,'有','无') as stock,content from sh_goods";
            //if (ID != 0)
            //{
            //    sql += " where category_id =" + ID;
            //    ProjectView = DB.GetDataSet(sql).Tables[0].DefaultView;
            //}
            //else                                                    //清空表格加载所有数据
            //{
            //skinDataGridView1.Rows.Clear();
            //ProjectView = DB.GetDataSet(sql).Tables[0].DefaultView;
            //}
            //for (int i = 0; i < ProjectView.Count; i++)             //逐行加载数据
            //{
            //    skinDataGridView1.Rows.Add();
            //    skinDataGridView1.Rows[i].Cells[0].Value = ProjectView[i][0].ToString();
            //    skinDataGridView1.Rows[i].Cells[1].Value = ProjectView[i][1].ToString();
            //    skinDataGridView1.Rows[i].Cells[2].Value = ProjectView[i][2].ToString();
            //    skinDataGridView1.Rows[i].Cells[3].Value = ProjectView[i][3].ToString();
            //    skinDataGridView1.Rows[i].Cells[4].Value = ProjectView[i][4].ToString();
            //}


           string SelectedTag = skinTreeView1.SelectedNode.Tag.ToString();
           string sql = @"DROP FUNCTION IF EXISTS queryChildren;
                               CREATE FUNCTION queryChildren(CategoryId INT)
                               RETURNS VARCHAR(4000)
                               BEGIN
                                    DECLARE sTemp VARCHAR(4000);
                                    DECLARE sTempChd VARCHAR(4000);
                                    SET sTemp = '$';
                                    SET sTempChd = CAST(CategoryId AS CHAR);
                                    WHILE sTempChd IS NOT NULL DO
                                    SET sTemp = CONCAT(sTemp, ',', sTempChd);
                                    SELECT GROUP_CONCAT(id) INTO sTempChd FROM sh_goods_category WHERE FIND_IN_SET(parent_id, sTempChd)> 0;
                                    END WHILE;
                                    RETURN sTemp;
                                    END;
                                    SELECT queryChildren('" + SelectedTag + "');";
            string[] choose = DB.GetData(sql).ToString().Split(',');
            string final = null;
            for (int i = 0; i < choose.Length; i++)
            {
                final += "'" + choose[i] + "'";
                if (i != choose.Length - 1)
                {
                    final += ",";
                }
            }

            skinDataGridView1.DataSource = null;
            string sql2 = @"select id as 编号, category_id as 类别, name as 名称, price as 价格, IF(stock > 0,'有','无') as 是否有货, content as 备注 from sh_goods where category_id in(" + final + ") and is_on_sale = 0";
            skinDataGridView1.DataSource = DB.GetDataSet(sql2).Tables[0].DefaultView;
            skinDataGridView1.Columns["类别"].Visible = false;
        }

        /// <summary>
        /// 加载商品目录
        /// </summary>
        private void LoadProductCategory()
        {
            TreeNode root = new TreeNode();
            root.Tag = "0";
            root.Text = "所有商品";
            skinTreeView1.Nodes.Add(root);

            List<TreeNode> parent = new List<TreeNode>();
            string sql = "select * from sh_goods_category where parent_id=0";
            DataView dv = DB.GetDataSet(sql).Tables[0].DefaultView;
            for (int i = 0; i < dv.Count; i++)
            {
                TreeNode node = new TreeNode();
                node.Text = dv[i]["name"].ToString();
                node.Tag = dv[i]["id"].ToString();
                parent.Add(node);
                root.Nodes.Add(node);
                //MessageBox.Show(node.Tag.ToString());
            }
            LevelSearch(parent);


            skinTreeView1.ExpandAll();//展开所有节点
            // skinTreeView1.TopNode.Checked = true;
            skinTreeView1.SelectedNode = root; //最顶端节点选中
        }

        /// <summary>
        /// 递归函数，搜寻某一节点下的所有子节点
        /// </summary>
        /// <param name="parent"></param>
        private void LevelSearch(List<TreeNode> parent)
        {
            foreach (TreeNode node in parent) 
            {
                string sql = "select * from sh_goods_category where parent_id=" + node.Tag.ToString();
                DataView dv = DB.GetDataSet(sql).Tables[0].DefaultView;
                List<TreeNode> T = new List<TreeNode>();
                for (int i = 0; i < dv.Count; i++)
                {
                    TreeNode tree = new TreeNode();
                    tree.Text = dv[i]["name"].ToString();
                    tree.Tag = dv[i]["id"].ToString();
                    node.Nodes.Add(tree);
                    T.Add(tree);
                }
                LevelSearch(T);
            }
        }

        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DB.Dispose();
        }

        private void Form_Main_Shown(object sender, EventArgs e)
        {


        }
        /// <summary>
        /// 点击节点 筛选商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void skinTreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {

            //if (e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.ByKeyboard)
            //{
            //    SetChildNodeCheckedState(e.Node, e.Node.Checked);
            //}
            //skinDataGridView1.Rows.Clear();
            //TreeNodeCollection nodes = skinTreeView1.Nodes;
            //foreach (TreeNode n in nodes)
            //{
            //    nodelist.AddRange(CheckedNodes(n));
            //}
            //var message = string.Join(Environment.NewLine, nodelist);
            //MessageBox.Show(message);

            //CallRecursive(skinTreeView1);

        }
        #region useless code
        //private void SetChildNodeCheckedState(TreeNode currNode, bool isCheckedOrNot)
        //{
        //    if (currNode.Nodes == null)
        //    {
        //        return;                                     //没有子节点返回
        //    }
        //    foreach (TreeNode tmpNode in currNode.Nodes)
        //    {
        //        tmpNode.Checked = isCheckedOrNot;
        //        SetChildNodeCheckedState(tmpNode, isCheckedOrNot);
        //    }
        //}
        //private void LoadSelectedCategoryProduct(TreeNode treeNode)
        //{

        //    foreach (TreeNode tn in treeNode.Nodes)
        //    {
        //        if (tn.Checked)
        //        {
        //            LoadProductsInfo(Convert.ToInt32(tn.Name));
        //        }
        //        LoadSelectedCategoryProduct(tn);

        //    }

        //}
        //private void CallRecursive(TreeView treeView)
        //{
        //    TreeNodeCollection nodes = treeView.Nodes;
        //    foreach (TreeNode n in nodes)
        //    {
        //        LoadSelectedCategoryProduct(n);
        //    }
        ////}
        //public List<int> CheckedNodes(TreeNode parent)
        //{
        //    List<int> NodeList = new List<int> { };
        //    TreeNode node = parent;
        //    if (node != null)
        //    {
        //        if (node.Checked == true && node.FirstNode == null)
        //        {
        //            NodeList.Add(Convert.ToInt32(node.Name));
        //        }
        //        if (node.FirstNode != null)////如果node节点还有子节点则进入遍历
        //        {
        //            if (node.Checked)
        //            {
        //                NodeList.Add(Convert.ToInt32(node.Name));
        //            }
        //            CheckedNodes(node.FirstNode);
        //        }
        //        if (node.NextNode != null)////如果node节点后面有同级节点则进入遍历
        //        {
        //            if (node.Checked)
        //            {
        //                NodeList.Add(Convert.ToInt32(node.Name));
        //            }
        //            CheckedNodes(node.NextNode);
        //        }
        //    }
        //    return NodeList;
        //}
        #endregion

        private void skinTreeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void skinTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadProductsInfo();
        }
        /// <summary>
        /// 添加到购物车      
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinDataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            string ProductID = skinDataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            AddToCart addToCart = new AddToCart();
            addToCart.good_price = Convert.ToDouble(skinDataGridView1.SelectedRows[0].Cells[3].Value);
            addToCart.goods_num = 0;
            addToCart.ShowDialog();
            if (addToCart.IsAddtoCart)
            {
                string sql = string.Format(@"insert into sh_user_shopcart(
                                        user_id, goods_id, goods_price, goods_num, is_select, create_time, update_time)values(
                                                '{0}','{1}','{2}','{3}',{4},'{5}','{6}')",
                                                UserID,
                                                ProductID,
                                                addToCart.good_price,
                                                addToCart.goods_num,
                                                0,
                                                DateTime.Now.ToString("yyyymmdd"),
                                                DateTime.Now.ToString("yyyymmdd")
                                                    );
                DB.Execute(sql);
                LoadCartInfo();
            }

        }
        /// <summary>
        /// 结算界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinButton3_Click(object sender, EventArgs e)
        {
            Form_PayInfo form_PayInfo = new Form_PayInfo();
            form_PayInfo.PayuserID = UserID;
            form_PayInfo.Loadingmode = 0;
            form_PayInfo.totalPay = sumPay;
            form_PayInfo.ShowDialog();
            DeleteFromCart();
        }
        /// <summary>
        /// 更改购物车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            string sql = @"update sh_user_shopcart set is_select =" + (e.Item.Checked ? 0:1)+" where id='"+e.Item.Tag.ToString()+"'";
            DB.Execute(sql);
            sumFinalPay();
        }
        /// <summary>
        /// 计算保存位置
        /// </summary>
        private void sumFinalPay()
        {
            sumPay = 0;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked)
                {
                    sumPay += Convert.ToDouble(listView1.Items[i].SubItems[3].Text);
                }
            }
            skinLabel1.Text = "总价为" + sumPay.ToString();
        }

        private void 修改数量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count<=0)
            {

            }
        }
        /// <summary>
        /// 从购物车中删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinButton2_Click(object sender, EventArgs e)
        {
            //    if (listView1.CheckedItems.Count == 0)
            //    {
            //        MessageBox.Show("没有选择项目，请选中多个项目！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    string final = string.Empty;
            //    for (int i = 0; i < listView1.CheckedItems.Count; i++)
            //    {
            //        final += "'" + listView1.CheckedItems[i].Tag.ToString() + "'";
            //        if (i != listView1.CheckedItems.Count - 1)
            //        {
            //            final += ",";
            //        }
            //    }

            //    string sql = @"delete from sh_user_shopcart where id in('"+final+"')";
            DeleteFromCart();
        }
        /// <summary>
        /// 从购物车中删除
        /// </summary>
        private void DeleteFromCart()
        {
            string sql = "delete from sh_user_shopcart where is_select = 0 and user_id = '" + UserID + "'";
            DB.Execute(sql);
            LoadCartInfo();
        }
    }
}
