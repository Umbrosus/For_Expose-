
using Proj1.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
//////using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proj1.Forms
{
    public partial class Frm : FmBase
    {
        private BarcodeScannerBase scanner;
        private int id = -1;
        private string ScanProductMessage = "barCode";
        private string WaitMessage = "Wait";
        private DataTable ControlTable;
        SqlDataAdapter dataAdapter = new SqlDataAdapter();
        SqlConnection connection = new SqlConnection(PublicConst.ConStr);
        DataSet dtSet = new DataSet();

        #region initForm
        public Frm( int id)
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            if (PublicConst.BarCodeScannerType == BarCodeScannerTypes.ComPortScanner)
            {
                scanner = new Scanner(this, ProcessScan, ProcessScanError);
            }

            this.id = id;
        }

        private DataTable GetDataTableFromDGV(DataGridView dgv)
        {
            var dt = new DataTable();
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                //if (column.Visible)
                {
                    DataColumn dtCol;
                    if (column.ValueType == null)
                    {
                        dtCol = new DataColumn(column.Name);
                    }
                    else
                    {
                        dtCol = new DataColumn(column.Name, column.ValueType);
                    }
                    dt.Columns.Add(dtCol);
                }
            }
            return dt;
        }


        private void Frm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F4))
            {
                this.Close();
            }
        }

        private void LoadDataFromDB()
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dtSet = new DataSet();
            if (connection != null)
            {
                string queryString = " SELECT [id],[SlotId],[SlotSequenceNumber],[Title],[OrderNbr], FROM [dbo].[table1] where id = @id";
                dataAdapter.SelectCommand = new SqlCommand(queryString, connection);
                try
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, connection))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int);
                        cmd.Parameters["@id"].Value = this.id;
                        dataAdapter.SelectCommand = cmd;
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        dataAdapter.Fill(dtSet);
                        GetTableForGrid(dtSet.Tables[0]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        private void GetTableForGrid(DataTable table)
        {
            dataGridView1.Rows.Clear();
            foreach (DataRow row in table.Rows)
            {
                bool isDBNull = !(row["dateTime"] is DBNull || row["dateTime"]==null);
                int colStatus = 0;
                if (isDBNull)
                    colStatus = 1;
                dataGridView1.Rows.Add(row["id"], row["Title"], row["OrderNbr"], isDBNull, colStatus);
            }
        }

        #endregion

        #region ClosingForm
        private void closeButton_Click_1(object sender, EventArgs e)
        {
            formClosing();
        }

        private void okBt_Click(object sender, EventArgs e)
        {
            formClosing();
        }

        private void formClosing()
        {
            string orderId = string.Empty;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (((bool)row.Cells["IsScanCol"].Value) == false)
                {
                    orderId += row.Cells["OrderIDCol"].Value + " ";
                }
            }
            if (!String.IsNullOrEmpty(orderId))
            {
                DialogResult dialogResult = MessageBox.Show("Text", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
                this.Close();
        }

        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<DataRow> dataRow = ControlTable.Rows.Cast<DataRow>().Where(r => (r["ErrorColumn"].ToString().Length > 0) || (int.Parse(r["ColStatus"].ToString()) < 1)).ToList();

            if (dataRow.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Text", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            ControlTable.Rows.Clear();
        }

        #endregion

        #region ScanFunc

        private void SetScanInfoText(string msg, Color color)
        {
            labelInf.ForeColor = color;
            labelInf.Text = msg;
            labelInf.Update();
        }

        private void ProcessScanError(object EventInfo)
        {
            MessageBox.Show("Scan Error");
        }

        private bool ProcessScan(string Code)
        {
            BcDocumentType documentType;
            int documentId;
            bool result = false;
            string errorText = String.Empty;

            result = BarcodeInterpreter.Decode(Code, out region, out documentType, out documentId);
            SetScanInfoText(WaitMessage, Color.Red);
            if (region == PublicConst.Region)
            {
                if (documentType == BcDocumentType.ShippingBox)
                { 
                    AddReceiving(Code);
                }
                else  
                {
                    MessageBox.Show("Text", "Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Text", "Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SetScanInfoText(ScanProductMessage, Color.Black);
            return result;
        }

        private void AddReceiving(string barCode)
        {
            int UserID = -1, errorCode = -9999;
            string errorText = String.Empty;
            if (id != -1)
            {
                
                SqlProcWrapper.pr_ScanBox(barCode, id, UserID, out errorCode, out errorText);
                // процедура сканування штрих-коду
                if (String.IsNullOrEmpty(errorText))
                {
                    LoadDataFromDB();
                }
                else
                {
                    MessageBox.Show(errorText, "Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Text", "Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region paintDataGridRow

        private void ChangeRowVisual(DataRow row, RowVisualStatus status)
        {
            row[4] = (int)status;
        }

        private void dgvCheck_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex >= 0) && (e.ColumnIndex < dataGridView1.Columns.Count))
            {
                int st = int.Parse(dataGridView1.Rows[e.RowIndex].Cells["ColStatus"].Value.ToString());
                Color c, sc;
                switch (st)
                {
                    case -2:
                        c = Color.Red;
                        sc = Color.Red;
                        break;
                    case -1:
                        c = Color.Black;
                        sc = Color.White;
                        break;
                    case 0:
                        c = Color.Black;
                        sc = Color.White;
                        break;
                    case 1:
                        c = Color.DarkGray;
                        sc = Color.White;
                        break;
                    default:
                        c = Color.Black;
                        sc = Color.White;
                        break;
                }
                e.CellStyle.ForeColor = c;
                e.CellStyle.SelectionForeColor = sc;
            }

        }

        private void dgvCheck_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //if ((this.dataGridView1.Columns["ColStatus"].Index == e.ColumnIndex) && (e.RowIndex >= 0))
            if ((e.ColumnIndex == -1) && (e.RowIndex >= 0))
            {
                e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
                e.Graphics.DrawRectangle(Pens.Gray, new Rectangle(e.CellBounds.Left + 1, e.CellBounds.Top - 1, e.CellBounds.Width - 2, e.CellBounds.Height));
                int st = -1;
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    if ( col.Name.Equals("ColStatus"))
                  {
                      st = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[col.Index].Value.ToString());
                  }
                }
                if(st!=-1)
                {        
                        if (st == 0)
                            using (Pen pen = new Pen(Color.Red, 5))
                            {
                                e.Graphics.DrawLine(pen, e.CellBounds.Left + 5, e.CellBounds.Top + e.CellBounds.Height / 2, e.CellBounds.Right - 5, e.CellBounds.Top + e.CellBounds.Height / 2); // Красный "Минус"
                            }
                        }
                        else
                            if (st == 1) 
                            {

                                Point[] p = new Point[3];
                                p[0] = new Point(e.CellBounds.Left + 5, e.CellBounds.Top + 5);
                                p[1] = new Point(e.CellBounds.Left + e.CellBounds.Width / 3, e.CellBounds.Bottom - 7);
                                p[2] = new Point(e.CellBounds.Right - 5, e.CellBounds.Top + 5);


                                using (Pen pen = new Pen(Color.Green, 5))
                                {
                                    e.Graphics.DrawLines(pen, p);
                                }
                            }
                            else
                                if (st == 0) 
                                {
                                    int h = e.CellBounds.Height - 4;
                                    int d = (e.CellBounds.Width - h) / 2;
                                    using (Pen pen = new Pen(Color.MediumVioletRed, 5))
                                    {
                                        e.Graphics.DrawLine(pen, e.CellBounds.Left + d, e.CellBounds.Top + e.CellBounds.Height / 2, e.CellBounds.Right - d, e.CellBounds.Top + e.CellBounds.Height / 2);
                                        e.Graphics.DrawLine(pen, e.CellBounds.Left + e.CellBounds.Width / 2, e.CellBounds.Top + 2, e.CellBounds.Left + e.CellBounds.Width / 2, e.CellBounds.Bottom - 2);
                                    }
                                }
                    e.Handled = true;
                }
            }
        }
        #endregion

        private void FrmRefund_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (scanner != null) scanner.Close();
        }

        private void FrmRefund_Enter(object sender, EventArgs e)
        {
            if (scanner != null) scanner.Open();
        }

        private void FrmRefund_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (scanner != null) scanner.Open();
            }
            else
            {
                scanner.Close();
            }
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            this.Text += " №" + id;
            LoadDataFromDB();
            dataGridView1.Focus();
        }
    }

    enum RowVisualStatus { Error = -2, Scaned = 0, Retuned = 1 };
}
