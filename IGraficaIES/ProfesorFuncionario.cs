using IGraficaIES;
using System;

namespace _2HerenciaSimpleIES
{
    // Clase que representa a un profesor funcionario, hereda de Profesor y cumple la interfaz IEmpleadoPublico
    public class ProfesorFuncionario : Profesor, IEmpleadoPublico
    {
        // Campos privados
        private int anyoIngresoCuerpo; // Año en que ingresó al cuerpo de funcionarios
        private bool definitivo; // Indica si es funcionario definitivo
        private TipoMed tipoMedico; // Tipo de seguro médico (Seguridad Social o Muface)

        // Propiedades públicas
        public int AnyoIngresoCuerpo
        {
            get { return anyoIngresoCuerpo; }
            set { anyoIngresoCuerpo = value; }
        }

        public bool DestinoDefinitivo
        {
            get { return definitivo; }
            set { definitivo = value; }
        }

        public TipoMed TipoMedico
        {
            get { return tipoMedico; }
            set { tipoMedico = value; }
        }

        // Relación con la clase extendida ProfesorExtendido
        public ProfesorExtendido ProfesorExtendido { get; set; }

        // Constructores
        public ProfesorFuncionario() { }

        public ProfesorFuncionario(int edad, string apellidos, string nombre)
            : base(edad, apellidos, nombre) { }

        public ProfesorFuncionario(string nombre,
                                   string apellidos,
                                   int edad,
                                   string materia,
                                   TipoFuncionario tipoProfesor,
                                   int anyoIngreso,
                                   bool definitivo,
                                   TipoMed tipoMedico)
            : base(edad, apellidos, nombre)
        {
            Materia = materia;
            TipoProfesor = tipoProfesor;
            DestinoDefinitivo = definitivo;
            AnyoIngresoCuerpo = anyoIngreso;
            TipoMedico = tipoMedico;
        }

        public ProfesorFuncionario(string nombre,
                                   string apellidos,
                                   int edad,
                                   string materia,
                                   TipoFuncionario tipoProfesor,
                                   int anyoIngreso,
                                   bool definitivo,
                                   TipoMed tipoMedico,
                                   string rutaFoto)
            : base(rutaFoto, edad, apellidos, nombre)
        {
            Materia = materia;
            TipoProfesor = tipoProfesor;
            DestinoDefinitivo = definitivo;
            AnyoIngresoCuerpo = anyoIngreso;
            TipoMedico = tipoMedico;
        }

        public ProfesorFuncionario(string rutaFoto, int edad, string apellidos, string nombre)
            : base(rutaFoto, edad, apellidos, nombre) { }

        // Métodos para calcular antigüedad en sexenios y trienios
        public int GetSexenios()
        {
            return (DateTime.Today.Year - AnyoIngresoCuerpo) / 6;
        }

        public int GetTrienios()
        {
            return (DateTime.Today.Year - AnyoIngresoCuerpo) / 3;
        }

        // Método para calcular el tiempo total de servicio en años, meses y días
        public (int anyos, int meses, int dias) TiempoServicio()
        {
            DateTime fechaIngreso = new DateTime(AnyoIngresoCuerpo, 9, 1); // Fecha de ingreso al cuerpo
            TimeSpan diferencia = DateTime.Now - fechaIngreso;
            int dias = diferencia.Days % 365 % 30;    // Dias restantes
            int meses = diferencia.Days % 365 / 30;   // Meses restantes
            int anyos = diferencia.Days / 365;       // Años completos
            return (anyos, meses, dias);
        }

        // Sobreescritura de ToString() para mostrar toda la información del profesor
        public override string ToString()
        {
            return base.ToStringProfesor() + string.Format("{0}{1}{2}",
                anyoIngresoCuerpo.ToString().PadRight(10),
                definitivo ? "SI".PadRight(20) : "NO".PadRight(20),
                tipoMedico.ToString().PadRight(20));
        }
    }
}
