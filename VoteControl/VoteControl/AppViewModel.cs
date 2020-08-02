using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Vote.Commons;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Telegram.Bot;
using VoteControl.Commands;
using VoteControl.EthMessages;

namespace VoteControl
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private readonly TelegramBotClient botClient;
        private readonly Nethereum.Web3.Web3 ethClient;
        private readonly List<string> seenTxHashes = new List<string>();

        #region ControlAddress
        private string controlAddress;
        public string ControlAddress
        {
            get => controlAddress;
            set
            {
                if (this.controlAddress != value)
                {
                    this.controlAddress = value;
                    OnPropertyChanged(nameof(ControlAddress));
                }
            }
        }
        #endregion

        #region VotingTheme
        private string votingTheme;
        public string VotingTheme
        {
            get => this.votingTheme;
            set
            {
                if (this.votingTheme != value)
                {
                    this.votingTheme = value;
                    OnPropertyChanged(nameof(VotingTheme));
                }
            }
        }
        #endregion

        #region VotingExpires
        private DateTime votingExpires;
        public DateTime VotingExpires
        {
            get => this.votingExpires;
            set
            {
                if (this.votingExpires != value)
                {
                    this.votingExpires = value;
                    OnPropertyChanged(nameof(VotingExpires));
                }
            }
        }
        #endregion

        public BindingList<VotingEntry> CurrentVotings { get; } = new BindingList<VotingEntry>();

        #region StartVotingCommand
        private ICommand startVoting;
        public ICommand StartVoting
        {
            get
            {
                if (startVoting == null)
                    startVoting = new VoteControlCommand(StartVotingHandler);

                return startVoting;
            }
        }
        #endregion

        #region RefreshVotings
        private ICommand refreshVotings;
        public ICommand RefreshVotings
        {
            get
            {
                if (refreshVotings == null)
                    refreshVotings = new VoteControlCommand(RefreshVotingsHandler);

                return refreshVotings;
            }
        }
        #endregion

        //#region AddMemberCommand
        //private ICommand addMember;
        //public ICommand AddMember
        //{
        //    get
        //    {
        //        if (addMember == null)
        //            addMember = new AddMemberCommand(AddMemberCommand);

        //        return addMember;
        //    }
        //}
        //#endregion

        #region SendFileCommand
        private ICommand sendFile;
        public ICommand SendFile
        {
            get
            {
                if (sendFile == null)
                    sendFile = new VoteControlCommand(SendFileHandler);

                return sendFile;
            }
        }
        #endregion

        private volatile int threadsLoading = 0;

        #region PollState
        private string pollState = "-";
        public string PollState
        {
            get => pollState;
            set
            {
                if (pollState != value)
                {
                    pollState = value;
                    OnPropertyChanged(nameof(PollState));
                }
            }
        }
        #endregion

        public AppViewModel(TelegramBotClient botClient, Nethereum.Web3.Web3 ethClient)
        {
            this.botClient = botClient;
            this.ethClient = ethClient;

            this.VotingExpires = DateTime.Now;
            this.ControlAddress = "1f911378af8d5ab2bf5868e1b75d84684acb58b4";

            Task.Run(LoadCurrentVotings);
        }

        private void StartVotingHandler()
        {
            var me = botClient.GetMeAsync().Result;

            var contract = ethClient.Eth.GetContract(CommonStuff.ContractAbi, CommonStuff.ContractAddress);
            var newProposalHandler = ethClient.Eth.GetContractTransactionHandler<NewProposalMessage>();
            ethClient.TransactionManager.DefaultGas = new System.Numerics.BigInteger(3000000);

            var newProposal = new NewProposalMessage
            {
                Description = this.VotingTheme,
                Link = "https://t.me/joinchat/AAAAAEcq4OCa2bdGCmt_tg",
                EndDate = this.VotingExpires.Ticks,
                Gas = 3000000
            };
            var transactionReceipt = newProposalHandler.SendRequestAndWaitForReceiptAsync(CommonStuff.ContractAddress, newProposal).Result;

            var getMemberHandler = ethClient.Eth.GetContractQueryHandler<GetMemberMessage>();
            var getInfoMsg = new GetMemberMessage() { Address = this.ControlAddress };
            var memberData = getMemberHandler.QueryDeserializingToObjectAsync<GetMemberResult>(getInfoMsg, CommonStuff.ContractAddress).Result;

            var etherscanlink = "https://kovan.etherscan.io/address/" + CommonStuff.ContractAddress;
            var messageText = $"● *{memberData.Name}* started voting on subject `{this.VotingTheme}`.\r\nDeadline: {this.VotingExpires} \r\nETH transaction: `{transactionReceipt.TransactionHash}`\r\nEtherscan: {etherscanlink}";

            var msg = botClient.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(576530041),
                messageText,
                Telegram.Bot.Types.Enums.ParseMode.Markdown,
                true, false).Result;
        }

        private void RefreshVotingsHandler()
        {
            Task.Run(LoadCurrentVotings);
        }

        private async Task LoadCurrentVotings()
        {
            var prevValue = Interlocked.CompareExchange(ref threadsLoading, 1, 0);
            if (prevValue > 0)
                return;

            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate
                            {
                                this.CurrentVotings.Clear();
                            }
                   ));

            try
            {
                var contract = ethClient.Eth.GetContract(CommonStuff.ContractAbi, CommonStuff.ContractAddress);
                var numProposalsFunc = contract.GetFunction("numProposals");
                var numProposal = await numProposalsFunc.CallAsync<BigInteger>();

                var getProposalInfoHandler = ethClient.Eth.GetContractQueryHandler<GetProposalInfoMessage>();

                for (var idx = 0; idx < numProposal; idx++)
                {
                    var getInfoMsg = new GetProposalInfoMessage() { Idx = idx };
                    var votingInfo = await getProposalInfoHandler.QueryDeserializingToObjectAsync<GetProposalInfoResult>(getInfoMsg, CommonStuff.ContractAddress);


                    var newVotingEntry = new VotingEntry();
                    newVotingEntry.Id = idx + 1;
                    newVotingEntry.Name = votingInfo.Description;

                    var endDate = new DateTime((long)votingInfo.EndDate);
                    newVotingEntry.Status = votingInfo.IsFinished ? VotingStatus.Completed : (endDate >= DateTime.Now ? VotingStatus.Running: VotingStatus.Aborted);

                    System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate
                            {
                                this.CurrentVotings.Add(newVotingEntry);
                                OnPropertyChanged(nameof(CurrentVotings));
                            }
                   ));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Interlocked.CompareExchange(ref threadsLoading, 0, 1);
            }
        }

        //private void AddMemberCommand()
        //{


        //    //ethClient.TransactionManager.DefaultGas = new System.Numerics.BigInteger(3000000);
        //    //ethClient.TransactionManager.DefaultGasPrice = Nethereum.Signer.Transaction.DEFAULT_GAS_PRICE;

        //    var contract = ethClient.Eth.GetContract(contractAbi, contractAddress);

        //    //var handler = ethClient.Eth.GetContractTransactionHandler<AddMemberMessage>();



        //    //var estimate = handler.EstimateGasAsync(contractAddress, msg).Result;

        //    //var estimate = contract.GetFunction("updateMember").EstimateGasAsync().Result;

        //    var transferHandler = ethClient.Eth.GetContractTransactionHandler<AddMemberMessage>();
        //    var msg = new AddMemberMessage
        //    {
        //        TargetMember = this.ControlAddress,
        //        MemberName = "Andrei",
        //        NN = 1,
        //        // Gas = estimate
        //    };
        //    var transactionReceipt = transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, msg).Result;
        //}

        #region NotifyPropertyChanged stuff
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void SendFileHandler()
        {
            GenerateReport(0);
        }

        private void GenerateReport(int votingIndex)
        {
            var contract = ethClient.Eth.GetContract(CommonStuff.ContractAbi, CommonStuff.ContractAddress);
            var numMembersFunc = contract.GetFunction("numMembers");
            var numMembers = numMembersFunc.CallAsync<BigInteger>().Result;

            var numSharesFunc = contract.GetFunction("totalNNN");
            var totalShares = numSharesFunc.CallAsync<BigInteger>().Result;

            var getMemberHandler = ethClient.Eth.GetContractQueryHandler<GetMemberMessage>();
            var memberAddressHandler = ethClient.Eth.GetContractQueryHandler<MemberAddressMessage>();

            var voters = new List<(string name, string address, int shares)>();

            for (var idx = 0; idx < numMembers; idx++)
            {
                var msg = new MemberAddressMessage { Idx = new BigInteger(idx) };
                var getAddrFunc = contract.GetFunction("membersAddr");
                var address = getAddrFunc.CallAsync<string>(new BigInteger(idx)).Result;

                var getInfoMsg = new GetMemberMessage() { Address = address };
                var memberData = getMemberHandler.QueryDeserializingToObjectAsync<GetMemberResult>(getInfoMsg, CommonStuff.ContractAddress).Result;

                var name = string.IsNullOrEmpty(memberData.Name) ? address : memberData.Name;
                voters.Add((name: name, address: address, shares: (int)memberData.MemberShare));
            }

            var getProposalInfoHandler = ethClient.Eth.GetContractQueryHandler<GetProposalInfoMessage>();
            var proposalInfomsg = new GetProposalInfoMessage() { Idx = votingIndex };
            var votingInfo = getProposalInfoHandler.QueryDeserializingToObjectAsync<GetProposalInfoResult>(proposalInfomsg, CommonStuff.ContractAddress).Result;
            var overallResult = !votingInfo.ProposalRejected;

            var getProposalInfoByAddrHandler = ethClient.Eth.GetContractQueryHandler<GetProposalInfoByAddressMessage>();

            var votersWithResults = new List<(string name, string address, int shares, bool accepts)>();
            foreach (var v in voters)
            {
                var msg = new GetProposalInfoByAddressMessage
                {
                    Index = votingIndex,
                    Address = v.Item2
                };

                var queryResult = getProposalInfoByAddrHandler.QueryDeserializingToObjectAsync<GetProposalInfoByAddressResult>(msg, CommonStuff.ContractAddress).Result;

                votersWithResults.Add((v.name, v.address, v.shares, queryResult.SupportsProposal));
            }

            using (var pdf = new MemoryStream(BuildPdf(votingInfo.Description, votersWithResults, votingIndex, (int)totalShares, overallResult)))
            {
                var msg = botClient.SendDocumentAsync(new Telegram.Bot.Types.ChatId(-1001477965035),
                    new Telegram.Bot.Types.InputFiles.InputOnlineFile(pdf, "protocol.pdf"),
                    "Protocol",
                    Telegram.Bot.Types.Enums.ParseMode.Default).Result;
            }
        }

        private byte[] BuildPdf(string subject, IEnumerable<(string name, string address, int shares, bool accepts)> voters, int voteIndex, int totalShares, bool overallResult)
        {
            var memoryStream = new MemoryStream();

            var document = new Document(PageSize.A4, 100, 100, 140, 100);

            var pdfWriter = PdfWriter.GetInstance(document, memoryStream);
            pdfWriter.CompressionLevel = PdfStream.DEFAULT_COMPRESSION;
            pdfWriter.RgbTransparencyBlending = true;
            pdfWriter.SetPageSize(PageSize.A4);

            document.AddCreationDate();

            var calibriPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\Calibri.ttf";
            var baseFont = BaseFont.CreateFont(calibriPath, "Cp1251", BaseFont.EMBEDDED);
            var head = new Font(baseFont, 14f, Font.NORMAL, BaseColor.BLACK);
            var normal = new Font(baseFont, 10f, Font.NORMAL, BaseColor.BLACK);

            document.Open();

            var para1 = new Paragraph("Protocol of Voting #" + voteIndex.ToString(), head);
            para1.Alignment = Element.ALIGN_CENTER;
            document.Add(para1);
            document.Add(new Paragraph("\r\n\r\n"));

            var para5 = new Paragraph("Participants:\r\n", head);
            para5.Alignment = Element.ALIGN_LEFT;
            document.Add(para5);

            var voterIdx = 1;
            foreach (var item in voters)
            {
                document.Add(new Paragraph($"{voterIdx++}. {item.name} - {item.shares * 100f / totalShares}% shares", normal));
            }

            document.Add(new Paragraph("\r\n\r\n"));

            var para6 = new Paragraph("Subject:\r\n", head);
            para6.Alignment = Element.ALIGN_LEFT;
            document.Add(para6);

            document.Add(new Paragraph("1.  Bla Bla", normal));
            document.Add(new Paragraph("2.  Foo Bar", normal));

            document.Add(new Paragraph("\r\n\r\n"));

            var para7 = new Paragraph("Results:", head);
            para7.Alignment = Element.ALIGN_LEFT;
            document.Add(para7);

            foreach(var itm in voters)
            {
                var outcome = itm.accepts ? "For" : "Against";
                var sharesPcnt = itm.shares * 100f / totalShares;
                var para7_5 = new Paragraph($"{itm.name} - {outcome} ({sharesPcnt:F2}% shares)", normal);
                document.Add(para7_5);
            }

            var para8 = new Paragraph(overallResult ? "Accepted" : "Rejected", normal);
            para8.Alignment = Element.ALIGN_LEFT;
            document.Add(para8);

            var totalYes = voters.Where(x => x.accepts).Sum(x => x.shares);
            var totalNo = voters.Where(x => x.accepts == false).Sum(x => x.shares);
            document.Add(new Paragraph($"{totalYes*100f/totalShares:F2}% - For; {totalNo * 100f / totalShares:F2}% - Against", normal));

            document.Close();

            return memoryStream.ToArray();
        }

        public Task StartWorkerThread()
        {
            Task previous = Task.Delay(500);
            Action<Task> continuation = null;

            continuation = t =>
            {
                previous = previous.ContinueWith(t1 => CheckEvents())
                                    .Unwrap()
                                    .ContinueWith(t2 => Task.Delay(1000))
                                    .Unwrap()
                                    .ContinueWith(t3 => previous.ContinueWith(continuation))
                                    ;
            };

            previous.ContinueWith(continuation);

            return previous;
        }

        private async Task CheckEvents()
        {
            switch (PollState)
            {
                case "-": PollState = "\\"; break;
                case "\\": PollState = "|"; break;
                case "|": PollState = "/"; break;
                case "/": PollState = "-"; break;
            }

            var voteEndedEventHandler = ethClient.Eth.GetEvent<VoteEndedEvent>(CommonStuff.ContractAddress);
            var filterAllTransferEventsForContract = voteEndedEventHandler.CreateFilterInput();
            var allTransferEventsForContract = await voteEndedEventHandler.GetAllChanges(filterAllTransferEventsForContract);

            var newEvents = allTransferEventsForContract.Where(x => this.seenTxHashes.Contains(x.Log.TransactionHash) == false)
                                                                .Select(x => new { x.Event.Idx, x.Log.TransactionHash })
                                                                .ToList();

            foreach (var evt in newEvents)
            {
                try
                {
                    GenerateReport((int)evt.Idx);
                }
                catch (Exception ex)
                {
                    throw;
                }

                this.seenTxHashes.Add(evt.TransactionHash);
            }
        }
    }
}
