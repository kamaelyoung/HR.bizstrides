﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Attendance.BAL
{
    public class EmployeeBL
    {
        public bool UpdatePasswordByUserID(int userid, string Oldpassword, string Newpassword)
        {
            bool success = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AttendanceConn"].ToString());
                con.Open();
                SqlCommand command = new SqlCommand("[USP_UpdatePasswordByUserID]", con);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Userid", SqlDbType.Int).Value = userid;
                // command.Parameters.Add("@EmpID", SqlDbType.VarChar).Value = objInfo.EmpID;
                command.Parameters.Add("@Oldpwd", SqlDbType.VarChar).Value = Oldpassword;
                command.Parameters.Add("@NewPwd", SqlDbType.VarChar).Value = Newpassword;
              //  command.Parameters.Add("@Startdate", SqlDbType.DateTime).Value = Location;
                command.ExecuteNonQuery();
                con.Close();
                success = true;
            }
            catch (Exception ex)
            {
            }
            return success;
        }


        public bool UpdatePasscodeByUserID(int userid, string oldpasscode, string newpasscode)
        {
            bool success = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AttendanceConn"].ToString());
                con.Open();
                SqlCommand command = new SqlCommand("[USP_UpdatePassCodeByUserID]", con);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Userid", SqlDbType.Int).Value = userid;
                // command.Parameters.Add("@EmpID", SqlDbType.VarChar).Value = objInfo.EmpID;
                command.Parameters.Add("@OldPasscode", SqlDbType.VarChar).Value = oldpasscode;
                command.Parameters.Add("@NewPasscode", SqlDbType.VarChar).Value = newpasscode;
                //  command.Parameters.Add("@Startdate", SqlDbType.DateTime).Value = Location;
                command.ExecuteNonQuery();
                con.Close();
                success = true;
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        public DataTable GetEmployyeDetailsByUserID(int userid)
        {
            DataSet ds = new DataSet();

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AttendanceConn"].ToString());
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter("[USP_GetEmployeeDetailsByUserID]", con);

                da.SelectCommand.Parameters.Add(new SqlParameter("@EmpID", userid));
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(ds);

                DataTable dt = ds.Tables[0];

            }
            catch (Exception ex)
            {
            }

            return ds.Tables[0];
        }

        public string AddNewSchedule(string SchStart,string SchEnd,string LunchStart,string LunchEnd,bool fiveDays,bool SixDays,bool SevenDays,string IP,DateTime CurrentDt,int UserID)
        {
            bool success = false;
            string ID = "";
            try
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AttendanceConn"].ToString());
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter("[USP_AddSchedule]", con);
                DataSet ds = new DataSet();
                da.SelectCommand.Parameters.Add(new SqlParameter("@StartTime", SchStart));
                da.SelectCommand.Parameters.Add(new SqlParameter("@EndTime", SchEnd));
                da.SelectCommand.Parameters.Add(new SqlParameter("@LunchBreakStart", LunchStart));
                da.SelectCommand.Parameters.Add(new SqlParameter("@LunchBreakEnd", LunchEnd));
                da.SelectCommand.Parameters.Add(new SqlParameter("@IsFiveDays", fiveDays));
                da.SelectCommand.Parameters.Add(new SqlParameter("@IsSixDays", SixDays));
                da.SelectCommand.Parameters.Add(new SqlParameter("@IsSevenDays", SevenDays));
                da.SelectCommand.Parameters.Add(new SqlParameter("@ipaddress", IP));
                da.SelectCommand.Parameters.Add(new SqlParameter("@enteredBy", UserID));
                da.SelectCommand.Parameters.Add(new SqlParameter("@CurrentDt", CurrentDt));
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(ds);

                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    ID = dt.Rows[0]["ScheduleID"].ToString();
                }

               
              
            }
            catch (Exception ex)
            {
            }
            return ID;
        }

    }
}
