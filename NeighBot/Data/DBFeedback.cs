using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighBot
{
    public class DBFeedback
    {
        public long ID { get; set; }
        public DateTime CreateTime { get; set; }
        public long User { get; set; }
        public string Feedback { get; set; }

        public DBFeedback()
        {
        }

        public DBFeedback(string feedback)
        {
            Feedback = feedback;
        }
    }
}
