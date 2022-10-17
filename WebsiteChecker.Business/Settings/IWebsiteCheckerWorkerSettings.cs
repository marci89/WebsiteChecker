using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteChecker.Business.Settings
{
	/// <summary>
	///  IWebsiteCheckerWorkerSettings interface
	/// </summary>
	public interface IWebsiteCheckerWorkerSettings
	{
		/// <summary>
		/// worker IsEnabled
		/// </summary>
		bool IsEnabled { get; }

		/// <summary>
		/// time interval for backgroundservice
		/// </summary>
		int TimeIntervalInMiliSeconds { get; }

		/// <summary>
		/// response time limit
		/// </summary>
		int TimeLimitInMiliSeconds { get; }
	}
}
