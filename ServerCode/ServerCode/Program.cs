using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace GetIPServer
{
	class MainClass
	{
		public static TcpListener _server;
		public static RootObject info = new RootObject();
		public static string template;
		public static string input;

		public static void Main (string[] args)
		{
			template = File.ReadAllText ("template.txt");
			input = File.ReadAllText ("data.txt");

			info.Objects = new List<Object> ();
			_server = new TcpListener (IPAddress.Any, 2001);
			_server.Start ();

			if (input.Length > 0) {
				info = JsonConvert.DeserializeObject<RootObject> (input);
			}
			while (true)
			{
				Socket client = _server.AcceptSocket();
				Console.WriteLine("Connection accepted.");
																			
				var childSocketThread = new Thread(() =>
				{
					string retVal = "";
					byte[] data = new byte[100];
					int size = client.Receive(data);
					Console.WriteLine("Recieved data: ");
					for (int i = 0; i < size; i++)
						retVal += Convert.ToChar(data[i]);

					Console.WriteLine(retVal);

					Update(retVal);
					client.Close();
				});
				childSocketThread.Start();
			}
		}

		public static bool Update (string retVal) 
		{
			try 
			{
				input = File.ReadAllText ("data.txt");
				if (input.Length > 0) 
				{
					info = JsonConvert.DeserializeObject<RootObject> (input);
				}

				string[] retArray = retVal.Split('=');
				if (info.Objects.Any(x => x.mac == retArray[0]))
				{	
					Object obj = info.Objects.First(x => x.mac == retArray[0]);
					obj.ip = retArray[1];
					obj.time = DateTime.Now.ToString ();
				}
				else  
				{
					info.Objects.Add(new Object(retArray[0], retArray[1].Replace("\n", ""), DateTime.Now.ToString()));
				}
				string output = JsonConvert.SerializeObject(info, Formatting.Indented);
				File.WriteAllText("data.txt", output);

				string response = "";
				if (info.Objects != null) {
					foreach (Object o in info.Objects) 
					{
						DateTime SavedTime = DateTime.Parse(o.time);
						TimeSpan Difference = DateTime.Now.Subtract(SavedTime);
						if (Difference.Minutes < 10) {
						response += "<tr><td>" + o.mac + "</td><td>" + o.ip + "</td><td>" + o.name + "</td><td>" + Difference.Minutes  + "  mins" + "</td></tr>";
						}
					}
				}
				else {
					response += "</tr>";
				}

				string html = template.Replace("<DATA>", response);
				File.WriteAllText ("index.html", html);

				return true;
			}
			catch (Exception e) 
			{
				Console.WriteLine (e.Message);
				Console.WriteLine (e.StackTrace);

				return false;
			}
		}
	}

	public class RootObject 
	{
		public List<Object> Objects {get; set; }
	}

	public class Object 
	{
		public string mac {get; set; }
		public string ip {get; set; }
		public string name {get; set;}
		public string time { get; set;}

		public Object (string mac, string ip, string time, string name = "unknown") 
		{
			this.mac = mac;
			this.ip = ip;
			this.time = time;
			this.name = name;
		}
	}
}