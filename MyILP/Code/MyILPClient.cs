using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using System.IO;

namespace MyILP.Code
{
    class MyILPClient
    {
        public enum FeedbackStatus { New, Old, Error }
        const string CONTACTS_FILENAME = "EmergencyContacts_Cache.json";
        const string SCHEDULE_FILENAME_PREFIX = "iD351r3";

        const string BASE_URL = "http://theinspirer.in/ilpscheduleapp/";

        /// <summary>
        /// Gets the schedule of specified batch for a day
        /// </summary>
        /// <param name="batch"> Batch code to fetch the data for </param>
        /// <param name="date"> Format = YYYY-MM-DD </param>
        public async static Task<ScheduleModel> GetSchedule(string batch, string date)
        {
            ScheduleModel returnObject;

            //if available locally, read that
            //otherwise, fetch from server
            string filename = string.Format("{0}_{1}_{2}.json", SCHEDULE_FILENAME_PREFIX, batch, date);
            string urlContent = await ReadContentsFromDisk(filename);

            if (urlContent.Length == 0)
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("batch", batch); p.Add("date", date);
                urlContent = await GetURLContentAsString("schedulelist_json.php", p);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[SCHEDULE] Disk Read FAILURE, {0} does not exist.", filename);
#endif

                returnObject = ParseSchedule(urlContent);
                //Write file to disk
                if (returnObject.ScheduleItems.Count > 0)
                {
                    SaveContentsToDisk(filename, urlContent);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("[SCHEDULE] File {0} written to disk.", filename);
#endif
                }
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[SCHEDULE] Disk Read SUCCESS");
#endif
                returnObject = ParseSchedule(urlContent);
            }

            return returnObject;
        }
        public async static Task<ScheduleModel> GetSchedule_NetworkPriority(string batch, string date)
        {
            ScheduleModel returnObject;

            //if available on internet, read that
            //otherwise, fetch from disk
            string filename = string.Format("{0}_{1}_{2}.json", SCHEDULE_FILENAME_PREFIX, batch, date);

            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("batch", batch); p.Add("date", date);
            string urlContent = await GetURLContentAsString("schedulelist_json.php", p);

            if (urlContent.Length == 0)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[SCHEDULE] Network Read FAILURE");
#endif
                urlContent = await ReadContentsFromDisk(filename);
                returnObject = ParseSchedule(urlContent);
            }
            else
            {
                returnObject = ParseSchedule(urlContent);
                if (returnObject.ScheduleItems.Count > 0)
                {
                    SaveContentsToDisk(filename, urlContent);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("[SCHEDULE] New Schedule written to disk, filename = " + filename);
#endif
                }
            }

            if (urlContent.Length == 0)
            {
                // Network + disk read fail
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[SCHEDULE] Network and Disk Read FAILURE");
#endif
            }

            return returnObject;
        }
        public async static Task<LeaderboardModel> GetLeaderboard()
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("empid", UserInformation.EmployeeId);
            p.Add("batch", UserInformation.LGName);
            string urlContent = await GetURLContentAsString("leaderboard_iD351r3_json.php", p);
            return ParseLeaderboard(urlContent);
        }
        public async static void UpdatePushURIOnServer(string devID, string uri)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("--------PUSH URI UPDATE CALLED ON SERVER---------");
#endif
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("empid", UserInformation.EmployeeId);
            p.Add("devid", devID);
            p.Add("batch", UserInformation.LGName);
            p.Add("push_uri", uri);
            string urlContent = await GetURLContentAsString("updateDeviceUri_iD351r3_json.php", p);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("SERVER_RESPONSE: " + urlContent);
