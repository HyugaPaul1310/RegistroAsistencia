namespace RegistroAsistencia
{
    public partial class App : Application
    {
        public static string ConnectionString { get; } =
            "Server=localhost;Port=3306;Database=registrodeasistencias;User=root;Password=;";

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

    }
}