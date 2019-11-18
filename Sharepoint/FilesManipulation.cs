using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System.Net;

namespace Sharepoint
{
	class FilesManipulation
	{
		ClientContext _ctx;
		public void Run()
		{
			try
			{
				_ctx = new ClientContext(Config.SPURL);
				//pass the network credentials (adminusername, password, domain)
				_ctx.Credentials = new NetworkCredential("", "", "");
				Web spWeb = _ctx.Web;
				_ctx.Load(spWeb);
				_ctx.ExecuteQuery();

				
				UpdateUsernameAndBackDateFiles();
			}
			catch (Exception ex)
			{
				WriteError(ex.Message);
			}

		}

		void UpdateUsernameAndBackDateFiles()
		{
			WriteLog("++++++++++++++++++++++++++++++++++Starting BackDating Process+++++++++++++++++++++++++");
			WriteLog("");
			try
			{
				//get title of the folder
				List spList = _ctx.Web.Lists.GetByTitle("");
				_ctx.Load(spList);
				_ctx.ExecuteQuery();
				string[] pds = Config.FilePath.Split(',');
				if (spList != null && spList.ItemCount > 0)
				{
					foreach (string pd in pds)
					{
						CamlQuery camlQuery = new CamlQuery();
						//get the folder url with unique pds?
						camlQuery.FolderServerRelativeUrl = "" + int.Parse(pd) + "";

						ListItemCollection listItems = spList.GetItems(camlQuery);

						//load the items needed to be modified
						_ctx.Load(listItems, item => item.Include(x => x.File, x => x.ContentType, x => x["Archived"], x => x["CalcEAExist"], x => x["Created"], x => x["CreatedById"], x => x["Author"], x => x["Editor"], x => x["FileLeafRef"], x => x.Folder.ServerRelativeUrl));
						_ctx.ExecuteQuery();
						if (listItems.Count > 0)
						{
							WriteLog("Items Found. Starting to process.");
							
							//backdate
							var date = Convert.ToDateTime("01/23/2019").Date;

							//updated username
							var userName = "brank@wedgewood-inc.com";

							ModifyFilesAndFolders(listItems, date, userName);
						}

						WriteLog("Property /Lists/Property Documents/" + int.Parse(pd) + "/ processed.");
						WriteLog("");
					}
				}
				WriteLog("");
				WriteLog("++++++++++++++++++++++++++++++++++Finished BackDating Process+++++++++++++++++++++++++");
			}

			catch (Exception ex)
			{
				throw ex;
			}
		}

		private FieldUserValue GetUsersDetails(User userName)
		{
			var userValue = new FieldUserValue();

			userValue.LookupId = userName.Id;

			return userValue;
		}

		void ModifyFilesAndFolders(ListItemCollection listItems, DateTime timeToSet, string userName)
		{
			foreach (var listItem in listItems)
			{
				//get title of the folder
				List spList = _ctx.Web.Lists.GetByTitle("");
				_ctx.Load(spList);
				spList.EnableVersioning = false;
				spList.Update();
				_ctx.ExecuteQuery();

				var usr = _ctx.Web.EnsureUser(userName);
				_ctx.Load(usr);
				_ctx.ExecuteQuery();

				//checking for folder else check for the files
				if (listItem.ContentType.Name.ToLower() == "folder")
				{
					CamlQuery camlQuery = new CamlQuery();
					camlQuery.FolderServerRelativeUrl = listItem.Folder.ServerRelativeUrl;
					listItems = spList.GetItems(camlQuery);

					//load the items needed to be modified
					_ctx.Load(listItems, item => item.Include(x => x.File, x => x.ContentType, x => x["Archived"], x => x["CalcEAExist"], x => x["Created"], x => x["CreatedById"], x => x["Author"], x => x["Editor"], x => x["FileLeafRef"], x => x.Folder.ServerRelativeUrl));
					_ctx.ExecuteQuery();

					if (listItems.Count > 0)
					{
						ModifyFilesAndFolders(listItems, timeToSet, userName);
						WriteLog("Changing folder, " + listItem.Folder.ServerRelativeUrl + ", Created/Modified to " + timeToSet + ".");
						listItem["Created"] = timeToSet;
						listItem["Modified"] = timeToSet;

						var author = this.GetUsersDetails(usr);

						listItem["Author"] = author;
						listItem["Editor"] = author;

						listItem.Update();

						spList.EnableVersioning = true;
						spList.Update();

						Console.WriteLine("After Update " + listItem["Modified"]);
						_ctx.ExecuteQuery();
						Console.WriteLine("After Modified " + listItem["Modified"]);
						WriteLog("Folder ," + listItem.Folder.ServerRelativeUrl + ", modified.");
					}
				}
				else
				{
					File file = listItem.File;
					FileInformation fileInfo = File.OpenBinaryDirect(_ctx, file.ServerRelativeUrl);

					//add the file name condition
					if (file.Name.ToUpper().Contains(""))
					{
						listItem["Created"] = timeToSet;
						listItem["Modified"] = timeToSet;

						var author = this.GetUsersDetails(usr);
						listItem["Author"] = author;
						listItem["Editor"] = author;

						listItem.Update();

						spList.EnableVersioning = true;
						spList.Update();

						_ctx.ExecuteQuery();

						WriteLog("File ," + listItem.File.ServerRelativeUrl + ", modified.");
					}											
				}
			}
		}

		void WriteLog(string log)
		{
			using (System.IO.StreamWriter sWriter = new System.IO.StreamWriter(@"C:\CustomJob\Log-" + DateTime.Today.ToString("MMddyyyy") + ".txt", true))
			{
				Console.WriteLine(log);
				sWriter.WriteLine(log);
			}
		}

		void WriteError(string log)
		{
			using (System.IO.StreamWriter sWriter = new System.IO.StreamWriter(@"C:\CustomJob\Error-" + DateTime.Today.ToString("MMddyyyy") + ".txt", true))
			{
				Console.WriteLine(log);
				sWriter.WriteLine(log);
			}
		}
	}
}