#endif

        }
        private static LeaderboardModel ParseLeaderboard(string jsonString)
        {
            LeaderboardModel model = new LeaderboardModel();
            try
            {
                JObject o = JObject.Parse(jsonString);
                JArray a = (JArray)o["Android"];
                string myPointsString = (string)o["MyPoints"];
                model.MyPoints = int.Parse(myPointsString);

                foreach (JObject i in a)
                {
                    LeaderboardItem lbItem = new LeaderboardItem() { 
                        Batch = (string)i["emp_batch"],
                        EmployeeId = (string)i["emp_id"],
                        EmployeeName = (string)i["emp_name"],
                        Points = int.Parse((string)i["points"])
                    };
                    model.LeaderboardItems.Add(lbItem);
                }
            }
            catch (Exception)
            {
                throw new HttpRequestException("Please make sure you have an active internet connection");
            }

            return model;
        }

        public async static Task<FeedbackResult> RegisterFacultyFeedback(ScheduleItem scheduleItem, FeedbackObject feedbackItem)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("faculty", scheduleItem.Faculty);
            p.Add("course", scheduleItem.Course);
            p.Add("comment", feedbackItem.Comments);
            p.Add("rate", feedbackItem.Rating.ToString());
            p.Add("empid", UserInformation.EmployeeId);
            p.Add("empname", UserInformation.FullName);
            p.Add("emploc", UserInformation.LocationName);
            p.Add("empbatch", UserInformation.LGName);
            p.Add("slot", scheduleItem.Slot);
            p.Add("date", scheduleItem.Date1);

            string urlContent = await GetURLContentAsString("feedback_json.php", p);
            return ParseFeedbackResult(urlContent);
        }
        public async static Task<AvgRatingObject> GetAvgRating(ScheduleItem scheduleItem)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("faculty", scheduleItem.Faculty);
            p.Add("course", scheduleItem.Course);
            p.Add("slot", scheduleItem.Slot);
            p.Add("date", scheduleItem.Date1);

            string urlContent = await GetURLContentAsString("faculty_json.php", p);
            return ParseAvgRatingResult(urlContent);
        }

        private static AvgRatingObject ParseAvgRatingResult(string jsonString)
        {
            AvgRatingObject ratingObject = new AvgRatingObject(/* iD351r3 */);
            try
            {
                JObject o = JObject.Parse(jsonString);
                JArray a = (JArray)o["Android"];

                foreach (JObject i in a)
                {
                    ratingObject.TotalComments = (int)i["count"];
                    ratingObject.AvgRating = double.Parse((string)i["avg_count"]);
                    ratingObject.Success = ((string)i["faculty_result"] == "success");
                }
            }
            catch (Exception)
            {
                return new AvgRatingObject();
            }

            return ratingObject;
        }
        private static FeedbackResult ParseFeedbackResult(string jsonString)
        {
            FeedbackResult feedResult = new  FeedbackResult();
            try
            {
                JObject o = JObject.Parse(jsonString);
                JArray a = (JArray)o["Android"];

                foreach (JObject i in a)
                {
                    string feed_result = (string)i["feed_result"];
                    if (feed_result == "success") feedResult.FeedStatus = FeedbackResult.FeedbackStatusType.NEW;
                    else if (feed_result == "already") feedResult.FeedStatus = FeedbackResult.FeedbackStatusType.OLD;
                    else feedResult.FeedStatus = FeedbackResult.FeedbackStatusType.ERROR;
                }
            }
            catch (Exception)
            {
                return new FeedbackResult(){ FeedStatus =  FeedbackResult.FeedbackStatusType.ERROR };
            }

            return feedResult;
        }
        public async static Task<EContactModel> GetEContacts()
        {
            //if available locally, read that
            //otherwise, fetch from server
            string urlContent = await ReadContentsFromDisk(CONTACTS_FILENAME);

            if (urlContent.Length == 0)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[CONTACTS] Fetching contacts from server for " + UserInformation.LocationName);
#endif
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("ilp", UserInformation.LocationName);
                urlContent = await GetURLContentAsString("getEmergencyContacts.php", p);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[CONTACTS] Disk Read FAILURE. File " + CONTACTS_FILENAME + " not found");
#endif

                //Write file to disk
                SaveContentsToDisk(CONTACTS_FILENAME, urlContent);

#if DEBUG
                System.Diagnostics.Debug.WriteLine("[CONTACTS] Contacts saved to local file " + CONTACTS_FILENAME);
#endif
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[CONTACTS] Read success from local file " + CONTACTS_FILENAME);
#endif
            }

            return ParseEContacts(urlContent);
        }

        private static EContactModel ParseEContacts(string jsonString)
        {
            EContactModel model = new EContactModel();
            try
            {
                JArray a = JArray.Parse(jsonString);

                foreach (JObject i in a)
                {
                    foreach (var p in i)
                    {
                        EContactItem eContactItem = new EContactItem();
                        eContactItem.Name = p.Key;
                        eContactItem.Contact = (string)p.Value;
                        model.EContactItems.Add(eContactItem);
                    }
                }
            }
            catch (Exception)
            {
                throw new HttpRequestException("Internet connection is required to download the contact list for the first time");
            }

            return model;
        }
        
        public async static Task<NotificationModel> GetNotifications()
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            string urlContent = await GetURLContentAsString("notify_json.php", p);
            return ParseNotifications(urlContent);
        }
        private async static Task<string> GetURLContentAsString(string relativeURL, Dictionary<string,string> postParameters)
        {
            HttpClient client = new HttpClient();
            string url = BASE_URL + relativeURL;
            var response = await client.PostAsync(url, new FormUrlEncodedContent(postParameters));
            //response.EnsureSuccessStatusCode();

#if DEBUG
            System.Diagnostics.Debug.WriteLine("*******\nURL\t\t\t\t: " + url + "\nPayload length\t: " + response.Content.Headers.ContentLength + " byte(s)\n*******");
#endif

            return response.Content.ReadAsStringAsync().Result;
        }

        private static NotificationModel ParseNotifications(string jsonString)
        {
            NotificationModel model = new NotificationModel();
            try
            {
                JObject o = JObject.Parse(jsonString);
                JArray a = (JArray)o["Android"];

                foreach (var i in a)
                {
                    model.NotificationItems.Add(new NotificationItem()
                    {
                        Date = DateTime.Parse((string)i["msg_date"]),
                        SerialNo = int.Parse((string)i["s_no"]),
                        Message = (string)i["message"]
                    });
                }
            }
            catch (Exception)
            {
                return new NotificationModel();
            }

            return model;
        }
        private static ScheduleModel ParseSchedule(string jsonString)
        {
            ScheduleModel model = new ScheduleModel();
            
            try
            {
                JObject o = JObject.Parse(jsonString);
                JArray a = (JArray)o["Android"];

                foreach (var i in a)
                {
                    model.ScheduleItems.Add(new ScheduleItem()
                    {
                        Batch = (string)i["batch"],
                        Course = (string)i["course"],
                        Date1 = (string)i["date1"],
                        Faculty = (string)i["faculty"],
                        Result = (string)i["result"],
                        Room = (string)i["room"],
                        Slot = (string)i["slot"]
                    }); 
                }
            }
            catch (Exception)
            {
                return new ScheduleModel();
            }

            return model;
        }

        public static async void ClearLocalStorage()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var localFiles = await localFolder.GetFilesAsync();

