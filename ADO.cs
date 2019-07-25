 public static bool pr_InitSession(string userName,string password,out int userID,out int userSessionID, out string userName, out string errorText, out bool isMain)
        {
            errorText = String.Empty;
            userID = -1;
            userSessionID = -1;
            userName = String.Empty;
            isMain = false;

            SqlCommand cmd = new SqlCommand("pr_InitSession");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            SqlParameter parUserName = cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 1024);
            parUserName.Direction = ParameterDirection.Input;

            SqlParameter parPassword = cmd.Parameters.Add("@Password", SqlDbType.VarChar, 1024);
            parPassword.Direction = ParameterDirection.Input;

            SqlParameter parUserID = cmd.Parameters.Add("@UserID", SqlDbType.Int);
            parUserID.Direction = ParameterDirection.Output;

            SqlParameter parUserSessionID = cmd.Parameters.Add("@UserSessionID", SqlDbType.Int);
            parUserSessionID.Direction = ParameterDirection.Output;

            SqlParameter parUserName = cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 1024);
            parUserName.Direction = ParameterDirection.Output;

            SqlParameter parErrorText = cmd.Parameters.Add("@ErrorText", SqlDbType.VarChar, 1024);
            parErrorText.Direction = ParameterDirection.Output;

            SqlParameter parisMain = cmd.Parameters.Add("@isMain", SqlDbType.Bit);
            parisMain.Direction = ParameterDirection.Output;

            parUserName.Value = userName;
            parPassword.Value = password;

            using (SqlConnection con1 = new SqlConnection(PublicConst.ConStr))
            {
                try
                {
                    cmd.Connection = con1;
                    con1.Open();
                    cmd.ExecuteNonQuery();
                    con1.Close();
                    errorText = parErrorText.Value.ToString();
                    if (string.IsNullOrEmpty(errorText))
                    {
                        userID = int.Parse(parUserID.Value.ToString());
                        userName = parUserName.Value.ToString();
                        userSessionID = int.Parse(parUserSessionID.Value.ToString());
                        isMain =  (bool)parisMain.Value;
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    errorText = ex.Message;
                    return false;
                }
            }

            return false;
        }

        public static bool pr_PerformerCloseSession( int userSessionID, out string errorText)
        {
            errorText = String.Empty;
           
            SqlCommand cmd = new SqlCommand("pr_PerformerCloseSession");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            SqlParameter parUserSessionID = cmd.Parameters.Add("@userSessionID", SqlDbType.Int);
            parUserSessionID.Direction = ParameterDirection.Input;

            SqlParameter parErrorText = cmd.Parameters.Add("@ErrorText", SqlDbType.VarChar, 1024);
            parErrorText.Direction = ParameterDirection.Output;

            parUserSessionID.Value = userSessionID;

            using (SqlConnection con1 = new SqlConnection(PublicConst.ConStr))
            {
                try
                {
                    cmd.Connection = con1;
                    con1.Open();
                    cmd.ExecuteNonQuery();
                    con1.Close();
                    errorText = parErrorText.Value.ToString();
                    if (string.IsNullOrEmpty(errorText))
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    errorText = ex.Message;
                    return false;
                }
            }

            return false;
        }