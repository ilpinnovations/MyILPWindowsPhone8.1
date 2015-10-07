using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyILP.Code
{
    class UserInformation
    {
        public enum ILP_Planet { Ahmedabad, Chennai, Guwahati, Hyderabad, Trivandrum }
        static string _employeeId, _fullName, _lgName, _emailId;
        static int _locationIndex;
        
        public static string EmployeeId { get { return _employeeId; } set { _employeeId = value; } }
        public static string FullName { get { return _fullName; } set { _fullName = value; } }
        public static string LGName { get { return _lgName; } set { _lgName = value.ToUpper(); } }
        public static string EmailId { get { return _emailId; } set { _emailId = value; } }
        public static int LocationIndex { get { return _locationIndex; } set { _locationIndex = value; LocationName = ((ILP_Planet)value).ToString();} }
        public static string LocationName { get; set; }
        public static void UpdateUserInformation(string employeeId,
            string fullName, string lgName, string emailId, int locationIndex)
        {
            EmployeeId = employeeId;
            FullName = fullName;
            LGName = lgName;
            EmailId = emailId;
            LocationIndex = locationIndex;

            //Save to disk
            SaveUserInformation();
        }
        public static bool GetUserInformation()
        {
            bool IsFirstLaunch = false;

            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("NOT_FIRST_LAUNCH"))
            {
                EmployeeId      = (string)localSettings.Values["KEY_EMP_ID"];
                FullName        = (string)localSettings.Values["KEY_FULL_NAME"];
                LGName          = (string)localSettings.Values["KEY_LG_NAME"];
                EmailId         = (string)localSettings.Values["KEY_EMAIL_ID"];
                LocationIndex   = (int)localSettings.Values["KEY_LOCATION_INDEX"];
            }
            else
            {
                EmployeeId = FullName = LGName = EmailId = string.Empty;
                LocationIndex = 4;
                IsFirstLaunch = true;
            }

            return IsFirstLaunch;
        }
        public static void ClearUserInformation()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
        }
        public static void SaveUserInformation()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            localSettings.Values["KEY_EMP_ID"] = EmployeeId;
            localSettings.Values["KEY_FULL_NAME"] = FullName;
            localSettings.Values["KEY_LG_NAME"] = LGName;
            localSettings.Values["KEY_EMAIL_ID"] = EmailId;
            localSettings.Values["KEY_LOCATION_INDEX"] = LocationIndex;

            #region MiscSaves
            //WARNING! App depends on the following key, do not delete!
            localSettings.Values["NOT_FIRST_LAUNCH"] = "Developed by: Milind Gour";
            #endregion        
        }
    }
}
