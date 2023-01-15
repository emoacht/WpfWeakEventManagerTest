using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfWeakEventManagerTest;

public partial class ItemViewModel : ObservableObject
{
	public string? Name
	{
		get => _name;
		set
		{
			if (_name != value)
			{
				Trace.WriteLine($"Changed: {value}");
				SetProperty(ref _name, value);
			}
		}
	}
	private string? _name;
}