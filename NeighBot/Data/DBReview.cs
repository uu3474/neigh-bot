using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighBot
{
    public class DBReview
    {
        public long ID { get; set; }
        public DateTime CreateTime { get; set; }
        public long FromUser { get; set; }
        public long ToUser { get; set; }
        public short Grade { get; set; }
        public string Review { get; set; }
    
        public DBReview()
        {
        }

        public DBReview(short grade, string review)
        {
            Grade = grade;
            Review = review;
        }
    }
}
