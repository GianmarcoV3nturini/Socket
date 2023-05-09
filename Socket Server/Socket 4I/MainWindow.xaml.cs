using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Socket_4I
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //server
    {

        Socket socket = null;
        DispatcherTimer dTimer = null;
        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // creazione della socket

            IPAddress local_address = IPAddress.Any; //creo il mio indirizzo locale per la comunicazione
            IPEndPoint local_endpoint = new IPEndPoint(local_address, 11000);//creazione del punto di ascolto

            socket.Bind(local_endpoint); //associo la socket al punto di ascolto

            socket.Blocking = false; //permetto alla socket di inviare tutto, compresi messaggi broadcast
            socket.EnableBroadcast = true;//abilito la chat broadcast

            //creo e avvio il timer per ricevere i messaggi
            dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(aggiornamento_dTimer);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dTimer.Start();

        }

        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int nBytes = 0;

            if ((nBytes = socket.Available) > 0) //se ho ricevuto dei caratteri eseguo
            {
                byte[] buffer = new byte[nBytes]; //imposto il buffer al numero di caratteri ricevuti
                EndPoint remoreEndPoint = new IPEndPoint(IPAddress.Any, 0); //salvo la provenienza
                nBytes = socket.ReceiveFrom(buffer, ref remoreEndPoint); //metto i caratteri nel buffer

                string from = ((IPEndPoint)remoreEndPoint).Address.ToString(); //metto la provenienza in una stringa
                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes); //converto il messaggio
                lstMessaggi.Items.Add(from + ": " + messaggio);

            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            IPAddress remote_address = IPAddress.Parse("255.255.255.255");//utilizzo l'indirizzo broadcast per inviare il messaggio, a tutti i client
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 12000); //imposto l'end point al quale inviare il messaggio (destinazione)
            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text); //converto il messaggio

            socket.SendTo(messaggio, remote_endpoint);//invio il messaggio
        }
    }
}
