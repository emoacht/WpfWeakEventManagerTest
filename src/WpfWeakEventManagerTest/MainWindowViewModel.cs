using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfWeakEventManagerTest;

public partial class MainWindowViewModel : ObservableObject
{
	public ObservableCollection<ItemViewModel> Items { get; } = new();
	private readonly object _lock = new();

	public MainWindowViewModel()
	{
		BindingOperations.EnableCollectionSynchronization(Items, _lock);
	}

	private readonly PropertyChangedEventListener<ItemViewModel> _listener = new(OnReceived);

	private static void OnReceived(ItemViewModel item, string? propertyName)
	{
		switch (propertyName)
		{
			case nameof(ItemViewModel.Name):
				Trace.WriteLine($"Received: {item.Name}");
				break;
		}
	}

	private void Add(ItemViewModel item)
	{
		lock (_lock)
		{
			Items.Add(item);
			PropertyChangedEventManager.AddListener(item, _listener, nameof(ItemViewModel.Name));
		}
	}

	private void Remove(ItemViewModel item)
	{
		lock (_lock)
		{
			Items.Remove(item);
			PropertyChangedEventManager.RemoveListener(item, _listener, nameof(ItemViewModel.Name));
		}
	}

	[RelayCommand]
	public async Task TestSingleThreadAsync()
	{
		Trace.WriteLine("-- Add A & B --");
		var itemA = new ItemViewModel { Name = "A 0" };
		var itemB = new ItemViewModel { Name = "B 0" };
		Add(itemA);
		Add(itemB);

		await Task.Delay(TimeSpan.FromSeconds(1));

		Trace.WriteLine("-- Change names --");
		itemA.Name = "A 1";
		itemB.Name = "B 1";

		await Task.Delay(TimeSpan.FromSeconds(1));

		Trace.WriteLine("-- Remove A and then change names --");
		Remove(itemA);
		itemA.Name = "A 2";
		itemB.Name = "B 2";

		await Task.Delay(TimeSpan.FromSeconds(1));

		Trace.WriteLine("-- Remove B and then change names --");
		Remove(itemB);
		itemA.Name = "A 3";
		itemB.Name = "B 3";
	}

	[RelayCommand]
	public async Task TestMultiThreadAsync()
	{
		Trace.WriteLine("-- Add A & B --");
		var itemA = new ItemViewModel { Name = "A 0" };
		var itemB = new ItemViewModel { Name = "B 0" };
		await Task.Run(() => Add(itemA));
		await Task.Run(() => Add(itemB));

		await Task.Delay(TimeSpan.FromSeconds(1));

		Trace.WriteLine("-- Change names --");
		itemA.Name = "A 1";
		itemB.Name = "B 1";

		await Task.Delay(TimeSpan.FromSeconds(1));

		Trace.WriteLine("-- Remove A and then change names --");
		await Task.Run(() => Remove(itemA));
		itemA.Name = "A 2";
		itemB.Name = "B 2";

		await Task.Delay(TimeSpan.FromSeconds(1));

		Trace.WriteLine("-- Remove B and then change names --");
		await Task.Run(() => Remove(itemB));
		itemA.Name = "A 3";
		itemB.Name = "B 3";
	}
}