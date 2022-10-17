using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteChecker.Business.Models;
using WebsiteChecker.Business.Settings;

namespace WebsiteChecker.Worker
{
	public class WebsiteCheckerWorker : BackgroundService
	{
		private readonly ILogger<WebsiteCheckerWorker> _logger;
		private readonly IWebsiteCheckerWorkerSettings _settings;
		private string Path = Directory.GetCurrentDirectory();
		public WebsiteCheckerWorker(ILogger<WebsiteCheckerWorker> logger, IWebsiteCheckerWorkerSettings settings)
		{
			_logger = logger;
			_settings = settings;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			if (!_settings.IsEnabled)
			{
				_logger.LogInformation($"The WebsiteCheckerWorker is not enabled on the configuration. Please check the {nameof(WebsiteCheckerWorkerSettings)}.{nameof(_settings.IsEnabled)} entry.");
				return;
			}

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

				try
				{
					await Check(stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Error occured during the {nameof(WebsiteCheckerWorkerSettings)} running");
				}

				_logger.LogInformation("Worker running end at: {time}", DateTimeOffset.Now);

				await Task.Delay(_settings.TimeIntervalInMiliSeconds, stoppingToken);
			}
		}

		#region File methods

		/// <summary>
		/// Read websites from file
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task<List<string>> ListWebsites(CancellationToken cancellationToken)
		{
			var rows = await File.ReadAllLinesAsync(Path + "/doc/Websites.csv", Encoding.UTF8, cancellationToken);
			if (rows.Any())
			{
				return rows.ToList();
			}

			return new List<string>();
		}

		/// <summary>
		/// The result will be written to file
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		private async Task WriteResult(CancellationToken cancellationToken, WebsiteDetailModel request)
		{
			using (StreamWriter file = new StreamWriter(Path + "/doc/WebsiteDetails.csv", append: true)){
				await file.WriteLineAsync($"url: {request.URL}; time in miliseconds: {request.time}; status: {request.Status.ToString()}");
			}
		}

		#endregion

		#region Monitoring 

		/// <summary>
		/// Monitoring websites
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task Check(CancellationToken cancellationToken)
		{
			var websites = await ListWebsites(cancellationToken);
			foreach (var site in websites)
			{
				try
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site);

					System.Diagnostics.Stopwatch timer = new Stopwatch();
					timer.Start();

					HttpWebResponse response = (HttpWebResponse)request.GetResponse();

					timer.Stop();

					TimeSpan timeTaken = timer.Elapsed;
					var milliseconds = (int)timeTaken.Milliseconds;

					await WriteResult(cancellationToken, new WebsiteDetailModel
					{
						URL = site,
						time = milliseconds,
						Status = _settings.TimeLimitInMiliSeconds<= milliseconds ?  Status.Error : Status.Ok
				});

					_logger.LogInformation("Site answer time: {timeTaken}", DateTimeOffset.Now);

				} catch (Exception ex)
				{
					_logger.LogError(ex.ToString(), DateTimeOffset.Now);
					await WriteResult(cancellationToken, new WebsiteDetailModel
					{
						URL = site,
						time = 0,
						Status = Status.Error
					});
				}
			}	
		}

		#endregion
	}
}
