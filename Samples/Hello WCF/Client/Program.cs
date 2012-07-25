using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace WCFArchitect.Samples.HelloWCF
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("WCF Architect Sample Client");
			Console.WriteLine("Press Escape to terminate.");
			Console.WriteLine();
			Console.Write("Connecting to the Server ... ");

			//Connect to the server using the code generated for us by WCF Architect.
			InstanceContext IC = new InstanceContext(new Callbacks());
			UsersClient Service = new UsersClient(IC, new Bindings.NamedPipeBinding(), UsersClient.CreatePipeEndpointEndpoint());
			Service.Open();						//Open the connection
			Guid UID = Service.Connect();		//Call out connect function to initialize the service.

			Console.WriteLine("Connected!");

			User UI = new User();
			Console.Write("Please enter your first name: ");
			UI.FirstName = Console.ReadLine();
			Console.Write("Please enter your last name: ");
			UI.LastName = Console.ReadLine();
			Console.Write("Please enter your address: ");
			UI.Address = Console.ReadLine();
			Console.Write("Please enter your city: ");
			UI.City = Console.ReadLine();
			Console.Write("Please enter your state: ");
			UI.State = Console.ReadLine();
			Console.Write("Please enter your zip code: ");
			UI.Zip = Console.ReadLine();
			Console.WriteLine("Please select your favorite color:");
			Console.WriteLine("\t1. Red");
			Console.WriteLine("\t2. Green");
			Console.WriteLine("\t3. Blue");
			try
			{
				UI.UserColor = (FavoriteColor)System.Enum.Parse(typeof(FavoriteColor), Console.ReadLine());
			}
			catch
			{
				Service.Close();
				Console.WriteLine("Unexpected Input! The Connection has been closed, please restart and try again.");
			}
			Service.SetUserInfo(UI);

			Console.WriteLine("Press the any key to disconnect and exit this Sample.");
			Console.ReadKey();

			Service.Disconnect(UID);		//Disconnect the service
			Service.Close();				//Close the connection

			Console.WriteLine("Service Closed!");

		}
	}
}
