// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	/// <summary>
	/// </summary>
	[Gui.Design.DisplayName("PROM")]
	public class PROM : OptimizationType
	{
		/// <summary>
		/// Return the performance value of a backtesting result.
		/// </summary>
		/// <param name="systemPerformance"></param>
		/// <returns></returns>
		public override double GetPerformanceValue(SystemPerformance systemPerformance)
		{
			return (systemPerformance.AllTrades.TradesPerformance.GrossProfit + systemPerformance.AllTrades.TradesPerformance.GrossLoss - systemPerformance.AllTrades.TradesPerformance.MaxConsecWinner * systemPerformance.AllTrades.TradesPerformance.Currency.AvgProfit - systemPerformance.AllTrades.TradesPerformance.Currency.LargestWinner - Math.Sqrt(systemPerformance.AllTrades.TradesCount) * systemPerformance.AllTrades.TradesPerformance.Currency.AvgProfit) / (Math.Pow(-1 * Math.Min(systemPerformance.AllTrades.TradesPerformance.Currency.DrawDown,Strategy.Variable3),1.0)/1.0 + Strategy.Variable2);
		}
	}
}
