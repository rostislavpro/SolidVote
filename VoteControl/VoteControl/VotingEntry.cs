using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteControl
{
    public class VotingEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VotingStatus Status { get; set; }
    }
}
