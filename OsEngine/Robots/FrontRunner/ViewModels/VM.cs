using Com.Lmax.Api;
using System.Windows.Input;
using TradesTSLAB.Commands;
using WPF_MVVM.ViewModels;

namespace OsEngine.Robots.FrontRunner.ViewModels
{
    public class VM:BaseVM
    {
        #region Properties===========================================

        public decimal BigVolume { get => bot.bigVolume; set { bot.bigVolume = value; OnPropertyChanged(nameof(BigVolume)); } }
        public int Offset { get => bot.offset; set { bot.offset = value; OnPropertyChanged(nameof(Offset)); } }
        public int Take { get => bot.take; set { bot.take = value; OnPropertyChanged(nameof(Take)); } }
        public decimal Lot { get => bot.lot; set { bot.lot = value; OnPropertyChanged(nameof(Lot)); } }
        public Edit Edit { get => bot.Edit; set { bot.Edit = value; OnPropertyChanged(nameof(Edit)); } }
        public decimal Volume { get => bot.position.OpenVolume; set { OnPropertyChanged(nameof(Volume)); } }
        public decimal Price { get => bot.position.EntryPrice; set { OnPropertyChanged(nameof(Price)); } }
        public decimal TakePrice { get => bot.takePrice; set { OnPropertyChanged(nameof(TakePrice)); } }

        public ICommand CommandStart
        {
            get
            {
                if (commandStart == null)                
                    commandStart = new DelegateCommand(Start);
                
                return commandStart;
            }
        }

        #endregion

        #region Field================================================

        Models.FrontRunner bot;

        #endregion

        #region Methods==============================================
        
        public VM(Models.FrontRunner bot)
        {
            this.bot = bot;
        }

        private void Start(object obj)
        {
            if (bot.Edit == Edit.Start)
                bot.Edit = Edit.Stop;
            else
                bot.Edit = Edit.Start;
        }
        #endregion

        #region Commands=============================================

        private DelegateCommand commandStart;

        #endregion
    }
    public enum Edit
    {
        Start, Stop
    }
}