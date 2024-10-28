using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Data.SqlClient;

namespace Sovelluskehitys2024
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PaivitaDataGrid(object sender, RoutedEventArgs e)
        {
            string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2200423\\Documents\\testitietokanta.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string kysely = "SELECT * FROM tuotteet";
            SqlCommand komento = yhteys.CreateCommand();
            komento.CommandText = kysely;

            SqlDataAdapter adapteri = new SqlDataAdapter(komento);
            DataTable dt = new DataTable("tuotteet");
            adapteri.Fill(dt);

            tuotelista.ItemsSource = dt.DefaultView;

            yhteys.Close();
        }
        private void PaivitaComboBox(object sender, RoutedEventArgs e)
        {
            string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2200423\\Documents\\testitietokanta.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM tuotteet",yhteys);
            SqlDataReader lukija = komento.ExecuteReader();

            tuotelista_cb.Items.Clear();

            while (lukija.Read())
            {
                tuotelista_cb.Items.Add(lukija.GetString(1));

            }
            lukija.Close();

            yhteys.Close();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PaivitaDataGrid(sender, e);
            PaivitaComboBox(sender, e);

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2200423\\Documents\\testitietokanta.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string kysely = "INSERT INTO tuotteet (nimi, hinta) VALUES ('" + tuotenimi.Text + "'," + tuotehinta.Text + ")";
            SqlCommand komento = new SqlCommand(kysely,yhteys);
            komento.ExecuteNonQuery();

            yhteys.Close();

            PaivitaDataGrid(sender, e);

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}