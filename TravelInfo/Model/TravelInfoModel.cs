using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Web.Http.Filters;
using Newtonsoft.Json;
using PlatzInfo.Helpers;
using TravelInfo.NRServiceReference;

namespace TravelInfo.Model
{
    public class TravelInfoModel
    {
        private bool timersrunning;

        public List<CallbackDetail> CallbackDetails;

        public delegate void ChangedDelegate(CallbackDetail cbdetail);
        public event ChangedDelegate Changed;

        public void StartTimers()
        {
            this.timersrunning = false;

            if (this.CallbackDetails == null)
            {
                this.CallbackDetails = new List<CallbackDetail>();

                CallbackDetail cb = new CallbackDetail()
                {
                    ResultType = eResultType.NR,
                    StopPointId = "SYD",
                    DueTime = 0,
                    Period = 60000
                };

                this.CallbackDetails.Add(cb);
                cb = new CallbackDetail()
                {
                    ResultType = eResultType.TFL,
                    StopPointId = "490009750P",
                    DueTime = 0,
                    Period = 10000
                };

                this.CallbackDetails.Add(cb);
                cb = new CallbackDetail()
                {
                    ResultType = eResultType.TFL,
                    StopPointId = "940GZZLUCWR",
                    DueTime = 0,
                    Period = 30000
                };

                this.CallbackDetails.Add(cb);
                cb = new CallbackDetail()
                {
                    ResultType = eResultType.TFL,
                    StopPointId = "940GZZLULNB",
                    DueTime = 0,
                    Period = 30000
                };

                this.CallbackDetails.Add(cb);
                cb = new CallbackDetail()
                {
                    ResultType = eResultType.TFL,
                    StopPointId = "940GZZLUGPK",
                    DueTime = 1000,
                    Period = 30000
                };

                this.CallbackDetails.Add(cb);
                cb = new CallbackDetail()
                {
                    ResultType = eResultType.TFLStatus,
                    StopPointId = "northern,jubilee,victoria",
                    DueTime = 1000,
                    Period = 30000
                };

                this.CallbackDetails.Add(cb);

                foreach (CallbackDetail callback in this.CallbackDetails)
                {
                    if (callback.ResultType == eResultType.NR)
                    {
                        Timer newTimer = new Timer(this.NrCallback, callback, callback.DueTime, callback.Period);
                        callback.CallbackTimer = newTimer;
                    }
                    else if (callback.ResultType == eResultType.TFLStatus)
                    {
                        Timer newTimer = new Timer(this.TflStatusCallback, callback, callback.DueTime, callback.Period);
                        callback.CallbackTimer = newTimer;
                    }
                    else
                    {
                        Timer newTimer = new Timer(this.TflCallback, callback, callback.DueTime, callback.Period);
                        callback.CallbackTimer = newTimer;
                    }
                }
            }
            else
            {
                this.RestartTimers();
            }

            this.timersrunning = true;
        }

        public void RestartTimers()
        {
            foreach (CallbackDetail callback in this.CallbackDetails)
            {
                callback.CallbackTimer.Change(callback.DueTime, callback.Period);
            }

            this.timersrunning = true;
        }

