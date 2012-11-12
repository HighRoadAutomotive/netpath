#pragma once

namespace WCFArchitect
{
	namespace Toolkit
	{
		namespace WinRT
		{
			
			public ref class DependencyObjectEx : Windows::UI::Xaml::DependencyObject
			{
			internal:
				DependencyObjectEx();

			public:
				Object^ GetValueThreaded(Windows::UI::Xaml::DependencyProperty^ dp);
				void SetValueThreaded(Windows::UI::Xaml::DependencyProperty^ dp, Object^ value);
			};
		}
	}
}