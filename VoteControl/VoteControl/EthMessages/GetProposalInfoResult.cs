using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VoteControl.EthMessages
{
    [FunctionOutput]
    public class GetProposalInfoResult : IFunctionOutputDTO
    {
        [Parameter("bool", "supportsProposal", 1)]
        public bool SupportsProposal { get; set; }

        [Parameter("bool", "isVoted", 2)]
        public bool IsVoted { get; set; }

        [Parameter("string", "description", 3)]
        public string Description { get; set; }

        [Parameter("uint256", "numberOfVotes", 4)]
        public BigInteger NumberOfVotes { get; set; }

        [Parameter("bool", "isFinished", 5)]
        public bool IsFinished { get; set; }

        [Parameter("bool", "proposalRejected", 6)]
        public bool ProposalRejected { get; set; }

        [Parameter("uint256", "endDate", 7)]
        public BigInteger EndDate { get; set; }

    }
}