        public void KillTimers()
        {
            this.timersrunning = false;
            foreach (CallbackDetail callback in this.CallbackDetails)
            {
                callback.CallbackTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async void TflStatusCallback(object state)
        {
            try
            {
                CallbackDetail callbackdetail = state as CallbackDetail;
                if (callbackdetail != null)
                {
                    if (Interlocked.Exchange(ref callbackdetail.Running, 1) == 0)
                    {
                        callbackdetail.TflStatuses.Clear();

                        if (this.timersrunning)
                        {
                            callbackdetail.TflStatuses = await this.GetStatusFromTfl(callbackdetail.StopPointId);
                            if (callbackdetail.TflStatuses.Any())
                            {
                                callbackdetail.Timestamp = DateTime.Now;
                            }

                            this.Changed?.Invoke(callbackdetail);
                        }

                        Interlocked.Decrement(ref callbackdetail.Running);
                    }
                }
            }
            catch (Exception)
            {
                Debug.Write("TflStatusCallback exception");
            }
        }

        private async void TflCallback(object state)
        {
            try
            {
                CallbackDetail callbackdetail = state as CallbackDetail;
                if (callbackdetail != null)
                {
                    if (Interlocked.Exchange(ref callbackdetail.Running, 1) == 0)
                    {
                        callbackdetail.TflPredictions.Clear();
                        if (this.timersrunning)
                        {
                            callbackdetail.TflPredictions =
                                await this.GetDestinationBoardsFromTfl(callbackdetail.StopPointId);
                            if (callbackdetail.TflPredictions.Any())
                            {
                                callbackdetail.Timestamp = callbackdetail.TflPredictions[0].Timestamp;
                            }

                            this.Changed?.Invoke(callbackdetail);
                        }

                        Interlocked.Decrement(ref callbackdetail.Running);
                    }
                }
            }
            catch (Exception)
            {
                Debug.Write("TflCallback exception");
            }
        }

        private async void NrCallback(object state)
        {
            CallbackDetail callbackdetail = state as CallbackDetail;
            if (callbackdetail != null)
            {
                if (Interlocked.Exchange(ref callbackdetail.Running, 1) == 0)
                {
                    try
                    {
                        callbackdetail.NrPredictions.Clear();

                        if (this.timersrunning)
                        {
                            GetDepartureBoardResponse response = await this.GetTrainsFromNationalRail(callbackdetail.StopPointId);
                            StationBoard stationBoard = response.GetStationBoardResult;

                            callbackdetail.Timestamp = stationBoard.generatedAt;
                            foreach (ServiceItem serviceItem in stationBoard.trainServices)
                            {
                                NrPrediction train = new NrPrediction(serviceItem);
                                callbackdetail.NrPredictions.Add(train);
                            }

                            this.Changed?.Invoke(callbackdetail);
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("NrCallback threw exception");
                    }
                    finally
                    {
                        Interlocked.Exchange(ref callbackdetail.Running, 0);
                    }
                }
            }
        }

        private Task<GetDepartureBoardResponse> GetTrainsFromNationalRail(string stoppoint)
        {
            Debug.WriteLine("GetTrainsFromNationalRail");
            LDBServiceSoapClient client = new LDBServiceSoapClient();

            AccessToken accessToken = new AccessToken();
            accessToken.TokenValue = AppConfiguration.NrToken;

            using (new OperationContextScope(client.InnerChannel))
            {

                MessageHeader messageHeader = MessageHeader.CreateHeader("AccessToken", "http://thalesgroup.com/RTTI/2013-11-28/Token/types", accessToken);

                OperationContext.Current.OutgoingMessageHeaders.Add(messageHeader);

                return client.GetDepartureBoardAsync(100, stoppoint, "", FilterType.to, 0, 128);
            }
        }

        private async Task<List<TflLine>> GetStatusFromTfl(string stoppoints)
        {
            Debug.WriteLine("GetStatusFromTfl");
            List<TflLine> tflstatus = new List<TflLine>();
            //Create an HTTP client object
            HttpBaseProtocolFilter RootFilter = new HttpBaseProtocolFilter();


            RootFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;
            RootFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;

            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient(RootFilter);

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri(string.Format("https://api.tfl.gov.uk/Line/{0}/Status/?detail=False&app_id={1}&app_key={2}", stoppoints, AppConfiguration.TflAppId, AppConfiguration.TflAppKey));

            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                tflstatus = JsonConvert.DeserializeObject<List<TflLine>>(httpResponseBody);
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            return tflstatus;
        }


        private async Task<List<TflPrediction>> GetDestinationBoardsFromTfl(string stoppoint)
        {
            Debug.WriteLine("GetDestinationBoardsFromTfl");
            List<TflPrediction> tflpredictions = new List<TflPrediction>();

            HttpBaseProtocolFilter RootFilter = new HttpBaseProtocolFilter();
            RootFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;
            RootFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;
            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient(RootFilter);

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri(string.Format("https://api.tfl.gov.uk/StopPoint/{0}/Arrivals?app_id={1}&app_key={2}", stoppoint, AppConfiguration.TflAppId, AppConfiguration.TflAppKey));

            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                tflpredictions = JsonConvert.DeserializeObject<List<TflPrediction>>(httpResponseBody);
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            return tflpredictions;
        }
    }
}