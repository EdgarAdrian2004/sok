using System.Configuration;
using System.Data;
using System.Windows;

namespace SOK.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
// Nota: se califica System.Windows.Application porque dentro del espacio de
// nombres SOK.* el nombre "Application" resuelve primero a la capa SOK.Application.
public partial class App : System.Windows.Application
{
}

