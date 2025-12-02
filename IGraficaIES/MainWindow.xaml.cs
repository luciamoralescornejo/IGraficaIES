using _2HerenciaSimpleIES;
using Microsoft.Win32;
using System.IO;
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
using static IGraficaIES.ClaseWPFAuxiliar;

namespace IGraficaIES
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Fuente y Tamaño de la letra
        public FontFamily fuente = new FontFamily("Arial");
        public double tamaño = 14;
        // Lista para los controles
        public List<Control> controlesGridCentral;
        public List<Control> controlesGridBotones;
        // Listas para profesores
        private List<ProfesorFuncionario> profesores = new List<ProfesorFuncionario>();
        private List<ProfesorExtendido> profesoresEx = ProfesorExtendido.CrearListaProfesores();
        // Indice del profesor que se muestra
        private int profActual = 0;
        public EstadoAPP estado = EstadoAPP.SinCargar;



        public MainWindow()
        {
            InitializeComponent();
            // Deshabilito las herramientas hasta que se introduzca un archivo
            Deshabilitar([gridCent, menuFiltros, menuAgrupacion, gridBtn]);
            // Deshabilito los botones excepto el boton añadir
            btnAnadir.On();
            // Uso este metodo para crearme una lista con todos los controles del grid central y grid de los botones
            controlesGridCentral = ObtenerControles(gridCent);
            controlesGridBotones = ObtenerControles(gridBtn);
            // Modifico la familia de la fuente y el tamaño a mi gusto
            controlesGridCentral.ForEach(x => { x.FontFamily = fuente; x.FontSize = tamaño; });

            // Añado los valores del ComboBox
            for (int i = 22; i <= 70; i++)
            {
                cmbEdad.Items.Add(i);
            }
            // Añado los valores del ListBox
            lsbSegMedico.Items.Add("S.Social");
            lsbSegMedico.Items.Add("Muface");

            // Cargo todas las imagenes de la interfaz exceptuando la de la foto de los profesores
            CargarImagen(ref imgPrimero, "first_page.png");
            CargarImagen(ref imgSiguiente, "arrow_forward.png");
            CargarImagen(ref imgAnadir, "person_add.png");
            CargarImagen(ref imgModificar, "person_edit.png");
            CargarImagen(ref imgEliminar, "person_remove.png");
            CargarImagen(ref imgGuardar, "save.png");
            CargarImagen(ref imgCancelar, "cancel.png");
            CargarImagen(ref imgAnterior, "arrow_back.png");
            CargarImagen(ref imgUltimo, "last_page.png");
            CargarImagen(ref menuAbrir, "file_open.png");
            CargarImagen(ref menuSalir, "exit.png");
            CargarImagen(ref menuNegrita, "format_bold.png");
            CargarImagen(ref menuCursiva, "format_italic.png");
            CargarImagen(ref menuEstatura, "straighten.png");
            CargarImagen(ref menuMedico, "local.png");

        }

        public void Abrir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "txt files (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                profesores.Clear();

                foreach (var line in File.ReadLines(openFileDialog.FileName))
                {
                    string[] split = line.Split(';');

                    if (split.Length != 10)
                    {
                        MessageBox.Show("Línea incorrecta:\n" + line);
                        continue;
                    }

                    TipoFuncionario tipoProfesor =
                        split[5] == "De Carrera" ? TipoFuncionario.DeCarrera :
                        split[5] == "En Practicas" ? TipoFuncionario.EnPracticas :
                        TipoFuncionario.Interino;

                    TipoMed tipoMedico =
                        split[8] == "SS" ? TipoMed.SeguridadSocial :
                        TipoMed.Muface;

                    bool definitivo = bool.Parse(split[7]);

                    profesores.Add(new ProfesorFuncionario(
                        split[0], split[1], int.Parse(split[2]),
                        split[4], tipoProfesor,
                        int.Parse(split[6]), definitivo,
                        tipoMedico, split[9]
                    ));
                }

                if (profesores.Count > 0)
                {
                    profActual = 0;
                    estado = EstadoAPP.Listar;
                    Habilitar([menuFiltros, menuAgrupacion, gridBtn]);
                    RellenarDatos(profesores[0], this);
                }
            }
        }



        public void Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // Uso la lista de controles que me cree anteriormente para CAMBIAR la letra NEGRITA y CURSIVA
        public void Negrita_Checked(object sender, RoutedEventArgs e)
        {
            controlesGridCentral.ForEach(x => x.FontWeight = FontWeights.Bold);
        }
        public void Negrita_Unchecked(object sender, RoutedEventArgs e)
        {
            controlesGridCentral.ForEach(x => x.FontWeight = FontWeights.Normal);
        }
        public void Cursiva_Checked(object sender, RoutedEventArgs e)
        {
            controlesGridCentral.ForEach(x => x.FontStyle = FontStyles.Italic);
        }
        public void Cursiva_Unchecked(object sender, RoutedEventArgs e)
        {
            controlesGridCentral.ForEach(x => x.FontStyle = FontStyles.Normal);
        }
        //Metodos para el funcionamiento de los botonos, Se bloquean los botones al principio y final de la lista
        public void btnPrimero_Click(object sender, RoutedEventArgs e)
        {
            estado = EstadoAPP.Listar;
            profActual = 0;
            ControlLista(this, profActual, profesores.Count);
            RellenarDatos(profesores[profActual], this);
        }
        public void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            estado = EstadoAPP.Listar;
            profActual--;
            ControlLista(this, profActual, profesores.Count);
            RellenarDatos(profesores[profActual], this);
        }
        public void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            estado = EstadoAPP.Listar;
            profActual++;
            ControlLista(this, profActual, profesores.Count);
            RellenarDatos(profesores[profActual], this);
        }
        public void btnUltimo_Click(object sender, RoutedEventArgs e)
        {
            estado = EstadoAPP.Listar;
            profActual = profesores.Count - 1;
            ControlLista(this, profActual, profesores.Count);
            RellenarDatos(profesores[profActual], this);
        }
        // Metodos para los botones con funcion CRUD
        public void btnAnadir_Click(object sender, RoutedEventArgs e)
        {
            estado = EstadoAPP.Inserccion;
            BorrarCampos(this);
            AlternarFoto(this);
            controlesGridBotones.Deshabilitar();
            Habilitar([btnCancelar, btnGuardar, gridCent]);
            txtEmail.Off();

        }
        public void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            estado = EstadoAPP.Modificacion;
            AlternarFoto(this);
            controlesGridBotones.Deshabilitar();
            Habilitar([btnCancelar, btnGuardar, gridCent]);
            Deshabilitar([txtNombre, txtApellidos, txtEmail]);

        }
        public void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (profesores.Count == 0) return;

            BorrarDatos(profesores[profActual]);
            profesores.RemoveAt(profActual);

            // Ajustar profActual después de borrar
            if (profActual >= profesores.Count)
            {
                profActual = profesores.Count - 1; // Último elemento
            }

            if (profesores.Count == 0)
            {
                estado = EstadoAPP.SinCargar;
                Deshabilitar([gridCent, menuFiltros, menuAgrupacion, gridBtn]);
                btnAnadir.On();
                BorrarCampos(this);
            }
            else
            {
                RellenarDatos(profesores[profActual], this);
                ControlLista(this, profActual, profesores.Count);
            }

            MessageBox.Show("La operacion de borrado ha tenido exito", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea Cancelar la operación?", "FILTRAR POR Edad", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                // Controlo si tengo elementos en la lista  si no los tengo devuelvo la app al estado inicial
                if (profesores.Count != 0)
                {
                    // Habilito lo que anteriormente deshabilite
                    Habilitar([menuFiltros, menuAgrupacion, gridBtn]);
                    // Relleno los datos del primero de la lista y deshabilito los botones necesarios Con un metodo de extension nuevo

                    ControlLista(this, profActual, profesores.Count);
                    ProfesorFuncionario p = (ProfesorFuncionario)profesores[profActual];
                    RellenarDatos(p, this);
                }
                else
                {
                    estado = EstadoAPP.SinCargar;
                    Deshabilitar([gridCent, menuFiltros, menuAgrupacion, gridBtn]);
                    btnAnadir.On();
                    BorrarCampos(this);
                    AlternarFoto(this);
                }
            }
        }

        public void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string controlErroneo;
            if (CheckCampos(controlesGridCentral, out controlErroneo))
            {
                ProfesorFuncionario p = new ProfesorFuncionario(
                    txtNombre.Text,
                    txtApellidos.Text,
                    Int32.Parse(cmbEdad.SelectedValue.ToString()),
                    "",
                    Enum.Parse<TipoFuncionario>(((bool)rdbDeCarrera.IsChecked ? rdbDeCarrera : rdbEnPracticas).Name.Substring(3)),
                    Int32.Parse(txtAnioIngreso.Text),
                    (bool)chkDestino.IsChecked,
                    lsbSegMedico.SelectedValue == "Muface" ? TipoMed.Muface : TipoMed.SeguridadSocial,
                    txtRutaFoto.Text
                );

                switch (estado)
                {
                    case EstadoAPP.Inserccion:
                        // Inserto en la base de datos
                        InsertarDatos(p);
                        // Inserto en la lista en memoria
                        profesores.Add(p);
                        profActual = profesores.Count - 1; // Último elemento
                        break;

                    case EstadoAPP.Modificacion:
                        // Actualizo en la base de datos
                        ModificarDatos(p);
                        // Reemplazo en la lista en memoria de forma segura
                        if (profActual >= 0 && profActual < profesores.Count)
                        {
                            profesores[profActual] = p;
                        }
                        else
                        {
                            // Si por algún motivo profActual es inválido, lo ponemos al último
                            profActual = profesores.Count - 1;
                            profesores[profActual] = p;
                        }
                        break;
                }

                // Rellenamos la interfaz y controlamos los botones
                if (profActual >= 0 && profActual < profesores.Count)
                {
                    RellenarDatos(profesores[profActual], this);
                }
                ControlLista(this, profActual, profesores.Count);

                MessageBox.Show("La operacion de " + estado.ToString() + " ha tenido exito",
                    "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No has introducido un " + controlErroneo + " valido",
                    "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Metodo para rellenar el campo de email cuando el usuario rellena el campo de apellidos
        private void txtApellidos_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtApellidos.Text) && !string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                Profesor p = new ProfesorFuncionario(0, txtApellidos.Text, txtNombre.Text);
                txtEmail.Text = p.Id;
            }
        }
        // Filtros
        public void Filtro1_Click(object sender, RoutedEventArgs e)
        {
            var mayoresDe35 = profesores
                .Where(x => x.Edad > 35)
                .Select(x => new
                {
                    x.Nombre,
                    x.Apellidos,
                    x.Edad,
                    x.Materia
                });
            string salida = CrearStringMensaje(mayoresDe35);
            MessageBox.Show(salida, "FILTRAR POR Edad", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void Filtro2_Click(object sender, RoutedEventArgs e)
        {
            var posteriorIgualA2010 = profesores
                .Where(x => x.AnyoIngresoCuerpo >= 2010)
                .Select(x => x);
            string salida = CrearStringMensaje(posteriorIgualA2010);

            MessageBox.Show(salida, "FILTRAR POR Año de Ingreso", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void Filtro3_Click(object sender, RoutedEventArgs e)
        {
            var anyo2010YCasado = profesores
                .Join(profesoresEx,
                (pf => pf.Id),
                (px => px.ProfesorFuncionarioId),
                (pf, px) => new
                {
                    pf.Nombre,
                    pf.Apellidos,
                    pf.AnyoIngresoCuerpo,
                    px.ECivil
                })
                .Where(x => x.AnyoIngresoCuerpo >= 2010)
                .Where(x => x.ECivil == EstadCivil.Casado);
            string salida = CrearStringMensaje(anyo2010YCasado);

            MessageBox.Show(salida, "FILTRAR POR Año de Ingreso Y Estado Civil", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void Filtro4_Click(object sender, RoutedEventArgs e)
        {
            var Estatura160 = profesores
                .Join(profesoresEx,
                (pf => pf.Id),
                (px => px.ProfesorFuncionarioId),
                (pf, px) => new
                {
                    pf.Nombre,
                    pf.Apellidos,
                    pf.Edad,
                    px.Estatura,
                    px.Peso
                })
                .Where(x => x.Estatura >= 160)
                .OrderByDescending(x => x.Estatura)
                .ThenByDescending(x => x.Peso);
            string salida = CrearStringMensaje(Estatura160);

            MessageBox.Show(salida, "FILTRAR POR Estatura", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        //Agrupaciones
        public void Agrupacion1_Click(object sender, RoutedEventArgs e)
        {
            var gruposEcivil = profesores
                .Join(profesoresEx,
                (pf => pf.Id),
                (px => px.ProfesorFuncionarioId),
                (pf, px) => new
                {
                    pf.Nombre,
                    pf.Apellidos,
                    pf.Edad,
                    px.Peso,
                    px.Estatura,
                    px.ECivil
                }).GroupBy(x => x.ECivil);
            string salida = CrearStringMensaje(gruposEcivil, false);
            MessageBox.Show(salida, "AGRUPAR POR Estado Civil", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void Agrupacion2_Click(object sender, RoutedEventArgs e)
        {
            var gruposEcivilCuenta = profesoresEx
               .GroupBy(p => p.ECivil)
               .Select(p => new
               {
                   Grupo = p.Key,
                   Total = p.Count()
               });
            string salida = CrearStringMensaje(gruposEcivilCuenta);
            MessageBox.Show(salida, "AGRUPAR POR Estado Civil con Contador", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void Agrupacion3_Click(object sender, RoutedEventArgs e)
        {
            var gruposEcivilCuenta = profesores
               .Select(p => new
               {
                   p.Nombre,
                   p.Apellidos,
                   p.Edad,
                   Madurez = p.Edad < 40 ? "Joven" : p.Edad < 60 ? "Maduro" : "Por Jubilarse"
               })
               .OrderByDescending(x => x.Edad)
               .GroupBy(x => x.Madurez);
            string salida = CrearStringMensaje(gruposEcivilCuenta, false);
            MessageBox.Show(salida, "AGRUPAR POR Madurez", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void Agrupacion4_Click(object sender, RoutedEventArgs e)
        {
            var gruposSMedico = profesores
                .Where(x => x.Edad >= 40)
                .Join(profesoresEx,
                (pf => pf.Id),
                (px => px.ProfesorFuncionarioId),
                (pf, px) => new
                {
                    pf.Nombre,
                    pf.Apellidos,
                    pf.TipoMedico,
                    px.Peso,
                })
                .OrderBy(x => x.Peso)
                .ThenBy(x => x.Apellidos)
                .GroupBy(x => x.TipoMedico);
            string salida = CrearStringMensaje(gruposSMedico, true);
            MessageBox.Show(salida, "AGRUPAR POR Seguro Medico", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}