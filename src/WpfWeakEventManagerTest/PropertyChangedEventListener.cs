using System;
using System.ComponentModel;
using System.Windows;

namespace WpfWeakEventManagerTest;

public class PropertyChangedEventListener<T> : IWeakEventListener
{
	private readonly Action<T, string?> _action;

	public PropertyChangedEventListener(Action<T, string?> action) => _action = action;

	public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType != typeof(PropertyChangedEventManager))
			return false;

		_action.Invoke((T)sender, ((PropertyChangedEventArgs)e).PropertyName);
		return true;
	}
}