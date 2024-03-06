
using MySql.Data.MySqlClient;
using System.Data;

namespace MyDataBase
{
    public partial class Form1 : Form
    {
        DB db = new DB();

        MySqlDataAdapter adapter = new MySqlDataAdapter();

        public Form1()
        {
            InitializeComponent();
        }

        public DataTable GetTable(string command)
        {
            this.db.OpenConnection();

            MySqlCommand cmd = new MySqlCommand(command, this.db.GetConnection());

            DataTable table = new DataTable();

            this.adapter.SelectCommand = cmd;
            this.adapter.Fill(table);

            this.db.CloseConnection();

            return table;
        }

        public void AddTablesToComboBox()
        {
            string command = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = 'online-shop'";
            foreach (DataRow row in GetTable(command).Rows)
            {
                comboBox1.Items.Add(row["TABLE_NAME"]);
            }
        }

        private void SelectDataOfTable()
        {
            dataGridView1.DataSource = GetTable($"SELECT * FROM {comboBox1.SelectedItem}");
        }

        private void UpdateDataOfTable()
        {
            DataTable? datatable = dataGridView1.DataSource as DataTable;

            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(adapter);

            try
            {
                if (datatable != null) { adapter.Update(datatable); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DeleteDataOfTable()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView1.SelectedRows[0].Index;
                try
                {
                    dataGridView1.Rows.RemoveAt(selectedIndex);
                }
                catch (InvalidOperationException) { MessageBox.Show("Нельзя удалить пустую строку"); }
                

                UpdateDataOfTable();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectDataOfTable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateDataOfTable();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeleteDataOfTable();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                AddTablesToComboBox();
            }
        }

        private void comboBox1_MouseDown(object sender, MouseEventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }
    }
}
