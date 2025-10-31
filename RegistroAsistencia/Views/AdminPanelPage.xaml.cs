using RegistroAsistencia.ViewModels;

namespace RegistroAsistencia.Views;

public partial class AdminPanelPage : ContentPage
{
	public AdminPanelPage()
	{
		InitializeComponent();
    }

    private async void OnGoToRegistroAsistenciaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegistroAsistenciaPage");
    }
}