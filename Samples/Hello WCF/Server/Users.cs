using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFArchitect.Samples.HelloWCF
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true, ValidateMustUnderstand = true, MaxItemsInObjectGraph = 6553600, Namespace = "http://tempuri.org/WCFArchitect/Samples/HelloWCF/")]
	class Users : IUsers
	{
		private User Info;

		private Guid UserID;
		private IUsersCallback Callback;

		public User UserInfo
		{
			get { return Info; }
		}

		public void SetUserInfo(User UserInfo)
		{
			Info = UserInfo;

			Console.WriteLine("User Info for '" + Info.FirstName + " " + Info.LastName + "' has been successfully updated.");

			//The Concurrency mode is single so we have to send the callback request on a separate thread or the service will deadlock.
			Task T = new Task(new Action(() => Callback.UserInfoUpdated(UserInfo)), TaskCreationOptions.None);
			T.Start();
		}

		public Guid Connect()
		{
			Callback = OperationContext.Current.GetCallbackChannel<IUsersCallback>();

			return UserID = Guid.NewGuid();
		}

		public void Disconnect(Guid UserID)
		{
			if (UserID == this.UserID)
				return;
		}
	}
}