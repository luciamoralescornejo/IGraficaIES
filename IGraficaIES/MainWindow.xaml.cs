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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            // Cargo todas las imagenes de la interfaz menos la foto de los profesores
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

        // se ejecuta al hacer click en el botón abrir
        public void Abrir_Click(object sender, RoutedEventArgs e)
        {

            // Crea un objeto OpenFileDialog para permitir que el usuario elija un archivo
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // abre el explorador en la carpeta donde está ejecutándose el programa.
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;

            //solo permite seleccionar archivos con extensión .txt.
            openFileDialog.Filter = "txt files (*.txt)|*.txt";

            // ShowDialog() abre el diálogo
            // Si el usuario selecciona un archivo y pulsa "Aceptar", devuelve true
            if (openFileDialog.ShowDialog() == true)
            {
                // Limpia la lista para evitar mezclar datos antiguos con nuevos
                profesores.Clear();

                // Lee el archivo una línea por cada iteración, sin cargarlo entero a memoria
                foreach (var line in File.ReadLines(openFileDialog.FileName))
                {
                    // Dividir la línea por el carácter ;
                    string[] split = line.Split(';');

                    // Verificar que la línea tenga exactamente 10 campos
                    if (split.Length != 10)
                    {
                        MessageBox.Show("Línea incorrecta:\n" + line);
                        continue;
                    }

                    // Convertir texto en valores del enum TipoFuncionario
                    TipoFuncionario tipoProfesor =
                        split[5] == "De Carrera" ? TipoFuncionario.DeCarrera :
                        split[5] == "En Practicas" ? TipoFuncionario.EnPracticas :
                        TipoFuncionario.Interino;

                    // Si el campo 8 contiene "SS" = Seguridad, si no = Muface
                    TipoMed tipoMedico =
                    split[8] == "SS" ? TipoMed.SeguridadSocial :
                    TipoMed.Muface;

                    // Convertir a booleano el campo definitivo
                    bool definitivo = bool.Parse(split[7]);

                    // Se crea un nuevo profesor con los datos convertidos y se añade a la lista
                    profesores.Add(new ProfesorFuncionario(
                        split[0], split[1], int.Parse(split[2]),
                        split[4], tipoProfesor,
                        int.Parse(split[6]), definitivo,
                        tipoMedico, split[9]
                    ));
                }

                // Si se cargaron profesores, actualizar la interfaz
                if (profesores.Count > 0)
                {
                    // el primer profesor es el actual
                    profActual = 0;
                    // la app pasa a modo listado
                    estado = EstadoAPP.Listar;
                    // activa los menús y botones
                    Habilitar([menuFiltros, menuAgrupacion, gridBtn]);
                    // muestra en pantalla los datos del primer profesor cargado
                    RellenarDatos(profesores[0], this);
                }
            }
        }

        // método para cerrar la ventana
        public void Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // métodos para el cambio de tipo de letra
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

        // Método mostrar el PRIMER profesor de la lista
        public void btnPrimero_Click(object sender, RoutedEventArgs e)
        {
            // La aplicación pasa al estado "Listar", indicando que solo se están visualizando datos,
            // no editando ni creando un nuevo profesor
            estado = EstadoAPP.Listar;

            // Establecemos el índice del profesor actual en 0, que corresponde al PRIMER elemento de la lista
            profActual = 0;

            // Se llama al método auxiliar "ControlLista", que  habilita o deshabilita los botones de navegación
            // según la posición actual.  Como estamos en el índice 0, deshabilitará "Primero" y "Anterior"
            ControlLista(this, profActual, profesores.Count);

            // Finalmente, mostramos en la interfaz los datos del profesor que está en la posición profActual 
            RellenarDatos(profesores[profActual], this);
        }

        // Método mostrar el profesor que está justo antes del actual
        public void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            // Cambiamos el estado de la aplicación porque solo estamos listando, no modificando datos
            estado = EstadoAPP.Listar;

            // Restamos 1 al índice actual para movernos al profesor anterior en la lista
            profActual--;

            // Llamamos al método auxiliar ControlLista, que se encarga de:
            // - Activar o desactivar botones de navegación según la nueva posición.
            // - Deshabilitar Guardar y Cancelar en modo Listar.
            ControlLista(this, profActual, profesores.Count);

            // Mostramos en la interfaz los datos del profesor cuyo índice es ahora profActual (uno menos al que teníamos)
            RellenarDatos(profesores[profActual], this);
        }

        // Método mostrar el profesor que está justo después del actual
        public void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            // Cambiamos el estado de la aplicación porque solo estamos listando, no modificando datos
            estado = EstadoAPP.Listar;

            // Sumamos 1 al índice actual para movernos al profesor siguiente en la lista
            profActual++;

            // Llamamos al método auxiliar ControlLista, que se encarga de:
            // - Activar o desactivar botones de navegación según la nueva posición.
            // - Deshabilitar Guardar y Cancelar en modo Listar.
            ControlLista(this, profActual, profesores.Count);

            // Mostramos en la interfaz los datos del profesor cuyo índice es ahora profActual (uno más al que teníamos)
            RellenarDatos(profesores[profActual], this);
        }

        // Método mostrar el último profesor de la lista
        public void btnUltimo_Click(object sender, RoutedEventArgs e)
        {
            // Cambiamos el estado de la aplicación porque solo estamos listando, no modificando datos
            estado = EstadoAPP.Listar;

            // Establecemos como índice actual el del último elemento de la lista
            profActual = profesores.Count - 1;

            // Llamamos al método auxiliar ControlLista, que se encarga de:
            // - Activar o desactivar botones de navegación según la nueva posición.
            // - Deshabilitar Guardar y Cancelar en modo Listar.
            ControlLista(this, profActual, profesores.Count);

            // Mostramos en la interfaz los datos del último profesor
            RellenarDatos(profesores[profActual], this);
        }

        // Método añadir nuevo profesor a la lista
        public void btnAnadir_Click(object sender, RoutedEventArgs e)
        {
            // Cambiamos el estado de la aplicación a Insercción (modo añadir)
            estado = EstadoAPP.Inserccion;

            // borramos todos los campos
            BorrarCampos(this);

            // Alterna la visualización de la foto
            AlternarFoto(this);

            // Deshabilitamos las flechas para que no se pueda mover entre profesores mientras se está añadiendo uno nuevo
            controlesGridBotones.Deshabilitar();

            // Habilitamos solo los controles necesarios para añadir un profesor: cancelar, guardar y el grid central para añadir datos
            Habilitar([btnCancelar, btnGuardar, gridCent]);

            // Deshabilitamos la edición del campo Email porque se genera automáticamente 
            txtEmail.Off();

        }

        // Método modificar profesor de la lista
        public void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            // Cambiamos el estado de la aplicación a Insercción (modo añadir)
            estado = EstadoAPP.Modificacion;

            // Alterna la visualización de la foto
            AlternarFoto(this);

            // Deshabilitamos las flechas para que no se pueda mover entre profesores mientras se está editando
            controlesGridBotones.Deshabilitar();

            // Habilitamos solo los controles necesarios para editar un profesor: cancelar, guardar y el grid central para añadir datos
            Habilitar([btnCancelar, btnGuardar, gridCent]);

            // No se pueden editar ni nombre, ni apellidos ni email 
            Deshabilitar([txtNombre, txtApellidos, txtEmail]);

        }

        // Método eliminar profesor de la lista
        public void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            // Si no hay profesores en la lista, no hacemos nada
            if (profesores.Count == 0) return;

            // Borra los datos del profesor actual
            BorrarDatos(profesores[profActual]);

            // Eliminamos el profesor de la lista
            profesores.RemoveAt(profActual);

            // Ajustar profActual después de borrar
            if (profActual >= profesores.Count)
            {
                profActual = profesores.Count - 1; // Nuevo último elemento
            }

            // Si ya no quedan profesores en la lista
            if (profesores.Count == 0)
            {
                // Cambiamos el estado a "SinCargar", indicando que no hay datos cargados
                estado = EstadoAPP.SinCargar;

                // Deshabilitamos todos los controles relacionados con la edición y navegación
                Deshabilitar([gridCent, menuFiltros, menuAgrupacion, gridBtn]);

                // Habilitamos el botón Añadir para permitir crear un nuevo profesor
                btnAnadir.On();

                // Limpiamos los campos de la interfaz
                BorrarCampos(this);
            }
            else
            {
                // Mostramos los datos del profesor que ahora corresponde al índice profActual
                RellenarDatos(profesores[profActual], this);
                // Ajustamos los botones de navegación según la posición actual
                ControlLista(this, profActual, profesores.Count);
            }
            // Mostramos un mensaje informando que el borrado fue exitoso
            MessageBox.Show("La operacion de borrado ha tenido exito", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Método botón cancelar 
        public void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Mostramos un mensaje de confirmación antes de cancelar
            MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea Cancelar la operación?", "FILTRAR POR Edad", MessageBoxButton.OKCancel, MessageBoxImage.Information);

            // Solo se cancela realmente si el usuario pulsa OK
            if (result == MessageBoxResult.OK)
            {
                // Si la lista tiene profesores, volvemos al estado normal de navegación
                if (profesores.Count != 0)
                {
                    // Volvemos a habilitar los menús y botones que estaban deshabilitados
                    Habilitar([menuFiltros, menuAgrupacion, gridBtn]);
                    // Ajustamos los botones de navegación según la posición actual
                    ControlLista(this, profActual, profesores.Count);
                    // Recuperamos el profesor actual para volver a mostrar sus datos en pantalla
                    ProfesorFuncionario p = (ProfesorFuncionario)profesores[profActual];
                    // Rellenamos la interfaz con los datos del profesor actual
                    RellenarDatos(p, this);
                }
                else
                {
                    // Si NO hay profesores en la lista tras cancelar, la app vuelve al estado inicial
                    estado = EstadoAPP.SinCargar;
                    // Deshabilitamos todos los controles de navegación, filtros y contenido porque no hay datos
                    Deshabilitar([gridCent, menuFiltros, menuAgrupacion, gridBtn]);
                    // El botón Añadir sí se habilita para permitir crear un nuevo profesor
                    btnAnadir.On();
                    // Limpiamos los campos de la interfaz
                    BorrarCampos(this);
                    // Alternamos la visualización de la foto para dejar la interfaz coherente
                    AlternarFoto(this);
                }
            }
        }

        // Método para el botoón guardar 
        public void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string controlErroneo;

            // Verificamos si los datos introducidos en los controles son válidos
            // CheckCampos devuelve true si están bien y si es false mete el error en controlErroneo
            if (CheckCampos(controlesGridCentral, out controlErroneo))
            {
                // Creamos un nuevo objeto ProfesorFuncionario usando los datos de la interfaz
                ProfesorFuncionario p = new ProfesorFuncionario(
                    txtNombre.Text,
                    txtApellidos.Text,
                    Int32.Parse(cmbEdad.SelectedValue.ToString()),
                    "",
                    // Determinamos el enum TipoFuncionario según el radio button marcado
                    Enum.Parse<TipoFuncionario>(
                        ((bool)rdbDeCarrera.IsChecked ? rdbDeCarrera : rdbEnPracticas)
                        .Name.Substring(3)), // Substring(3) quita "rdb" para obtener el nombre del enum
                    Int32.Parse(txtAnioIngreso.Text),
                    (bool)chkDestino.IsChecked,
                    // Determinamos el tipo de seguro médico según la opción seleccionada en la lista
                    lsbSegMedico.SelectedValue == "Muface" ? TipoMed.Muface : TipoMed.SeguridadSocial,
                    txtRutaFoto.Text
                );

                // Según el estado de la aplicación decidimos si estamos añadiendo o modificando
                switch (estado)
                {
                    case EstadoAPP.Inserccion:
                        // Guardamos en la base de datos
                        InsertarDatos(p);
                        // Añadimos el nuevo profesor a la lista en memoria
                        profesores.Add(p);
                        // El profesor recién añadido será el actual (último en la lista)
                        profActual = profesores.Count - 1;
                        break;

                    case EstadoAPP.Modificacion:
                        // Actualizamos la entrada en la base de datos
                        ModificarDatos(p);

                        // Actualizamos el profesor en la lista de memoria, siempre comprobando que el índice sea válido
                        if (profActual >= 0 && profActual < profesores.Count)
                        {
                            profesores[profActual] = p;
                        }
                        else
                        {
                            // Si por algún motivo el índice es incorrecto, lo corregimos apuntando al último elemento
                            profActual = profesores.Count - 1;
                            profesores[profActual] = p;
                        }
                        break;
                }

                // Rellenamos los controles con los datos recién insertados o modificados
                if (profActual >= 0 && profActual < profesores.Count)
                {
                    RellenarDatos(profesores[profActual], this);
                }

                // Reconfiguramos los botones de navegación y la interfaz
                ControlLista(this, profActual, profesores.Count);

                // Mensaje de éxito
                MessageBox.Show("La operacion de " + estado.ToString() + " ha tenido exito",
                    "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Si hay algún error en los campos, avisamos al usuario indicando cuál falló
                MessageBox.Show("No has introducido un " + controlErroneo + " valido",
                    "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Metodo para rellenar el campo de email cuando el usuario rellena el campo de apellidos
        private void txtApellidos_LostFocus(object sender, RoutedEventArgs e)
        {
            // Comprobamos que tanto el nombre como los apellidos estén rellenos
            // Solo si ambos tienen texto, generamos el email
            if (!string.IsNullOrWhiteSpace(txtApellidos.Text) && !string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                // Creamos un objeto ProfesorFuncionario temporal SOLO para obtener  el email generado automáticamente en
                // su propiedad Id. Usamos 0 como parámetro para el DNI u otro valor no relevante.
                Profesor p = new ProfesorFuncionario(0, txtApellidos.Text, txtNombre.Text);
                txtEmail.Text = p.Id;
            }
        }

        // Filtros
        public void Filtro1_Click(object sender, RoutedEventArgs e)
        {
            // Aplicamos un filtro LINQ sobre la lista de profesores:
            // Nos quedamos solo con los profesores cuya edad sea mayor de 35 años.
            var mayoresDe35 = profesores
                .Where(x => x.Edad > 35)
                .Select(x => new
                {
                    x.Nombre,
                    x.Apellidos,
                    x.Edad,
                    x.Materia
                });
            // Convertimos el resultado filtrado en un string listo para mostrar,
            // usando el método auxiliar CrearStringMensaje que recibe una colección
            string salida = CrearStringMensaje(mayoresDe35);
            // Mostramos el resultado del filtro en una ventana emergente
            MessageBox.Show(salida, "FILTRAR POR Edad", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Filtro2_Click(object sender, RoutedEventArgs e)
        {
            // Filtramos los profesores para obtener solo aquellos cuyo año de ingreso en el cuerpo docente sea 2010 o posterior
            var posteriorIgualA2010 = profesores
                .Where(x => x.AnyoIngresoCuerpo >= 2010)
                .Select(x => x); // Seleccionamos el objeto completo (sin proyección)

            // Convertimos el resultado filtrado en un string formateado para mostrarlo
            string salida = CrearStringMensaje(posteriorIgualA2010);

            // Mostramos el resultado en un MessageBox como información
            MessageBox.Show(salida, "FILTRAR POR Año de Ingreso", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public void Filtro3_Click(object sender, RoutedEventArgs e)
        {
            var anyo2010YCasado = profesores
                .Join(profesoresEx, // Segunda colección
                (pf => pf.Id), // Clave en profesores
                (px => px.ProfesorFuncionarioId), // Clave en profesoresEx
                (pf, px) => new // Resultado del Join
                {
                    pf.Nombre,
                    pf.Apellidos,
                    pf.AnyoIngresoCuerpo,
                    px.ECivil
                })
                .Where(x => x.AnyoIngresoCuerpo >= 2010) // Filtro 1: Año de ingreso >= 2010
                .Where(x => x.ECivil == EstadCivil.Casado); // Filtro 2: Estado civil = Casado
            string salida = CrearStringMensaje(anyo2010YCasado);

            MessageBox.Show(salida, "FILTRAR POR Año de Ingreso Y Estado Civil", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public void Filtro4_Click(object sender, RoutedEventArgs e)
        {
            var Estatura160 = profesores
                .Join(profesoresEx, // Segunda colección con datos extra
                (pf => pf.Id), // Clave de unión en profesores
                (px => px.ProfesorFuncionarioId), // Clave de unión en profesoresEx
                (pf, px) => new // Resultado del Join
                {
                    pf.Nombre,
                    pf.Apellidos,
                    pf.Edad,
                    px.Estatura,
                    px.Peso
                })
                .Where(x => x.Estatura >= 160) // Filtro: estatura mínima 160 cm
                .OrderByDescending(x => x.Estatura) // Orden principal: estatura de mayor a menor
                .ThenByDescending(x => x.Peso); // Orden secundario: peso de mayor a menor

            // Convertimos el resultado a un texto legible
            string salida = CrearStringMensaje(Estatura160);

            MessageBox.Show(salida, "FILTRAR POR Estatura", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public void Agrupacion1_Click(object sender, RoutedEventArgs e)
        {
            var gruposEcivil = profesores
                .Join(
                    profesoresEx, // Segunda colección con datos extendidos
                    (pf => pf.Id), // Clave en profesores
                    (px => px.ProfesorFuncionarioId), // Clave en profesoresEx
                    (pf, px) => new // Resultado del Join
                    {
                        pf.Nombre,
                        pf.Apellidos,
                        pf.Edad,
                        px.Peso,
                        px.Estatura,
                        px.ECivil
                    }
                )
                .GroupBy(x => x.ECivil); // Agrupación por estado civil

            // Convertimos el resultado en texto, indicando que NO queremos numerar los elementos
            string salida = CrearStringMensaje(gruposEcivil, false);

            // Mostramos los grupos en un MessageBox
            MessageBox.Show(salida, "AGRUPAR POR Estado Civil", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Agrupacion2_Click(object sender, RoutedEventArgs e)
        {
            var gruposEcivilCuenta = profesoresEx
               .GroupBy(p => p.ECivil) // Agrupamos por estado civil
               .Select(p => new // Proyección del grupo
               {
                   Grupo = p.Key, // La clave del grupo (el estado civil)
                   Total = p.Count() // Cuántos elementos tiene ese grupo
               });

            // Convertimos los resultados a un string legible
            string salida = CrearStringMensaje(gruposEcivilCuenta);

            // Mostramos los datos agrupados con contador
            MessageBox.Show(salida, "AGRUPAR POR Estado Civil con Contador", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Agrupacion3_Click(object sender, RoutedEventArgs e)
        {
            // Proyectamos cada profesor en un nuevo objeto anónimo que incluye:
            // Nombre, Apellidos, Edad y una nueva categoría llamada "Madurez" según la edad
            var gruposEcivilCuenta = profesores
               .Select(p => new
               {
                   p.Nombre,
                   p.Apellidos,
                   p.Edad,
                   Madurez = p.Edad < 40 ? "Joven" // Si edad < 40 = "Joven"
                            : p.Edad < 60 ? "Maduro" // Si edad < 60 = "Maduro"
                            : "Por Jubilarse" // Si edad ≥ 60 = "Por Jubilarse"
               })
               .OrderByDescending(x => x.Edad) // Ordenamos la lista por edad de mayor a menor
               .GroupBy(x => x.Madurez); // Agrupamos los profesores por la categoría "Madurez"

            // Convertimos los grupos en un texto legible para mostrar al usuario
            string salida = CrearStringMensaje(gruposEcivilCuenta, false);

            // Mostramos los grupos en un MessageBox con título descriptivo
            MessageBox.Show(salida, "AGRUPAR POR Madurez", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Agrupacion4_Click(object sender, RoutedEventArgs e)
        {
            // Filtramos solo los profesores con edad >= 40
            var gruposSMedico = profesores
                .Where(x => x.Edad >= 40)

                // Realizamos un Join con la colección profesoresEx para obtener datos adicionales
                .Join(profesoresEx,
                      pf => pf.Id, // Clave de la lista principal
                      px => px.ProfesorFuncionarioId, // Clave de la lista extendida
                      (pf, px) => new // Proyección de los datos combinados
                      {
                          pf.Nombre,
                          pf.Apellidos,
                          pf.TipoMedico, // Tipo de seguro médico del profesor
                          px.Peso // Peso desde la lista extendida
                      })

                // Ordenamos primero por Peso ascendente
                .OrderBy(x => x.Peso)
                // Y si hay empate, por Apellidos ascendente
                .ThenBy(x => x.Apellidos)

                // Agrupamos los resultados por Tipo de Seguro Médico
                .GroupBy(x => x.TipoMedico);

            // Convertimos los grupos en un texto legible para mostrar
            string salida = CrearStringMensaje(gruposSMedico, true);

            // Mostramos los resultados en un MessageBox con título descriptivo
            MessageBox.Show(salida, "AGRUPAR POR Seguro Medico", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}