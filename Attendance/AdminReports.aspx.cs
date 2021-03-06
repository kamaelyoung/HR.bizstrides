﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Attendance.BAL;
using System.Data.SqlClient;

namespace Attendance
{
    public partial class AdminReports : System.Web.UI.Page
    {
        public GeneralFunction objFun = new GeneralFunction();
        int MonScIN = 0;
        int MonScOut = 0;
        int MonSignIn = 0;
        int MonSignOut = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TodayBannerDate"] != null)
            {
                comanyname.Text = CommonFiles.ComapnyName;
                if (!IsPostBack)
                {

                    string timezone = "";
                    if (Convert.ToInt32(Session["TimeZoneID"]) == 2)
                    {
                        timezone = "Eastern Standard Time";
                    }
                    else
                    {
                        timezone = "India Standard Time";

                    }
                    DateTime ISTTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezone));

                    var CurentDatetime = ISTTime;

                    lblDate2.Text = CurentDatetime.ToString("dddd MMMM dd yyyy, hh:mm:ss tt ");
                    hdnTodaydt.Value = CurentDatetime.ToString("MM/dd/yyyy");
                    lblTimeZoneName.Text = Session["TimeZoneName"].ToString().Trim();
                    lblLocation.Text = Session["LocationName"].ToString();
                    lblHeadSchedule.Text = Session["ScheduleInOut"].ToString();
                    getLocations();

                    ViewState["Location"] = Session["LocationName"].ToString();
                    ddlLocation.SelectedIndex = ddlLocation.Items.IndexOf(ddlLocation.Items.FindByText(lblLocation.Text.Trim()));
                    lblEmployyName.Text = Session["EmpName"].ToString().Trim();
                    Photo.Src = Session["Photo"].ToString().Trim();
                    Session["TodayDate"] = Session["TodayBannerDate"];
                    DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    Session["TodayDate1"] = Convert.ToDateTime(Session["TodayDate"]);
                    if (GeneralFunction.GetFirstDayOfWeekDate(TodayDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }


                    DateTime StartDate = GeneralFunction.GetFirstDayOfWeekDate(TodayDate);
                    DateTime EndDate = GeneralFunction.GetLastDayOfWeekDate(TodayDate);

                    ViewState["CurrentStart"] = StartDate;
                    ViewState["CurrentEnd"] = EndDate;
                    int userid = Convert.ToInt32(Session["UserID"]);
                    string Ismanage = Session["IsManage"].ToString();
                    string IsAdmin = Session["IsAdmin"].ToString();

                    btnFreeze.Visible = true;
                    lblFreeze.Visible = true;
                    //   DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartOfMonth = StartDate.AddDays(-1);
                    DateTime FreezeDate = StartOfMonth;
                    lblFreezedate.Text = FreezeDate.ToString("MM/dd/yyyy");
                    hdnFreeze.Value = FreezeDate.ToString("MM/dd/yyyy");
                    Attendance.BAL.Report obj = new Report();
                    //int CNT = obj.GetFreezedDate(FreezeDate);
                    DateTime CNT = obj.GetFreezedDate(FreezeDate, ddlLocation.SelectedItem.Text.ToString().Trim());
                    lblFreezedate.Text = FreezeDate.ToString("MM/dd/yyyy");
                    hdnFreeze.Value = FreezeDate.ToString("MM/dd/yyyy");
                    if (CNT.ToString("MM/dd/yyyy") != "01/01/1900")
                    {
                        lblFreeze.Text = "Attendance freezed until " + CNT.ToString("MM/dd/yyyy");
                        ViewState["FreezeDate"] = CNT.ToString("MM/dd/yyyy");
                        btnFreeze.CssClass = "btn btn-warning btn-small disabled";
                        btnFreeze.Enabled = false;
                    }
                    else
                    {
                        lblFreeze.Visible = false;
                        btnFreeze.CssClass = "btn btn-warning btn-small enabled";
                        btnFreeze.Enabled = true;
                    }


                    DataTable ds = new DataTable();
                    ds = GetReportAdmin(StartDate, EndDate, Convert.ToInt32(ddlLocation.SelectedValue));
                    lblGrdLocaton.Visible = true;
                    ddlLocation.Visible = true;
                    Session["AtnAdminDetails"] = ds;
                    if (ds.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = ds;
                        grdAttandence.DataBind();
                    }


                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }

