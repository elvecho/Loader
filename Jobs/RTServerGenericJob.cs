using Loader.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Loader.Models;
using Loader.Persistence;
using System.Configuration.Internal;

namespace Loader.Jobs
{
	public abstract class RtServerGenericJob : IJob
	{
		protected readonly ILogger<RtServerStatusJob> Logger;
		private readonly IHttpClientFactory _clientFactory;
		protected readonly IOptions<JobsConfig> Config;

		protected RtServerGenericJob(
			ILogger<RtServerStatusJob> logger,
			IOptions<JobsConfig> config,
			IHttpClientFactory clientFactory)
		{
			Logger = logger;
			Config = config;
			_clientFactory = clientFactory;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			Logger.LogInformation("Executing task {class}", GetType());

			try
			{
				RT_ChecksContext dbContext = NewDbContext();
				var listRtServers = await dbContext.RtServer.Include(st => st.RtServerStatus).ToListAsync();
				//var srv = listRtServers.Where(x => x.SzRtServerId.Contains("88S25000770")).ToList();

				Logger.LogDebug($"END: Return list of all RT Servers length:{listRtServers.Count}");
				await CreateServerStatus(listRtServers, dbContext);

				var responses = new List<bool>();

				if (Config.Value.ParallelServersProcessing)
				{
					Logger.LogInformation("Working in parallel on: {servers} servers", listRtServers.Count);
					var tasks = new List<Task<bool>>();
					int counter = 1;
					//ToDelete
					//foreach (var rtServer in SortServers(srv))

					//END ToDelete
					foreach (var rtServer in SortServers(listRtServers))
					{
						if (rtServer.BOnDutyFlag == true)
						{
							tasks.Add(JobPayload(rtServer));
						}
						if (counter++ == 3)
						{
							responses.AddRange(await Task.WhenAll(tasks));
							counter = 1;
							tasks.Clear();
						}
					}
					if (tasks.Count > 0) responses.AddRange(await Task.WhenAll(tasks));
				}
				else
				{
					Logger.LogInformation("Working in serial on: {servers} servers", listRtServers.Count);
					foreach (RtServer rtServer in SortServers(listRtServers))
					{
						if (rtServer.BOnDutyFlag == true)
						{
							responses.Add(await JobPayload(rtServer));
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.LogError("Other error: {e}", e);
			}
		}

		private async Task CreateServerStatus(List<RtServer> listServers, RT_ChecksContext dbContext)
		{
			foreach (var rtServer in listServers)
			{
				rtServer.RtServerStatus ??= new RtServerStatus
				{
					SzRtServerId = rtServer.SzRtServerId,
					LRetailStoreId = rtServer.LRetailStoreId,
					LStoreGroupId = rtServer.LStoreGroupId,
					BOnErrorFlag = false
				};
				try
				{
					dbContext.Update(rtServer);
					await dbContext.SaveChangesAsync();
					Logger.LogDebug("DB info successfully updated for = {rtServer}", rtServer.SzRtServerId);
				}
				catch (Exception e)
				{
					Logger.LogError("Error updating DB info: {e}", e);
				}
			}
		}

		private IEnumerable<RtServer> SortServers(List<RtServer> listServers)
		{
			listServers = listServers.Where(rt => rt.RtServerStatus != null).ToList();

			if (GetType().Name == "RtServerTransactionsJob")
			{
				return listServers.OrderBy(rt => rt.RtServerStatus.DLastDateTimeTransactionsCollected).ToList();
			}
			if (GetType().Name == "RtServerTransmissionsJob")
			{
				return listServers.OrderBy(rt => rt.RtServerStatus.DLastDateTimeTransmissionsCollected).ToList();
			}
			return listServers;
		}

		protected abstract Task<bool> JobPayload(RtServer rtServer);
		/*
				 * Editor:Soukaina
				 * Date: 19/02/2021
				 * Client:Sogegross
				 * Description:
				 * Timeout increased from 60s to 2m
				 *	Add Using (var client=_clientFactory.CreateClient()
				 * 
				 * 
				 */
		/*
		* Editor:Soukaina
		* Date: 19/02/2021
		* Client:Sogegross
		* Description:
		* Timeout increased from 120s to 180s
		*	Add Using (var client=_clientFactory.CreateClient()
		* 
		* 
		*/
		protected async Task<HttpResponseMessage> GenericHttpGet(RtServer rtServer, string endpoint, string queryString)
		{
			var requestUri = new UriBuilder("https://" + rtServer.SzIpAddress + endpoint + queryString).ToString();
			using (var client= _clientFactory.CreateClient("RtServerGeneric"))
			{
				//var client = _clientFactory.CreateClient("RtServerGeneric");
			var encodedUserPassword = Encoding.ASCII.GetBytes(rtServer.SzUsername + ":" + rtServer.SzPassword);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(encodedUserPassword));
			client.Timeout = new TimeSpan(0, 0, Config.Value.HttpRequestTimeOut);

			var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
			return await client.SendAsync(request);
			}
		}
		/*
				 * Editor:Soukaina
				 * Date: 19/02/2021
				 * Client:Sogegross
				 * Description:
				 * Timeout increased from 60s to 2m
				 *	Add Using (var client=_clientFactory.CreateClient()
				 * 
				 * 
				 */
		protected async Task<HttpResponseMessage> GenericHttpPost(RtServer rtServer, string endpoint, string content)
		{

			var requestUri = new UriBuilder("https://" + rtServer.SzIpAddress + endpoint).ToString();
			using (var client = _clientFactory.CreateClient("RtServerGeneric"))
			{

			
			//var client = _clientFactory.CreateClient("RtServerGeneric");
			var encodedUserPassword = Encoding.ASCII.GetBytes(rtServer.SzUsername + ":" + rtServer.SzPassword);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(encodedUserPassword));
				client.Timeout = new TimeSpan(0, 0, Config.Value.HttpRequestTimeOut);

			var httpContent = new StringContent(content, Encoding.UTF8, "application/xml");
			return await client.PostAsync(requestUri, httpContent);
			}
		}

		protected string GetXmlFromGenericRequest<T>(T request)
		{
			var serializer = new XmlSerializer(typeof(T));
			var settings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};

			using var stringWriter = new StringWriter();
			using var writer = XmlWriter.Create(stringWriter, settings);
			try
			{
				serializer.Serialize(writer, request);
				var xml = stringWriter.ToString();
				Logger.LogDebug("XML = {content}", xml);
				return xml;
			}
			catch (Exception e)
			{
				Logger.LogError("Cannot Serialize XML. Error: {e}", e);
				return "";
			}
		}

		protected static RT_ChecksContext NewDbContext()
		{
			IConfigurationRoot configuration = new ConfigurationBuilder()
						.SetBasePath(@"C:\Retex\loader\config")
						.AddJsonFile("appsettings.json")
						.Build();
			var optionsBuilder = new DbContextOptionsBuilder<RT_ChecksContext>();
			optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(600)) ;
			return new RT_ChecksContext(optionsBuilder.Options);
		}
	}
}
