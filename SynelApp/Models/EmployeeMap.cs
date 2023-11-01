using CsvHelper.Configuration;
using SynelApp.Extensions;

namespace SynelApp.Models
{
    /// <summary>
    /// This class is responsible for converting csv files to .Net Employee object
    /// </summary>
    public class EmployeeMap : ClassMap<Employee>
    {
        private const string _payrollColumn = "Personnel_Records.Payroll_Number";
        private const string _forenamesColumn = "Personnel_Records.Forenames";
        private const string _dobColumn = "Personnel_Records.Date_of_Birth";
        private const string _telephoneColumn = "Personnel_Records.Telephone";
        private const string _mobileColumn = "Personnel_Records.Mobile";
        private const string _addressColumn = "Personnel_Records.Address";
        private const string _address2Column = "Personnel_Records.Address_2";
        private const string _postcodeColumn = "Personnel_Records.Postcode";
        private const string _emailColumn = "Personnel_Records.EMail_Home";
        private const string _startDateColumn = "Personnel_Records.Start_Date";

        public EmployeeMap()
        {
            Map(e => e.PayrollNumber).Name(_payrollColumn);
            Map(e => e.Forenames).Name(_forenamesColumn);
            Map(e => e.Telephone).Name(_telephoneColumn);
            Map(e => e.Mobile).Name(_mobileColumn);
            Map(e => e.Address).Name(_addressColumn);
            Map(e => e.Address2).Name(_address2Column).Optional();
            Map(e => e.Postcode).Name(_postcodeColumn);
            Map(e => e.EmailHome).Name(_emailColumn).Optional();

            Map(e => e.DateOfBirth)
                .Name(_dobColumn)
                .Convert(args =>
                {
                    string dateValue = args.Row.GetField(_dobColumn) ?? "";
                    return dateValue.ToDateTime();
                });

            Map(e => e.StartDate)
                .Name(_startDateColumn)
                .Convert(args =>
                {
                    string dateValue = args.Row.GetField(_startDateColumn) ?? "";
                    DateTime result = dateValue.ToDateTime();
                    if (result == DateTime.MinValue)
                        // if conversion failed, set the today's day as the starting day
                        result = DateTime.UtcNow;
                    return result;
                });
        }
    }
}
