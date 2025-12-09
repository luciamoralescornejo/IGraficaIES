namespace _2HerenciaSimpleIES
{
    // Enum que define los tipos de funcionarios/profesores
    public enum TipoFuncionario : uint
    {
        Interino = 1, // Profesor temporal
        EnPracticas = 2, // Profesor en prácticas
        DeCarrera = 3 // Profesor de carrera
    }

    // Clase abstracta Profesor que hereda de Persona
    // No se puede instanciar directamente, solo a través de subclases
    public abstract class Profesor : Persona
    {
        private string materia; // Materia que imparte el profesor
        private TipoFuncionario tipoProfesor; // Tipo de funcionario

        // Constructores protegidos, accesibles solo por subclases
        protected Profesor() { }

        protected Profesor(int edad, string apellidos, string nombre)
            : base(edad, apellidos, nombre) { }

        protected Profesor(string rutaFoto, int edad, string apellidos, string nombre)
            : base(rutaFoto, edad, apellidos, nombre) { }

        // Propiedad para acceder y modificar el tipo de funcionario
        public TipoFuncionario TipoProfesor
        {
            get { return tipoProfesor; }
            set { tipoProfesor = value; }
        }

        // Propiedad para acceder y modificar la materia
        public string Materia
        {
            get { return materia; }
            set { materia = value; }
        }

        // Método abstracto ToString que obliga a las subclases a implementarlo
        public abstract override string ToString();

        // Método adicional que devuelve una cadena con información del profesor
        // Incluye datos base (nombre, apellidos, edad, etc.) + materia + tipo de funcionario
        public string ToStringProfesor()
        {
            return base.ToString() + string.Format("{0}{1}",
                materia.PadRight(20), // Alinea la materia a la derecha en 20 caracteres
                tipoProfesor.ToString().PadRight(20)); // Alinea el tipo de funcionario a la derecha en 20 caracteres
        }
    }
}
