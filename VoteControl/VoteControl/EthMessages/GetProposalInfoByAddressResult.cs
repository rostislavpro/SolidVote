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
    public class GetProposalInfoByAddressResult : IFunctionOutputDTO
    {
        [Parameter("bool", "supportsProposal", 1)]
        public bool SupportsProposal { get; set; }

        [Parameter("bool", "isVoted", 2)]
        public bool IsVoted { get; set; }

        [Parameter("string", "description", 3)]
        public string description { get; set; }

        [Parameter("uint256", "numberOfVotes", 4)]
        public BigInteger NumberOfVotes { get; set; }

        [Parameter("uint256", "currentResult", 5)]
        public BigInteger CurrentResult { get; set; }

        [Parameter("bool", "isFinished", 6)]
        public bool isFinished { get; set; }
    }
}
