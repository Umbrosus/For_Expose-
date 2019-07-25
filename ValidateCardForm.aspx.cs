using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PriceCertificateConfirmer
{
    public partial class ValidateCardForm : System.Web.UI.Page
    {
        static String strCon = @"";
        SqlConnection connection = new SqlConnection(strCon);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session.Count <= 0)
                {
                    lbError.Text = "";
                    ScriptManager.RegisterStartupScript(this, typeof(string), "Alert", "alert('Text');", true);
                    Server.Transfer("MainForm.aspx", true);
                  //  GoToStep0();
                }
                else
                {
                    if (Session["OldBarCode"] != null && Session["OldValue"] != null && Session["OldNumber"] != null)
                    {
                        tbOldBarСode.Text = Session["OldBarCode"].ToString();
                        tbOldNumber.Text = Session["OldNumber"].ToString();
                        tbOldValue.Text = Session["OldValue"].ToString();
                        GoToStep2();
                        if (Session["NewBarCode"] != null && Session["NewValue"] != null && Session["NewNumber"] != null)
                        {
                            tbNewBarСode.Text = Session["NewBarCode"].ToString();
                            tbNewNumber.Text = Session["NewNumber"].ToString();
                            tbNewValue.Text = Session["NewValue"].ToString();
                            GoToStep3();
                        }
                    }
                    LbStore.Text = "Text: " + Session["ClientName"].ToString();
                    if (Session["StoreNumber"] != null && Session["StoreNumber"] is int && Session["StoreNumber"].ToString() != "-1")
                    {
                        LbStore.Text = "Text: " + Session["StoreNumber"].ToString();
                    }
                    else
                    {
                        LbStore.Text = "Text: " + Session["ClientName"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lbError.Text = ex.Message.ToString();
            }
        }

        protected void ToMainPage_Click(object sender, EventArgs e)
        {
            Server.Transfer("MainForm.aspx", true);
        }

        protected void btNextStep1_Click(object sender, EventArgs e)
        {
            lbError.Text = String.Empty;
            Int64 barCode = -1;
            Int64 number = -1;
            int value = -1;
            int rowID = -1;
            if (Int64.TryParse(tbOldBarСode.Text, out barCode) &&
                Int64.TryParse(tbOldNumber.Text, out number) &&
                int.TryParse(tbOldValue.Text, out value))
            {
                string strCr = CreateCertificateCardsTran();
                if (!int.TryParse(strCr, out rowID))
                {
                    lbError.Text = strCr;
                    GoToStep1();
                }
                else
                {
                    Session["rowID"] = rowID;
                    string str = ValidateOldCertificateCard(barCode, number, value);
                    if (string.IsNullOrEmpty(str))
                    {
                        Session["OldBarCode"] = barCode;
                        Session["OldNumber"] = number;
                        Session["OldValue"] = value;
                        GoToStep2();
                    }
                    else
                        lbError.Text = str;
                }
            }
            else
            {
                lbError.Text = "Text.";
            }
        }

        protected void btNextStep2_Click(object sender, EventArgs e)
        {
            lbError.Text = String.Empty;
            Int64 barCode = -1;
            Int64 number = -1;
            int value = -1;
            if (Int64.TryParse(tbNewBarСode.Text, out barCode) &&
                Int64.TryParse(tbNewNumber.Text, out number) &&
                int.TryParse(tbNewValue.Text, out value))
            {
                if (value != int.Parse(Session["OldValue"].ToString()))
                {
                    lbError.Text = "Text.";
                }
                else
                {
                    string str = ValidateNewCertificateCard(barCode, number, value);
                    if (string.IsNullOrEmpty(str))
                    {
                        Session["NewBarCode"] = barCode;
                        Session["NewNumber"] = number;
                        Session["NewValue"] = value;
                        GoToStep3();
                    }
                    else
                        lbError.Text = str;
                }
            }
            else
            {
                lbError.Text = "Text.";
            }
        }

        protected void btFinish_Click(object sender, EventArgs e)
        {
            string str = UpdateCertificateCards();
            lbSucssed.Text = str;
            ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('Text');", true);
        }

        protected void btRetrive_Click(object sender, EventArgs e)
        {
            Session["NewBarCode"] = 
            Session["NewNumber"] = 
            Session["NewValue"] = 
            Session["OldBarCode"] = 
            Session["OldNumber"] = 
            Session["OldValue"] = null;
            GoToStep1();
        }

        private string ValidateOldCertificateCard(Int64 barCode, Int64 number, int value)
        {
            try
            {
                string res;
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"GCC_ValidateOldCertificateCard", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@barCode", SqlDbType.BigInt);
                    cmd.Parameters["@barCode"].Value = barCode;
                    cmd.Parameters.Add("@number", SqlDbType.BigInt);
                    cmd.Parameters["@number"].Value = number;
                    cmd.Parameters.Add("@NominalValue", SqlDbType.Int);
                    cmd.Parameters["@NominalValue"].Value = value;
                    cmd.Parameters.Add("@rowID", SqlDbType.Int);
                    cmd.Parameters["@rowID"].Value = Session["rowID"];
                    cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Direction = ParameterDirection.Output;
                    cmd.Parameters["@ErrorMessage"].Value = "";
                    cmd.Parameters["@ErrorMessage"].Size = 1000;
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["@ErrorMessage"].Value.ToString();
                }
                return res;
            }
            catch (Exception ex)
            { 
                lbError.Text = ex.Message;
                return ex.Message;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        private string ValidateNewCertificateCard(Int64 barCode, Int64 number, int value)
        {
            try
            {
                string res;
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"GCC_ValidateNewCertificateCard", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@barCode", SqlDbType.BigInt);
                    cmd.Parameters["@barCode"].Value = barCode;
                    cmd.Parameters.Add("@number", SqlDbType.BigInt);
                    cmd.Parameters["@number"].Value = number;
                    cmd.Parameters.Add("@NominalValue", SqlDbType.Int);
                    cmd.Parameters["@NominalValue"].Value = value;
                    cmd.Parameters.Add("@rowID", SqlDbType.Int);
                    cmd.Parameters["@rowID"].Value = Session["rowID"];
                    cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Direction = ParameterDirection.Output;
                    cmd.Parameters["@ErrorMessage"].Value = "";
                    cmd.Parameters["@ErrorMessage"].Size = 1000;
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["@ErrorMessage"].Value.ToString();
                }
                return res;
            }
            catch (Exception ex)
            {
                lbError.Text = ex.Message;
                return ex.Message;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        private string UpdateCertificateCards()
        {
            try
            {
                string res;
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"GCC_UpdateCertificateCards", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@oldbarCode", SqlDbType.BigInt).Value = Session["OldBarCode"];

                    cmd.Parameters.Add("@oldnumber", SqlDbType.BigInt);
                    cmd.Parameters["@oldnumber"].Value = Session["OldNumber"];
                    cmd.Parameters.Add("@oldvalue", SqlDbType.Int);
                    cmd.Parameters["@oldvalue"].Value = Session["OldValue"];

                    cmd.Parameters.Add("@newbarCode", SqlDbType.BigInt).Value = Session["NewBarCode"];
                    cmd.Parameters.Add("@newnumber", SqlDbType.BigInt);
                    cmd.Parameters["@newnumber"].Value = Session["NewNumber"];
                    cmd.Parameters.Add("@newvalue", SqlDbType.Int);
                    cmd.Parameters["@newvalue"].Value = Session["NewValue"];

                    cmd.Parameters.Add("@rowID", SqlDbType.Int);
                    cmd.Parameters["@rowID"].Value = Session["rowID"];

                    cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Direction = ParameterDirection.Output;
                    cmd.Parameters["@ErrorMessage"].Value = "";
                    cmd.Parameters["@ErrorMessage"].Size = 1000;
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["@ErrorMessage"].Value.ToString();
                }
                GoToStep4();
                return res;
            }
            catch (Exception ex)
            {
                lbError.Text = ex.Message;
                return ex.Message;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        private string CreateCertificateCardsTran()
        {
            try
            {
                string res;
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"GCC_CreateCertificateCardsTran", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserInfo", SqlDbType.VarChar);
                    cmd.Parameters["@UserInfo"].Value = Session["ClientName"].ToString();
                    cmd.Parameters["@UserInfo"].Size = 255;
                    cmd.Parameters.Add("@clientIP", SqlDbType.VarChar);
                    cmd.Parameters["@clientIP"].Size = 1000;
                    cmd.Parameters["@clientIP"].Value = Session["ClietIP"];
                    cmd.Parameters.Add("@sessionID", SqlDbType.VarChar);
                    cmd.Parameters["@sessionID"].Value = Session["SessionID"];
                    cmd.Parameters["@sessionID"].Size = 1000;
                    cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Direction = ParameterDirection.Output;
                    cmd.Parameters["@ErrorMessage"].Value = "Ok";
                    cmd.Parameters["@ErrorMessage"].Size = 1000;
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["@ErrorMessage"].Value.ToString();
                }
                return res;
            }
            catch (Exception ex)
            {
                lbError.Text = ex.Message;
                return ex.Message;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        private void GoToStep1()
        {
            lbError.Text = "";
            lblCaption.Text = "Text";
            lbSucssed.Text = String.Empty;
            lbTask.Text = "Text.";
            tbNewBarСode.Enabled = false;
            tbNewNumber.Enabled = false;
            tbNewValue.Enabled = false;
            tbOldBarСode.Enabled = true;
            tbOldNumber.Enabled = true;
            tbOldValue.Enabled = true;

            tbNewBarСode.Text = String.Empty;
            tbNewNumber.Text = String.Empty;
            tbNewValue.Text = String.Empty;
            tbOldBarСode.Text = String.Empty;
            tbOldNumber.Text = String.Empty;
            tbOldValue.Text = String.Empty;

            btStep2.Enabled = false;
            btNext.Enabled = true;
            btNext.Visible = true;
            btStep2.Visible = true;
            btFinish.Enabled = false;
        }

        private void GoToStep0()
        {
            tbNewBarСode.Enabled = false;
            tbNewNumber.Enabled = false;
            tbNewValue.Enabled = false;
            tbOldBarСode.Enabled = false;
            tbOldNumber.Enabled = false;
            tbOldValue.Enabled = false;
            btStep2.Enabled = false;
            btNext.Enabled = false;
            btRetrive.Enabled = false;
        }

        private void GoToStep2()
        {
            lblCaption.Text = "Крок 2";
            lbTask.Text = "Потрібно ввести дані з нового сертифікату.";
            tbNewBarСode.Enabled = true;
            tbNewNumber.Enabled = true;
            tbNewValue.Enabled = true;
            tbOldBarСode.Enabled = false;
            tbOldNumber.Enabled = false;
            tbOldValue.Enabled = false;
            btNext.Enabled = false;
            btStep2.Enabled = true;
        }

        private void GoToStep3()
        {
            lblCaption.Text = "Крок 3";
            lbTask.Text = "Дані готові до обробки.";
            tbNewBarСode.Enabled = false;
            tbNewNumber.Enabled = false;
            tbNewValue.Enabled = false;
            btNext.Visible = false;
            btStep2.Visible = false;
            btFinish.Enabled = true;
        }

        private void GoToStep4()
        {
            lblCaption.Text = "Крок 3";
            lbTask.Text = "Дані підтверджені.";
            btFinish.Enabled = false;
        }
    }
}