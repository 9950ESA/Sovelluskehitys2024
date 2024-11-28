using System.Data;
using System.Reflection;
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
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Microsoft.Data.SqlClient;

namespace Sovelluskehitys2024
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2200423\\Documents\\testitietokanta.mdf;Integrated Security=True;Connect Timeout=30";
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Kirjastosovellus";
            this.MinHeight = 500;
            this.MinWidth = 800;

            ThemeManager.Current.ChangeTheme(this, "Light.Blue");

            try
            {
                PaivitaDataGrid("SELECT * FROM kirjat","kirjat", kirjalista);
                PaivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakaslista);
                PaivitaDataGrid("SELECT ti.id as id, a.nimi as asiakas, tu.nimi as tuote FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.haettu='0'", "lainat", lainalista);
                PaivitaDataGrid("SELECT ti.id as id, a.nimi as asiakas, tu.nimi as tuote FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.haettu='1'", "lainat", lainattulista);

                PaivitaKirjaComboBox();
                PaivitaAsiakasComboBox();
            }
            catch
            {
                tilaviesti.Text = "Ei tietokantayhteyttä";
            }
            
        }

        private void PaivitaDataGrid(string kysely, string taulu, DataGrid grid)
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = yhteys.CreateCommand();
            komento.CommandText = kysely;

            SqlDataAdapter adapteri = new SqlDataAdapter(komento);
            DataTable dt = new DataTable("kirjat");
            adapteri.Fill(dt);

            grid.ItemsSource = dt.DefaultView;

            yhteys.Close();
        }
        private void PaivitaKirjaComboBox()
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM kirjat",yhteys);
            SqlDataReader lukija = komento.ExecuteReader();

            DataTable taulu = new DataTable();
            taulu.Columns.Add("ID", typeof(string));
            taulu.Columns.Add("NIMI", typeof(string));

            kirjalista_cb.ItemsSource = taulu.DefaultView;
            kirjalista_cb.DisplayMemberPath = "NIMI";
            kirjalista_cb.SelectedValuePath = "ID";


            while (lukija.Read())
            {
                int id = lukija.GetInt32(0);
                string nimi = lukija.GetString(1);
                taulu.Rows.Add(id, nimi);
                //tuotelista_cb.Items.Add(lukija.GetString(1));

            }
            lukija.Close();

            yhteys.Close();
        }
        private void PaivitaAsiakasComboBox()
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM asiakkaat", yhteys);
            SqlDataReader lukija = komento.ExecuteReader();

            DataTable taulu = new DataTable();
            taulu.Columns.Add("ID", typeof(string));
            taulu.Columns.Add("NIMI", typeof(string));

            asiakaslista_cb.ItemsSource = taulu.DefaultView;
            asiakaslista_cb.DisplayMemberPath = "NIMI";
            asiakaslista_cb.SelectedValuePath = "ID";


            while (lukija.Read())
            {
                int id = lukija.GetInt32(0);
                string nimi = lukija.GetString(1);
                taulu.Rows.Add(id, nimi);
                //tuotelista_cb.Items.Add(lukija.GetString(1));

            }
            lukija.Close();

            yhteys.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PaivitaDataGrid("SELECT * FROM kirjat", "kirjat", kirjalista);
            PaivitaKirjaComboBox();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string kysely = "INSERT INTO kirjat (nimi, vuosi, tekija) VALUES ('" + kirjanimi.Text + "'," + kirjavuosi.Text + ")";
            SqlCommand komento = new SqlCommand(kysely,yhteys);
            komento.ExecuteNonQuery();
            yhteys.Close();

            PaivitaDataGrid("SELECT * FROM kirjat", "kirjat", kirjalista);
            PaivitaKirjaComboBox();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string id = kirjalista_cb.SelectedValue.ToString();
            string kysely = "DELETE FROM tuotteet WHERE id='" + id + "';";
            SqlCommand komento = new SqlCommand(kysely, yhteys);
            komento.ExecuteNonQuery();
            yhteys.Close();

            PaivitaDataGrid("SELECT * FROM kirjat", "kirjat", kirjalista);
            PaivitaKirjaComboBox();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string kysely = "INSERT INTO asiakkaat (nimi, osoite, puhelin) VALUES ('" + asiakasnimi.Text + "','" + asiakasosoite.Text + "','" + asiakaspuhelin.Text + "')";
            SqlCommand komento = new SqlCommand(kysely, yhteys);
            komento.ExecuteNonQuery();
            yhteys.Close();

            PaivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakaslista);
            PaivitaAsiakasComboBox();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string asiakasID = asiakaslista_cb.SelectedValue.ToString();
            string tuoteID  = kirjalista_cb.SelectedValue.ToString();

            string sql = "INSERT INTO tilaukset (asiakas_id, tuote_id, toimitettu) VALUES ('" + asiakasID + "','" + tuoteID + "',0)";

            SqlCommand komento = new SqlCommand(sql, yhteys);
            komento.ExecuteNonQuery();
            yhteys.Close();

            PaivitaDataGrid("SELECT ti.id as id, a.nimi as asiakas, tu.nimi as tuote FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.haettu='0'", "lainat", lainalista);

        }

        private void laina_haettu_click(object sender, RoutedEventArgs e)
        {
            DataRowView rivinäkymä = (DataRowView)((Button)e.Source).DataContext;
            String tilaus_id = rivinäkymä[0].ToString();

            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string sql = "UPDATE tilaukset SET toimitettu=1 WHERE id='" + tilaus_id + "';";

            SqlCommand komento = new SqlCommand(sql, yhteys);
            komento.ExecuteNonQuery();

            yhteys.Close();

            PaivitaDataGrid("SELECT ti.id as id, a.nimi as asiakas, tu.nimi as tuote FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.haettu='0'", "lainat", lainalista);
            PaivitaDataGrid("SELECT ti.id as id, a.nimi as asiakas, tu.nimi as tuote FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.haettu='1'", "lainat", lainalista);

        }
        private void laina_palautettu_click(object sender, RoutedEventArgs e)
        {
            DataRowView rivinäkymä = (DataRowView)((Button)e.Source).DataContext;
            String tilaus_id = rivinäkymä[0].ToString();

            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string sql = "UPDATE tilaukset SET toimitettu=1 WHERE id='" + tilaus_id + "';";

            SqlCommand komento = new SqlCommand(sql, yhteys);
            komento.ExecuteNonQuery();

            yhteys.Close();

            PaivitaDataGrid("SELECT ti.id as id, a.nimi as asiakas, tu.nimi as tuote FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.toimitettu='0'", "lainat", lainalista);
            PaivitaDataGrid("SELECT ti.id as id, a.nimi as asiakas, tu.nimi as tuote FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.toimitettu='1'", "lainat", lainalista);

        }
    }
}