using System;
using System.IO;
using System.Net;
using System.Text;
using cfm = System.Configuration.ConfigurationManager;


public enum HttpVerb {
    GET
    , POST
    , PUT
    , DELETE
}

namespace Jira.BO.HttpUtils {
    /// <summary>
    /// Object to call web service REST api.
    /// </summary>
    public class RestClient {
        
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }


        private string _baseurl = cfm.AppSettings["RestURL"];
        public string EndPoint {
            get { return _baseurl; }
            set { _baseurl = value; }
        }


        private string _username = cfm.AppSettings["Username"];
        public string Username {
            get { return _username; }
            set { _username = value; }
        }

        private string _password = cfm.AppSettings["Password"];
        public string Password {
            get { return _password; }
            set { _password = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient"/> class.
        /// </summary>
        public RestClient() {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public RestClient(string endpoint) {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="method">The method.</param>
        public RestClient(string endpoint, HttpVerb method) {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="method">The method.</param>
        /// <param name="postdata">The postdata.</param>
        public RestClient(string endpoint, HttpVerb method, string postdata) {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postdata;
        }


        public RestClient(string endpoint, HttpVerb method, string postdata, string username, string password) {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postdata;
            Username = username;
            Password = password;
        }


        /// <summary>
        /// Makes the request with no parameters.
        /// </summary>
        /// <returns></returns>
        public string MakeRequest() {
            return MakeRequest("");
        }

        /// <summary>
        /// Makes the request with added parameters.
        /// </summary>
        /// <param name="parameters">string Parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public string MakeRequest(string parameters) {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST) {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;
                using (var writeStream = request.GetRequestStream()) {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            string base64Credentials = GetEncodedCredentials();
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            using (var response = (HttpWebResponse)request.GetResponse()) {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK) {
                    var message = string.Format("Request failed.  Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream()) {
                    if (responseStream != null) {
                        using (var reader = new StreamReader(responseStream)) {
                            responseValue = reader.ReadToEnd();
                        }
                    }
                }

                return responseValue;
            }
        }


        private string GetEncodedCredentials() {
            string mergedCredentials = string.Format("{0}:{1}", Username, Password);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }


        public string MakeRequestFromURI(string endPoint, string parameters = null) {
            parameters = parameters ?? "";
            var request = (HttpWebRequest)WebRequest.Create(endPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST) {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;
                using (var writeStream = request.GetRequestStream()) {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            string base64Credentials = GetEncodedCredentials();
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            using (var response = (HttpWebResponse)request.GetResponse()) {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK) {
                    var message = string.Format("Request failed.  Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream()) {
                    if (responseStream != null) {
                        using (var reader = new StreamReader(responseStream)) {
                            responseValue = reader.ReadToEnd();
                        }
                    }
                }

                return responseValue;
            }
        }
    }
}
