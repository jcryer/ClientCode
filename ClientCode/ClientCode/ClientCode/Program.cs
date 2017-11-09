using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientCode
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TcpClient _client = new TcpClient ();

			string ipoutput = GetNetworkInfo ().Split('=')[1];

			string[] ip = ipoutput.Split ('.');
			while (true) 
			{
				_client = new TcpClient ();
				if (ip.Length > 0) {	
				for (int i = 1; i < 255; i++) {
					_client = new TcpClient ();

						Console.WriteLine (ip [0] + "." + ip [1] + "." + ip [2] + "." + i);
						if (Connect (_client, (ip [0] + "." + ip [1] + "." + ip [2] + "." + i))) {
							break;
						}
					}
				} else {
					Console.WriteLine ("No IP address");
					break;
				}
				_client.Close ();
				Thread.Sleep (300000);
			}
		}

		public static string GetNetworkInfo()
		{
			string info = "";

			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				if(nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
				{
					foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses)
					{
						if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
								string mac = BitConverter.ToString (nic.GetPhysicalAddress ().GetAddressBytes ()).Replace ('-', ':');

								info += string.Format ("{0}={1}\n", mac, ip.Address);
						}
					}
				}
			}


			return info;
		}

		public static bool Connect(TcpClient _client, string ip) {
			try 
			{
				if (_client.ConnectAsync(ip, 2001).Wait(500)) 
				{
					Stream stm = _client.GetStream ();
					Console.WriteLine ("Connecting to " + ip);

					ASCIIEncoding asen = new ASCIIEncoding ();
					byte[] ba = asen.GetBytes (GetNetworkInfo ());

					stm.Write (ba, 0, ba.Length);
					byte[] bb = new byte[100];
					int k = stm.Read (bb, 0, 100);

					for (int i=0; i<k; i++)
					{
						Console.Write (Convert.ToChar (bb [i]));

					}
					return true;
				}
				else 
				{
					return false;
				}
			} 
			catch (Exception e) 
			{
				return false;

			}
		}
	}
}
