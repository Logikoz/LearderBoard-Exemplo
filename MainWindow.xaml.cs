using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LearderBoard
{
    public partial class MainWindow : Window
    {
        private List<UsuariosModel> usuarios;
        public MainWindow() => Iniciar();
        private void Iniciar()
        {
            InitializeComponent();
            DeserialzarDadosAsync();
        }
        private async void DeserialzarDadosAsync()
        {
            //preparando arquivo para deserializaçao.
            using StreamReader arq = File.OpenText($@"{AppDomain.CurrentDomain.BaseDirectory}\users.json");
            //lendo todo o arquivo json e fazendo a deserializaçao do mesmo.
            usuarios = JsonConvert.DeserializeObject<List<UsuariosModel>>(await arq.ReadToEndAsync());
            //colocando os dados dos usuarios em ordem crecente da reputaçao.
            usuarios = usuarios.OrderByDescending(a => a.Reputacao).ToList();
        }
        private void MostrarDados(ushort tamanhoLista)
        {
            //verificando se o tamanho a ser mostrado é maior que a quantidade de usuarios disponiveis.
            if (tamanhoLista >= usuarios.Count)
            {
                _ = MessageBox.Show("Nao ha como listar essa quantidade.");
                return;
            }
            //Adicionando as linhas na leaderboard.
            for (int i = 0; i < tamanhoLista; i++)
                _ = ListaUsuarios_sp.Children.Add(NovaLinha(usuarios[i]));
        }
        private StackPanel NovaLinha(UsuariosModel user)
        {
            //pegando a posicao do usuario na lista.
            int posicaoUsuario = usuarios.IndexOf(user);
            StackPanel linha = new StackPanel() { Orientation = Orientation.Horizontal };
            //Adicionando a posicao do usuario.
            _ = linha.Children.Add(new TextBlock
            {
                Text = (posicaoUsuario + 1).ToString(),
                ToolTip = user.UsuarioID,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(3),
                Foreground =
                //verificando se ele está nas 3 primeiras posiçoes para ficar com uma corzinha diferenciada.
                posicaoUsuario switch
                {
                    0 => Brushes.Green,
                    1 => Brushes.Yellow,
                    2 => Brushes.Red,
                    _ => Brushes.Black
                }
            });
            //adicionando nome do usuario.
            _ = linha.Children.Add(new TextBlock
            {
                Text = user.Nome,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(3)
            });
            //adicionando a parte que mostra o numero de reputaçoes do usuario.
            _ = linha.Children.Add(new TextBlock
            {
                Text = user.Reputacao.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(3)
            });
            //retornando a linha pronta.
            return linha;
        }
        //mostrarndo 10 usuarios
        private void Button_Click(object sender, RoutedEventArgs e) => MostrarDados(10);
        //Mostrando 30 usuarios
        private void Button_Click_1(object sender, RoutedEventArgs e) => MostrarDados(30);
        //mostrando 100 usuarios
        private void Button_Click_2(object sender, RoutedEventArgs e) => MostrarDados(100);
        //suporte a arrastar...
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }
        //fecharndo janela
        private void Button_Click_3(object sender, RoutedEventArgs e) => Close();
    }

    internal class UsuariosModel
    {
        public Guid? UsuarioID { get; set; }
        public string Nome { get; set; }
        public ushort Reputacao { get; set; }
    }
}
