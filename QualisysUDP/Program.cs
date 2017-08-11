using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QTMRealTimeSDK;
using QTMRealTimeSDK.Data;
using QTMRealTimeSDK.Settings;
using System.Net.Sockets;
using System.Threading;

namespace QualisysUDP
{
    class Program
    {
        // UDP settings
        private static string udpToIP = "192.168.1.107";
        private static int udpToPort = 64853;

        // QTM settings
        private static ushort qtmDiscoveryPort = 4547;
        private static string qtmName = "QUALISYS147";

        static void Main(string[] args)
        {
            Console.WriteLine("Establishing connection...");
            RTProtocol rtProtocol = EstablishConnection(qtmName);

            if (rtProtocol.IsConnected())
            {
                Console.WriteLine("Streaming 6DOF (Euler) data to port {0} of {1}...", udpToPort, udpToIP);
                Task.Run(() =>
                {
                    while (true)
                    {
                        rtProtocol.StreamFrames(
                            StreamRate.RateFrequency,
                            20,
                            ComponentType.Component6dEuler,
                            udpToPort,
                            udpToIP);
                    }
                });
            }
            else
            {
                Console.WriteLine("Cannot continue without QTM connection and bodies.");
            }

            Console.WriteLine("Press ESC to quit.");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                return;
            }
        }

        private static RTProtocol EstablishConnection(string qtmName)
        {
            RTProtocol rtProtocol = new RTProtocol();

            Console.WriteLine("Discovering QTM servers on the network...");
            // We know the name of the computer we want to connect to, find out its IP
            DiscoveryResponse qtmToConnect = new DiscoveryResponse();
            if (rtProtocol.DiscoverRTServers(qtmDiscoveryPort))
            {
                var discoveryResponses = rtProtocol.DiscoveryResponses;
                if (discoveryResponses.Count != 0)
                {

                    foreach (var discoveryResponse in discoveryResponses)
                    {
                        Console.WriteLine(
                            "Discovered {0,16} {1,16} {2,16} {3,3} cameras",
                            discoveryResponse.HostName,
                            discoveryResponse.IpAddress,
                            discoveryResponse.InfoText,
                            discoveryResponse.CameraCount
                        );
                        if (discoveryResponse.HostName == qtmName)
                        {
                            qtmToConnect = discoveryResponse;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No QTM servers found on available networks!");
                }
            }

            // Only connect if the desired computer name is found
            if (qtmToConnect.HostName == qtmName)
            {
                // Try max. 5 times, then admit failure
                for (int i = 0; i < 5; i++)
                {
                    if (rtProtocol.IsConnected())
                    {
                        break;
                    }
                    if (!rtProtocol.Connect(qtmToConnect.IpAddress))
                    {
                        Console.WriteLine("Trying to connect...");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Console.WriteLine(
                            "Connected to {0} @ {1}!",
                            qtmToConnect.HostName,
                            qtmToConnect.IpAddress
                        );
                    }
                }
            }
            else
            {
                Console.WriteLine("The desired QTM server ({0}) was not found!", qtmName);
            }

            // Get settings if connected to QTM, otherwise alert user and continue
            if (rtProtocol.IsConnected())
            {
                // 6DOF settings
                Console.WriteLine("Getting 6DOF settings...");
                // Try max. 5 times, then admit failure
                for (int i = 0; i < 5; i++)
                {
                    if (rtProtocol.Get6dSettings())
                    {
                        Settings6D settings6D = rtProtocol.Settings6DOF;
                        int bodyCount = settings6D.BodyCount;
                        Console.WriteLine("{0} 6DOF bodies found.", bodyCount);
                        List<Settings6DOF> qtmBodies = settings6D.Bodies;
                        foreach (Settings6DOF body in qtmBodies)
                        {
                            Console.WriteLine("\t Found 6DOF body: {0}", body.Name);
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Failed to get 6DOF settings!");
                    }
                }

            }
            else
            {
                Console.WriteLine("Could not communicate with QTM!");
            }

            // Disconnect on exit
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => OnProcessExit(sender, e, rtProtocol);

            return rtProtocol;
        }

        private static void OnProcessExit(object sender, EventArgs e, RTProtocol rtProtocol)
        {
            if (rtProtocol != null)
            {
                if (rtProtocol.IsConnected())
                {
                    rtProtocol.StreamFramesStop();
                    rtProtocol.Disconnect();
                }
            }

        }
    }
}
