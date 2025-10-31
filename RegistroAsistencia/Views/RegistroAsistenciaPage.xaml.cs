using RegistroAsistencia.ViewModels;

namespace RegistroAsistencia.Views;

public partial class RegistroAsistenciaPage : ContentPage
{
	public RegistroAsistenciaPage()
	{
		InitializeComponent();
        //BindingContext = new RegistroAsistenciaViewModel();
    }

    private async void OnPanelAdminClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("AdminPanelPage");
    }
}