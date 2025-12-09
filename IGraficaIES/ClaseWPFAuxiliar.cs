using _2HerenciaSimpleIES;
using IGraficaIES.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace IGraficaIES
{
    public class ClaseWPFAuxiliar
    {
        public const string rutaFija = "..\\..\\..\\recursos\\";
        public enum EstadoAPP : uint
        {
            SinCargar = 0, // No se ha cargado ningún dato aún
            Listar = 1, // Se está mostrando la lista de profesores
            Inserccion = 2, // Se está añadiendo un nuevo profesor
            Modificacion = 3, // Se está modificando un profesor existente
            Eliminacion = 4 // Se está eliminando un profesor
        }

        // Metodo que recibe una lista y habilita todos sus elementos
        public static void Habilitar(IEnumerable<UIElement> lista)
        {
            foreach (UIElement element in lista)
            {
                // En caso de ser un panel habilita todos sus hijos
                if (element is Panel)
                {
                    Habilitar(ObtenerControles((Panel)element));
                }
                else
                {
                    element.IsEnabled = true;
                }
            }
        }

        // Metodo que recibe una lista y deshabilita todos sus elementos
        public static void Deshabilitar(IEnumerable<UIElement> lista)
        {
            foreach (UIElement element in lista)
            {
                // En caso de ser un panel deshabilita todos sus hijos
                if (element is Panel)
                {
                    Deshabilitar(ObtenerControles((Panel)element));
                }
                else
                {
                    element.IsEnabled = false;
                }
            }
        }

        // Metodo para altenernar entre el campo txtRutaFoto y imgFoto
        public static void AlternarFoto(MainWindow window)
        {
            // Se revisa el estado actual de visibilidad del TextBox de la ruta de la foto
            switch (window.txtRutaFoto.Visibility)
            {
                // Si el TextBox es visible, se oculta y se muestra la imagen.
                case Visibility.Visible:
                    window.txtRutaFoto.Visibility = Visibility.Hidden; // Oculta el TextBox
                    window.lblRutaFoto.Visibility = Visibility.Hidden; // Oculta la etiqueta de la ruta
                    window.imgFoto.Visibility = Visibility.Visible; // Muestra la imagen
                    break;

                // Si el TextBox está oculto, se muestra y se oculta la imagen.
                case Visibility.Hidden:
                    window.txtRutaFoto.Visibility = Visibility.Visible; // Muestra el TextBox
                    window.lblRutaFoto.Visibility = Visibility.Visible; // Muestra la etiqueta de la ruta
                    window.imgFoto.Visibility = Visibility.Hidden; // Oculta la imagen
                    break;
            }
        }
        //Metodo para cargar todos los controles de un Panel (grid, StackPanel, etc)
        public static List<Control> ObtenerControles(Panel contenedor)
        {
            List<Control> controles = new List<Control>();
            foreach (var item in contenedor.Children)
            {
                // Diferencio entre Control y Panel
                if (item is Control)
                {

                    controles.Add((Control)item);
                }
                else if (item is Panel)
                {
                    // Uso recursividad para sacar los metodos que haya en paneles Hijos del principal
                    controles.AddRange(ObtenerControles((Panel)item));
                }
            }
            return controles;

        }
        // Método genérico que recibe una lista de objetos y devuelve un string con todas sus propiedades y valores
        public static string CrearStringMensaje<T>(IEnumerable<T> profesores)
        {
            // Cadena donde se irá acumulando la información de todos los objetos
            string cadena = "";

            // Usamos reflexión para obtener todas las propiedades públicas del tipo T
            System.Reflection.PropertyInfo[] listaPropiedades = typeof(T).GetProperties();

            // Recorremos cada objeto de la lista
            foreach (var item in profesores)
            {
                // Recorremos cada propiedad del objeto
                foreach (System.Reflection.PropertyInfo propiedad in listaPropiedades)
                {
                    // Obtenemos el nombre de la propiedad y su valor en el objeto actual
                    // y lo agregamos a la cadena
                    cadena += string.Format("{0}: {1} \n", propiedad.Name, propiedad.GetValue(item));
                }

                // Agregamos un salto de línea entre objetos para mejor legibilidad
                cadena += "\n";
            }

            // Devolvemos la cadena resultante con toda la información
            return cadena;
        }

        // Método genérico que recibe grupos de objetos y devuelve un string con todas sus propiedades y valores
        // TClave: tipo de la clave del grupo (por ejemplo, Estado Civil, Madurez, TipoMedico)
        // TValor: tipo de los objetos dentro de cada grupo
        // mostrarPropiedadAgrupada: indica si se debe mostrar la propiedad por la que se agrupa
        public static string CrearStringMensaje<TClave, TValor>(
            IEnumerable<IGrouping<TClave, TValor>> grupoProfesores,
            bool mostrarPropiedadAgrupada)
        {
            // Usamos StringBuilder para construir la cadena de salida eficientemente
            StringBuilder sb = new StringBuilder();

            // Obtenemos todas las propiedades públicas del tipo TValor mediante reflexión
            System.Reflection.PropertyInfo[] listaPropiedades = typeof(TValor).GetProperties();

            // Recorremos cada grupo de la colección
            foreach (var grupo in grupoProfesores)
            {
                // Agregamos un encabezado para identificar el grupo
                sb.AppendFormat("\n----------- Grupo {0} -----------\n", grupo.Key);

                // Recorremos cada objeto dentro del grupo
                foreach (var item in grupo)
                {
                    // Contador para colocar 2 propiedades por línea
                    int cont = 0;

                    // Recorremos cada propiedad del objeto
                    foreach (System.Reflection.PropertyInfo propiedad in listaPropiedades)
                    {
                        // Evitamos mostrar la propiedad por la que se agrupó a menos que se indique con mostrarPropiedadAgrupada
                        if (!(grupo.Key.ToString() == propiedad.GetValue(item).ToString()) || mostrarPropiedadAgrupada)
                        {
                            // Agregamos el nombre de la propiedad y su valor al StringBuilder
                            // Si cont == 0, agregamos espacios para alinear 2 propiedades por línea
                            sb.AppendFormat("{0}: {1}{2}", propiedad.Name, propiedad.GetValue(item), cont == 0 ? "         " : "\n");

                            // Actualizamos el contador (0 o 1) para alternar líneas
                            cont = (cont + 1) % 2;
                        }
                    }
                    // Salto de línea adicional entre objetos
                    sb.AppendLine("\n");
                }
            }

            // Devolvemos la cadena construida
            return sb.ToString();
        }

        // Metodo para verificar la validez de los campos
        // Los campos pueden tener las etiquetas, Opcional y/o Numero. 
        public static bool CheckCampos(List<Control> controles, out string controlErroneo)
        {
            bool valido = true;
            controlErroneo = "";
            // Lista para controlar los Grupos de RadioButton
            List<String> listaGruposRadio = new List<String>();
            foreach (Control c in controles)
            {
                string tag = c.Tag == null ? "" : c.Tag.ToString();

                // A los controles desde el XAML se le pueden añadir etiquetas
                // He usado esto para controlar cuando un campo es opcional o un textBox es un numero
                bool esOpcional = tag.Contains("Opcional");
                bool esArchivo = tag.Contains("Archivo");
                bool esNumero = tag.Contains("Numero");

                // Compruebo si la bandera sigue siendo valida
                if (valido)
                {
                    // Compruebo si es un RadioButton
                    if (c is RadioButton rdb)
                    {
                        // La logica de comprobar si un Grupo de RadioButton es valido es mas compleja
                        // Mi solucion es ir guardando los nombres de los grupos en una lista
                        // Cuando encuentro un RadioButton seleccionado pongo el nombre del grupo en mayuscula
                        string grupo = rdb.GroupName.ToLower();
                        if (grupo.StartsWith("opc") || !listaGruposRadio.Contains(grupo.ToUpper()))
                        {
                            if (!listaGruposRadio.Contains(grupo))
                            {
                                listaGruposRadio.Add(grupo);
                            }
                            if ((bool)rdb.IsChecked)
                            {
                                listaGruposRadio[listaGruposRadio.IndexOf(grupo)] = grupo.ToUpper();
                            }
                        }
                    }
                    // Compruebo si es un textBox
                    if (c is TextBox txt)
                    {
                        // Compruebo si el campo es un numero en caso de serlo compruebo su validez haciendo un parseo
                        if (esNumero)
                        {
                            valido = Int32.TryParse(txt.Text, out int resul);
                        }
                        // Con esto compruebo si es un campo de archivo que el archivo exista y no haya puesto una ruta que NO es valida
                        else if (esArchivo)
                        {
                            valido = File.Exists(rutaFija + txt.Text);
                        }
                        else
                        {
                            valido = !string.IsNullOrWhiteSpace(txt.Text);
                        }
                        // En caso de no ser valido compruebo si era opcional y estaba vacio
                        if (!valido)
                        {
                            valido = esOpcional && string.IsNullOrWhiteSpace(txt.Text);
                        }
                    }
                    // Compruebo si es un ComboBox
                    else if (c is ComboBox cmb)
                    {
                        valido = cmb.SelectedValue != null || esOpcional;
                    }
                    // Compruebo si es un ListBox
                    else if (c is ListBox lsb)
                    {
                        valido = lsb.SelectedValue != null || esOpcional;
                    }
                    // Detecto el control que ha fallado, le pongo el foco y guardo el nombre quitandole el prefijo(txt,cmb,lsb)
                    if (!valido)
                    {
                        c.Focus();
                        controlErroneo = c.Name.Substring(3);
                    }
                }

            }
            // Cuando ya he recorrido todos los controles compruebo cuales son los grupos que estan en minuscula
            // Se le puede añadir el Prefijo OPC para que un grupo sea opcional
            for (int i = 0; listaGruposRadio.Count > i && valido; i++)
            {
                string grupo = listaGruposRadio[i];
                valido = grupo == grupo.ToUpper() || grupo.StartsWith("opc");
                if (!valido)
                {
                    controlErroneo = grupo;
                }
            }
            return valido;
        }

        // Método que se usa para rellenar la interfaz cada vez que se cambia de profesor
        // Se pasa la ventana (MainWindow) como contexto para poder acceder a los controles
        public static void RellenarDatos(ProfesorFuncionario p, MainWindow window)
        {
            // Me aseguro de que se muestre siempre la imagen y no la ruta de la foto
            if (window.imgFoto.Visibility == Visibility.Hidden)
            {
                AlternarFoto(window);
            }

            // Rellenamos los campos de texto con los datos del profesor
            window.txtNombre.Text = p.Nombre;
            window.txtApellidos.Text = p.Apellidos;
            window.txtEmail.Text = p.Id;

            // Seleccionamos la edad correspondiente en el ComboBox
            window.cmbEdad.SelectedValue = p.Edad;

            // Año de ingreso en el cuerpo
            window.txtAnioIngreso.Text = p.AnyoIngresoCuerpo.ToString();

            // Checkbox destino definitivo
            window.chkDestino.IsChecked = p.DestinoDefinitivo;

            // Selección del seguro médico en la lista
            window.lsbSegMedico.SelectedValue = p.TipoMedico == TipoMed.SeguridadSocial ? "S.Social" : "Muface";

            // Rellenamos la ruta de la foto en el campo correspondiente
            window.txtRutaFoto.Text = p.RutaFoto;

            // Marcamos el RadioButton correspondiente según el tipo de funcionario
            switch (p.TipoProfesor)
            {
                case TipoFuncionario.Interino:
                    // No se marca ninguno
                    break;
                case TipoFuncionario.EnPracticas:
                    window.rdbEnPracticas.IsChecked = true;
                    break;
                case TipoFuncionario.DeCarrera:
                    window.rdbDeCarrera.IsChecked = true;
                    break;
                default:
                    break;
            }

            // Si no tiene foto asignada, se carga una imagen por defecto
            if (p.RutaFoto == "")
            {
                CargarImagen(ref window.imgFoto, "No_imagen_disponible.gif");
            }
            else
            {
                // Si tiene foto, se carga la imagen correspondiente
                CargarImagen(ref window.imgFoto, p.RutaFoto);
            }
        }

        // Metodo para vaciar los campos
        public static void BorrarCampos(MainWindow window)
        {
            window.txtNombre.Text = "";
            window.txtApellidos.Text = "";
            window.txtEmail.Text = "";
            window.cmbEdad.SelectedValue = null;
            window.txtAnioIngreso.Text = "";
            window.chkDestino.IsChecked = false;
            window.lsbSegMedico.SelectedValue = null;
            window.txtRutaFoto.Text = "";
            window.rdbDeCarrera.IsChecked = false;
            window.rdbEnPracticas.IsChecked = false;
            window.imgFoto.Source = null;
        }

        // Método para simplificar la carga de imágenes en un control Image
        // Se pasa la referencia del control Image y el nombre del archivo de imagen
        public static void CargarImagen(ref Image img, string nombre)
        {
            try
            {
                // Se construye la ruta completa usando 'rutaFija' + nombre del archivo
                // Se convierte la ruta en un ImageSource y se asigna al control
                img.Source = new ImageSourceConverter().ConvertFromString(rutaFija + nombre) as ImageSource;
            }
            catch (Exception)
            {
                // Si ocurre algún error (archivo no encontrado, ruta inválida, etc.)
                // se muestra un mensaje de error
                MessageBox.Show("No se ha encontrado el archivo de imagen", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método genérico para insertar un objeto en la base de datos
        // T puede ser cualquier entidad del modelo (ProfesorFuncionario, ProfesorEx, etc.)
        public static void InsertarDatos<T>(T o)
        {
            // Usamos un bloque 'using' para garantizar que el contexto se cierre correctamente al finalizar
            using (MyDbContext context = new MyDbContext())
            {
                // Agregamos el objeto al contexto (marcándolo para inserción)
                context.Add(o);

                // Guardamos los cambios en la base de datos
                context.SaveChanges();
            }
        }

        // Método genérico para insertar varios objetos en la base de datos a través de una lista
        // T puede ser cualquier entidad del modelo (ProfesorFuncionario, ProfesorEx, etc.)
        public static void InsertarDatos<T>(ref List<T> l)
        {
            // Usamos un bloque 'using' para garantizar que el contexto se cierre correctamente al finalizar
            using (MyDbContext context = new MyDbContext())
            {
                // Recorremos cada objeto de la lista
                foreach (T t in l)
                {
                    // Marcamos cada objeto para ser insertado en la base de datos
                    context.Add(t);
                }

                // Guardamos todos los cambios de una sola vez
                context.SaveChanges();
            }
        }

        // Metodo para modificar un objeto de la base de datos
        // Metodo generico
        public static void ModificarDatos<T>(T o)
        {
            using (MyDbContext context = new MyDbContext())
            {
                context.Update(o);
                context.SaveChanges();
            }
        }
        // Metodo para eliminar un objeto de la base de datos
        // Metodo generico
        public static void BorrarDatos<T>(T o)
        {
            // Usamos un bloque using para asegurarnos de que el contexto se cierre automáticamente al finalizar la operación
            using (MyDbContext context = new MyDbContext())
            {
                // Indicamos al contexto que elimine el objeto pasado como parámetro
                context.Remove(o);
                // Guardamos los cambios en la base de datos, realizando realmente la eliminación
                context.SaveChanges();
            }
        }

        // Metodo para obtener una lista de todos los objetos en una tabla de la base de datos
        // Metodo generico, usamos la sentencia where T : class para asegurarnos de que no es un tipo primitivo
        public static List<T> ObternerLista<T>() where T : class
        {
            using (MyDbContext context = new MyDbContext())
            {
                return context.Set<T>().ToList();
            }
        }

        // Metodo para el control de los botones de avanzar y retroceder en al lista
        public static void ControlLista(MainWindow window, int profActual, int profCount)
        {
            // Indica que estamos viendo elementos de la lista, no editando, agregando o eliminando
            window.estado = EstadoAPP.Listar;

            // Habilitar todos los controles del grid de botones
            window.controlesGridBotones.Habilitar();

            // Deshabilitar “Primero” y “Anterior” si estamos en el primer elemento
            if (profActual == 0)
            {
                Deshabilitar([window.btnPrimero, window.btnAnterior]);
            }

            // Deshabilitar “Siguiente” y “Último” si estamos en el último elemento
            if (profActual == profCount - 1)
            {
                Deshabilitar([window.btnSiguiente, window.btnUltimo]);
            }

            // Si solo hay un profesor, deshabilitar todos los botones de navegación
            if (profCount == 1)
            {
                Deshabilitar([window.btnSiguiente, window.btnUltimo, window.btnPrimero, window.btnAnterior]);
            }

            // Deshabilitar “Guardar” y “Cancelar” porque solo estamos listando profesores
            Deshabilitar([window.btnGuardar, window.btnCancelar]);
        }
    }
}
