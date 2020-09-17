using System;
using System.Diagnostics;
using System.Text;

namespace Volo.Abp
{
	public class CommonHelper
    {
		/// <summary>
		/// 计时器开始
		/// </summary>
		/// <returns></returns>
		public static Stopwatch TimerStart()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Reset();
			stopwatch.Start();
			return stopwatch;
		}

		/// <summary>
		/// 计时器结束
		/// </summary>
		/// <param name="watch"></param>
		/// <returns></returns>
		public static string TimerEnd(Stopwatch watch)
		{
			watch.Stop();
			return ((double)watch.ElapsedMilliseconds).ToString();
		}

		/// <summary>
		/// 生成0-9随机数
		/// </summary>
		/// <param name="codeNum">生成长度</param>
		/// <returns></returns>
		public static string RndNum(int codeNum)
		{
			StringBuilder stringBuilder = new StringBuilder(codeNum);
			Random random = new Random();
			for (int i = 1; i < codeNum + 1; i++)
			{
				int num = random.Next(9);
				stringBuilder.AppendFormat("{0}", num);
			}
			return stringBuilder.ToString();
		}
	}
}
