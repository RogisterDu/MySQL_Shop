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

        public Form_Main()
        {
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            Form_Login form_Login = new Form_Login();
            form_Login.ShowDialog();
            if (!form_Login.isLogin)
            {
                System.Environment.Exit(0);
                return;
            }
            else
            {
                this.Visible = true;
            }

            LoadItem();//加载商品信息
            //CallRecursive(skinTreeView1);
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
        }

        /// <summary>
        /// 加载商品目录
        /// </summary>
        private void LoadProductCategory()
        {
            DataSet ds = DB.GetDataSet("select * from sh_goods_category");
            skinTreeView1.Nodes.Clear();
            TreeNode tn = new TreeNode();
            tn.Text = "所有商品";
            tn.Name = "0";
            tn.Tag = "0";
            tn.ImageIndex = 0;

            tn.SelectedImageIndex = 0;
            skinTreeView1.Nodes.Add(tn);//该TreeView命名为tn 
            skinTreeView1.SelectedNode = skinTreeView1.TopNode;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                tn = new TreeNode();
                tn.Text = dr["name"].ToString();
                tn.Name = dr["id"].ToString();//Name作为ID   
                tn.Tag = dr["id"].ToString();//Tag作为RootID   
                tn.ImageIndex = 1;
                tn.SelectedImageIndex = 1;
                //判断是否为主节点   
                if (dr["id"].ToString() == dr["parent_id"].ToString())
                {
                    //主节点   
                    skinTreeView1.SelectedNode = skinTreeView1.TopNode;
                }
                else
                {
                    //其他节点   
                    if (skinTreeView1.SelectedNode.Name != dr["parent_id"].ToString())
                    {
                        TreeNode[] tn_temp = skinTreeView1.Nodes.Find(dr["parent_id"].ToString(), true); //通过ParentID查找父节点   
                        if (tn_temp.Length > 0)
                        {
                            skinTreeView1.SelectedNode = tn_temp[0]; //选中查找到的节点   
                        }
                    }
                }
                skinTreeView1.SelectedNode.Nodes.Add(tn);
            }
            skinTreeView1.ExpandAll();//展开所有节点
            skinTreeView1.SelectedNode = skinTreeView1.TopNode; //最顶端节点选中   
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

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinDataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            string SelectedProductID = skinDataGridView1.SelectedRows[0].Tag.ToString();

        }
    }
}