#if DEBUG
                System.Diagnostics.Debug.WriteLine("[CLEANER] Cleaning local storage...");
#endif

            foreach (var file in localFiles)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[CLEANER] Deleting file " + file.Name);
#endif
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine("[CLEANER] Cleaning local settings...");
#endif
                UserInformation.ClearUserInformation();

#if DEBUG
                System.Diagnostics.Debug.WriteLine("[CLEANER] Cleaning completed");
#endif
        }
        private static async Task<string> ReadContentsFromDisk(string filename)
        {
            string content = "";

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

#if DEBUG
            var files = await localFolder.GetFilesAsync();

            System.Diagnostics.Debug.WriteLine("-------------------------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("Following {0} files exist in the storage:", files.Count);
            ulong totalFileSize = 0;
            foreach (var file in files)
            {
                var properties = await file.GetBasicPropertiesAsync();
                totalFileSize += properties.Size;
                bool isJson = Path.GetExtension(file.Name) == ".json";
                System.Diagnostics.Debug.WriteLine(file.Name + ", File Size = " + properties.Size + " bytes");
            }
            System.Diagnostics.Debug.WriteLine("Total Size of Local Folder = {0} bytes", totalFileSize);
            System.Diagnostics.Debug.WriteLine("-------------------------------------------------------------------");
#endif   
            
            try
            {
                StorageFile contactsFile = await localFolder.GetFileAsync(filename);
                using (var stream = await contactsFile.OpenStreamForReadAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                return "";
            }            

            return content;
        }
        private static async void SaveContentsToDisk(string filename, string contents)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile contactsFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var stream = await contactsFile.OpenStreamForWriteAsync())
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(contents);
                }
            }
        }
    }
}

