
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using WebsiteChecker.Worker;

namespace WebsiteChecker.Business.Settings
{
	/// <summary>
	/// WebsiteCheckerWorkerSettings class
	/// </summary>
	public class WebsiteCheckerWorkerSettings : IWebsiteCheckerWorkerSettings
	{
		public WebsiteCheckerWorkerSettings(IConfiguration configuration)
		{
			IsEnabled = configuration.GetSection(nameof(WebsiteCheckerWorker)).GetValue(nameof(IsEnabled), false);
			TimeIntervalInMiliSeconds = configuration.GetSection(nameof(WebsiteCheckerWorker)).GetValue(nameof(TimeIntervalInMiliSeconds), 1000);
			TimeLimitInMiliSeconds = configuration.GetSection(nameof(WebsiteCheckerWorker)).GetValue(nameof(TimeLimitInMiliSeconds), 1000);
		}

		/// <summary>
		/// worker IsEnabled
		/// </summary>
		public bool IsEnabled { get; private set; }

		/// <summary>
		/// time interval for backgroundservice
		/// </summary>
		public int TimeIntervalInMiliSeconds { get; private set; }

		/// <summary>
		/// TimeIntervalInSeconds
		/// </summary>
		public int TimeLimitInMiliSeconds { get; private set; }
	}
}
