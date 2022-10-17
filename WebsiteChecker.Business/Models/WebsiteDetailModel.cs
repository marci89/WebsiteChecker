using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteChecker.Business.Models
{
	/// <summary>
	/// monitoring answer data model
	/// </summary>
	public class WebsiteDetailModel
	{
		/// <summary>
		/// Website url
		/// </summary>
		public string URL { get; set; } = string.Empty;

		/// <summary>
		/// answer time
		/// </summary>
		public int time { get; set; }

		/// <summary>
		/// status
		/// </summary>
		public Status Status { get; set; }
	}
}
