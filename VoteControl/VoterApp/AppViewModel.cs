using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vote.Commons;
using VoterApp.Commands;
using VoterApp.EthMessages;

namespace VoterApp
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public List<Founder> Founders { get; } = new List<Founder>();
        public BindingList<VotingEntry> CurrentVotings { get; } = new BindingList<VotingEntry>();

        public Boolean HideFinished { get; set; }

        private Dictionary<string, BitmapImage> avatars = new Dictionary<string, BitmapImage>()
        {
            { "Alice", new BitmapImage(new Uri(@"pack://application:,,,/Alice.png"))},
            { "Bob", new BitmapImage(new Uri(@"pack://application:,,,/Bob.png"))},
            { "Carol", new BitmapImage(new Uri(@"pack://application:,,,/Carol.png"))},
        };

        public BitmapImage Avatar
        {
            get
            {
                if (selectedFounder != null && avatars.Keys.Contains(selectedFounder.Name))
                {
                    return avatars[selectedFounder.Name];
                }
                return null;
            }
        }

        #region SelectedFounder
        private Founder selectedFounder;
        public Founder SelectedFounder
        {
            get => selectedFounder;
            set
            {
                if (selectedFounder != value)
                {
                    selectedFounder = value;
                    OnFounderChanged();
                    OnPropertyChanged(nameof(SelectedFounder));
                    OnPropertyChanged(nameof(Avatar));
                }
            }
        }
        #endregion

        #region NotifyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region SelectedVoting
        private VotingEntry selectedVoting;
        public VotingEntry SelectedVoting
        {
            get => selectedVoting;
            set
            {
                if (selectedVoting != value)
                {
                    this.selectedVoting = value;
                    OnPropertyChanged(nameof(SelectedVoting));
                    OnPropertyChanged(nameof(ShowVotingDetails));
                }
            }
        }
        #endregion

        #region VoteCommand
        private ICommand voteCommand;
        public ICommand VoteCommand
        {
            get
            {
                if (voteCommand == null)
                    voteCommand = new VoteAppCommand(VoteHandler);

                return voteCommand;
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
                    refreshVotings = new VoteAppCommand(RefreshVotingsHandler);

                return refreshVotings;
            }
        }
        #endregion

        #region UpdateInProgress
        private bool updateInProgress;
        public bool UpdateInProgress
        {
            get => updateInProgress;
            set
            {
                if (updateInProgress != value)
                {
                    updateInProgress = value;
                    OnPropertyChanged(nameof(UpdateInProgress));
                }
            }
        } 
        #endregion

        //public VoteOption SelectedVoteOption { get; set; }

        public bool ShowVotingDetails => SelectedVoting != null;

        private Nethereum.Web3.Web3 ethClient;

        private volatile int threadsLoading = 0;

        public AppViewModel()
        {
            this.Founders.Add(new Founder
            {
                Name = "Alice",
                PublicKey = "9E9de8735ACc15eDcd7aB629E5D8eb6FC39144dD",
                PrivateKey = "b929edff20d2647cb762f4f9021e5675e41bb3abacdba5b84dc3939a14508dae"
            });

            this.Founders.Add(new Founder
            {
                Name = "Bob",
                PublicKey = "c7433EA82E5f970e6B56d46aEb9189757869b9f8",
                PrivateKey = "39d6f515eb415b3c41200676cdd1e458caadaa7111d3cb979269a16a6f224c0c"
            });

            this.Founders.Add(new Founder
            {
                Name = "Carol",
                PublicKey = "40B7C305130C5f8EaFf7DF71a4F045d5010D3B4A",
                PrivateKey = "84b3b35aec2013445026801654155390f60898c016711fd1f5c90c09739d5c1c"
            });
        }

        private void OnFounderChanged()
        {
            if (SelectedFounder == null)
                return;

            var privateKey = Enumerable.Range(0, SelectedFounder.PrivateKey.Length / 2).Select(x => Convert.ToByte(SelectedFounder.PrivateKey.Substring(x * 2, 2), 16)).ToArray();
            ethClient = new Nethereum.Web3.Web3(new Nethereum.Web3.Accounts.Account(privateKey), "https://kovan.infura.io/v3/9b3e6ee424c647a09069c69770048b58");

            Task.Run(LoadCurrentVotings);
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

            UpdateInProgress = true;

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
                    newVotingEntry.Id = idx;
                    newVotingEntry.Name = votingInfo.Description;
                    newVotingEntry.EndsAt = new DateTime((long)votingInfo.EndDate);
                    newVotingEntry.IsVoted = votingInfo.IsVoted;
                    newVotingEntry.FounderVote = votingInfo.IsVoted ? (votingInfo.SupportsProposal ? VoteOption.Yes : VoteOption.No) : VoteOption.NotSet;
                    newVotingEntry.Status = votingInfo.IsFinished ? VotingStatus.Completed : (newVotingEntry.EndsAt >= DateTime.Now ? VotingStatus.Running : VotingStatus.Aborted);

                    if (newVotingEntry.Status == VotingStatus.Running || !HideFinished)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(delegate
                                {
                                    this.CurrentVotings.Add(newVotingEntry);
                                    OnPropertyChanged(nameof(CurrentVotings));
                                }
                       ));
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateInProgress = false;
                Interlocked.CompareExchange(ref threadsLoading, 0, 1);
            }
        }

        private void VoteHandler()
        {
            var contract = ethClient.Eth.GetContract(CommonStuff.ContractAbi, CommonStuff.ContractAddress);
            var voteHandler = ethClient.Eth.GetContractTransactionHandler<VoteMessage>();

            var newVote = new VoteMessage
            {
                Idx = SelectedVoting.Id,
                Yes = SelectedVoting.FounderVote == VoteOption.Yes,
                Comment = "",
                Gas = 3000000
            };

            var transactionReceipt = voteHandler.SendRequestAndWaitForReceiptAsync(CommonStuff.ContractAddress, newVote).Result;

            this.SelectedVoting = null;
        }
    }
}
