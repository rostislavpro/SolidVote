using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterApp
{
    public class VotingEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VotingStatus Status { get; set; }
        public DateTime EndsAt { get; set; }
        public bool IsVoted { get; set; }
        public bool CanVote => !IsVoted;
        public VoteOption FounderVote { get; set; } 
    }
}
