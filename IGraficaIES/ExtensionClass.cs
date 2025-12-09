using System.Windows;
using System.Windows.Controls;

namespace _2HerenciaSimpleIES
{
    // Clase estática para definir métodos de extensión
    public static class ExtensionClass
    {
        // Extensión para contar palabras en un string
        public static int WordCount(this String str)
        {
            // Divide la cadena usando espacio, punto o signo de interrogación como separadores
            // y cuenta los elementos que no están vacíos
            return str.Split(new char[] { ' ', '.', '?' },
            StringSplitOptions.RemoveEmptyEntries).Length;
        }

        // Extensión para poner la primera letra de un string en mayúscula
        public static string FirstLetterToUpper(this String str)
        {
            if (str != null)
            {
                str = str.Trim(); // eliminamos espacios en blanco al inicio y final
                if (str.Length == 1)
                {
                    // Si solo hay un carácter, lo convertimos a mayúscula
                    str = str.ToUpper();
                }
                else
                {
                    // Mayúscula para la primera letra y minúscula para el resto
                    str = str[0].ToString().ToUpper() + str.Substring(1).ToLower();
                }
            }
            return str;
        }

        // Extensión para eliminar un objeto Persona de una lista
        public static bool SeekRemove(this List<Persona> list, Persona personaBuscada)
        {
            // Devuelve true si se eliminó correctamente, false si no se encontró
            return list.Remove(personaBuscada);
        }

        // Extensión para habilitar todos los controles de una lista de UIElement
        public static void Habilitar(this IEnumerable<UIElement> lista)
        {
            foreach (UIElement element in lista)
            {
                element.IsEnabled = true;
            }
        }

        // Extensión para deshabilitar todos los controles de una lista de UIElement
        public static void Deshabilitar(this IEnumerable<UIElement> lista)
        {
            foreach (UIElement element in lista)
            {
                element.IsEnabled = false;
            }
        }

        // Extensión para habilitar un solo control
        public static void On(this UIElement b)
        {
            b.IsEnabled = true;
        }

        // Extensión para deshabilitar un solo control
        public static void Off(this UIElement b)
        {
            b.IsEnabled = false;
        }
    }
}
