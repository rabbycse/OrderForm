using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace OrderItem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection("Data Source=RABBI-LAPTOP;Initial Catalog=OrderDb;Integrated Security=True");
        public int OrderNo;
        private void Form1_Load(object sender, EventArgs e)
        {
            GetOrderItems();
        }

        private void GetOrderItems()
        {
            SqlCommand cmd = new SqlCommand("Select OrderNo as 'SL', Item, Quantity from OrderItems", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();

            orderItemGridView.DataSource = dt;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO OrderItems VALUES (@Item, @Quantity)", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Item", textItem.Text);
                cmd.Parameters.AddWithValue("@Quantity", textQuantity.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                GetOrderItems();
            }
        }

        private bool isValid()
        {
            if (textItem.Text == string.Empty)
            {
                MessageBox.Show("Order Item is required", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            orderItemGridView[1, orderItemGridView.Rows.Count - 1].Value = "Total Quantity";
            orderItemGridView.Rows[orderItemGridView.Rows.Count - 1].Cells[2].Style.BackColor = Color.White;
            orderItemGridView.Rows[orderItemGridView.Rows.Count - 1].Cells[2].Style.ForeColor = Color.Black;
            
            decimal total = 0;
            for(int i = 0; i < orderItemGridView.RowCount - 1; i++)
            {
                var value = orderItemGridView.Rows[i].Cells[2].Value;
                if(value != DBNull.Value)
                {
                    total += Convert.ToDecimal(value);
                }
            }
            orderItemGridView.Rows[orderItemGridView.Rows.Count - 1].Cells[2].Value = total.ToString();
            MessageBox.Show("New order is successfully saved in the database", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (OrderNo > 0)
            {
                SqlCommand cmd = new SqlCommand("update OrderItems set Item=@Item,Quantity=@Quantity where OrderNo=@No", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Item", textItem.Text);
                cmd.Parameters.AddWithValue("@Quantity", textQuantity.Text);
                cmd.Parameters.AddWithValue("@No", OrderNo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("OrderItem is successfully update in the database", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                GetOrderItems();
            }
            else
            {
                MessageBox.Show("Please select an item to update", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (OrderNo != 0)
            {
                SqlCommand cmd = new SqlCommand("delete OrderItems where OrderNo=@No", con);
                con.Open();
                cmd.Parameters.AddWithValue("@No", OrderNo);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Item Deleted Successfully!");
                GetOrderItems();

            }
            else
            {
                MessageBox.Show("Please Select Item to Delete");
            }
        }

        private void orderItemGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            OrderNo = Convert.ToInt32(orderItemGridView.Rows[0].Cells[0].Value);
            textItem.Text = orderItemGridView.SelectedRows[0].Cells[1].Value.ToString();
            textQuantity.Text = orderItemGridView.SelectedRows[0].Cells[2].Value.ToString();
        }
    }
}
