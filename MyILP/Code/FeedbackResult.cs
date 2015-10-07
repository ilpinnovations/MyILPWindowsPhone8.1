using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyILP.Code
{
    class FeedbackResult
    {
        public enum FeedbackStatusType { NEW, OLD, ERROR }
        public FeedbackStatusType FeedStatus { get; set; }
    }
}
