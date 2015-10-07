using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyILP.Code
{
    public class AvgRatingObject
    {
        public double AvgRating { get; set; }
        public int TotalComments { get; set; }
        public bool Success { get; set; }

        public AvgRatingObject(int totalComments, double rating, bool success)
        {
            AvgRating = rating;
            TotalComments = totalComments;
            Success = success;
        }
        public AvgRatingObject()
        {
            AvgRating = 0;
            TotalComments = 0;
            Success = false;
        }
    }
}
