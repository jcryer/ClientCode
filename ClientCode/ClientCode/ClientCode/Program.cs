using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace ClientCode
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine (GetNetworkInfo());

			try {
				TcpClient _client = new TcpClient();
				Console.WriteLine("Connecting.....");

				_client.Connect("10.48.220.37",2001);

				Console.WriteLine("Connected");

				Stream stm = _client.GetStream();

				ASCIIEncoding asen= new ASCIIEncoding();
				byte[] ba=asen.GetBytes(GetNetworkInfo());
				Console.WriteLine("Transmitting.....");

				stm.Write(ba,0,ba.Length);

				byte[] bb=new byte[100];
				int k=stm.Read(bb,0,100);

				for (int i=0;i<k;i++)
					Console.Write(Convert.ToChar(bb[i]));

				_client.Close();
			}

			catch (Exception e) {
				Console.WriteLine("Error..... " + e.StackTrace);
				Console.WriteLine (e.Message);
				Console.ReadLine ();
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
	}
}
