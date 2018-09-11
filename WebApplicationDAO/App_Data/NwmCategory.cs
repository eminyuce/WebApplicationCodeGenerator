using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebApplicationDAO
{
    public class NwmCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int ParentCategoryId { get; set; }
        public bool IsActive { get; set; }



    }
}
