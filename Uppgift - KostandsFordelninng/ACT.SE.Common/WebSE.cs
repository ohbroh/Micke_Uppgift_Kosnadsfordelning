// .NET
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace ACT.SE.Common
{
    /// <summary>Snygg text</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        internal class WebSE
        {
            /// <summary>InternetAccess contains methods for checking if a web page or web service is alive. https can be tricky</summary>
            internal class InternetAccess
            {
                #region internal static bool IsReachable(string webPage)
                [Obsolete("Do not use this method to check if a webservice is alive. Use WebService.IsAlive(...) instead")]
                internal static bool IsReachable(string webPage)
                {
                    try
                    {
                        // Create a request for the URL.         
                        WebRequest request = WebRequest.Create(webPage);

                        // If required by the server, set the credentials.
                        //request.Credentials = CredentialCache.DefaultCredentials;

                        // Get the response.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            //// Get the stream containing content returned by the server.
                            //Stream dataStream = response.GetResponseStream();
                            //// Open the stream using a StreamReader for easy access.
                            //StreamReader reader = new StreamReader(dataStream);

                            //// Read the content.
                            //string responseFromServer = reader.ReadToEnd();

                            //// Cleanup the streams and the response.
                            //reader.Close();

                            //dataStream.Close();

                            //response.Close();
                        }

                        return true;
                    }
                    catch
                    {
                        return InternetAccess.TryPing(webPage);
                    }
                }
                #endregion

                #region private static bool TryPing(string webPage)
                private static bool TryPing(string webPage)
                {
                    Ping pingSender = new Ping();
                    PingOptions options = new PingOptions();

                    // Use the default Ttl value which is 128, 
                    // but change the fragmentation behavior.
                    options.DontFragment = true;

                    // Create a buffer of 32 bytes of data to be transmitted. 
                    string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    int timeout = 120;

                    try
                    {
                        PingReply reply = pingSender.Send(webPage, timeout, buffer, options);

                        if (reply.Status == IPStatus.Success)
                        {
                            Console.WriteLine("Address: {0}", reply.Address.ToString());
                            Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                            Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                            Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                            Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);

                            return true;
                        }
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                        return false;
                    }
                }
                #endregion
            }

            /// <summary>WebService contains methods for checking if a service is alive, not to be confused with reachable which is not the same thing.</summary>
            internal static class WebService
            {
                /// <summary>The returned object from the IsAlive call.</summary>
                internal class AliveCheckReturn
                {
                    internal AliveCheckReturn(TimeSpan ts)
                    {
                        this.UsedTime = ts;
                        this.Ok = true;
                        this.LastErrorMessage = string.Empty; // Just to be on the safe side
                    }

                    internal AliveCheckReturn(TimeSpan ts, int i, string lastErrorMessage)
                    {
                        this.UsedTime = ts;
                        this.NumberOfTries = i;
                        this.LastErrorMessage = lastErrorMessage;
                        this.Ok = false;
                    }

                    internal TimeSpan UsedTime { get; private set; }
                    internal bool Ok { get; private set; }
                    internal string LastErrorMessage { get; private set; }
                    internal int NumberOfTries { get; private set; }
                }

                /// <summary>
                /// Check that the service of interest is up and running, e.g. responds to the call to an About method
                /// <example>
                /// // To use the IsAlive call you need a number of things. In this example we use the TimeSheetService
                /// 
                /// TimesheetV201205SoapClient client = new TimesheetV201205SoapClient(...); // You should supply an EndPointAddress object and a relevant Binding object.
                /// 
                /// // Usage
                /// WebService.AliveCheckReturn acr = WebService.IsAlive(client);
                /// 
                /// if (!acr.Ok)
                /// {
                ///     CurrentContext.Message.Display("The webservice did not repond after " + acr.NumberOfTries.ToString() + " nof tries, last error message was: " + acr.LastErrorMessage + ". Time used: " + acr.UsedTime.ToString());
                /// }
                /// else
                /// {
                ///     // Do the stuff with the wevservice you intended.
                /// }
                /// </example>
                /// </summary>
                /// <param name="webServiceClient">The webservice object</param>
                /// <param name="testMethod">The method to call, use a method with no arguments. Default is About (which is present on all Agresso webservices)</param>
                /// <param name="numberOfTries">The number of tries this call will make in case no reply from the webservice, after the set number and still no reply it will consider the webservice sleeping. This number cannot be lower than 2 and larger than 50. Default is 15</param>
                /// <param name="sleepInMilliseconds">The number of milliseconds between tries if there is no answer from the webservice, this number cannot be smaller than 10 and larger than 60000. Default is 250</param>
                /// <returns>A WebService.AliveCheckReturn object with properties set accordingly.</returns>
                internal static AliveCheckReturn IsAlive(object webServiceClient, string testMethod = "About", int numberOfTries = 15, int sleepInMilliseconds = 250, bool appendInnerException = false)
                {
                    if (sleepInMilliseconds < 10)
                        sleepInMilliseconds = 250;
                    else if (sleepInMilliseconds > 60000)
                        sleepInMilliseconds = 60000;

                    if (numberOfTries < 2)
                        numberOfTries = 15;
                    else if (numberOfTries > 50)
                        numberOfTries = 50;

                    AliveCheckReturn ret = null;

                    TimeSpan ts = new TimeSpan();

                    DateTime start = DateTime.Now;

                    string lastErrorMessage = string.Empty;

                    int i;

                    for (i = 0; i < numberOfTries; i++)
                    {
                        try
                        {
                            object s = webServiceClient.GetType().InvokeMember(testMethod, System.Reflection.BindingFlags.InvokeMethod, null, webServiceClient, null); // The actual call.

                            ts = DateTime.Now - start;

                            ret = new AliveCheckReturn(ts);

                            break;
                        }
                        catch (Exception ex)
                        {
                            ts = DateTime.Now - start;

                            lastErrorMessage = ex.Message;

                            if (appendInnerException && ex.InnerException != null)
                                lastErrorMessage += ": " + ex.InnerException.Message;

                            Thread.Sleep(sleepInMilliseconds);
                        }
                    }

                    if (ret == null) // If this happens none of the tries where successful...
                        ret = new AliveCheckReturn(ts, i, lastErrorMessage);

                    return ret;
                }
            }
        }
    }
}
