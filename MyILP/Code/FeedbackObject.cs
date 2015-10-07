using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyILP.Code
{
    public class FeedbackObject
    {
        public string FacultyName { get; set; }
        public string CourseName { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }

        public FeedbackObject(string facultyName, string courseName, int rating, string comments)
        {
            FacultyName = facultyName;
            CourseName = courseName;
            Rating = rating;
            Comments = comments;
        }
    }
}