        protected void lnkChangepwd_Click(object sender, EventArgs e)
        {
            try
            {

                txtOldpwd.Text = "";
                txtNewPwd.Text = "";
                lblPwdName.Text = Session["EmpName"].ToString().Trim();
                txtConfirmPwd.Text = "";
                mdlChangePwd.Show();
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnCancelPwd_Click(object sender, EventArgs e)
        {
            txtOldpwd.Text = "";
            txtNewPwd.Text = "";
            txtConfirmPwd.Text = "";
            mdlChangePwd.Hide();
        }

        protected void btnUpdatePwd_Click(object sender, EventArgs e)
        {
            try
            {
                string location = Session["LocationName"].ToString();
                string EmpName = Session["EmpName"].ToString().Trim();
                int userid = Convert.ToInt32(Session["UserID"]);
                string oldPwd = txtOldpwd.Text.Trim();
                string NewPwd = txtNewPwd.Text.Trim();

                Attendance.BAL.EmployeeBL obj = new EmployeeBL();
                bool bnew = obj.UpdatePasswordByUserID(userid, oldPwd, NewPwd);
                if (bnew)
                {
                    txtOldpwd.Text = "";
                    txtNewPwd.Text = "";
                    txtConfirmPwd.Text = "";
                    mdlChangePwd.Hide();
                    //lblAlertName.Text="Password ";
                    //mdlSuccessfullAlert.Show();
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Password changed successfully..');", true);
                }

            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdatePassCode_Click(object sender, EventArgs e)
        {
            try
            {
                string location = Session["LocationName"].ToString();
                string EmpName = Session["EmpName"].ToString().Trim();
                int userid = Convert.ToInt32(Session["UserID"]);
                string oldPasscode = txtOldpasscode.Text.Trim();
                string NewPasscode = txtNewPasscode.Text.Trim();

                Attendance.BAL.EmployeeBL obj = new EmployeeBL();
                bool bnew = obj.UpdatePasscodeByUserID(userid, oldPasscode, NewPasscode);
                if (bnew)
                {
                    txtOldpasscode.Text = "";
                    txtNewPasscode.Text = "";
                    txtConfirmPasscode.Text = "";
                    mdlChangePasscode.Hide();
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Passcode changed successfully..');", true);
                }

            }
            catch (Exception ex)
            {
            }
        }

        protected void btnCancelPasscode_Click(object sender, EventArgs e)
        {
            txtOldpasscode.Text = "";
            txtNewPasscode.Text = "";
            txtConfirmPasscode.Text = "";
            mdlChangePasscode.Hide();
        }

        protected void lnkChangePasscode_Click(object sender, EventArgs e)
        {
            try
            {

                txtOldpasscode.Text = "";
                txtNewPasscode.Text = "";
                lblPasscodeName.Text = Session["EmpName"].ToString().Trim();
                txtConfirmPasscode.Text = "";
                mdlChangePasscode.Show();
            }
            catch (Exception ex)
            {
            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", " $('#spinner').show();", true);
            Session.Abandon();
            Response.Redirect("Default.aspx");
        }


        private void getLocations()
        {
            try
            {
                Attendance.BAL.Report obj = new Report();
                DataTable dt = obj.GetLocations();
                ddlLocation.DataSource = dt;
                ddlLocation.DataTextField = "LocationName";
                ddlLocation.DataValueField = "LocationId";
                ddlLocation.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        public DataTable GetReportAdmin(DateTime StartDate, DateTime EndDate, int LocationID)
        {
            lblWeekReportheading.Text = "Weekly attendance report";
            lblWeekReport.Text = "( " + StartDate.ToString("MM/dd/yyyy") + " - " + EndDate.ToString("MM/dd/yyyy") + " )";
            DataSet ds = new DataSet();
            DataTable dtAttandence = new DataTable();
            dtAttandence.Columns.Add("empid", typeof(string));
            dtAttandence.Columns.Add("Empname", typeof(string));
            dtAttandence.Columns.Add("Termdate", typeof(DateTime));
            dtAttandence.Columns.Add("Startdate", typeof(DateTime));

            dtAttandence.Columns.Add("MonSchIn", typeof(string));
            dtAttandence.Columns.Add("MonSchOut", typeof(string));
            dtAttandence.Columns.Add("MonSignIn", typeof(string));
            dtAttandence.Columns.Add("MonSignOut", typeof(string));
            dtAttandence.Columns.Add("MonHrs", typeof(string));
            dtAttandence.Columns.Add("MonLogUserID", typeof(int));
            dtAttandence.Columns.Add("MonLoginNotes", typeof(string));
            dtAttandence.Columns.Add("MonLogoutNotes", typeof(string));
            dtAttandence.Columns.Add("MonLoginFlag", typeof(string));
            dtAttandence.Columns.Add("MonLogoutFlag", typeof(string));
            dtAttandence.Columns.Add("MonFreeze", typeof(string));


            dtAttandence.Columns.Add("TueSchIn", typeof(string));
            dtAttandence.Columns.Add("TueSchOut", typeof(string));
            dtAttandence.Columns.Add("TueSignIn", typeof(string));
            dtAttandence.Columns.Add("TueSignOut", typeof(string));
            dtAttandence.Columns.Add("TueHrs", typeof(string));
            dtAttandence.Columns.Add("TueLogUserID", typeof(int));
            dtAttandence.Columns.Add("TueLoginNotes", typeof(string));
            dtAttandence.Columns.Add("TueLogoutNotes", typeof(string));
            dtAttandence.Columns.Add("TueLoginFlag", typeof(string));
            dtAttandence.Columns.Add("TueLogoutFlag", typeof(string));
            dtAttandence.Columns.Add("TueFreeze", typeof(string));


            dtAttandence.Columns.Add("WedSchIn", typeof(string));
            dtAttandence.Columns.Add("WedSchOut", typeof(string));
            dtAttandence.Columns.Add("WedSignIn", typeof(string));
            dtAttandence.Columns.Add("WedSignOut", typeof(string));
            dtAttandence.Columns.Add("WedHrs", typeof(string));
            dtAttandence.Columns.Add("WedLogUserID", typeof(int));
            dtAttandence.Columns.Add("WedLoginNotes", typeof(string));
            dtAttandence.Columns.Add("WedLogoutNotes", typeof(string));
            dtAttandence.Columns.Add("WedLoginFlag", typeof(string));
            dtAttandence.Columns.Add("WedLogoutFlag", typeof(string));
            dtAttandence.Columns.Add("WedFreeze", typeof(string));

            dtAttandence.Columns.Add("ThuSchIn", typeof(string));
            dtAttandence.Columns.Add("ThuSchOut", typeof(string));
            dtAttandence.Columns.Add("ThuSignIn", typeof(string));
            dtAttandence.Columns.Add("ThuSignOut", typeof(string));
            dtAttandence.Columns.Add("ThuHrs", typeof(string));
            dtAttandence.Columns.Add("ThuLogUserID", typeof(int));
            dtAttandence.Columns.Add("ThuLoginNotes", typeof(string));
            dtAttandence.Columns.Add("ThuLogoutNotes", typeof(string));
            dtAttandence.Columns.Add("ThuLoginFlag", typeof(string));
            dtAttandence.Columns.Add("ThuLogoutFlag", typeof(string));
            dtAttandence.Columns.Add("ThuFreeze", typeof(string));

            dtAttandence.Columns.Add("FriSchIn", typeof(string));
            dtAttandence.Columns.Add("FriSchOut", typeof(string));
            dtAttandence.Columns.Add("FriSignIn", typeof(string));
            dtAttandence.Columns.Add("FriSignOut", typeof(string));
            dtAttandence.Columns.Add("FriHrs", typeof(string));
            dtAttandence.Columns.Add("FriLogUserID", typeof(int));
            dtAttandence.Columns.Add("FriLoginNotes", typeof(string));
            dtAttandence.Columns.Add("FriLogoutNotes", typeof(string));
            dtAttandence.Columns.Add("FriLoginFlag", typeof(string));
            dtAttandence.Columns.Add("FriLogoutFlag", typeof(string));
            dtAttandence.Columns.Add("FriFreeze", typeof(string));

            dtAttandence.Columns.Add("SatSchIn", typeof(string));
            dtAttandence.Columns.Add("SatSchOut", typeof(string));
            dtAttandence.Columns.Add("SatSignIn", typeof(string));
            dtAttandence.Columns.Add("SatSignOut", typeof(string));
            dtAttandence.Columns.Add("SatHrs", typeof(string));
            dtAttandence.Columns.Add("SatLogUserID", typeof(int));
            dtAttandence.Columns.Add("SatLoginNotes", typeof(string));
            dtAttandence.Columns.Add("SatLogoutNotes", typeof(string));
            dtAttandence.Columns.Add("SatLoginFlag", typeof(string));
            dtAttandence.Columns.Add("SatLogoutFlag", typeof(string));
            dtAttandence.Columns.Add("SatFreeze", typeof(string));

            dtAttandence.Columns.Add("SunSchIn", typeof(string));
            dtAttandence.Columns.Add("SunSchOut", typeof(string));
            dtAttandence.Columns.Add("SunSignIn", typeof(string));
            dtAttandence.Columns.Add("SunSignOut", typeof(string));
            dtAttandence.Columns.Add("SunHrs", typeof(string));
            dtAttandence.Columns.Add("SunLogUserID", typeof(int));
            dtAttandence.Columns.Add("SunLoginNotes", typeof(string));
            dtAttandence.Columns.Add("SunLogoutNotes", typeof(string));
            dtAttandence.Columns.Add("SunLoginFlag", typeof(string));
            dtAttandence.Columns.Add("SunLogoutFlag", typeof(string));
            dtAttandence.Columns.Add("PresentDays", typeof(string));
            dtAttandence.Columns.Add("SunFreeze", typeof(string));

            dtAttandence.Columns.Add("TotalHours", typeof(string));

            dtAttandence.Rows.Add();


            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AttendanceConn"].ToString());
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter("[USP_FnAdmin]", con);
                da.SelectCommand.Parameters.Add(new SqlParameter("@LocationID", LocationID));
                da.SelectCommand.Parameters.Add(new SqlParameter("@startdate", StartDate));
                da.SelectCommand.Parameters.Add(new SqlParameter("@EndDate", EndDate));
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(ds);
                int MonSignInCnt = 0;
                int TueSignInCnt = 0;
                int WedSignInCnt = 0;
                int ThuSignInCnt = 0;
                int FriSignInCnt = 0;
                int SatSignInCnt = 0;
                int SunSignInCnt = 0;
                int MonSignOutCnt = 0;
                int TueSignOutCnt = 0;
                int WedSignOutCnt = 0;
                int ThuSignOutCnt = 0;
                int FriSignOutCnt = 0;
                int SatSignOutCnt = 0;
                int SunSignOutCnt = 0;





                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables.Count > 1)
                    {
                        DataTable dt = ds.Tables[1];
                        DataView dv = dt.DefaultView;
                        DataTable dtname = new DataTable();
                        DateTime NextDate = GeneralFunction.GetNextDayOfWeekDate(StartDate);

                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            int presentdays = 0;

                            dv.RowFilter = "empid='" + ds.Tables[0].Rows[j]["empid"].ToString() + "'";
                            dtname = dv.ToTable();
                            dtAttandence.Rows[j]["empid"] = ds.Tables[0].Rows[j]["empid"].ToString();
                            dtAttandence.Rows[j]["Empname"] = ds.Tables[0].Rows[j]["firstName"].ToString() + " " + ds.Tables[0].Rows[j]["lastname"].ToString(); ;
                            dtAttandence.Rows[j]["Termdate"] = ds.Tables[0].Rows[j]["Termdate"].ToString() == "NULL" ? Convert.ToDateTime("01/01/1900") : ds.Tables[0].Rows[j]["Termdate"].ToString() == "" ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(Convert.ToDateTime(ds.Tables[0].Rows[j]["Termdate"]).ToString("MM/dd/yyyy"));
                            dtAttandence.Rows[j]["Startdate"] = ds.Tables[0].Rows[j]["Startdate"].ToString() == "NULL" ? Convert.ToDateTime("01/01/1900") : ds.Tables[0].Rows[j]["Startdate"].ToString() == "" ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(Convert.ToDateTime(ds.Tables[0].Rows[j]["Startdate"]).ToString("MM/dd/yyyy"));
                            dtAttandence.Rows[j]["SunSchIn"] = ds.Tables[0].Rows[j]["startTime"].ToString();
                            dtAttandence.Rows[j]["SunSchOut"] = ds.Tables[0].Rows[j]["EndTime"].ToString();
                            dtAttandence.Rows[j]["MonSchIn"] = ds.Tables[0].Rows[j]["startTime"].ToString();
                            dtAttandence.Rows[j]["MonSchOut"] = ds.Tables[0].Rows[j]["EndTime"].ToString();
                            dtAttandence.Rows[j]["TueSchIn"] = ds.Tables[0].Rows[j]["startTime"].ToString();
                            dtAttandence.Rows[j]["TueSchOut"] = ds.Tables[0].Rows[j]["EndTime"].ToString();
                            dtAttandence.Rows[j]["WedSchIn"] = ds.Tables[0].Rows[j]["startTime"].ToString();
                            dtAttandence.Rows[j]["WedSchOut"] = ds.Tables[0].Rows[j]["EndTime"].ToString();
                            dtAttandence.Rows[j]["ThuSchIn"] = ds.Tables[0].Rows[j]["startTime"].ToString();
                            dtAttandence.Rows[j]["ThuSchOut"] = ds.Tables[0].Rows[j]["EndTime"].ToString();
                            dtAttandence.Rows[j]["FriSchIn"] = ds.Tables[0].Rows[j]["startTime"].ToString();
                            dtAttandence.Rows[j]["FriSchOut"] = ds.Tables[0].Rows[j]["EndTime"].ToString();
                            dtAttandence.Rows[j]["SatSchIn"] = ds.Tables[0].Rows[j]["startTime"].ToString();
                            dtAttandence.Rows[j]["SatSchOut"] = ds.Tables[0].Rows[j]["EndTime"].ToString();

                            if (dtname.Rows.Count > 0)
                            {
                                DateTime startDate = StartDate;
                                DateTime nextdate = NextDate;
                                for (int i = 0; i < 7; i++)
                                {
                                    DataView dv1 = dtname.DefaultView;
                                    dv1.RowFilter = "Logindate >= #" + startDate + "# and Logindate<#" + nextdate + "#";
                                    DataTable dt1 = dv1.ToTable();

                                    if (dt1.Rows.Count > 0)
                                    {
                                        DayOfWeek GetDay = Convert.ToDateTime(dt1.Rows[0]["Logindate"]).DayOfWeek;

                                        presentdays = presentdays + 1;
                                        switch (GetDay)
                                        {
                                            case DayOfWeek.Sunday:

                                                SunSignInCnt = SunSignInCnt + 1;
                                                dtAttandence.Rows[j]["SunSignIn"] = dt1.Rows[0]["Logindate"].ToString().Trim();
                                                dtAttandence.Rows[j]["SunSignOut"] = dt1.Rows[0]["Logoutdate"].ToString().Trim() == "" ? "N/A" : dt1.Rows[0]["Logoutdate"].ToString().Trim();
                                                if (dtAttandence.Rows[j]["SunSignOut"].ToString() != "" && dtAttandence.Rows[j]["SunSignOut"].ToString() != "N/A")
                                                {

                                                    SunSignOutCnt = SunSignOutCnt + 1;


                                                }

                                                dtAttandence.Rows[j]["SunHrs"] = dt1.Rows[0]["total hours worked"].ToString().Trim() == "" ? "N/A" : dt1.Rows[0]["total hours worked"].ToString().Trim();
                                                dtAttandence.Rows[j]["SunLogUserID"] = Convert.ToInt32(dt1.Rows[0]["LogUserID"]);
                                                dtAttandence.Rows[j]["SunLoginNotes"] = dt1.Rows[0]["loginnotes"].ToString();
                                                dtAttandence.Rows[j]["SunLogoutNotes"] = dt1.Rows[0]["logoutnotes"].ToString();
                                                dtAttandence.Rows[j]["SunLoginFlag"] = dt1.Rows[0]["loginflag"].ToString();
                                                dtAttandence.Rows[j]["SunLogoutFlag"] = dt1.Rows[0]["logoutflag"].ToString();

                                                dtAttandence.Rows[j]["SunFreeze"] = dt1.Rows[0]["Freeze"].ToString();

                                                break;

                                            case DayOfWeek.Monday:
                                                MonSignInCnt = MonSignInCnt + 1;
                                                //dtAttandence.Rows[j]["MonSchIn"] = dt1.Rows[0]["startTime"].ToString();
                                                //dtAttandence.Rows[j]["MonSchOut"] = dt1.Rows[0]["EndTime"].ToString();
                                                dtAttandence.Rows[j]["MonSignIn"] = dt1.Rows[0]["Logindate"].ToString().Trim();
                                                dtAttandence.Rows[j]["MonSignOut"] = dt1.Rows[0]["Logoutdate"].ToString() == "" ? "N/A" : dt1.Rows[0]["Logoutdate"].ToString().Trim();
                                                if (dtAttandence.Rows[j]["MonSignOut"].ToString() != "" && dtAttandence.Rows[j]["MonSignOut"].ToString() != "N/A")
                                                {

                                                    MonSignOutCnt = MonSignOutCnt + 1;


                                                }

                                                dtAttandence.Rows[j]["MonHrs"] = dt1.Rows[0]["total hours worked"].ToString() == "" ? "N/A" : dt1.Rows[0]["total hours worked"].ToString().Trim();
                                                dtAttandence.Rows[j]["MonLogUserID"] = Convert.ToInt32(dt1.Rows[0]["LogUserID"]);
                                                dtAttandence.Rows[j]["MonLoginNotes"] = dt1.Rows[0]["loginnotes"].ToString();
                                                dtAttandence.Rows[j]["MonLogoutNotes"] = dt1.Rows[0]["logoutnotes"].ToString();
                                                dtAttandence.Rows[j]["MonLoginFlag"] = dt1.Rows[0]["loginflag"].ToString();
                                                dtAttandence.Rows[j]["MonLogoutFlag"] = dt1.Rows[0]["logoutflag"].ToString();
                                                dtAttandence.Rows[j]["MonFreeze"] = dt1.Rows[0]["Freeze"].ToString();
                                                break;

                                            case DayOfWeek.Tuesday:
                                                TueSignInCnt = TueSignInCnt + 1;
                                                //dtAttandence.Rows[j]["TueSchIn"] = dt1.Rows[0]["startTime"].ToString();
                                                //dtAttandence.Rows[j]["TueSchOut"] = dt1.Rows[0]["EndTime"].ToString();
                                                dtAttandence.Rows[j]["TueSignIn"] = dt1.Rows[0]["Logindate"].ToString().Trim();
                                                dtAttandence.Rows[j]["TueSignOut"] = dt1.Rows[0]["Logoutdate"].ToString() == "" ? "N/A" : dt1.Rows[0]["Logoutdate"].ToString().Trim();
                                                if (dtAttandence.Rows[j]["TueSignOut"].ToString() != "" && dtAttandence.Rows[j]["TueSignOut"].ToString() != "N/A")
                                                {

                                                    TueSignOutCnt = TueSignOutCnt + 1;


                                                }

                                                dtAttandence.Rows[j]["TueHrs"] = dt1.Rows[0]["total hours worked"].ToString() == "" ? "N/A" : dt1.Rows[0]["total hours worked"].ToString().Trim();
                                                dtAttandence.Rows[j]["TueLogUserID"] = Convert.ToInt32(dt1.Rows[0]["LogUserID"]);
                                                dtAttandence.Rows[j]["TueLoginNotes"] = dt1.Rows[0]["loginnotes"].ToString();
                                                dtAttandence.Rows[j]["TueLogoutNotes"] = dt1.Rows[0]["logoutnotes"].ToString();
                                                dtAttandence.Rows[j]["TueLoginFlag"] = dt1.Rows[0]["loginflag"].ToString();
                                                dtAttandence.Rows[j]["TueLogoutFlag"] = dt1.Rows[0]["logoutflag"].ToString();
                                                dtAttandence.Rows[j]["TueFreeze"] = dt1.Rows[0]["Freeze"].ToString();
                                                break; // TODO: might not be correct. Was : Exit Select
                                            case DayOfWeek.Wednesday:
                                                WedSignInCnt = WedSignInCnt + 1;
                                                //dtAttandence.Rows[j]["WedSchIn"] = dt1.Rows[0]["startTime"].ToString();
                                                //dtAttandence.Rows[j]["WedSchOut"] = dt1.Rows[0]["EndTime"].ToString();
                                                dtAttandence.Rows[j]["WedSignIn"] = dt1.Rows[0]["Logindate"].ToString().Trim();
                                                dtAttandence.Rows[j]["WedSignOut"] = dt1.Rows[0]["Logoutdate"].ToString() == "" ? "N/A" : dt1.Rows[0]["Logoutdate"].ToString().Trim();
                                                if (dtAttandence.Rows[j]["WedSignOut"].ToString() != "" && dtAttandence.Rows[j]["WedSignOut"].ToString() != "N/A")
                                                {

                                                    WedSignOutCnt = WedSignOutCnt + 1;


                                                }

                                                dtAttandence.Rows[j]["WedHrs"] = dt1.Rows[0]["total hours worked"].ToString() == "" ? "N/A" : dt1.Rows[0]["total hours worked"].ToString().Trim();
                                                dtAttandence.Rows[j]["WedLogUserID"] = Convert.ToInt32(dt1.Rows[0]["LogUserID"]);
                                                dtAttandence.Rows[j]["WedLoginNotes"] = dt1.Rows[0]["loginnotes"].ToString();
                                                dtAttandence.Rows[j]["WedLogoutNotes"] = dt1.Rows[0]["logoutnotes"].ToString();
                                                dtAttandence.Rows[j]["WedLoginFlag"] = dt1.Rows[0]["loginflag"].ToString();
                                                dtAttandence.Rows[j]["WedLogoutFlag"] = dt1.Rows[0]["logoutflag"].ToString();
                                                dtAttandence.Rows[j]["WedFreeze"] = dt1.Rows[0]["Freeze"].ToString();
                                                break;
                                            case DayOfWeek.Thursday:
                                                ThuSignInCnt = ThuSignInCnt + 1;
                                                //dtAttandence.Rows[j]["ThuSchIn"] = dt1.Rows[0]["startTime"].ToString();
                                                //dtAttandence.Rows[j]["ThuSchOut"] = dt1.Rows[0]["EndTime"].ToString();
                                                dtAttandence.Rows[j]["ThuSignIn"] = dt1.Rows[0]["Logindate"].ToString().Trim();
                                                dtAttandence.Rows[j]["ThuSignOut"] = dt1.Rows[0]["Logoutdate"].ToString() == "" ? "N/A" : dt1.Rows[0]["Logoutdate"].ToString().Trim();
                                                if (dtAttandence.Rows[j]["ThuSignOut"].ToString() != "" && dtAttandence.Rows[j]["ThuSignOut"].ToString() != "N/A")
                                                {

                                                    ThuSignOutCnt = ThuSignOutCnt + 1;


                                                }


                                                dtAttandence.Rows[j]["ThuHrs"] = dt1.Rows[0]["total hours worked"].ToString() == "" ? "N/A" : dt1.Rows[0]["total hours worked"].ToString().Trim();
                                                dtAttandence.Rows[j]["ThuLogUserID"] = Convert.ToInt32(dt1.Rows[0]["LogUserID"]);
                                                dtAttandence.Rows[j]["ThuLoginNotes"] = dt1.Rows[0]["loginnotes"].ToString();
                                                dtAttandence.Rows[j]["ThuLogoutNotes"] = dt1.Rows[0]["logoutnotes"].ToString();
                                                dtAttandence.Rows[j]["ThuLoginFlag"] = dt1.Rows[0]["loginflag"].ToString();
                                                dtAttandence.Rows[j]["ThuLogoutFlag"] = dt1.Rows[0]["logoutflag"].ToString();
                                                dtAttandence.Rows[j]["ThuFreeze"] = dt1.Rows[0]["Freeze"].ToString();
                                                break;
                                            case DayOfWeek.Friday:
                                                FriSignInCnt = FriSignInCnt + 1;
                                                //dtAttandence.Rows[j]["FriSchIn"] = dt1.Rows[0]["startTime"].ToString();
                                                //dtAttandence.Rows[j]["FriSchOut"] = dt1.Rows[0]["EndTime"].ToString();
                                                dtAttandence.Rows[j]["FriSignIn"] = dt1.Rows[0]["Logindate"].ToString().Trim();
                                                dtAttandence.Rows[j]["FriSignOut"] = dt1.Rows[0]["Logoutdate"].ToString().Trim() == "" ? "N/A" : dt1.Rows[0]["Logoutdate"].ToString().Trim();
                                                if (dtAttandence.Rows[j]["FriSignOut"].ToString() != "" && dtAttandence.Rows[j]["FriSignOut"].ToString() != "N/A")
                                                {

                                                    FriSignOutCnt = FriSignOutCnt + 1;


                                                }

                                                dtAttandence.Rows[j]["FriHrs"] = dt1.Rows[0]["total hours worked"].ToString().Trim() == "" ? "N/A" : dt1.Rows[0]["total hours worked"].ToString().Trim();
                                                dtAttandence.Rows[j]["FriLogUserID"] = Convert.ToInt32(dt1.Rows[0]["LogUserID"]);
                                                dtAttandence.Rows[j]["FriLoginNotes"] = dt1.Rows[0]["loginnotes"].ToString();
                                                dtAttandence.Rows[j]["FriLogoutNotes"] = dt1.Rows[0]["logoutnotes"].ToString();
                                                dtAttandence.Rows[j]["FriLoginFlag"] = dt1.Rows[0]["loginflag"].ToString();
                                                dtAttandence.Rows[j]["FriLogoutFlag"] = dt1.Rows[0]["logoutflag"].ToString();
                                                dtAttandence.Rows[j]["FriFreeze"] = dt1.Rows[0]["Freeze"].ToString();
                                                break;
                                            case DayOfWeek.Saturday:
                                                SatSignInCnt = SatSignInCnt + 1;
                                                //dtAttandence.Rows[j]["SatSchIn"] = dt1.Rows[0]["startTime"].ToString();
                                                //dtAttandence.Rows[j]["SatSchOut"] = dt1.Rows[0]["EndTime"].ToString();
                                                dtAttandence.Rows[j]["SatSignIn"] = dt1.Rows[0]["Logindate"].ToString().Trim();
                                                dtAttandence.Rows[j]["SatSignOut"] = dt1.Rows[0]["Logoutdate"].ToString().Trim() == "" ? "N/A" : dt1.Rows[0]["Logoutdate"].ToString().Trim();

                                                if (dtAttandence.Rows[j]["SatSignOut"].ToString() != "" && dtAttandence.Rows[j]["SatSignOut"].ToString() != "N/A")
                                                {

                                                    SatSignOutCnt = SatSignOutCnt + 1;

                                                }
                                                dtAttandence.Rows[j]["SatHrs"] = dt1.Rows[0]["total hours worked"].ToString().Trim() == "" ? "N/A" : dt1.Rows[0]["total hours worked"].ToString().Trim();
                                                dtAttandence.Rows[j]["SatLogUserID"] = Convert.ToInt32(dt1.Rows[0]["LogUserID"]);
                                                dtAttandence.Rows[j]["SatLoginNotes"] = dt1.Rows[0]["loginnotes"].ToString();
                                                dtAttandence.Rows[j]["SatLogoutNotes"] = dt1.Rows[0]["logoutnotes"].ToString();
                                                dtAttandence.Rows[j]["SatLoginFlag"] = dt1.Rows[0]["loginflag"].ToString();
                                                dtAttandence.Rows[j]["SatLogoutFlag"] = dt1.Rows[0]["logoutflag"].ToString();
                                                dtAttandence.Rows[j]["SatFreeze"] = dt1.Rows[0]["Freeze"].ToString();
                                                break;
                                        }
                                    }
                                    dv1.RowFilter = null;
                                    startDate = nextdate;
                                    nextdate = GeneralFunction.GetNextDayOfWeekDate(nextdate);
                                }


                                int SumHours = (dtAttandence.Rows[j]["SunHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["SunHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["SunHrs"])));
                                SumHours = SumHours + (dtAttandence.Rows[j]["MonHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["MonHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["MonHrs"])));
                                SumHours = SumHours + (dtAttandence.Rows[j]["TueHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["TueHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["TueHrs"])));
                                SumHours = SumHours + (dtAttandence.Rows[j]["WedHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["WedHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["WedHrs"])));
                                SumHours = SumHours + (dtAttandence.Rows[j]["ThuHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["ThuHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["ThuHrs"])));
                                SumHours = SumHours + (dtAttandence.Rows[j]["FriHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["FriHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["FriHrs"])));
                                SumHours = SumHours + (dtAttandence.Rows[j]["SatHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["SatHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["SatHrs"])));
                                dtAttandence.Rows[j]["TotalHours"] = (SumHours).ToString();

                            }
                            dtAttandence.Rows[j]["PresentDays"] = presentdays;

                            dtAttandence.Rows.Add();
                        }

