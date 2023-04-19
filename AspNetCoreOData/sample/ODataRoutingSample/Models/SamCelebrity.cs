using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataRoutingSample.Models
{
    public class SamCelebrity
    {
        public SamCelebrity()
        {
            Properties = new Dictionary<string, object>();
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get; set;
        }

        [NotMapped]
        public Dictionary<string, object> Properties
        {
            get;
            set;
        }
    }
}