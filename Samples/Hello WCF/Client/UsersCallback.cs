using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace WCFArchitect.Samples.HelloWCF
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true, IncludeExceptionDetailInFaults = true, ValidateMustUnderstand = true, MaxItemsInObjectGraph = Int32.MaxValue)]
	class Callbacks : UsersCallback
	{
		public void UserInfoUpdated(User NewUserInfo)
		{
			Console.WriteLine("The Server has successfully updated the User Info for " + NewUserInfo.FirstName + " " + NewUserInfo.LastName + ".");
		}
	}
}