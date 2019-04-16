/* 
 Portions copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.


 Optimizer code copyright (c) 2007, OV Trading, all rights reserved.

 Here is an optimization based on the coefficient of determination r squared.  
 The coeffient of determination is calculated based on the trade number vs. cumalative trade profit.  
 This optimization will maximize the linearity of the equity curve.
 
 Giving credit where credit is due:  
 Code was based on, and portions copied, from the "SQN.cs" optimization by Peter in public forum. 

 ele RSquared: eleven
 Revision 0.1 - 11/6/2011, eleven
 
 Revision 0.11 - 11/8/2011, eleven
 Removed unnecessary code

 Revision 0.12 - 11/10/2011, eleven
 Fixed error in calculation
*/

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
	/// System Quality Number
	/// </summary>
	[Gui.Design.DisplayName("SUPERPROM")]
	public class SUPERPROM : OptimizationType
	{
		public static double sLastRSq;
		public static int sMinTrades = 0;
		
		/// <summary>
		/// Return the performance value of a backtesting result.
		/// </summary>
		/// <param name="systemPerformance"></param>
		/// <returns></returns>
		public override double GetPerformanceValue(SystemPerformance systemPerformance)
		{
			// Allow override of minimum number of trades to return a value
			// Parameter is RSqMinTrades
			int minTrades = 80;
			if (sMinTrades > 0)
			{
				minTrades = sMinTrades;
			}
			else
			{
				int n;
				for (n = 0; n < Strategy.Parameters.Count; n++)
				{
					if ("RSqMinTrades".CompareTo(Strategy.Parameters[n].Name) == 0)
					{
						minTrades = (int)Strategy.Parameters[n].Value;
						break;
					}
				}
			}
		
			double numTrades = systemPerformance.AllTrades.Count;
			
			if (numTrades < minTrades)
				return 0;
			
			// This calc comes from NT standard net profit opt type
			double avgProfit = (systemPerformance.AllTrades.TradesPerformance.GrossProfit +
				systemPerformance.AllTrades.TradesPerformance.GrossLoss) / numTrades;

			double x; double y; double sumOfX = 0; double sumOfY =0; double sumCodeviates = 0; 
			double sumOfXSq = 0; double sumOfYSq = 0;
			int i; double value = 0; double yCalc;
			
			foreach (Trade t in systemPerformance.AllTrades)
			{
				x = (t.TradeNumber + 1);
				yCalc = (t.ProfitPoints * t.Quantity * t.Entry.Instrument.MasterInstrument.PointValue);

                for (i = (t.TradeNumber + 1); i <= x; i++)
				{
					value += yCalc;
				}

				y = value;
				sumCodeviates+= (x * y);
				sumOfX += x;
				sumOfY += y;
				sumOfXSq = sumOfXSq + (x * x);
				sumOfYSq = sumOfYSq + (y * y);
				
//				Strategy.Print("  Trade#: " + x.ToString("N2") + "  tradeProf: " + yCalc.ToString("N2") 
//				+ "  SumX: " + sumOfX.ToString("N2") + "  SumY: " + sumOfY.ToString("N2") 
//				+ "  SumXY: " + sumCodeviates.ToString("N2") + "  SumXSq: " + sumOfXSq.ToString("N2")
//				+ "  SumYSq: " + sumOfYSq.ToString("N2"));
			}
			
			double RNumerator = (numTrades * sumCodeviates) - (sumOfX * sumOfY);
			double RDenom = (numTrades * sumOfXSq - (Math.Pow(sumOfX, 2)))
				* (numTrades * sumOfYSq - (Math.Pow(sumOfY, 2)));
			double dblR = RNumerator / Math.Sqrt(RDenom);

			double r_sqr = 0;
			if (dblR > 0)
			{
			r_sqr = Math.Pow(dblR, 2);
			}
			else
			{
			r_sqr = -1;
			}

			sLastRSq = r_sqr;
			return r_sqr * r_sqr * r_sqr * ((systemPerformance.AllTrades.TradesPerformance.GrossProfit + systemPerformance.AllTrades.TradesPerformance.GrossLoss - systemPerformance.AllTrades.TradesPerformance.MaxConsecWinner * systemPerformance.AllTrades.TradesPerformance.Currency.AvgProfit - systemPerformance.AllTrades.TradesPerformance.Currency.LargestWinner - Math.Sqrt(systemPerformance.AllTrades.TradesCount) * systemPerformance.AllTrades.TradesPerformance.Currency.AvgProfit) / (Math.Sqrt(Math.Max(1,systemPerformance.AllTrades.TradesPerformance.MaxTime2Recover.Days)) * (Math.Pow(-1 * Math.Min(systemPerformance.AllTrades.TradesPerformance.Currency.DrawDown,Strategy.Variable3),1.0)/1.0 + Strategy.Variable2)));
		}
	}

}
