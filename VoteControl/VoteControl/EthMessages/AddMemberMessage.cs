using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace VoteControl.EthMessages
{
    [Function("addMember", "string")]
    public class AddMemberMessage : FunctionMessage
    {
        [Parameter("address", "_targetMember", 1)]
        public string TargetMember { get; set; }

        [Parameter("string", "_memberName", 2)]
        public string MemberName { get; set; }

        [Parameter("uint", "_nnn", 3)]
        public int NN { get; set; }
    }
}
