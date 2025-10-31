using RegistroAsistencia.Views;

namespace RegistroAsistencia
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("AdminPanelPage", typeof(AdminPanelPage));
        }

        
    }
}