                        int WeeklySumSunHrs = 0;
                        int WeeklySumMonHrs = 0;
                        int WeeklySumTueHrs = 0;
                        int WeeklySumWedHrs = 0;
                        int WeeklySumThuHrs = 0;
                        int WeeklySumFriHrs = 0;
                        int WeeklySumSatHrs = 0;
                        int WeeklyTotalHrs = 0;
                        int totalPresent = 0;
                        for (int j = 0; j < dtAttandence.Rows.Count - 1; j++)
                        {
                            WeeklySumSunHrs += (dtAttandence.Rows[j]["SunHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["SunHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["SunHrs"])));
                            WeeklySumMonHrs += (dtAttandence.Rows[j]["MonHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["MonHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["MonHrs"])));
                            WeeklySumTueHrs += (dtAttandence.Rows[j]["TueHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["TueHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["TueHrs"])));
                            WeeklySumWedHrs += (dtAttandence.Rows[j]["WedHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["WedHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["WedHrs"])));
                            WeeklySumThuHrs += (dtAttandence.Rows[j]["ThuHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["ThuHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["ThuHrs"])));
                            WeeklySumFriHrs += (dtAttandence.Rows[j]["FriHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["FriHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["FriHrs"])));
                            WeeklySumSatHrs += (dtAttandence.Rows[j]["SatHrs"].ToString() == "" ? 0 : dtAttandence.Rows[j]["SatHrs"].ToString() == "N/A" ? 0 : GeneralFunction.CalNumericToint(Convert.ToDouble(dtAttandence.Rows[j]["SatHrs"])));
                            WeeklyTotalHrs += (dtAttandence.Rows[j]["TotalHours"].ToString() == "" ? 0 : dtAttandence.Rows[j]["TotalHours"].ToString() == "N/A" ? 0 : Convert.ToInt32(dtAttandence.Rows[j]["TotalHours"]));
                            totalPresent += Convert.ToInt32(dtAttandence.Rows[j]["PresentDays"].ToString());

                        }
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["SatSignIn"] = SatSignInCnt.ToString() == "0" ? "" : "<b>" + SatSignInCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["MonSignIn"] = MonSignInCnt.ToString() == "0" ? "" : "<b>" + MonSignInCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["TueSignIn"] = TueSignInCnt.ToString() == "0" ? "" : "<b>" + TueSignInCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["WedSignIn"] = WedSignInCnt.ToString() == "0" ? "" : "<b>" + WedSignInCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["ThuSignIn"] = ThuSignInCnt.ToString() == "0" ? "" : "<b>" + ThuSignInCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["FriSignIn"] = FriSignInCnt.ToString() == "0" ? "" : "<b>" + FriSignInCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["SunSignIn"] = SunSignInCnt.ToString() == "0" ? "" : "<b>" + SunSignInCnt.ToString() + "</b>";

                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["SatSignOut"] = SatSignOutCnt.ToString() == "0" ? "" : "<b>" + SatSignOutCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["MonSignOut"] = MonSignOutCnt.ToString() == "0" ? "" : "<b>" + MonSignOutCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["TueSignOut"] = TueSignOutCnt.ToString() == "0" ? "" : "<b>" + TueSignOutCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["WedSignOut"] = WedSignOutCnt.ToString() == "0" ? "" : "<b>" + WedSignOutCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["ThuSignOut"] = ThuSignOutCnt.ToString() == "0" ? "" : "<b>" + ThuSignOutCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["FriSignOut"] = FriSignOutCnt.ToString() == "0" ? "" : "<b>" + FriSignOutCnt.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["SunSignOut"] = SunSignOutCnt.ToString() == "0" ? "" : "<b>" + SunSignOutCnt.ToString() + "</b>";





                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["SunHrs"] = WeeklySumSunHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklySumSunHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["MonHrs"] = WeeklySumMonHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklySumMonHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["TueHrs"] = WeeklySumTueHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklySumTueHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["WedHrs"] = WeeklySumWedHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklySumWedHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["ThuHrs"] = WeeklySumThuHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklySumThuHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["FriHrs"] = WeeklySumFriHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklySumFriHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["SatHrs"] = WeeklySumSatHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklySumSatHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["TotalHours"] = WeeklyTotalHrs == 0 ? "" : "<b>" + GeneralFunction.ConverttoTime(WeeklyTotalHrs) + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["PresentDays"] = "<b>" + totalPresent.ToString() + "</b>";
                        dtAttandence.Rows[dtAttandence.Rows.Count - 1]["empid"] = "<b>Totals</b>";
                        dtAttandence.Rows.Add();
                    }

                }

            }
            catch (Exception ex)
            {
            }

            return dtAttandence;

        }


        protected void grdAttandence_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    Label lblStartDate = (Label)e.Row.FindControl("lblStartDate");
                    Label lblTermDate = (Label)e.Row.FindControl("lblTermDate");

                    if ((Convert.ToDateTime(lblStartDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["MonDate"]) < Convert.ToDateTime(lblStartDate.Text)))
                    {
                        Label lblMonScIn = (Label)e.Row.FindControl("lblMonScIn");
                        Label lblMonScOut = (Label)e.Row.FindControl("lblMonScOut");
                        lblMonScIn.Text = "";
                        lblMonScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblStartDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["TueDate"]) < Convert.ToDateTime(lblStartDate.Text))
                    {
                        Label lblTueScIn = (Label)e.Row.FindControl("lblTueScIn");
                        Label lblTueScOut = (Label)e.Row.FindControl("lblTueScOut");
                        lblTueScIn.Text = "";
                        lblTueScOut.Text = "";
                    }

                    if (Convert.ToDateTime(lblStartDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["WedDate"]) < Convert.ToDateTime(lblStartDate.Text))
                    {
                        Label lblWedScIn = (Label)e.Row.FindControl("lblWedScIn");
                        Label lblWedScOut = (Label)e.Row.FindControl("lblWedScOut");
                        lblWedScIn.Text = "";
                        lblWedScOut.Text = "";
                    }

                    if (Convert.ToDateTime(lblStartDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["ThuDate"]) < Convert.ToDateTime(lblStartDate.Text))
                    {
                        Label lblThuScIn = (Label)e.Row.FindControl("lblThuScIn");
                        Label lblThuScOut = (Label)e.Row.FindControl("lblThuScOut");
                        lblThuScIn.Text = "";
                        lblThuScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblStartDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["FriDate"]) < Convert.ToDateTime(lblStartDate.Text))
                    {
                        Label lblFriScIn = (Label)e.Row.FindControl("lblFriScIn");
                        Label lblFriScOut = (Label)e.Row.FindControl("lblFriScOut");
                        lblFriScIn.Text = "";
                        lblFriScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblStartDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["SatDate"]) < Convert.ToDateTime(lblStartDate.Text))
                    {
                        Label lblSatScIn = (Label)e.Row.FindControl("lblSatScIn");
                        Label lblSatScOut = (Label)e.Row.FindControl("lblSatScOut");
                        lblSatScIn.Text = "";
                        lblSatScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblStartDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["SunDate"]) < Convert.ToDateTime(lblStartDate.Text))
                    {
                        Label lblSunScIn = (Label)e.Row.FindControl("lblSunScIn");
                        Label lblSunScOut = (Label)e.Row.FindControl("lblSunScOut");
                        lblSunScIn.Text = "";
                        lblSunScOut.Text = "";
                    }

                    if ((Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["MonDate"]) > Convert.ToDateTime(lblTermDate.Text)))
                    {
                        Label lblMonScIn = (Label)e.Row.FindControl("lblMonScIn");
                        Label lblMonScOut = (Label)e.Row.FindControl("lblMonScOut");
                        lblMonScIn.Text = "";
                        lblMonScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["TueDate"]) > Convert.ToDateTime(lblTermDate.Text))
                    {
                        Label lblTueScIn = (Label)e.Row.FindControl("lblTueScIn");
                        Label lblTueScOut = (Label)e.Row.FindControl("lblTueScOut");
                        lblTueScIn.Text = "";
                        lblTueScOut.Text = "";
                    }

                    if (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["WedDate"]) > Convert.ToDateTime(lblTermDate.Text))
                    {
                        Label lblWedScIn = (Label)e.Row.FindControl("lblWedScIn");
                        Label lblWedScOut = (Label)e.Row.FindControl("lblWedScOut");
                        lblWedScIn.Text = "";
                        lblWedScOut.Text = "";
                    }

                    if (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["ThuDate"]) > Convert.ToDateTime(lblTermDate.Text))
                    {
                        Label lblThuScIn = (Label)e.Row.FindControl("lblThuScIn");
                        Label lblThuScOut = (Label)e.Row.FindControl("lblThuScOut");
                        lblThuScIn.Text = "";
                        lblThuScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["FriDate"]) > Convert.ToDateTime(lblTermDate.Text))
                    {
                        Label lblFriScIn = (Label)e.Row.FindControl("lblFriScIn");
                        Label lblFriScOut = (Label)e.Row.FindControl("lblFriScOut");
                        lblFriScIn.Text = "";
                        lblFriScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["SatDate"]) > Convert.ToDateTime(lblTermDate.Text))
                    {
                        Label lblSatScIn = (Label)e.Row.FindControl("lblSatScIn");
                        Label lblSatScOut = (Label)e.Row.FindControl("lblSatScOut");
                        lblSatScIn.Text = "";
                        lblSatScOut.Text = "";
                    }
                    if (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") != "01/01/1900" && Convert.ToDateTime(ViewState["SunDate"]) > Convert.ToDateTime(lblTermDate.Text))
                    {
                        Label lblSunScIn = (Label)e.Row.FindControl("lblSunScIn");
                        Label lblSunScOut = (Label)e.Row.FindControl("lblSunScOut");
                        lblSunScIn.Text = "";
                        lblSunScOut.Text = "";
                    }
                    LinkButton lblMonIn = (LinkButton)e.Row.FindControl("lblMonIn");
                    lblMonIn.Text = lblMonIn.Text == "" ? "" : Convert.ToDateTime(lblMonIn.Text).ToString("hh:mm tt");
                    if (lblMonIn.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["MonDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["MonDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["MonDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["MonDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblMonIn.Text = "N/A";
                            }
                        }
                    }



                    HiddenField hdnMonSignInFlag = (HiddenField)e.Row.FindControl("hdnMonSignInFlag");
                    if (hdnMonSignInFlag.Value == "True")
                    {
                        e.Row.Cells[4].BackColor = System.Drawing.Color.Orange;
                        lblMonIn.CssClass.Replace("greenTag", "");
                        //lblMonIn.ForeColor = System.Drawing.Color.Orange;
                    }
                    HiddenField hdnMonSigninNotes = (HiddenField)e.Row.FindControl("hdnMonSigninNotes");


                    Label lblName = (Label)e.Row.FindControl("lblName");
                    if (hdnMonSigninNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnMonSigninNotes.Value));
                        lblMonIn.Attributes.Add("rel", "tooltip");
                        lblMonIn.Attributes.Add("title", sTable);
                        e.Row.Cells[4].CssClass = "greenTag";
                    }

                    LinkButton lblMonOut = (LinkButton)e.Row.FindControl("lblMonOut");
                    lblMonOut.Text = lblMonOut.Text == "" ? "" : lblMonOut.Text == "N/A" ? "N/A" : Convert.ToDateTime(lblMonOut.Text).ToString("hh:mm tt");
                    if (lblMonOut.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["MonDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["MonDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["MonDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["MonDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblMonOut.Text = "N/A";
                            }

                        }
                    }

                    HiddenField hdnMonFreeze = (HiddenField)e.Row.FindControl("hdnMonFreeze");
                    if (hdnMonFreeze.Value == "True")
                    {
                        lblMonIn.Enabled = false;
                        lblMonIn.ForeColor = System.Drawing.Color.Black;
                        lblMonOut.Enabled = false;
                        lblMonOut.ForeColor = System.Drawing.Color.Black;
                    }

                    HiddenField hdnMonSignOutFlag = (HiddenField)e.Row.FindControl("hdnMonSignOutFlag");

                    if (hdnMonSignOutFlag.Value == "True")
                    {
                        e.Row.Cells[5].BackColor = System.Drawing.Color.Orange;
                        lblMonOut.CssClass.Replace("greenTag", "");
                    }

                    HiddenField hdnMonSignOutNotes = (HiddenField)e.Row.FindControl("hdnMonSignOutNotes");
                    if (hdnMonSignOutNotes.Value != "")
                    {

                        string sTable = CreateSignInTable(lblName.Text, (hdnMonSignOutNotes.Value));
                        lblMonOut.Attributes.Add("rel", "tooltip");
                        lblMonOut.Attributes.Add("title", sTable);
                        e.Row.Cells[5].CssClass = "greenTag";
                    }

                    LinkButton lblTueIn = (LinkButton)e.Row.FindControl("lblTueIn");
                    lblTueIn.Text = lblTueIn.Text == "" ? "" : Convert.ToDateTime(lblTueIn.Text).ToString("hh:mm tt");

                    if (lblTueIn.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["TueDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["TueDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {

                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["TueDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["TueDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblTueIn.Text = "N/A";
                            }


                        }
                    }


                    HiddenField hdnTueSignInFlag = (HiddenField)e.Row.FindControl("hdnTueSignInFlag");

                    if (hdnTueSignInFlag.Value == "True")
                    {
                        e.Row.Cells[9].BackColor = System.Drawing.Color.Orange;
                        //lblTueIn.ForeColor = System.Drawing.Color.Orange;
                        lblTueIn.CssClass.Replace("greenTag", "");
                    }
                    HiddenField hdnTueSigninNotes = (HiddenField)e.Row.FindControl("hdnTueSigninNotes");

                    if (hdnTueSigninNotes.Value != "")
                    {

                        string sTable = CreateSignInTable(lblName.Text, (hdnTueSigninNotes.Value));
                        lblTueIn.Attributes.Add("rel", "tooltip");
                        lblTueIn.Attributes.Add("title", sTable);
                        e.Row.Cells[9].CssClass = "greenTag";
                    }

                    LinkButton lblTueOut = (LinkButton)e.Row.FindControl("lblTueOut");
                    lblTueOut.Text = lblTueOut.Text == "" ? "" : lblTueOut.Text == "N/A" ? "N/A" : Convert.ToDateTime(lblTueOut.Text).ToString("hh:mm tt");

                    if (lblTueOut.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["TueDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["TueDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["TueDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["TueDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblTueOut.Text = "N/A";
                            }

                        }
                    }

                    HiddenField hdnTueSignOutFlag = (HiddenField)e.Row.FindControl("hdnTueSignOutFlag");

                    if (hdnTueSignOutFlag.Value == "True")
                    {
                        e.Row.Cells[10].BackColor = System.Drawing.Color.Orange;
                        //lblTueOut.ForeColor = System.Drawing.Color.Orange;
                        lblTueOut.CssClass.Replace("greenTag", "");
                    }

                    HiddenField hdnTueSignOutNotes = (HiddenField)e.Row.FindControl("hdnTueSignOutNotes");
                    if (hdnTueSignOutNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnTueSignOutNotes.Value));
                        lblTueOut.Attributes.Add("rel", "tooltip");
                        lblTueOut.Attributes.Add("title", sTable);
                        e.Row.Cells[10].CssClass = "greenTag";
                    }

                    HiddenField hdnTueFreeze = (HiddenField)e.Row.FindControl("hdnTueFreeze");
                    if (hdnTueFreeze.Value == "True")
                    {
                        lblTueIn.Enabled = false;
                        lblTueIn.ForeColor = System.Drawing.Color.Black;

                        lblTueOut.Enabled = false;
                        lblTueOut.ForeColor = System.Drawing.Color.Black;

                    }


                    LinkButton lblWedIn = (LinkButton)e.Row.FindControl("lblWedIn");
                    lblWedIn.Text = lblWedIn.Text == "" ? "" : Convert.ToDateTime(lblWedIn.Text).ToString("hh:mm tt");
                    if (lblWedIn.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["WedDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["WedDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["WedDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["WedDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblWedIn.Text = "N/A";
                            }

                        }
                    }

                    HiddenField hdnWedSignInFlag = (HiddenField)e.Row.FindControl("hdnWedSignInFlag");

                    if (hdnWedSignInFlag.Value == "True")
                    {
                        e.Row.Cells[14].BackColor = System.Drawing.Color.Orange;
                        lblWedIn.CssClass.Replace("greenTag", "");
                        // lblWedIn.ForeColor = System.Drawing.Color.Orange;
                    }

                    HiddenField hdnWedSignInNotes = (HiddenField)e.Row.FindControl("hdnWedSignInNotes");
                    if (hdnWedSignInNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnWedSignInNotes.Value));
                        lblWedIn.Attributes.Add("rel", "tooltip");
                        lblWedIn.Attributes.Add("title", sTable);
                        e.Row.Cells[14].CssClass = "greenTag";

                    }

                    LinkButton lblWedOut = (LinkButton)e.Row.FindControl("lblWedOut");
                    lblWedOut.Text = lblWedOut.Text == "" ? "" : lblWedOut.Text == "N/A" ? "N/A" : Convert.ToDateTime(lblWedOut.Text).ToString("hh:mm tt");

                    if (lblWedOut.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["WedDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["WedDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["WedDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["WedDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblWedOut.Text = "N/A";
                            }


                        }
                    }



                    HiddenField hdnWedSignOutFlag = (HiddenField)e.Row.FindControl("hdnWedSignOutFlag");

                    if (hdnWedSignOutFlag.Value == "True")
                    {
                        e.Row.Cells[15].BackColor = System.Drawing.Color.Orange;
                        lblWedOut.CssClass.Replace("greenTag", "");
                        //lblWedOut.ForeColor = System.Drawing.Color.Orange;
                    }
                    HiddenField hdnWedSignOutNotes = (HiddenField)e.Row.FindControl("hdnWedSignOutNotes");
                    if (hdnWedSignOutNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnWedSignOutNotes.Value));
                        lblWedOut.Attributes.Add("rel", "tooltip");
                        lblWedOut.Attributes.Add("title", sTable);
                        e.Row.Cells[15].CssClass = "greenTag";
                    }

                    HiddenField hdnWedFreeze = (HiddenField)e.Row.FindControl("hdnWedFreeze");
                    if (hdnWedFreeze.Value == "True")
                    {
                        lblWedIn.Enabled = false;
                        lblWedIn.ForeColor = System.Drawing.Color.Black;
                        lblWedOut.Enabled = false;
                        lblWedOut.ForeColor = System.Drawing.Color.Black;
                    }



                    LinkButton lblThuIn = (LinkButton)e.Row.FindControl("lblThuIn");
                    lblThuIn.Text = lblThuIn.Text == "" ? "" : Convert.ToDateTime(lblThuIn.Text).ToString("hh:mm tt");
                    if (lblThuIn.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["ThuDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["ThuDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {

                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["ThuDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["ThuDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblThuIn.Text = "N/A";
                            }


                        }
                    }


                    HiddenField hdnThuSignInFlag = (HiddenField)e.Row.FindControl("hdnThuSignInFlag");
                    if (hdnThuSignInFlag.Value == "True")
                    {
                        e.Row.Cells[19].BackColor = System.Drawing.Color.Orange;
                        lblThuIn.Attributes["class"] = "";
                        //lblThuIn.ForeColor = System.Drawing.Color.Orange;
                    }
                    HiddenField hdnThuSignInNotes = (HiddenField)e.Row.FindControl("hdnThuSignInNotes");
                    if (hdnThuSignInNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnThuSignInNotes.Value));
                        lblThuIn.Attributes.Add("rel", "tooltip");
                        lblThuIn.Attributes.Add("title", sTable);
                        e.Row.Cells[19].CssClass = "greenTag";
                    }
                    LinkButton lblThuOut = (LinkButton)e.Row.FindControl("lblThuOut");
                    lblThuOut.Text = lblThuOut.Text == "" ? "" : lblThuOut.Text == "N/A" ? "N/A" : Convert.ToDateTime(lblThuOut.Text).ToString("hh:mm tt");
                    if (lblThuOut.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["ThuDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["ThuDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["ThuDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["ThuDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblThuOut.Text = "N/A";
                            }

                        }
                    }


                    HiddenField hdnThuSignOutFlag = (HiddenField)e.Row.FindControl("hdnThuSignOutFlag");
                    if (hdnThuSignOutFlag.Value == "True")
                    {
                        e.Row.Cells[20].BackColor = System.Drawing.Color.Orange;
                        lblThuOut.CssClass.Replace("greenTag", "");
                    }
                    HiddenField hdnThuSignOutNotes = (HiddenField)e.Row.FindControl("hdnThuSignOutNotes");

                    if (hdnThuSignOutNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnThuSignOutNotes.Value));
                        lblThuOut.Attributes.Add("rel", "tooltip");
                        lblThuOut.Attributes.Add("title", sTable);
                        e.Row.Cells[20].CssClass = "greenTag";
                    }
                    HiddenField hdnThuFreeze = (HiddenField)e.Row.FindControl("hdnThuFreeze");
                    if (hdnThuFreeze.Value == "True")
                    {
                        lblThuIn.Enabled = false;
                        lblThuIn.ForeColor = System.Drawing.Color.Black;
                        lblThuOut.Enabled = false;
                        lblThuOut.ForeColor = System.Drawing.Color.Black;
                    }
                    LinkButton lblFriIn = (LinkButton)e.Row.FindControl("lblFriIn");
                    lblFriIn.Text = lblFriIn.Text == "" ? "" : Convert.ToDateTime(lblFriIn.Text).ToString("hh:mm tt");

                    if (lblFriIn.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["FriDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["FriDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FriDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["FriDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblFriIn.Text = "N/A";
                            }

                        }
                    }

                    HiddenField hdnFriSignInFlag = (HiddenField)e.Row.FindControl("hdnFriSignInFlag");
                    if (hdnFriSignInFlag.Value == "True")
                    {
                        e.Row.Cells[24].BackColor = System.Drawing.Color.Orange;
                        lblFriIn.CssClass.Replace("greenTag", "");

                    }
                    HiddenField hdnFriSignInNotes = (HiddenField)e.Row.FindControl("hdnFriSignInNotes");
                    if (hdnFriSignInNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnFriSignInNotes.Value));
                        lblFriIn.Attributes.Add("rel", "tooltip");
                        lblFriIn.Attributes.Add("title", sTable);
                        e.Row.Cells[24].CssClass = "greenTag";
                    }
                    LinkButton lblFriOut = (LinkButton)e.Row.FindControl("lblFriOut");
                    lblFriOut.Text = lblFriOut.Text == "" ? "" : lblFriOut.Text == "N/A" ? "N/A" : Convert.ToDateTime(lblFriOut.Text).ToString("hh:mm tt");
                    if (lblFriOut.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["FriDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["FriDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FriDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["FriDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblFriOut.Text = "N/A";
                            }


                        }
                    }


                    HiddenField hdnFriSignOutFlag = (HiddenField)e.Row.FindControl("hdnFriSignOutFlag");
                    if (hdnFriSignOutFlag.Value == "True")
                    {
                        e.Row.Cells[25].BackColor = System.Drawing.Color.Orange;
                        lblFriOut.CssClass.Replace("greenTag", "");

                    }
                    HiddenField hdnFriSignOutNotes = (HiddenField)e.Row.FindControl("hdnFriSignOutNotes");

                    if (hdnFriSignOutNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnFriSignOutNotes.Value));
                        lblFriOut.Attributes.Add("rel", "tooltip");
                        lblFriOut.Attributes.Add("title", sTable);
                        e.Row.Cells[25].CssClass = "greenTag";
                    }


                    HiddenField hdnFriFreeze = (HiddenField)e.Row.FindControl("hdnFriFreeze");
                    if (hdnFriFreeze.Value == "True")
                    {
                        lblFriIn.Enabled = false;
                        lblFriIn.ForeColor = System.Drawing.Color.Black;
                        lblFriOut.Enabled = false;
                        lblFriOut.ForeColor = System.Drawing.Color.Black;
                    }

                    LinkButton lblSatIn = (LinkButton)e.Row.FindControl("lblSatIn");
                    lblSatIn.Text = lblSatIn.Text == "" ? "" : Convert.ToDateTime(lblSatIn.Text).ToString("hh:mm tt");
                    HiddenField hdnSatSignInFlag = (HiddenField)e.Row.FindControl("hdnSatSignInFlag");
                    if (lblSatIn.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["SatDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["SatDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["SatDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["SatDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblSatIn.Text = "N/A";
                            }


                        }
                    }


                    if (hdnSatSignInFlag.Value == "True")
                    {
                        e.Row.Cells[29].BackColor = System.Drawing.Color.Orange;
                        lblSatIn.CssClass.Replace("greenTag", "");
                    }
                    HiddenField hdnSatSignInNotes = (HiddenField)e.Row.FindControl("hdnSatSignInNotes");
                    //lblSatIn.ForeColor = System.Drawing.Color.Orange;

                    if (hdnSatSignInNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnSatSignInNotes.Value));
                        lblSatIn.Attributes.Add("rel", "tooltip");
                        lblSatIn.Attributes.Add("title", sTable);
                        e.Row.Cells[29].CssClass = "greenTag";
                    }


                    LinkButton lblSatOut = (LinkButton)e.Row.FindControl("lblSatOut");
                    lblSatOut.Text = lblSatOut.Text == "" ? "" : lblSatOut.Text == "N/A" ? "N/A" : Convert.ToDateTime(lblSatOut.Text).ToString("hh:mm tt");
                    if (lblSatOut.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["SatDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["SatDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["SatDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["SatDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblSatOut.Text = "N/A";
                            }


                        }
                    }

                    HiddenField hdnSatSignOutFlag = (HiddenField)e.Row.FindControl("hdnSatSignOutFlag");
                    if (hdnSatSignOutFlag.Value == "True")
                    {
                        e.Row.Cells[30].BackColor = System.Drawing.Color.Orange;
                        lblSatOut.CssClass.Replace("greenTag", "");
                    }

                    HiddenField hdnSatSignOutNotes = (HiddenField)e.Row.FindControl("hdnSatSignOutNotes");

                    if (hdnSatSignOutNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnSatSignOutNotes.Value));
                        lblSatOut.Attributes.Add("rel", "tooltip");
                        lblSatOut.Attributes.Add("title", sTable);
                        e.Row.Cells[30].CssClass = "greenTag";
                    }


                    HiddenField hdnSatFreeze = (HiddenField)e.Row.FindControl("hdnSatFreeze");
                    if (hdnSatFreeze.Value == "True")
                    {
                        lblSatIn.Enabled = false;
                        lblSatIn.ForeColor = System.Drawing.Color.Black;
                        lblSatOut.Enabled = false;
                        lblSatOut.ForeColor = System.Drawing.Color.Black;
                    }


                    LinkButton lblSunIn = (LinkButton)e.Row.FindControl("lblSunIn");
                    lblSunIn.Text = lblSunIn.Text == "" ? "" : Convert.ToDateTime(lblSunIn.Text).ToString("hh:mm tt");
                    if (lblSunIn.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["SunDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["SunDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["SunDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["SunDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblSunIn.Text = "N/A";
                            }

                        }
                    }

                    HiddenField hdnSunSignInFlag = (HiddenField)e.Row.FindControl("hdnSunSignInFlag");

                    HiddenField hdnSunSignInNotes = (HiddenField)e.Row.FindControl("hdnSunSignInNotes");
                    if (hdnSunSignInNotes.Value == "True")
                    {
                        e.Row.Cells[34].BackColor = System.Drawing.Color.Orange;
                        lblSunIn.Attributes["class"] = "";
                        // lblSunIn.ForeColor = System.Drawing.Color.Orange;
                    }

                    if (hdnSunSignInNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnSunSignInNotes.Value));
                        lblSunIn.Attributes.Add("rel", "tooltip");
                        lblSunIn.Attributes.Add("title", sTable);
                        e.Row.Cells[34].CssClass = "greenTag";
                    }



                    LinkButton lblSunOut = (LinkButton)e.Row.FindControl("lblSunOut");
                    lblSunOut.Text = lblSunOut.Text == "" ? "" : lblSunOut.Text == "N/A" ? "N/A" : Convert.ToDateTime(lblSunOut.Text).ToString("hh:mm tt");
                    if (lblSunOut.Text == "")
                    {
                        if ((Convert.ToDateTime(ViewState["SunDate"]) >= Convert.ToDateTime(lblStartDate.Text)) && (Convert.ToDateTime(lblTermDate.Text).ToString("MM/dd/yyyy") == "01/01/1900" || Convert.ToDateTime(ViewState["SunDate"]) <= Convert.ToDateTime(lblTermDate.Text)))
                        {
                            if ((Convert.ToDateTime(Convert.ToDateTime(Session["TodayBannerDate"]).ToString("MM/dd/yyyy"))) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["SunDate"])).ToString("MM/dd/yyyy")) && (Convert.ToDateTime((Convert.ToDateTime(ViewState["SunDate"])).ToString("MM/dd/yyyy")) >= Convert.ToDateTime((Convert.ToDateTime(ViewState["FreezeDate"])).ToString("MM/dd/yyyy"))))
                            {
                                lblSunOut.Text = "N/A";
                            }

                        }
                    }



                    HiddenField hdnSunSignOutFlag = (HiddenField)e.Row.FindControl("hdnSunSignOutFlag");
                    if (hdnSunSignOutFlag.Value == "True")
                    {
                        e.Row.Cells[35].BackColor = System.Drawing.Color.Orange;
                        lblSunOut.CssClass.Replace("greenTag", "");
                        // lblSunOut.ForeColor = System.Drawing.Color.Orange;
                    }
                    HiddenField hdnSunSignOutNotes = (HiddenField)e.Row.FindControl("hdnSunSignOutNotes");

                    if (hdnSunSignInNotes.Value != "")
                    {
                        string sTable = CreateSignInTable(lblName.Text, (hdnSunSignOutNotes.Value));
                        lblSunOut.Attributes.Add("rel", "tooltip");
                        lblSunOut.Attributes.Add("title", sTable);
                        e.Row.Cells[35].CssClass = "greenTag";
                    }

                    HiddenField hdnSunFreeze = (HiddenField)e.Row.FindControl("hdnSunFreeze");
                    if (hdnSunFreeze.Value == "True")
                    {
                        lblSunIn.Enabled = false;
                        lblSunIn.ForeColor = System.Drawing.Color.Black;
                        lblSunOut.Enabled = false;
                        lblSunOut.ForeColor = System.Drawing.Color.Black;
                    }

                    Label lblMonHours = (Label)e.Row.FindControl("lblMonHours");
                    lblMonHours.Text = lblMonHours.Text == "N/A" ? "" : lblMonHours.Text == "" ? "" : GeneralFunction.ConverttoTime(GeneralFunction.CalNumericToint(Convert.ToDouble(lblMonHours.Text)));
                    Label lblTueHours = (Label)e.Row.FindControl("lblTueHours");
                    lblTueHours.Text = lblTueHours.Text == "N/A" ? "" : lblTueHours.Text == "" ? "" : GeneralFunction.ConverttoTime(GeneralFunction.CalNumericToint(Convert.ToDouble(lblTueHours.Text)));
                    Label lblWedHours = (Label)e.Row.FindControl("lblWedHours");
                    lblWedHours.Text = lblWedHours.Text == "N/A" ? "" : lblWedHours.Text == "" ? "" : GeneralFunction.ConverttoTime(GeneralFunction.CalNumericToint(Convert.ToDouble(lblWedHours.Text)));
                    Label lblThuHours = (Label)e.Row.FindControl("lblThuHours");
                    lblThuHours.Text = lblThuHours.Text == "N/A" ? "" : lblThuHours.Text == "" ? "" : GeneralFunction.ConverttoTime(GeneralFunction.CalNumericToint(Convert.ToDouble(lblThuHours.Text)));
                    Label lblFriHours = (Label)e.Row.FindControl("lblFriHours");
                    lblFriHours.Text = lblFriHours.Text == "N/A" ? "" : lblFriHours.Text == "" ? "" : GeneralFunction.ConverttoTime(GeneralFunction.CalNumericToint(Convert.ToDouble(lblFriHours.Text)));
                    Label lblSatHours = (Label)e.Row.FindControl("lblSatHours");
                    lblSatHours.Text = lblSatHours.Text == "N/A" ? "" : lblSatHours.Text == "" ? "" : GeneralFunction.ConverttoTime(GeneralFunction.CalNumericToint(Convert.ToDouble(lblSatHours.Text)));
                    Label lblSunHours = (Label)e.Row.FindControl("lblSunHours");
                    lblSunHours.Text = lblSunHours.Text == "N/A" ? "" : lblSunHours.Text == "" ? "" : GeneralFunction.ConverttoTime(GeneralFunction.CalNumericToint(Convert.ToDouble(lblSunHours.Text)));
                    Label lblTotalHours = (Label)e.Row.FindControl("lblTotalHours");
                    lblTotalHours.Text = lblTotalHours.Text == "0" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblTotalHours.Text));



                }
            }
            catch (Exception ex)
            {
            }
        }


        protected void grdAttandence_RowCreated(object sender, GridViewRowEventArgs e)
        {

            try
            {
                DateTime startdate = GeneralFunction.GetFirstDayOfWeekDate(Convert.ToDateTime(Session["TodayDate"]));

                if (e.Row.RowType == DataControlRowType.Header)
                {
                    GridView HeaderGrid = (GridView)sender;

                    GridViewRow HeaderGridRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                    TableCell HeaderCell = new TableCell();
                    HeaderCell.Text = "";
                    HeaderCell.ColumnSpan = 1;

                    HeaderGridRow.Cells.Add(HeaderCell);

                    HeaderCell = new TableCell();
                    HeaderCell.Text = "";
                    HeaderCell.ColumnSpan = 1;
                    HeaderGridRow.Cells.Add(HeaderCell);

                    HeaderCell = new TableCell();
                    HeaderCell.Text = startdate.ToString("dddd") + ", " + startdate.ToString("MMMM dd, yyyy");
                    ViewState["MonDate"] = startdate;
                    HeaderCell.Style["text-align"] = "center";
                    HeaderCell.ColumnSpan = 5;
                    HeaderGridRow.Cells.Add(HeaderCell);

                    HeaderCell = new TableCell();
                    HeaderCell.Text = startdate.AddDays(1).ToString("dddd") + ", " + startdate.AddDays(1).ToString("MMMM dd, yyyy");
                    ViewState["TueDate"] = startdate.AddDays(1);
                    HeaderCell.Style["text-align"] = "center";
                    HeaderCell.ColumnSpan = 5;
                    HeaderCell.Style["font-weight"] = "bold";
                    HeaderGridRow.Cells.Add(HeaderCell);

                    HeaderCell = new TableCell();
                    HeaderCell.Text = startdate.AddDays(2).ToString("dddd") + ", " + startdate.AddDays(2).ToString("MMMM dd, yyyy");
                    ViewState["WedDate"] = startdate.AddDays(2);
                    HeaderCell.Style["text-align"] = "center";
                    HeaderCell.ColumnSpan = 5;
                    HeaderGridRow.Cells.Add(HeaderCell);

                    HeaderCell = new TableCell();
                    HeaderCell.Text = startdate.AddDays(3).ToString("dddd") + ", " + startdate.AddDays(3).ToString("MMMM dd, yyyy");
                    ViewState["ThuDate"] = startdate.AddDays(3);
                    HeaderCell.Style["text-align"] = "center";
                    HeaderCell.ColumnSpan = 5;
                    HeaderGridRow.Cells.Add(HeaderCell);

                    HeaderCell = new TableCell();
                    HeaderCell.Text = startdate.AddDays(4).ToString("dddd") + ", " + startdate.AddDays(4).ToString("MMMM dd, yyyy");
                    ViewState["FriDate"] = startdate.AddDays(4);
                    HeaderCell.Style["text-align"] = "center";
                    HeaderCell.ColumnSpan = 5;
                    HeaderGridRow.Cells.Add(HeaderCell);

                    HeaderCell = new TableCell();
                    HeaderCell.Text = startdate.AddDays(5).ToString("dddd") + ", " + startdate.AddDays(5).ToString("MMMM dd, yyyy");
                    ViewState["SatDate"] = startdate.AddDays(5);
                    HeaderCell.Style["text-align"] = "center";
                    HeaderCell.ColumnSpan = 5;
                    HeaderGridRow.Cells.Add(HeaderCell);


                    HeaderCell = new TableCell();
                    HeaderCell.Text = startdate.AddDays(6).ToString("dddd") + ", " + startdate.AddDays(6).ToString("MMMM dd, yyyy");
                    ViewState["SunDate"] = startdate.AddDays(6);
                    HeaderCell.Style["text-align"] = "center";
                    HeaderCell.ColumnSpan = 5;


                    HeaderGridRow.Cells.Add(HeaderCell);



                    HeaderCell = new TableCell();
                    HeaderCell.Text = "Total";
                    HeaderCell.ColumnSpan = 2;
                    HeaderCell.Style["text-align"] = "center";
                    HeaderGridRow.Cells.Add(HeaderCell);


                    grdAttandence.Controls[0].Controls.AddAt(0, HeaderGridRow);

                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlReportType.SelectedItem.Value == "0")
                {
                    int userid = Convert.ToInt32(Session["UserID"]);
                    DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime PrevWeek = TodayDate.AddDays(-7);
                    Session["TodayDate"] = PrevWeek.ToString();


                    if (GeneralFunction.GetFirstDayOfWeekDate(PrevWeek).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }

                    DateTime PreWeekStart = GeneralFunction.GetFirstDayOfWeekDate(PrevWeek);
                    DateTime PreWeekEnd = GeneralFunction.GetLastDayOfWeekDate(PrevWeek);
                    //DataTable ds = GetReport(PreWeekStart, PreWeekEnd, userid);
                    string Ismanage = Session["IsManage"].ToString();
                    string IsAdmin = Session["IsAdmin"].ToString();
                    DataTable ds = new DataTable();

                    btnFreeze.Visible = true;
                    lblFreeze.Visible = true;
                    //   DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartOfMonth = PreWeekStart.AddDays(-1);
                    DateTime FreezeDate = StartOfMonth;

                    Attendance.BAL.Report obj = new Report();
                    // int CNT = obj.GetFreezedDate(FreezeDate);
                    DateTime CNT = obj.GetFreezedDate(FreezeDate, ddlLocation.SelectedItem.Text.ToString().Trim());
                    lblFreezedate.Text = FreezeDate.ToString("MM/dd/yyyy");
                    hdnFreeze.Value = FreezeDate.ToString("MM/dd/yyyy");
                    if (CNT.ToString("MM/dd/yyyy") != "01/01/1900")
                    {
                        lblFreeze.Text = "Attendance freezed until " + CNT.ToString("MM/dd/yyyy");
                        ViewState["FreezeDate"] = CNT.ToString("MM/dd/yyyy");
                        btnFreeze.CssClass = "btn btn-warning btn-small disabled";
                        btnFreeze.Enabled = false;
                    }
                    else
                    {
                        lblFreeze.Visible = false;
                        btnFreeze.CssClass = "btn btn-warning btn-small enabled";
                        btnFreeze.Enabled = true;
                    }


                    ds = GetReportAdmin(PreWeekStart, PreWeekEnd, Convert.ToInt32(ddlLocation.SelectedValue));
                    Session["AtnAdminDetails"] = ds;
                    grdAttandence.DataSource = ds;
                    grdAttandence.DataBind();
                    grdMonthlyAttendance.DataSource = null;
                    grdMonthlyAttendance.DataBind();
                    grdWeeklyAttendance.DataSource = null;
                    grdWeeklyAttendance.DataBind();
                }
                else if (ddlReportType.SelectedItem.Value == "1")
                {
                    int userid = Convert.ToInt32(Session["UserID"]);
                    DateTime todayDate = Convert.ToDateTime(ViewState["TodayDate1"]);
                    ViewState["TodayDate1"] = todayDate.AddDays(-28);
                    // DateTime startOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
                    DateTime startDate = GeneralFunction.GetFirstDayOfWeekDate(todayDate.AddDays(-28));
                    DateTime StartDate = startDate;
                    hdnWeeklyStartDt.Value = StartDate.ToString();
                    DateTime endDate = todayDate.AddDays(-1);

                    if (GeneralFunction.GetFirstDayOfWeekDate(startDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now.AddDays(-7)).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }

                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;


                    DataTable dt = GetWeeklyReport(StartDate, endDate);

                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = dt;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = null;
                        grdMonthlyAttendance.DataBind();
                    }



                }

                else if (ddlReportType.SelectedItem.Value == "2")
                {
                    //  int userid = Convert.ToInt32(Session["UserID"]);
                    DateTime startDate = Convert.ToDateTime(ViewState["StartMonth"]);
                    DateTime StartDate = startDate.AddMonths(-6);
                    hdnMonthlyStartDt.Value = StartDate.ToString();
                    DateTime endDate = startDate.AddSeconds(-1);
                    ViewState["StartMonth"] = StartDate;
                    DataTable dt = GetMonthlyreport(StartDate, endDate);
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;

                    if (endDate.ToString("MM/dd/yyyy") == (DateTime.Now.AddDays(1 - DateTime.Now.Day)).AddDays(-1).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = null;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = dt;
                        grdMonthlyAttendance.DataBind();
                    }

                }


            }
            catch (Exception ex)
            {
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {

                if (ddlReportType.SelectedItem.Value == "0")
                {
                    int userid = Convert.ToInt32(Session["UserID"]);
                    DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime NextWeek = TodayDate.AddDays(7);
                    Session["TodayDate"] = NextWeek.ToString();


                    if (GeneralFunction.GetFirstDayOfWeekDate(NextWeek).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }



                    DateTime NextWeekStart = GeneralFunction.GetFirstDayOfWeekDate(NextWeek);
                    DateTime NextWeekEnd = GeneralFunction.GetLastDayOfWeekDate(NextWeek);
                    //  DataTable ds = GetReport(NextWeekStart, NextWeekEnd, userid);
                    string Ismanage = Session["IsManage"].ToString();
                    string IsAdmin = Session["IsAdmin"].ToString();
                    DataTable ds = new DataTable();

                    btnFreeze.Visible = true;
                    lblFreeze.Visible = true;
                    //   DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartOfMonth = NextWeekStart.AddDays(-1);
                    DateTime FreezeDate = StartOfMonth;
                    Attendance.BAL.Report obj = new Report();
                    //  int CNT = obj.GetFreezedDate(FreezeDate);
                    DateTime CNT = obj.GetFreezedDate(FreezeDate, ddlLocation.SelectedItem.Text.ToString().Trim());
                    lblFreezedate.Text = FreezeDate.ToString("MM/dd/yyyy");
                    hdnFreeze.Value = FreezeDate.ToString("MM/dd/yyyy");
                    if (CNT.ToString("MM/dd/yyyy") != "01/01/1900")
                    {
                        lblFreeze.Text = "Attendance freezed until " + CNT.ToString("MM/dd/yyyy");
                        ViewState["FreezeDate"] = CNT.ToString("MM/dd/yyyy");
                        btnFreeze.CssClass = "btn btn-warning btn-small disabled";
                        btnFreeze.Enabled = false;
                    }
                    else
                    {
                        lblFreeze.Visible = false;
                        btnFreeze.CssClass = "btn btn-warning btn-small enabled";
                        btnFreeze.Enabled = true;
                    }



                    ds = GetReportAdmin(NextWeekStart, NextWeekEnd, Convert.ToInt32(ddlLocation.SelectedValue));
                    Session["AtnAdminDetails"] = ds;
                    grdAttandence.DataSource = ds;
                    grdAttandence.DataBind();
                    grdMonthlyAttendance.DataSource = null;
                    grdMonthlyAttendance.DataBind();
                    grdWeeklyAttendance.DataSource = null;
                    grdWeeklyAttendance.DataBind();

                }
                else if (ddlReportType.SelectedItem.Value == "1")
                {
                    int userid = Convert.ToInt32(Session["UserID"]);
                    DateTime todayDate = Convert.ToDateTime(ViewState["TodayDate1"]);
                    ViewState["TodayDate1"] = todayDate.AddDays(28);
                    // DateTime startOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
                    DateTime startDate = GeneralFunction.GetFirstDayOfWeekDate(todayDate.AddDays(28));
                    DateTime StartDate = startDate;
                    hdnWeeklyStartDt.Value = StartDate.ToString();
                    DateTime endDate = startDate.AddDays(27);


                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;
                    if (GeneralFunction.GetFirstDayOfWeekDate(endDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now.AddDays(-7)).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }

                    DataTable dt = GetWeeklyReport(StartDate, endDate);

                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = dt;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = null;
                        grdMonthlyAttendance.DataBind();

                    }
                }

                else if (ddlReportType.SelectedItem.Value == "2")
                {
                    //int userid = Convert.ToInt32(Session["UserID"]);
                    DateTime startDate = Convert.ToDateTime(ViewState["StartMonth"]);
                    DateTime StartDate = startDate.AddMonths(6);
                    hdnMonthlyStartDt.Value = StartDate.ToString();
                    DateTime endDate = StartDate.AddMonths(6).AddSeconds(-1);
                    ViewState["StartMonth"] = StartDate;
                    DataTable dt = GetMonthlyreport(StartDate, endDate);
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;

                    if (endDate.ToString("MM/dd/yyyy") == (DateTime.Now.AddDays(1 - DateTime.Now.Day)).AddDays(-1).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = null;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = dt;
                        grdMonthlyAttendance.DataBind();
                    }

                }
            }

            catch (Exception ex)
            {
            }
        }

        private string CreateSignInTable(string Employeename, string SignInNotes)
        {
            string strTransaction = string.Empty;
            strTransaction = "<table class=\"noPading\"  id=\"SalesStatus\" style=\"display: table; border-collapse:collapse;  width:100%; background-color:#FFFFFF;border:2px;border-color:Black; \">";
            strTransaction += "<tr id=\"CampaignsTitle1\" style=\"background-color:#ccc;color:#121212;\">";
            strTransaction += "<td align=\"center\" colspan=\"2\" >";
            strTransaction += "<b style=\"text-align:center; display:block\"  >" + Employeename + "</b>";
            strTransaction += "</td>";
            strTransaction += "</tr>";

            strTransaction += "<tr>";

            strTransaction += "<td style=\"width:150px;\">";
            strTransaction += "<div style=\"height:150px;overflow-y:scroll;overflow-x:hidden;\">";
            strTransaction += HttpUtility.HtmlDecode(SignInNotes).Replace("<br/>", "\n");
            strTransaction += "<div>";
            strTransaction += "</td>";
            strTransaction += "</tr>";

            strTransaction += "</table>";

            return strTransaction;

        }
        protected void grdAttandence_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                DataTable dt = Session["AtnAdminDetails"] as DataTable;
                DataView dv = dt.DefaultView;
                if (e.CommandName.ToString() == "LoginMonEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "MonLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();
                        hdnSchInTime.Value = dt1.Rows[0]["MonSchIn"].ToString();
                        hdnSignInTime.Value = dt1.Rows[0]["MonSignIn"].ToString() == "N/A" ? Convert.ToDateTime(ViewState["MonDate"]).ToString("MM/dd/yyyy hh:mm tt") : Convert.ToDateTime(dt1.Rows[0]["MonSignIn"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        hdnSignOutTimeHrs.Value = dt1.Rows[0]["MonSignOut"].ToString() == "N/A" ? dt1.Rows[0]["MonSchOut"].ToString() : Convert.ToDateTime(dt1.Rows[0]["MonSignOut"]).ToString("hh:mm tt").Trim();
                        txtSignIn.Text = dt1.Rows[0]["MonSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["MonSignIn"]).ToString("hh:mm tt").Trim();
                        lblLoginPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLoginDay.Text = "Monday";
                    }
                    else
                    {
                        GridViewRow gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                        int gvrowIndex = gvrow.RowIndex;
                        hdnLogUserID.Value = Loguserid.ToString();
                        Label lblMonScIn = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblMonScIn");
                        hdnSchInTime.Value = lblMonScIn.Text;
                        hdnSignInTime.Value = Convert.ToDateTime(ViewState["MonDate"]).ToString("MM/dd/yyyy hh:mm tt");
                        hdnSignOutTimeHrs.Value = "N/A";
                        txtSignIn.Text = "";
                        Label lblName = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblName");
                        lblLoginPopName.Text = lblName.Text.Trim();
                        lblLoginDay.Text = "Monday";
                        Label lblID = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblID");
                        hdnEmpID.Value = lblID.Text.Trim();
                    }
                    mdlLoginPopUpEdit.Show();
                }
                else if (e.CommandName.ToString() == "LoginTueEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {

                        hdnLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "TueLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();
                        hdnSchInTime.Value = dt1.Rows[0]["TueSchIn"].ToString();
                        hdnSignOutTimeHrs.Value = dt1.Rows[0]["TueSignOut"].ToString() == "N/A" ? dt1.Rows[0]["TueSchOut"].ToString() : Convert.ToDateTime(dt1.Rows[0]["TueSignOut"]).ToString("hh:mm tt").Trim();
                        hdnSignInTime.Value = dt1.Rows[0]["TueSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["TueSignIn"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignIn.Text = dt1.Rows[0]["TueSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["TueSignIn"]).ToString("hh:mm tt").Trim();
                        // dt1.Rows[0]["TueSignIn"].ToString();
                        lblLoginPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLoginDay.Text = "Tuesday";
                    }
                    else
                    {
                        GridViewRow gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                        int gvrowIndex = gvrow.RowIndex;
                        hdnLogUserID.Value = Loguserid.ToString();
                        Label lblTueScIn = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblTueScIn");
                        hdnSchInTime.Value = lblTueScIn.Text;
                        hdnSignInTime.Value = Convert.ToDateTime(ViewState["TueDate"]).ToString("MM/dd/yyyy hh:mm tt");
                        hdnSignOutTimeHrs.Value = "N/A";
                        txtSignIn.Text = "";
                        Label lblName = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblName");
                        lblLoginPopName.Text = lblName.Text.Trim();
                        lblLoginDay.Text = "Tuesday";
                        Label lblID = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblID");
                        hdnEmpID.Value = lblID.Text.Trim();
                    }
                    mdlLoginPopUpEdit.Show();

                }
                else if (e.CommandName.ToString() == "LoginWedEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "WedLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();
                        hdnSchInTime.Value = dt1.Rows[0]["WedSchIn"].ToString();
                        // hdnSchInTime.Value = dt1.Rows[0]["TueSchIn"].ToString();
                        hdnSignOutTimeHrs.Value = dt1.Rows[0]["WedSignOut"].ToString() == "N/A" ? dt1.Rows[0]["WedSchOut"].ToString() : Convert.ToDateTime(dt1.Rows[0]["WedSignOut"]).ToString("hh:mm tt").Trim();
                        hdnSignInTime.Value = dt1.Rows[0]["WedSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["WedSignIn"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignIn.Text = dt1.Rows[0]["WedSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["WedSignIn"]).ToString("hh:mm tt").Trim();
                        // dt1.Rows[0]["WedSignIn"].ToString();
                        lblLoginPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLoginDay.Text = "Wednesday";
                    }
                    else
                    {
                        GridViewRow gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                        int gvrowIndex = gvrow.RowIndex;
                        hdnLogUserID.Value = Loguserid.ToString();
                        Label lblWedScIn = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblWedScIn");
                        hdnSchInTime.Value = lblWedScIn.Text;
                        hdnSignInTime.Value = Convert.ToDateTime(ViewState["WedDate"]).ToString("MM/dd/yyyy hh:mm tt");
                        hdnSignOutTimeHrs.Value = "N/A";
                        txtSignIn.Text = "";
                        Label lblName = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblName");
                        lblLoginPopName.Text = lblName.Text.Trim();
                        lblLoginDay.Text = "Wednesday";
                        Label lblID = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblID");
                        hdnEmpID.Value = lblID.Text.Trim();
                    }
                    mdlLoginPopUpEdit.Show();
                }
                else if (e.CommandName.ToString() == "LoginThuEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "ThuLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();
                        hdnSchInTime.Value = dt1.Rows[0]["ThuSchIn"].ToString();
                        hdnSignOutTimeHrs.Value = dt1.Rows[0]["ThuSignOut"].ToString() == "N/A" ? dt1.Rows[0]["ThuSchOut"].ToString() : Convert.ToDateTime(dt1.Rows[0]["ThuSignOut"]).ToString("hh:mm tt").Trim();

                        hdnSignInTime.Value = dt1.Rows[0]["ThuSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["ThuSignIn"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignIn.Text = dt1.Rows[0]["ThuSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["ThuSignIn"]).ToString("hh:mm tt").Trim();
                        // dt1.Rows[0]["ThuSignIn"].ToString();
                        lblLoginPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLoginDay.Text = "Thursday";
                    }
                    else
                    {
                        GridViewRow gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                        int gvrowIndex = gvrow.RowIndex;
                        hdnLogUserID.Value = Loguserid.ToString();
                        Label lblThuScIn = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblThuScIn");
                        hdnSchInTime.Value = lblThuScIn.Text;
                        hdnSignInTime.Value = Convert.ToDateTime(ViewState["ThuDate"]).ToString("MM/dd/yyyy hh:mm tt");
                        hdnSignOutTimeHrs.Value = "N/A";
                        txtSignIn.Text = "";
                        Label lblName = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblName");
                        lblLoginPopName.Text = lblName.Text.Trim();
                        lblLoginDay.Text = "Thursday";
                        Label lblID = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblID");
                        hdnEmpID.Value = lblID.Text.Trim();
                    }
                    mdlLoginPopUpEdit.Show();
                }
                else if (e.CommandName.ToString() == "LoginFriEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "FriLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();

                        hdnSchInTime.Value = dt1.Rows[0]["FriSchIn"].ToString();
                        hdnSignOutTimeHrs.Value = dt1.Rows[0]["FriSignOut"].ToString() == "N/A" ? dt1.Rows[0]["FriSchOut"].ToString() : Convert.ToDateTime(dt1.Rows[0]["FriSignOut"]).ToString("hh:mm tt").Trim();


                        hdnSignInTime.Value = dt1.Rows[0]["FriSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["FriSignIn"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignIn.Text = dt1.Rows[0]["FriSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["FriSignIn"]).ToString("hh:mm tt").Trim();
                        // dt1.Rows[0]["FriSignIn"].ToString();
                        lblLoginPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLoginDay.Text = "Friday";
                    }
                    else
                    {
                        GridViewRow gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                        int gvrowIndex = gvrow.RowIndex;
                        hdnLogUserID.Value = Loguserid.ToString();
                        Label lblFriScIn = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblFriScIn");
                        hdnSchInTime.Value = lblFriScIn.Text;
                        hdnSignInTime.Value = Convert.ToDateTime(ViewState["FriDate"]).ToString("MM/dd/yyyy hh:mm tt");
                        hdnSignOutTimeHrs.Value = "N/A";
                        txtSignIn.Text = "";
                        Label lblName = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblName");
                        lblLoginPopName.Text = lblName.Text.Trim();
                        lblLoginDay.Text = "Friday";
                        Label lblID = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblID");
                        hdnEmpID.Value = lblID.Text.Trim();
                    }
                    mdlLoginPopUpEdit.Show();
                }

                else if (e.CommandName.ToString() == "LoginSatEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "SatLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();
                        hdnSchInTime.Value = dt1.Rows[0]["SatSchIn"].ToString();
                        hdnSignOutTimeHrs.Value = dt1.Rows[0]["SatSignOut"].ToString() == "N/A" ? dt1.Rows[0]["SatSchOut"].ToString() : Convert.ToDateTime(dt1.Rows[0]["SatSignOut"]).ToString("hh:mm tt").Trim();
                        hdnSignInTime.Value = dt1.Rows[0]["SatSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["SatSignIn"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignIn.Text = dt1.Rows[0]["SatSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["SatSignIn"]).ToString("hh:mm tt").Trim();
                        //dt1.Rows[0]["SatSignIn"].ToString();
                        lblLoginPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLoginDay.Text = "Saturday";
                    }
                    else
                    {
                        GridViewRow gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                        int gvrowIndex = gvrow.RowIndex;
                        hdnLogUserID.Value = Loguserid.ToString();
                        Label lblSatScIn = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblSatScIn");
                        hdnSchInTime.Value = lblSatScIn.Text;
                        hdnSignInTime.Value = Convert.ToDateTime(ViewState["SatDate"]).ToString("MM/dd/yyyy hh:mm tt");
                        hdnSignOutTimeHrs.Value = "N/A";
                        txtSignIn.Text = "";
                        Label lblName = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblName");
                        lblLoginPopName.Text = lblName.Text.Trim();
                        lblLoginDay.Text = "Saturday";
                        Label lblID = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblID");
                        hdnEmpID.Value = lblID.Text.Trim();
                    }

                    mdlLoginPopUpEdit.Show();
                }
                else if (e.CommandArgument.ToString() == "LoginSunEdit")
                {

                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "SunLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();

                        hdnSchInTime.Value = dt1.Rows[0]["SunSchIn"].ToString();
                        hdnSignOutTimeHrs.Value = dt1.Rows[0]["SunSignOut"].ToString() == "N/A" ? dt1.Rows[0]["SunSchOut"].ToString() : Convert.ToDateTime(dt1.Rows[0]["SunSignOut"]).ToString("hh:mm tt").Trim();


                        hdnSignInTime.Value = dt1.Rows[0]["SunSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["SunSignIn"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignIn.Text = dt1.Rows[0]["SunSignIn"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["SunSignIn"]).ToString("hh:mm tt").Trim();
                        //dt1.Rows[0]["SunSignIn"].ToString();
                        lblLoginPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLoginDay.Text = "Sunday";
                    }
                    else
                    {
                        GridViewRow gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                        int gvrowIndex = gvrow.RowIndex;
                        hdnLogUserID.Value = Loguserid.ToString();
                        Label lblSunScIn = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblSunScIn");
                        hdnSchInTime.Value = lblSunScIn.Text;
                        hdnSignInTime.Value = Convert.ToDateTime(ViewState["SunDate"]).ToString("MM/dd/yyyy hh:mm tt");
                        hdnSignOutTimeHrs.Value = "N/A";
                        txtSignIn.Text = "";
                        Label lblName = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblName");
                        lblLoginPopName.Text = lblName.Text.Trim();
                        lblLoginDay.Text = "Sunday";
                        Label lblID = (Label)grdAttandence.Rows[gvrowIndex].FindControl("lblID");
                        hdnEmpID.Value = lblID.Text.Trim();
                    }
                    mdlLoginPopUpEdit.Show();
                }

                else if (e.CommandName.ToString() == "LogOutMonEdit")
                {

                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogoutLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "MonLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();
                        hdnSchOutTime.Value = dt1.Rows[0]["MonSchOut"].ToString();
                        hdnSignInHrs.Value = dt1.Rows[0]["MonSignIn"].ToString() == "N/A" ? dt1.Rows[0]["MonSchIn"].ToString() : Convert.ToDateTime(dt1.Rows[0]["MonSignIn"]).ToString("hh:mm tt").Trim();

                        hdnSignoutTime.Value = dt1.Rows[0]["MonSignOut"].ToString() == "N/A" ? Convert.ToDateTime(dt1.Rows[0]["MonSignIn"]).ToString("MM/dd/yyyy") : Convert.ToDateTime(dt1.Rows[0]["MonSignOut"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignOut.Text = dt1.Rows[0]["MonSignOut"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["MonSignOut"]).ToString("hh:mm tt").Trim();
                        // dt1.Rows[0]["MonSignOut"].ToString();
                        lblLogOutPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLogoutDay.Text = "Monday";
                        mdlLogoutEditPopUp.Show();
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Sorry you are not able to update signout time unless you login');", true);
                    }


                }
                else if (e.CommandName.ToString() == "LogOutTueEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogoutLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "TueLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();

                        hdnSchOutTime.Value = dt1.Rows[0]["TueSchOut"].ToString();
                        hdnSignInHrs.Value = dt1.Rows[0]["TueSignIn"].ToString() == "N/A" ? dt1.Rows[0]["TueSchIn"].ToString() : Convert.ToDateTime(dt1.Rows[0]["TueSignIn"]).ToString("hh:mm tt").Trim();


                        hdnSignoutTime.Value = dt1.Rows[0]["TueSignOut"].ToString() == "N/A" ? Convert.ToDateTime(dt1.Rows[0]["TueSignIn"]).ToString("MM/dd/yyyy") : Convert.ToDateTime(dt1.Rows[0]["TueSignOut"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignOut.Text = dt1.Rows[0]["TueSignOut"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["TueSignOut"]).ToString("hh:mm tt").Trim();
                        //  dt1.Rows[0]["TueSignOut"].ToString();
                        lblLogOutPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLogoutDay.Text = "Tuesday";
                        mdlLogoutEditPopUp.Show();
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Sorry you are not able to update signout time unless you login');", true);
                    }



                }

                else if (e.CommandName.ToString() == "LogOutWedEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogoutLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "WedLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();

                        hdnSchOutTime.Value = dt1.Rows[0]["WedSchOut"].ToString();
                        hdnSignInHrs.Value = dt1.Rows[0]["WedSignIn"].ToString() == "N/A" ? dt1.Rows[0]["WedSchIn"].ToString() : Convert.ToDateTime(dt1.Rows[0]["WedSignIn"]).ToString("hh:mm tt").Trim();


                        hdnSignoutTime.Value = dt1.Rows[0]["WedSignOut"].ToString() == "N/A" ? Convert.ToDateTime(dt1.Rows[0]["WedSignIn"]).ToString("MM/dd/yyyy") : Convert.ToDateTime(dt1.Rows[0]["WedSignOut"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignOut.Text = dt1.Rows[0]["WedSignOut"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["WedSignOut"]).ToString("hh:mm tt").Trim();
                        //dt1.Rows[0]["WedSignOut"].ToString();
                        lblLogOutPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLogoutDay.Text = "WednesDay";
                        mdlLogoutEditPopUp.Show();
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Sorry you are not able to update signout time unless you login');", true);
                    }

                }

                else if (e.CommandName.ToString() == "LogOutThuEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogoutLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "ThuLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();

                        hdnSchOutTime.Value = dt1.Rows[0]["ThuSchOut"].ToString();
                        hdnSignInHrs.Value = dt1.Rows[0]["ThuSignIn"].ToString() == "N/A" ? dt1.Rows[0]["ThuSchIn"].ToString() : Convert.ToDateTime(dt1.Rows[0]["ThuSignIn"]).ToString("hh:mm tt").Trim();


                        hdnSignoutTime.Value = dt1.Rows[0]["ThuSignOut"].ToString() == "N/A" ? Convert.ToDateTime(dt1.Rows[0]["ThuSignIn"]).ToString("MM/dd/yyyy") : Convert.ToDateTime(dt1.Rows[0]["ThuSignOut"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignOut.Text = dt1.Rows[0]["ThuSignOut"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["ThuSignOut"]).ToString("hh:mm tt").Trim();
                        //dt1.Rows[0]["ThuSignOut"].ToString();
                        lblLogOutPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLogoutDay.Text = "Thursday";
                        mdlLogoutEditPopUp.Show();
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Sorry you are not able to update signout time unless you login');", true);
                    }

                }
                else if (e.CommandName.ToString() == "LogOutFriEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogoutLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "FriLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();

                        hdnSchOutTime.Value = dt1.Rows[0]["FriSchOut"].ToString();
                        hdnSignInHrs.Value = dt1.Rows[0]["FriSignIn"].ToString() == "N/A" ? dt1.Rows[0]["FriSchIn"].ToString() : Convert.ToDateTime(dt1.Rows[0]["FriSignIn"]).ToString("hh:mm tt").Trim();


                        hdnSignoutTime.Value = dt1.Rows[0]["FriSignOut"].ToString() == "N/A" ? Convert.ToDateTime(dt1.Rows[0]["FriSignIn"]).ToString("MM/dd/yyyy") : Convert.ToDateTime(dt1.Rows[0]["FriSignOut"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignOut.Text = dt1.Rows[0]["FriSignOut"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["FriSignOut"]).ToString("hh:mm tt").Trim();
                        //dt1.Rows[0]["FriSignOut"].ToString();
                        lblLogOutPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLogoutDay.Text = "Friday";
                        mdlLogoutEditPopUp.Show();
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Sorry you are not able to update signout time unless you login');", true);
                    }

                }

                else if (e.CommandName.ToString() == "LogOutSatEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogoutLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "SatLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();


                        hdnSchOutTime.Value = dt1.Rows[0]["SatSchOut"].ToString();
                        hdnSignInHrs.Value = dt1.Rows[0]["SatSignIn"].ToString() == "N/A" ? dt1.Rows[0]["SatSchIn"].ToString() : Convert.ToDateTime(dt1.Rows[0]["SatSignIn"]).ToString("hh:mm tt").Trim();


                        hdnSignoutTime.Value = dt1.Rows[0]["SatSignOut"].ToString() == "N/A" ? Convert.ToDateTime(dt1.Rows[0]["SatSignIn"]).ToString("MM/dd/yyyy") : Convert.ToDateTime(dt1.Rows[0]["SatSignOut"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignOut.Text = dt1.Rows[0]["SatSignOut"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["SatSignOut"]).ToString("hh:mm tt").Trim();
                        //dt1.Rows[0]["SatSignOut"].ToString();
                        lblLogOutPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLogoutDay.Text = "Saturday";
                        mdlLogoutEditPopUp.Show();
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Sorry you are not able to update signout time unless you login');", true);
                    }

                }
                else if (e.CommandName.ToString() == "LogOutSunEdit")
                {
                    int Loguserid = e.CommandArgument.ToString() == "" ? 0 : Convert.ToInt32(e.CommandArgument);
                    if (Loguserid != 0)
                    {
                        hdnLogoutLogUserID.Value = Loguserid.ToString();
                        dv.RowFilter = "SunLogUserID=" + Loguserid;
                        DataTable dt1 = dv.ToTable();

                        hdnSchOutTime.Value = dt1.Rows[0]["SunSchOut"].ToString();
                        hdnSignInHrs.Value = dt1.Rows[0]["SunSignIn"].ToString() == "N/A" ? dt1.Rows[0]["SunSchIn"].ToString() : Convert.ToDateTime(dt1.Rows[0]["SunSignIn"]).ToString("hh:mm tt").Trim();



                        hdnSignoutTime.Value = dt1.Rows[0]["SunSignOut"].ToString() == "N/A" ? Convert.ToDateTime(dt1.Rows[0]["SunSignIn"]).ToString("MM/dd/yyyy") : Convert.ToDateTime(dt1.Rows[0]["SunSignOut"]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                        txtSignOut.Text = dt1.Rows[0]["SunSignOut"].ToString() == "N/A" ? "" : Convert.ToDateTime(dt1.Rows[0]["SunSignOut"]).ToString("hh:mm tt").Trim();
                        //dt1.Rows[0]["SunSignOut"].ToString();
                        lblLogOutPopName.Text = dt1.Rows[0]["Empname"].ToString();
                        lblLogoutDay.Text = "Sunday";
                        mdlLogoutEditPopUp.Show();
                    }

                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Sorry you are not able to update signout time unless you login');", true);
                    }
                }

            }
            catch (Exception ex)
            {
            }

        }


        protected void btnCurrent_Click(object sender, EventArgs e)
        {
            try
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                string Ismanage = Session["IsManage"].ToString();
                string IsAdmin = Session["IsAdmin"].ToString();
                if (ddlReportType.SelectedValue == "0")
                {
                    DateTime StartDate = Convert.ToDateTime(ViewState["CurrentStart"].ToString());
                    DateTime EndDate = Convert.ToDateTime(ViewState["CurrentEnd"].ToString());

                    //DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    Session["TodayDate"] = StartDate;

                    //Session["TodayDate1"] = Convert.ToDateTime(Session["TodayDate"]);
                    if (GeneralFunction.GetFirstDayOfWeekDate(StartDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }

                    btnFreeze.Visible = true;
                    lblFreeze.Visible = true;
                    //   DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartOfMonth = GeneralFunction.GetFirstDayOfWeekDate(StartDate);
                    DateTime FreezeDate = StartOfMonth.AddDays(-1);
                    Attendance.BAL.Report obj = new Report();
                    //  int CNT = obj.GetFreezedDate(FreezeDate);
                    DateTime CNT = obj.GetFreezedDate(FreezeDate, ddlLocation.SelectedItem.Text.ToString().Trim());
                    lblFreezedate.Text = FreezeDate.ToString("MM/dd/yyyy");
                    hdnFreeze.Value = FreezeDate.ToString("MM/dd/yyyy");
                    if (CNT.ToString("MM/dd/yyyy") != "01/01/1900")
                    {
                        lblFreeze.Text = "Attendance freezed until " + CNT.ToString("MM/dd/yyyy");
                        ViewState["FreezeDate"] = CNT.ToString("MM/dd/yyyy");
                        btnFreeze.CssClass = "btn btn-warning btn-small disabled";
                        btnFreeze.Enabled = false;
                    }
                    else
                    {
                        lblFreeze.Visible = false;
                        btnFreeze.CssClass = "btn btn-warning btn-small enabled";
                        btnFreeze.Enabled = true;
                    }



                    DataTable ds = new DataTable();

                    ds = GetReportAdmin(StartDate, EndDate, Convert.ToInt32(ddlLocation.SelectedValue));
                    Session["AtnAdminDetails"] = ds;
                    grdAttandence.DataSource = ds;
                    grdAttandence.DataBind();
                    grdMonthlyAttendance.DataSource = null;
                    grdMonthlyAttendance.DataBind();
                    grdWeeklyAttendance.DataSource = null;
                    grdWeeklyAttendance.DataBind();


                }

                else if (ddlReportType.SelectedItem.Value == "1")
                {

                    ViewState["TodayDate1"] = ViewState["CurrentWeek"];
                    DateTime startWeek = Convert.ToDateTime(ViewState["CurrentWeek"]);
                    DateTime EndWeek = Convert.ToDateTime(ViewState["CrntWkEnd"]);

                    hdnWeeklyStartDt.Value = startWeek.ToString();
                    if (GeneralFunction.GetFirstDayOfWeekDate(EndWeek).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now.AddDays(-7)).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;

                    }

                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;

                    lblReport.Text = "Report type";
                    ddlReportType.Visible = true;
                    lblReport.Visible = true;

                    DataTable dt = GetWeeklyReport(startWeek, EndWeek);
                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = dt;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = null;
                        grdMonthlyAttendance.DataBind();
                    }
                }

                else if (ddlReportType.SelectedItem.Value == "2")
                {

                    DateTime StartDate = Convert.ToDateTime(ViewState["CurrentMonth"]);
                    ViewState["StartMonth"] = StartDate;
                    hdnMonthlyStartDt.Value = StartDate.ToString();
                    DateTime endDate = Convert.ToDateTime(ViewState["CrntMonthEnd"]);
                    DataTable dt = GetMonthlyreport(StartDate, endDate);
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;

                    if (endDate.ToString("MM/dd/yyyy") == (DateTime.Now.AddDays(1 - DateTime.Now.Day)).AddDays(-1).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }


                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = null;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = dt;
                        grdMonthlyAttendance.DataBind();
                    }





                }

            }
            catch (Exception ex)
            {

            }
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["Location"] = ddlLocation.SelectedItem.Text.Trim();
            try
            {
                if (ddlReportType.SelectedItem.Value == "0")
                {
                    lblReport.Text = "Report type";
                    ddlReportType.Visible = true;
                    lblReport.Visible = true;
                    lblGrdLocaton.Visible = true;
                    lblGrdLocaton.Text = "Location";
                    ddlLocation.Visible = true;
                    int LocationId = Convert.ToInt32(ddlLocation.SelectedItem.Value.ToString());
                    DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartDate = GeneralFunction.GetFirstDayOfWeekDate(TodayDate);
                    DateTime EndDate = GeneralFunction.GetLastDayOfWeekDate(TodayDate);


                    btnFreeze.Visible = true;
                    lblFreeze.Visible = true;
                    //   DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartOfMonth = StartDate.AddDays(-1);
                    DateTime FreezeDate = StartOfMonth;
                    Attendance.BAL.Report obj = new Report();
                    //  int CNT = obj.GetFreezedDate(FreezeDate);
                    DateTime CNT = obj.GetFreezedDate(FreezeDate, ddlLocation.SelectedItem.Text.ToString().Trim());
                    lblFreezedate.Text = FreezeDate.ToString("MM/dd/yyyy");
                    hdnFreeze.Value = FreezeDate.ToString("MM/dd/yyyy");
                    if (CNT.ToString("MM/dd/yyyy") != "01/01/1900")
                    {
                        lblFreeze.Text = "Attendance freezed until " + CNT.ToString("MM/dd/yyyy");
                        ViewState["FreezeDate"] = CNT.ToString("MM/dd/yyyy");
                        btnFreeze.CssClass = "btn btn-warning btn-small disabled";
                        btnFreeze.Enabled = false;
                    }
                    else
                    {
                        lblFreeze.Visible = false;
                        btnFreeze.CssClass = "btn btn-warning btn-small enabled";
                        btnFreeze.Enabled = true;
                    }

                    if (GeneralFunction.GetFirstDayOfWeekDate(TodayDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }

                    DataTable ds = GetReportAdmin(StartDate, EndDate, LocationId);
                    Session["AtnAdminDetails"] = ds;
                    grdAttandence.DataSource = ds;
                    grdAttandence.DataBind();
                    grdWeeklyAttendance.DataSource = null;
                    grdWeeklyAttendance.DataBind();
                    grdMonthlyAttendance.DataSource = null;
                    grdMonthlyAttendance.DataBind();
                }
                else if (ddlReportType.SelectedItem.Value == "1")
                {
                    lblReport.Text = "Report type";
                    ddlReportType.Visible = true;
                    lblReport.Visible = true;
                    lblGrdLocaton.Visible = true;
                    lblGrdLocaton.Text = "Location";
                    ddlLocation.Visible = true;
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;

                    int LocationId = Convert.ToInt32(ddlLocation.SelectedItem.Value.ToString());
                    DateTime todayDate = Convert.ToDateTime(Session["TodayDate1"]);

                    // DateTime startOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
                    DateTime startDate = GeneralFunction.GetFirstDayOfWeekDate(todayDate);
                    DateTime StartDate = startDate.AddDays(-28);
                    ViewState["TodayDate1"] = StartDate;
                    ViewState["CurrentWeek"] = StartDate;
                    hdnWeeklyStartDt.Value = StartDate.ToString();
                    DateTime endDate = startDate.AddDays(-1);
                    ViewState["CrntWkEnd"] = endDate;


                    if (GeneralFunction.GetFirstDayOfWeekDate(endDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now.AddDays(-7)).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;

                    }


                    DataTable dt = GetWeeklyReport(StartDate, endDate);

                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = dt;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = null;
                        grdMonthlyAttendance.DataBind();
                    }
                }



                else if (ddlReportType.SelectedItem.Value == "2")
                {
                    lblReport.Text = "Report type";
                    ddlReportType.Visible = true;
                    lblReport.Visible = true;
                    lblGrdLocaton.Visible = true;
                    lblGrdLocaton.Text = "Location";
                    ddlLocation.Visible = true;
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;

                    int LocationId = Convert.ToInt32(ddlLocation.SelectedItem.Value.ToString());


                    DateTime todayDate = Convert.ToDateTime(Session["TodayBannerDate"]);

                    // DateTime startOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
                    DateTime startDate = GeneralFunction.GetFirstDayOfWeekDate(todayDate);
                    DateTime StartDate = startDate.AddDays(1 - startDate.Day);
                    ViewState["StartMonth"] = StartDate.AddMonths(-6);
                    ViewState["CurrentMonth"] = StartDate.AddMonths(-6);
                    hdnMonthlyStartDt.Value = StartDate.AddMonths(-6).ToString();
                    DateTime endDate = StartDate.AddSeconds(-1);
                    ViewState["CrntMonthEnd"] = endDate;
                    DataTable dt = GetMonthlyreport(StartDate.AddMonths(-6), endDate);
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;


                    if (endDate.ToString("MM/dd/yyyy") == (DateTime.Now.AddDays(1 - DateTime.Now.Day)).AddDays(-1).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;

                    }





                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = null;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = dt;
                        grdMonthlyAttendance.DataBind();
                    }

                }


            }
            catch (Exception ex)
            {

            }
        }


        protected void btnCancelIn_Click(object sender, EventArgs e)
        {
            mdlLoginPopUpEdit.Hide();
        }

        protected void btnCancleOut_Click(object sender, EventArgs e)
        {
            mdlLogoutEditPopUp.Hide();
        }
        protected void btnUpdateOut_Click(object sender, EventArgs e)
        {
              try
                {
                    int userid = Convert.ToInt32(Session["UserID"]);
                    int loguserId = Convert.ToInt32(hdnLogoutLogUserID.Value);

                    string empname = Session["EmpName"].ToString().Trim();

                    string timezone = "";
                    if (Convert.ToInt32(Session["TimeZoneID"]) == 2)
                    {
                        timezone = "Eastern Standard Time";
                    }
                    else
                    {
                        timezone = "India Standard Time";

                    }
                    DateTime ISTTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezone));

                    var CurentDatetime = ISTTime;


                    string notes = "Sign out time changed by " + empname + " at -" + ISTTime.ToString("MM/dd/yyyy hh:mm:ss") + "\n";

                    string signOutTime = Convert.ToDateTime(hdnSignoutTime.Value).ToString("MM/dd/yyyy") + " " + txtSignOut.Text;
                    Business obj = new Business();
                    bool bnew = obj.UpdateSignOutTime(userid, loguserId, signOutTime, notes);
                    Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
                }
                catch (Exception ex)
                {
                }
          
        }

        protected void btnUpdateIn_Click(object sender, EventArgs e)
        {
            try
            {

                string EmpID = hdnEmpID.Value.ToString().Trim();
                int loguserId = Convert.ToInt32(hdnLogUserID.Value);
                string empname = Session["EmpName"].ToString().Trim();
                string timezone = "";
                if (Convert.ToInt32(Session["TimeZoneID"]) == 2)
                {
                    timezone = "Eastern Standard Time";
                }
                else
                {
                    timezone = "India Standard Time";

                }
                DateTime ISTTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezone));

                var CurentDatetime = ISTTime;

                string notes = "";
                string signinTime = "";


                if (loguserId != 0)
                {
                    notes = "Sign in time changed by " + empname + " at -" + ISTTime.ToString("MM/dd/yyyy hh:mm:ss") + "\n";
                    signinTime = Convert.ToDateTime(hdnSignInTime.Value).ToString("MM/dd/yyyy") + " " + txtSignIn.Text;
                    
                }
                else
                {
                    signinTime = Convert.ToDateTime(hdnSignInTime.Value).ToString("MM/dd/yyyy") + " " + txtSignIn.Text;
                    notes = "Sign in time added by " + empname + " at -" + ISTTime.ToString("MM/dd/yyyy hh:mm:ss") + "\n to " + signinTime;
                }
                Business obj = new Business();

                bool bnew = obj.UpdateSignTime(EmpID, loguserId, signinTime, notes);
                Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);


            }
            catch (Exception ex)
            {
            }
        }
        private DataTable GetWeeklyReport(DateTime startdate, DateTime enddate)
        {
            DataTable dtAttandence = new DataTable();
            try
            {
                lblWeekReportheading.Text = "Weekly summary attendance report";
                DateTime endOfMonth = GeneralFunction.GetFirstDayOfWeekDate(enddate);
                lblWeekReport.Text = "( Wk of " + startdate.ToString("MM/dd/yyyy") + " - Wk of " + endOfMonth.ToString("MM/dd/yyyy") + " )";
                dtAttandence.Columns.Add("empid", typeof(string));
                dtAttandence.Columns.Add("Empname", typeof(string));
                dtAttandence.Columns.Add("StatingDate", typeof(string));
                dtAttandence.Columns.Add("TermDate", typeof(string));
                dtAttandence.Columns.Add("TermReason", typeof(string));
                dtAttandence.Columns.Add("Days", typeof(int));
                dtAttandence.Rows.Add();
                Attendance.BAL.Report obj = new Report();
                enddate = startdate.AddDays(7);

                DataSet ds = obj.GetActiveUsersAdmin(startdate, enddate.AddDays(7), ViewState["Location"].ToString().Trim());

                for (int j = 0; j < 4; j++)
                {

                    DataSet dsResult = obj.GetWeeklyReportAdmin(startdate, enddate, ViewState["Location"].ToString().Trim());

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dtAttandence.Columns.Add("Week" + (j + 1), typeof(string));

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (j == 0)
                            {
                                dtAttandence.Rows.Add();
                            }
                            dtAttandence.Rows[i]["empid"] = ds.Tables[0].Rows[i]["empid"].ToString();
                            dtAttandence.Rows[i]["empname"] = ds.Tables[0].Rows[i]["firstName"].ToString() + " " + ds.Tables[0].Rows[i]["lastname"].ToString();
                            dtAttandence.Rows[i]["StatingDate"] = ds.Tables[0].Rows[i]["Startdate"].ToString();
                            dtAttandence.Rows[i]["TermDate"] = ds.Tables[0].Rows[i]["Termdate"].ToString();
                            if (dsResult.Tables.Count > 0)
                            {
                                if (dsResult.Tables[0].Rows.Count > 1)
                                {
                                    DataTable dt = dsResult.Tables[0];
                                    DataView dv = dt.DefaultView;
                                    DataTable dtname = new DataTable();
                                    dv.RowFilter = "empid='" + ds.Tables[0].Rows[i]["empid"].ToString() + "'";
                                    dtname = dv.ToTable();
                                    if (dtname.Rows.Count > 0)
                                    {
                                        // dtAttandence.Rows[i]["Week" + (j + 1)] = dtname.Rows[0]["weeklyhrs"].ToString();
                                        dtAttandence.Rows[i]["Week" + (j + 1)] = dtname.Rows[0]["weeklyhrs"].ToString() == "NULL" ? "" : dtname.Rows[0]["weeklyhrs"].ToString() == "" ? "" : GeneralFunction.CalNumericToint(Convert.ToDouble(dtname.Rows[0]["weeklyhrs"].ToString())).ToString();
                                        dtAttandence.Rows[i]["Days"] = (dtAttandence.Rows[i]["Days"].ToString() == "NULL" ? 0 : dtAttandence.Rows[i]["Days"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Days"].ToString())) + (dtname.Rows[0]["days"].ToString() == "NULL" ? 0 : Convert.ToInt32(dtname.Rows[0]["days"].ToString()));
                                    }
                                }
                            }


                        }
                        startdate = enddate.AddDays(1);
                        enddate = startdate.AddDays(7);
                    }

                }

                int TotalHrs1 = 0;
                int TotalHrs2 = 0;
                int TotalHrs3 = 0;
                int TotalHrs4 = 0;
                int TotalDays = 0;
                dtAttandence.Columns.Add("Totalhrs");
                for (int i = 0; i < dtAttandence.Rows.Count - 1; i++)
                {
                    TotalHrs1 = TotalHrs1 + ((dtAttandence.Rows[i]["Week1"].ToString() == "Null") ? 0 : (dtAttandence.Rows[i]["Week1"].ToString() == "") ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week1"]));
                    TotalHrs2 = TotalHrs2 + (dtAttandence.Rows[i]["Week2"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Week2"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week2"]));
                    TotalHrs3 = TotalHrs3 + ((dtAttandence.Rows[i]["Week3"].ToString() == "Null" ? 0 : (dtAttandence.Rows[i]["Week3"].ToString() == "") ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week3"])));
                    TotalHrs4 = TotalHrs4 + (dtAttandence.Rows[i]["Week4"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Week4"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week4"]));

                    int WeekHrs = ((dtAttandence.Rows[i]["Week1"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Week1"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week1"])) +
                                                     (dtAttandence.Rows[i]["Week2"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Week2"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week2"])) +
                                                     (dtAttandence.Rows[i]["Week3"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Week3"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week3"])) +
                                                     (dtAttandence.Rows[i]["Week4"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Week4"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Week4"])));
                    dtAttandence.Rows[i]["Totalhrs"] = WeekHrs == 0 ? "" : GeneralFunction.ConverttoTime(WeekHrs).ToString();
                    TotalDays += Convert.ToInt32(dtAttandence.Rows[i]["Days"]);
                }

                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Week1"] = TotalHrs1 == 0 ? "" : TotalHrs1.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Week2"] = TotalHrs2 == 0 ? "" : TotalHrs2.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Week3"] = TotalHrs3 == 0 ? "" : TotalHrs3.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Week4"] = TotalHrs4 == 0 ? "" : TotalHrs4.ToString();
                int SUM = TotalHrs1 + TotalHrs2 + TotalHrs3 + TotalHrs4;
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["empid"] = "<b>Totals</b>";
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Days"] = TotalDays;
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Totalhrs"] = SUM == 0 ? "" : GeneralFunction.ConverttoTime(SUM).ToString();
            }
            catch (Exception ex)
            {
            }
            return dtAttandence;
        }


        private DataTable GetMonthlyreport(DateTime startdate, DateTime endMonth)
        {
            DataTable dtAttandence = new DataTable();
            try
            {
                lblWeekReportheading.Text = "Monthly summary attendance report";

                lblWeekReport.Text = "( " + startdate.ToString("MM/dd/yyyy") + " - " + endMonth.ToString("MM/dd/yyyy") + " )";
                dtAttandence.Columns.Add("empid", typeof(string));
                dtAttandence.Columns.Add("Empname", typeof(string));
                dtAttandence.Columns.Add("StatingDate", typeof(string));
                dtAttandence.Columns.Add("TermDate", typeof(string));
                dtAttandence.Columns.Add("TermReason", typeof(string));
                dtAttandence.Columns.Add("Days", typeof(int));
                dtAttandence.Rows.Add();
                Attendance.BAL.Report obj = new Report();


                DateTime enddate = startdate.AddMonths(1).AddSeconds(-1);

                DataSet ds = obj.GetActiveUsersAdmin(startdate, endMonth, ViewState["Location"].ToString().Trim());

                for (int j = 0; j < 6; j++)
                {

                    DataSet dsResult = obj.GetWeeklyReportAdmin(startdate, enddate, ViewState["Location"].ToString().Trim());

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dtAttandence.Columns.Add("Month" + (j + 1), typeof(string));

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (j == 0)
                            {
                                dtAttandence.Rows.Add();
                            }
                            dtAttandence.Rows[i]["empid"] = ds.Tables[0].Rows[i]["empid"].ToString();
                            dtAttandence.Rows[i]["empname"] = ds.Tables[0].Rows[i]["firstName"].ToString() + " " + ds.Tables[0].Rows[i]["lastname"].ToString();
                            dtAttandence.Rows[i]["StatingDate"] = ds.Tables[0].Rows[i]["Startdate"].ToString();
                            dtAttandence.Rows[i]["TermDate"] = ds.Tables[0].Rows[i]["Termdate"].ToString();
                            if (dsResult.Tables.Count > 0)
                            {
                                if (dsResult.Tables[0].Rows.Count > 1)
                                {
                                    DataTable dt = dsResult.Tables[0];
                                    DataView dv = dt.DefaultView;
                                    DataTable dtname = new DataTable();
                                    dv.RowFilter = "empid='" + ds.Tables[0].Rows[i]["empid"].ToString() + "'";
                                    dtname = dv.ToTable();
                                    if (dtname.Rows.Count > 0)
                                    {
                                        // dtAttandence.Rows[i]["Month" + (j + 1)] = dtname.Rows[0]["weeklyhrs"].ToString();
                                        dtAttandence.Rows[i]["Month" + (j + 1)] = dtname.Rows[0]["weeklyhrs"].ToString() == "NULL" ? "" : dtname.Rows[0]["weeklyhrs"].ToString() == "" ? "" : GeneralFunction.CalNumericToint(Convert.ToDouble(dtname.Rows[0]["weeklyhrs"].ToString())).ToString();
                                        dtAttandence.Rows[i]["Days"] = (dtAttandence.Rows[i]["Days"].ToString() == "NULL" ? 0 : dtAttandence.Rows[i]["Days"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Days"].ToString())) + (dtname.Rows[0]["days"].ToString() == "NULL" ? 0 : Convert.ToInt32(dtname.Rows[0]["days"].ToString()));
                                    }
                                }
                            }


                        }
                        startdate = enddate.AddSeconds(1);
                        enddate = startdate.AddMonths(1).AddSeconds(-1);
                    }

                }

                int TotalHrs1 = 0;
                int TotalHrs2 = 0;
                int TotalHrs3 = 0;
                int TotalHrs4 = 0;
                int TotalHrs5 = 0;
                int TotalHrs6 = 0;
                int TotalDays = 0;
                dtAttandence.Columns.Add("Totalhrs");
                for (int i = 0; i < dtAttandence.Rows.Count - 1; i++)
                {
                    TotalHrs1 = TotalHrs1 + ((dtAttandence.Rows[i]["Month1"].ToString() == "Null") ? 0 : (dtAttandence.Rows[i]["Month1"].ToString() == "") ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month1"]));
                    TotalHrs2 = TotalHrs2 + (dtAttandence.Rows[i]["Month2"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month2"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month2"]));
                    TotalHrs3 = TotalHrs3 + ((dtAttandence.Rows[i]["Month3"].ToString() == "Null" ? 0 : (dtAttandence.Rows[i]["Month3"].ToString() == "") ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month3"])));
                    TotalHrs4 = TotalHrs4 + (dtAttandence.Rows[i]["Month4"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month4"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month4"]));
                    TotalHrs5 = TotalHrs5 + (dtAttandence.Rows[i]["Month5"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month5"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month5"]));
                    TotalHrs6 = TotalHrs6 + (dtAttandence.Rows[i]["Month6"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month6"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month6"]));

                    int WeekHrs = ((dtAttandence.Rows[i]["Month1"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month1"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month1"]))) +
                                                     (dtAttandence.Rows[i]["Month2"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month2"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month2"])) +
                                                     (dtAttandence.Rows[i]["Month3"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month3"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month3"])) +
                                                     (dtAttandence.Rows[i]["Month4"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month4"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month4"])) +
                                                     (dtAttandence.Rows[i]["Month5"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month5"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month5"])) +
                                                     (dtAttandence.Rows[i]["Month6"].ToString() == "Null" ? 0 : dtAttandence.Rows[i]["Month6"].ToString() == "" ? 0 : Convert.ToInt32(dtAttandence.Rows[i]["Month6"]));


                    dtAttandence.Rows[i]["Totalhrs"] = WeekHrs == 0 ? "" : GeneralFunction.ConverttoTime(WeekHrs);
                    TotalDays += Convert.ToInt32(dtAttandence.Rows[i]["Days"]);
                }

                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Month1"] = TotalHrs1 == 0 ? "" : TotalHrs1.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Month2"] = TotalHrs2 == 0 ? "" : TotalHrs2.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Month3"] = TotalHrs3 == 0 ? "" : TotalHrs3.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Month4"] = TotalHrs4 == 0 ? "" : TotalHrs4.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Month5"] = TotalHrs5 == 0 ? "" : TotalHrs5.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Month6"] = TotalHrs6 == 0 ? "" : TotalHrs6.ToString();
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["empid"] = "<b>Totals</b>";
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Days"] = TotalDays;
                int sum = TotalHrs1 + TotalHrs2 + TotalHrs3 + TotalHrs4 + TotalHrs5 + TotalHrs6;
                dtAttandence.Rows[dtAttandence.Rows.Count - 1]["Totalhrs"] = sum == 0 ? "" : GeneralFunction.ConverttoTime(sum);
            }
            catch (Exception ex)
            {
            }
            return dtAttandence;
        }

        protected void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                //  string IsAdmin = Session["IsAdmin"].ToString();


                if (ddlReportType.SelectedItem.Value == "2")
                {
                    DateTime todayDate = Convert.ToDateTime(Session["TodayBannerDate"]);

                    // DateTime startOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
                    DateTime startDate = GeneralFunction.GetFirstDayOfWeekDate(todayDate);
                    DateTime StartDate = startDate.AddDays(1 - startDate.Day);
                    ViewState["StartMonth"] = StartDate.AddMonths(-6);
                    ViewState["CurrentMonth"] = StartDate.AddMonths(-6);
                    hdnMonthlyStartDt.Value = StartDate.AddMonths(-6).ToString();
                    DateTime endDate = StartDate.AddSeconds(-1);
                    ViewState["CrntMonthEnd"] = endDate;
                    DataTable dt = GetMonthlyreport(StartDate.AddMonths(-6), endDate);
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;

                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = null;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = dt;
                        grdMonthlyAttendance.DataBind();
                    }

                }

                if (ddlReportType.SelectedItem.Value == "1")
                {
                    DateTime todayDate = Convert.ToDateTime(Session["TodayDate1"]);

                    // DateTime startOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
                    DateTime startDate = GeneralFunction.GetFirstDayOfWeekDate(todayDate);
                    DateTime StartDate = startDate.AddDays(-28);
                    ViewState["TodayDate1"] = StartDate;
                    ViewState["CurrentWeek"] = StartDate;
                    hdnWeeklyStartDt.Value = StartDate.ToString();
                    DateTime endDate = startDate.AddDays(-1);
                    ViewState["CrntWkEnd"] = endDate;
                    DataTable dt = GetWeeklyReport(StartDate, endDate);
                    btnFreeze.Visible = false;
                    lblFreeze.Visible = false;
                    if (GeneralFunction.GetFirstDayOfWeekDate(endDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now.AddDays(-7)).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;

                    }



                    if (dt.Rows.Count > 0)
                    {
                        grdAttandence.DataSource = null;
                        grdAttandence.DataBind();
                        grdWeeklyAttendance.DataSource = dt;
                        grdWeeklyAttendance.DataBind();
                        grdMonthlyAttendance.DataSource = null;
                        grdMonthlyAttendance.DataBind();
                    }

                }
                if (ddlReportType.SelectedItem.Value == "0")
                {
                    DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartDate = GeneralFunction.GetFirstDayOfWeekDate(TodayDate);
                    DateTime EndDate = GeneralFunction.GetLastDayOfWeekDate(TodayDate);

                    if (GeneralFunction.GetFirstDayOfWeekDate(TodayDate).ToString("MM/dd/yyyy") == GeneralFunction.GetFirstDayOfWeekDate(DateTime.Now).ToString("MM/dd/yyyy"))
                    {
                        btnNext.CssClass = "btn btn-danger btn-small disabled";
                        btnNext.Enabled = false;
                    }
                    else
                    {
                        btnNext.CssClass = "btn btn-danger btn-small enabled";
                        btnNext.Enabled = true;
                    }

                    btnFreeze.Visible = true;
                    lblFreeze.Visible = true;
                    //   DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                    DateTime StartOfMonth = StartDate.AddDays(-1);
                    DateTime FreezeDate = StartOfMonth;
                    Attendance.BAL.Report obj = new Report();
                    // int CNT = obj.GetFreezedDate(FreezeDate);
                    DateTime CNT = obj.GetFreezedDate(FreezeDate, ddlLocation.SelectedItem.Text.ToString().Trim());
                    lblFreezedate.Text = FreezeDate.ToString("MM/dd/yyyy");
                    hdnFreeze.Value = FreezeDate.ToString("MM/dd/yyyy");
                    if (CNT.ToString("MM/dd/yyyy") != "01/01/1900")
                    {
                        lblFreeze.Text = "Attendance freezed until " + CNT.ToString("MM/dd/yyyy");
                        ViewState["FreezeDate"] = CNT.ToString("MM/dd/yyyy");
                        btnFreeze.CssClass = "btn btn-warning btn-small disabled";
                        btnFreeze.Enabled = false;
                    }
                    else
                    {
                        lblFreeze.Visible = false;
                        btnFreeze.CssClass = "btn btn-warning btn-small enabled";
                        btnFreeze.Enabled = true;
                    }



                    DataTable ds = GetReportAdmin(StartDate, EndDate, Convert.ToInt32(ddlLocation.SelectedValue));
                    Session["AtnAdminDetails"] = ds;
                    grdAttandence.DataSource = ds;
                    grdAttandence.DataBind();
                    grdWeeklyAttendance.DataSource = null;
                    grdWeeklyAttendance.DataBind();
                    grdMonthlyAttendance.DataSource = null;
                    grdMonthlyAttendance.DataBind();
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void grdWeeklyAttendance_RowDataBound1(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {

                    Label lblWeek1Head = (Label)e.Row.FindControl("lblWeek1Head");
                    lblWeek1Head.Text = "Wk of " + Convert.ToDateTime(hdnWeeklyStartDt.Value).ToString("MM/dd/yyyy");
                    Label lblWeek2Head = (Label)e.Row.FindControl("lblWeek2Head");
                    lblWeek2Head.Text = "Wk of " + Convert.ToDateTime(hdnWeeklyStartDt.Value).AddDays(7).ToString("MM/dd/yyyy");
                    Label lblWeek3Head = (Label)e.Row.FindControl("lblWeek3Head");
                    lblWeek3Head.Text = "Wk of " + Convert.ToDateTime(hdnWeeklyStartDt.Value).AddDays(14).ToString("MM/dd/yyyy");
                    Label lblWeek4Head = (Label)e.Row.FindControl("lblWeek4Head");
                    lblWeek4Head.Text = "Wk of " + Convert.ToDateTime(hdnWeeklyStartDt.Value).AddDays(21).ToString("MM/dd/yyyy");
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    Label lblStartDt = (Label)e.Row.FindControl("lblStartDt");
                    lblStartDt.Text = lblStartDt.Text == "" ? "" : Convert.ToDateTime(lblStartDt.Text).ToString("MM/dd/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(lblStartDt.Text).ToString("MM/dd/yyyy");
                    Label lblTermDt = (Label)e.Row.FindControl("lblTermDt");
                    lblTermDt.Text = lblTermDt.Text == "" ? "" : Convert.ToDateTime(lblTermDt.Text).ToString("MM/dd/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(lblTermDt.Text).ToString("MM/dd/yyyy");
                    Label lblTotal = (Label)e.Row.FindControl("lblTotal");
                    lblTotal.Text = lblTotal.Text == "0" ? "" : lblTotal.Text;

                    Label lblWeek1 = (Label)e.Row.FindControl("lblWeek1");
                    lblWeek1.Text = lblWeek1.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblWeek1.Text));

                    Label lblWeek2 = (Label)e.Row.FindControl("lblWeek2");
                    lblWeek2.Text = lblWeek2.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblWeek2.Text));

                    Label lblWeek3 = (Label)e.Row.FindControl("lblWeek3");
                    lblWeek3.Text = lblWeek3.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblWeek3.Text));

                    Label lblWeek4 = (Label)e.Row.FindControl("lblWeek4");
                    lblWeek4.Text = lblWeek4.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblWeek4.Text));

                }
            }
            catch (Exception ex)
            {
            }
        }
        protected void btnFreeze_Click(object sender, EventArgs e)
        {
            try
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                string Ismanage = Session["IsManage"].ToString();
                string location = ViewState["Location"].ToString();
                Attendance.BAL.Report obj = new Report();
                // DateTime TodayDate = Convert.ToDateTime(Session["TodayDate"]);
                //  DateTime StartOfMonth = GeneralFunction.GetFirstDayOfWeekDate(TodayDate);
                DateTime FreezeDate = Convert.ToDateTime(hdnFreeze.Value);
                bool bnew = obj.UpdateFreeze(userid, location, FreezeDate);

                if (bnew)
                {
                    // mdlFeeze.Hide();
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "alert('Attendance is freezed upto " + FreezeDate.ToString("MM/dd/yyyy") + " ');", true);
                }

            }
            catch (Exception ex)
            {

            }
        }

        protected void btnFreezeCancle_Click(object sender, EventArgs e)
        {
            try
            {
                mdlFeeze.Hide();
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnFreezeOk_Click(object sender, EventArgs e)
        {


        }


        protected void grdMonthlyAttendance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {

                Label lblMonth1Head = (Label)e.Row.FindControl("lblMonth1Head");
                lblMonth1Head.Text = Convert.ToDateTime(hdnMonthlyStartDt.Value).ToString("MMM") + "-" + Convert.ToDateTime(hdnMonthlyStartDt.Value).ToString("yyyy");
                Label lblMonth2Head = (Label)e.Row.FindControl("lblMonth2Head");
                lblMonth2Head.Text = Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(1).ToString("MMM") + "-" + Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(1).ToString("yyyy");
                Label lblMonth3Head = (Label)e.Row.FindControl("lblMonth3Head");
                lblMonth3Head.Text = Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(2).ToString("MMM") + "-" + Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(2).ToString("yyyy");
                Label lblMonth4Head = (Label)e.Row.FindControl("lblMonth4Head");
                lblMonth4Head.Text = Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(3).ToString("MMM") + "-" + Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(3).ToString("yyyy");
                Label lblMonth5Head = (Label)e.Row.FindControl("lblMonth5Head");
                lblMonth5Head.Text = Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(4).ToString("MMM") + "-" + Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(4).ToString("yyyy");
                Label lblMonth6Head = (Label)e.Row.FindControl("lblMonth6Head");
                lblMonth6Head.Text = Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(5).ToString("MMM") + "-" + Convert.ToDateTime(hdnMonthlyStartDt.Value).AddMonths(5).ToString("yyyy");



            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Label lblStartDt = (Label)e.Row.FindControl("lblStartDt");
                lblStartDt.Text = lblStartDt.Text == "" ? "" : Convert.ToDateTime(lblStartDt.Text).ToString("MM/dd/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(lblStartDt.Text).ToString("MM/dd/yyyy");
                Label lblTermDt = (Label)e.Row.FindControl("lblTermDt");
                lblTermDt.Text = lblTermDt.Text == "" ? "" : Convert.ToDateTime(lblTermDt.Text).ToString("MM/dd/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(lblTermDt.Text).ToString("MM/dd/yyyy");
                Label lblTotal = (Label)e.Row.FindControl("lblTotal");
                lblTotal.Text = lblTotal.Text == "0" ? "" : lblTotal.Text;

                Label lblMonth1 = (Label)e.Row.FindControl("lblMonth1");
                lblMonth1.Text = lblMonth1.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblMonth1.Text));

                Label lblMonth2 = (Label)e.Row.FindControl("lblMonth2");
                lblMonth2.Text = lblMonth2.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblMonth2.Text));

                Label lblMonth3 = (Label)e.Row.FindControl("lblMonth3");
                lblMonth2.Text = lblMonth2.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblMonth2.Text));

                Label lblMonth4 = (Label)e.Row.FindControl("lblMonth4");
                lblMonth4.Text = lblMonth4.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblMonth4.Text));

                Label lblMonth5 = (Label)e.Row.FindControl("lblMonth5");
                lblMonth5.Text = lblMonth5.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblMonth5.Text));

                Label lblMonth6 = (Label)e.Row.FindControl("lblMonth6");
                lblMonth6.Text = lblMonth6.Text == "" ? "" : GeneralFunction.ConverttoTime(Convert.ToInt32(lblMonth6.Text));


            }

        }
    }
}
