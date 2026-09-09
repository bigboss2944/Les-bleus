using Maui_FilRouge.Views;

namespace Maui_FilRouge;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));
		Routing.RegisterRoute(nameof(CatalogPickerPage), typeof(CatalogPickerPage));
	}
}
