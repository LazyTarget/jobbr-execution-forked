﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Jobbr.Runtime.ForkedExecution.RestClient
{
    /// <summary>
    /// The jobbr run time client.
    /// </summary>
    public class ForkedExecutionRestClient : IDisposable
    {
        private readonly long jobRunId;

        private HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForkedExecutionRestClient"/> class.
        /// </summary>
        /// <param name="jobServer">
        /// The server url.
        /// </param>
        /// <param name="jobRunId"></param>
        public ForkedExecutionRestClient(string jobServer, long jobRunId)
        {
            this.jobRunId = jobRunId;

            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(jobServer + (jobServer.EndsWith("/") ? string.Empty : "/") + "fex/");
        }

        public bool PublishState(JobRunState state)
        {
            var url = $"jobRun/{this.jobRunId}";
            var content = new JobRunUpdateDto { State = state.ToString() };

            var serializeObject = JsonConvert.SerializeObject(content);

            var request = this.httpClient.PutAsync(url, new StringContent(serializeObject, Encoding.UTF8, "application/json"));
            var result = request.Result;

            return result.StatusCode == HttpStatusCode.Accepted;
        }

        public bool SendFiles(string[] files)
        {
            var multipartContent = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                multipartContent.Add(new StreamContent(File.OpenRead(file)), "result", fileName);
            }

            var url = $"jobRun/{this.jobRunId}/artefacts";
            var response = this.httpClient.PostAsync(url, multipartContent).Result;
        
            return response.StatusCode == HttpStatusCode.Accepted;
        }

        public JobRunInfoDto GetJobRunInfo()
        {
            var url = $"jobRun/{this.jobRunId}";

            var request = this.httpClient.GetAsync(url);
            var result = request.Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var content = result.Content.ReadAsStringAsync().Result;

                var dto = JsonConvert.DeserializeObject<JobRunInfoDto>(content);

                return dto;
            }

            return null;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (this.httpClient != null)
                {
                    this.httpClient.Dispose();
                    this.httpClient = null;
                }
            }
        }
    }
}
