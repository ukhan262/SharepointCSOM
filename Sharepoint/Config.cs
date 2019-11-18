using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace Sharepoint
{
	class Config
	{
		public static string FilePath
		{
			get
			{
				return ConfigurationManager.AppSettings["FilePath"] + "";
			}
		}

		public static string SPURL
		{
			get
			{
				return ConfigurationManager.AppSettings["SPURL"] + "";
			}
		}

		public static string DBConnString
		{
			get
			{
				return ConfigurationManager.AppSettings["DBConnString"] + "";
			}
		}
		public static string LocalDirectory
		{
			get
			{
				return ConfigurationManager.AppSettings["LocalDirectory"] + "";
			}
		}
		public static string LocalLogDirectory
		{
			get
			{
				return ConfigurationManager.AppSettings["LocalLogDirectory"] + "";
			}
		}
		public static DateTime StartTime
		{
			get
			{
				return DateTime.ParseExact(ConfigurationManager.AppSettings["StartTime"] + "", "HH:mm", CultureInfo.InvariantCulture);

			}
		}
		public static DateTime EndTime
		{
			get
			{
				return DateTime.ParseExact(ConfigurationManager.AppSettings["EndTime"] + "", "HH:mm", CultureInfo.InvariantCulture);

			}
		}
		public static string LogFile
		{
			get
			{
				return ConfigurationManager.AppSettings["LogFile"] + "";
			}
		}
		public static string SharedDrive
		{
			get
			{
				return ConfigurationManager.AppSettings["SharedDrive"] + "";
			}
		}

		public static string MailTo
		{
			get
			{
				return ConfigurationManager.AppSettings["MailTo"];
			}
		}
		public static string MailServer
		{
			get
			{
				return ConfigurationManager.AppSettings["MailServer"];
			}
		}
		public static string MailSubject
		{
			get
			{
				return ConfigurationManager.AppSettings["MailSubject"];
			}
		}
		public static string MailFrom
		{
			get
			{
				return ConfigurationManager.AppSettings["MailFrom"];
			}
		}
		public static string MailPassword
		{
			get
			{
				return ConfigurationManager.AppSettings["MailPassword"];
			}
		}
		public static string TraceLevel
		{
			get
			{
				return ConfigurationManager.AppSettings["TraceLevel"];
			}
		}
	}
}
