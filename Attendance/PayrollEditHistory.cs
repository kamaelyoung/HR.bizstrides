﻿using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Attendance
{
    public class PayrollEditHistory
    {
        public string ChanngeHistory(string fieldname, string oldvalue, string newValue, string Currentdate, string Location)
        {
            string Proper = "";
            try
            {

                if (fieldname == "SSN" || fieldname == "Deductions" || fieldname == "County" || fieldname == "Address1" || fieldname == "Address2" || fieldname == "StateID" || fieldname == "Zip" || fieldname == "firstname" ||
                    fieldname == "Lastname" || fieldname == "Date of birth" || fieldname == "EmpTypeID" || fieldname == "Salary")
                {


                    if (fieldname == "SSN")
                    {
                        oldvalue = GeneralFunction.FormatUsSSN(oldvalue);
                        newValue = GeneralFunction.FormatUsSSN(newValue);
                    }
                    if (fieldname == "Salary")
                    {
                        oldvalue = GeneralFunction.FormatCurrency(oldvalue, Location);
                        newValue = GeneralFunction.FormatCurrency(newValue, Location);
                    }

                    if (fieldname == "Date of birth")
                    {
                        oldvalue =oldvalue==""?"":Convert.ToDateTime(oldvalue).ToString("MM/dd/yyyy");
                        newValue = newValue == "" ? "" : Convert.ToDateTime(newValue).ToString("MM/dd/yyyy");
                    }


                    if (fieldname == "Address1")
                    {
                        fieldname = "Street";

                    }
                    else if (fieldname == "Address2")
                    {
                        fieldname = "City";

                    }
                    else if (fieldname == "Businessemail")
                    {
                        fieldname = "Business email";

                    }
                    else if (fieldname == "Personalemail")
                    {
                        fieldname = "Personal email";

                    }
                    else if (fieldname == "DrivingLicencenum")
                    {
                        fieldname = "Driving Licence";

                    }
                    else if (fieldname == "firstname")
                    {
                        fieldname = "First name";

                    }
                    else if (fieldname == "Lastname")
                    {
                        fieldname = "Last name";

                    }

                    else if (fieldname == "deptname")
                    {
                        fieldname = "Department";

                    }
                    else if (fieldname == "startDate")
                    {
                        fieldname = "Started date";
                    }

                    else if (fieldname == "TermDate")
                    {
                        fieldname = "Term date";
                    }
                    else if (fieldname == "TermReason")
                    {
                        fieldname = "Term reason";
                    }

                    else if (fieldname == "ScheduleID")
                    {
                        fieldname = "Schedule";

                    }
                    else if (fieldname == "Person1")
                    {
                        fieldname = "First conact name";

                    }
                    else if (fieldname == "Person2")
                    {
                        fieldname = "Second conact name";

                    }
                    else if (fieldname == "Person3")
                    {
                        fieldname = "Third conact name";

                    }

                    else if (fieldname == "P1Address1")
                    {
                        fieldname = "Street of first contact person";

                    }

                    else if (fieldname == "P2Address1")
                    {
                        fieldname = "Street of second contact person";

                    }
                    else if (fieldname == "P3Address1")
                    {
                        fieldname = "Street of third contact person";

                    }
                    else if (fieldname == "P1Address2")
                    {
                        fieldname = "City of first contact person";

                    }
                    else if (fieldname == "P2Address2")
                    {
                        fieldname = "City of second contact person";

                    }
                    else if (fieldname == "P3Address2")
                    {
                        fieldname = "City of third contact person";

                    }
                    else if (fieldname == "phone1")
                    {
                        fieldname = "Phone number of first contact person";

                    }
                    else if (fieldname == "phone2")
                    {
                        fieldname = "Phone number of second contact person";

                    }
                    else if (fieldname == "phone3")
                    {
                        fieldname = "Phone number of third contact person";
                    }

                    else if (fieldname == "email1")
                    {
                        fieldname = "Email of first contact person";

                    }
                    else if (fieldname == "email2")
                    {
                        fieldname = "Email of second contact person";

                    }
                    else if (fieldname == "email3")
                    {
                        fieldname = "Email of third contact person";

                    }

                    else if (fieldname == "Zip1")
                    {
                        fieldname = "Zip of first contact person";

                    }
                    else if (fieldname == "Zip2")
                    {
                        fieldname = "Zip of second contact person";

                    }
                    else if (fieldname == "Zip3")
                    {
                        fieldname = "Zip of third contact person";

                    }

                    else if (fieldname == "relation1")
                    {
                        fieldname = "Relation of first contact person";

                    }
                    else if (fieldname == "relation2")
                    {
                        fieldname = "Relation of second contact person";

                    }
                    else if (fieldname == "relation3")
                    {
                        fieldname = "Relation of third contact person";

                    }
                    else if (fieldname == "Marital status")
                    {
                        if (oldvalue == "1")
                        {
                            oldvalue = "Single";
                        }
                        else
                        {
                            oldvalue = "Married";
                        }
                        if (newValue == "1")
                        {
                            newValue = "Single";
                        }
                        else
                        {
                            newValue = "Married";
                        }

                    }
                    else if (fieldname == "ScheduleID")
                    {

                        fieldname = "Schedule";
                        oldvalue = GetSchedule(oldvalue);
                        newValue = GetSchedule(oldvalue);

                    }
                    else if (fieldname == "WageID")
                    {

                        fieldname = "Wage type";
                        oldvalue = GetWages(oldvalue);
                        newValue = GetWages(oldvalue);

                    }
                    else if (fieldname == "EmpTypeID")
                    {

                        fieldname = "Employee type";
                        oldvalue = GetEmployeetype(oldvalue);
                        newValue = GetEmployeetype(oldvalue);

                    }
                    else if (fieldname == "StateID")
                    {

                        fieldname = "State";
                        oldvalue = getStateName(oldvalue);
                        newValue = getStateName(oldvalue);
                    }

                    else if (fieldname == "StateID1")
                    {

                        fieldname = "State of first contact person";
                        oldvalue = getStateName(oldvalue);
                        newValue = getStateName(oldvalue);
                    }

                    else if (fieldname == "StateID2")
                    {

                        fieldname = "State of second contact person";
                        oldvalue = getStateName(oldvalue);
                        newValue = getStateName(oldvalue);
                    }
                    else if (fieldname == "StateID3")
                    {

                        fieldname = "State of third contact person";
                        oldvalue = getStateName(oldvalue);
                        newValue = getStateName(oldvalue);
                    }
                    else if (fieldname == "IsActive")
                    {
                        if (oldvalue == "1")
                        {
                            if (newValue == "0")
                            {
                                Proper = "Upadted to inactive(removed)";
                            }
                            else
                            {
                                Proper = "Joined";
                            }
                        }

                        if (oldvalue == "0")
                        {
                            if (newValue == "0")
                            {
                                Proper = "Upadted to inactive(removed)";
                            }
                            else
                            {
                                Proper = "Joined";
                            }
                        }
                    }
                    else
                    {
                        fieldname = fieldname;
                    }

                    if (oldvalue != "" && newValue != "")
                    {
                        Proper = fieldname + " &nbsp;updated to&nbsp; " + "<b>" + newValue + "</b>" + " &nbsp;at &nbsp;" + Currentdate;
                    }
                    else if (oldvalue == "" && newValue != "")
                    {
                        Proper = fieldname+": <b>" + newValue + "</b>" + " &nbsp;is &nbsp;added &nbsp; " + " &nbsp;at &nbsp;" + Currentdate;
                    }
                    else if (oldvalue == "" && newValue != "")
                    {
                        Proper = fieldname + ": <b>" + oldvalue + "</b>" + " &nbsp;is &nbsp;removed &nbsp; " + " &nbsp;at &nbsp;" + Currentdate;
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Proper;
        }

        private string GetSchedule(string old)
        {
            try
            {
                if (old != "")
                {
                    Attendance.BAL.Report obj = new Attendance.BAL.Report();
                    DataTable ds = obj.GetAllScheduleTypes();
                    DataTable dt = new DataTable();
                    DataView dv = ds.DefaultView;
                    dv.RowFilter = "ScheduleID=" + Convert.ToInt32(old);
                    dt = dv.ToTable();
                    old = dt.Rows[0]["ScheduleType"].ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return old;
        }

        private string getStateName(string old)
        {
            try
            {
                if (old != "")
                {
                    Attendance.BAL.Business obj = new Attendance.BAL.Business();
                    DataTable ds = obj.GetState();
                    DataTable dt = new DataTable();
                    DataView dv = ds.DefaultView;
                    dv.RowFilter = "StateID=" + Convert.ToInt32(old);
                    dt = dv.ToTable();
                    old = dt.Rows[0]["StateName"].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return old;
        }

        private string GetWages(string old)
        {
            try
            {
                if (old != "")
                {
                    Attendance.BAL.Report obj = new Attendance.BAL.Report();
                    DataTable ds = obj.GetAllWages();
                    DataTable dt = new DataTable();
                    DataView dv = ds.DefaultView;
                    dv.RowFilter = "WageID=" + Convert.ToInt32(old);
                    dt = dv.ToTable();
                    old = dt.Rows[0]["WageType"].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return old;
        }
        private string GetEmployeetype(string old)
        {
            try
            {
                if (old != "")
                {
                    Attendance.BAL.Report obj = new Attendance.BAL.Report();
                    DataTable ds = obj.GetAllEmployeetypes();
                    DataTable dt = new DataTable();
                    DataView dv = ds.DefaultView;
                    dv.RowFilter = "EmpTypeID=" + Convert.ToInt32(old);
                    dt = dv.ToTable();
                    old = dt.Rows[0]["EmpType"].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return old;
        }
        

    }
}
