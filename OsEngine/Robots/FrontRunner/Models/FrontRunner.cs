using OsEngine.Entity;
using OsEngine.OsOptimizer;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.ViewModels;
using OsEngine.Robots.FrontRunner.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.FrontRunner.Models
{
    public class FrontRunner : BotPanel
    {
        #region Properties===================================================================

        public Edit Edit
        {
            get => edit;
            set
            {
                edit = value;
                if (edit == Edit.Stop && position != null && position.State == PositionStateType.Opening)                
                    tab.CloseAllOrderInSystem();               
            }
        }

        #endregion

        #region Fileds=======================================================================

        public decimal bigVolume = 20000;
        public int offset = 1;
        public int take = 5;
        public decimal lot = 2;
        public Position position = null;
        public decimal takePrice;

        private BotTabSimple tab;
        private Edit edit = Edit.Stop;
        private StrategyParameterString editString;

        #endregion

        #region Methods======================================================================

        public FrontRunner(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);
            tab = TabsSimple[0];
            //editString = CreateParameter(nameof(Edit), ViewModels.Edit.Stop.ToString(), new string[2] { ViewModels.Edit.Stop.ToString(), ViewModels.Edit.Start.ToString() });
            //edit

            tab.MarketDepthUpdateEvent += Tab_MarketDepthUpdateEvent;
            tab.PositionOpeningFailEvent += Tab_PositionOpeningFailEvent;
        }

        private void Tab_PositionOpeningFailEvent(Position position)
        {
            position = null;
        }

        private void Tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            if (Edit == Edit.Stop)
                return;

            if (marketDepth.SecurityNameCode != tab.Securiti.Name)
                return;

            var positions = tab.PositionsOpenAll;
            //var positionsLong = tab.PositionsOpenAll.Where(p => p.Direction == Side.Buy).ToList();
            //var positionsShort = tab.PositionsOpenAll.Where(p => p.Direction == Side.Sell).ToList();

            if (positions != null && positions.Count > 0)
            {
                foreach (var position in positions)
                {
                    if (position.Direction == Side.Sell)
                    {
                        takePrice = position.EntryPrice - take * tab.Securiti.PriceStep;
                        tab.CloseAtProfit(position, takePrice, takePrice);
                    }
                    else if (position.Direction == Side.Buy)
                    {
                        takePrice = position.EntryPrice + take * tab.Securiti.PriceStep;
                        tab.CloseAtProfit(position, takePrice, takePrice);
                    }
                }
            }            

            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= bigVolume && position == null)
                {
                    var price = marketDepth.Asks[i].Price - offset * tab.Securiti.PriceStep;
                    position = tab.SellAtLimit(lot, price);
                    if (position.State != PositionStateType.Open && position.State != PositionStateType.Opening)
                        position = null;
                }
                if ((position != null && marketDepth.Asks[i].Price == position.EntryPrice +
                    offset * tab.Securiti.PriceStep && marketDepth.Asks[i].Ask < bigVolume / 2)
                    || (marketDepth.Asks[0].Price > position.EntryPrice + offset * tab.Securiti.PriceStep))
                {
                    if (position.State == PositionStateType.Open)
                        tab.CloseAtMarket(position, position.OpenVolume);
                    else if (position.State == PositionStateType.Opening)
                        tab.CloseAllOrderInSystem();
                }
                else if (position != null && position.State == PositionStateType.Opening && marketDepth.Asks[i].Ask >= bigVolume &&
                    marketDepth.Asks[i].Price < position.EntryPrice + offset * tab.Securiti.PriceStep)
                {
                    tab.CloseAllOrderInSystem();
                    position = null;
                    break;
                }
            }

            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                if (marketDepth.Bids[i].Bid >= bigVolume && position == null)
                {
                    var price = marketDepth.Bids[i].Price + offset * tab.Securiti.PriceStep;                    
                    position = tab.BuyAtLimit(lot, price);
                    if (position.State != PositionStateType.Open && position.State != PositionStateType.Opening)
                        position = null;
                }
                if ((position != null && marketDepth.Bids[i].Price == position.EntryPrice -
                    offset * tab.Securiti.PriceStep && marketDepth.Bids[i].Bid < bigVolume / 2)
                    || (marketDepth.Bids[0].Price < position.EntryPrice - offset * tab.Securiti.PriceStep))
                {
                    if (position.State == PositionStateType.Open)
                        tab.CloseAtMarket(position, position.OpenVolume);
                    else if (position.State == PositionStateType.Opening)
                        tab.CloseAllOrderInSystem();
                }
                else if (position!=null && position.State == PositionStateType.Opening && marketDepth.Bids[i].Bid >= bigVolume && 
                    marketDepth.Bids[i].Price > position.EntryPrice - offset * tab.Securiti.PriceStep)
                {
                    tab.CloseAllOrderInSystem();
                    position = null;
                    break;
                }
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(FrontRunner);
        }

        public override void ShowIndividualSettingsDialog()
        {
            try
            {
                var window = new FrontRunnerUi(this);
                window.Show();
            }
            catch
            {
               
            }
        }

        #endregion
    }
}