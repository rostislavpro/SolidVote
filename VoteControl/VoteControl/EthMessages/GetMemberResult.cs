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
    public class GetMemberResult : IFunctionOutputDTO
    {
        [Parameter("bool", "active", 1)]
        public bool IsActive { get; set; }

        [Parameter("bool", "isMember", 2)]
        public bool IsMember { get; set; }

        [Parameter("string", "name", 3)]
        public string Name { get; set; }

        [Parameter("uint256", "memberSince", 4)]
        public BigInteger MemberSince { get; set; }

        [Parameter("uint256", "memberShare", 5)]
        public BigInteger MemberShare { get; set; }
    }
}
