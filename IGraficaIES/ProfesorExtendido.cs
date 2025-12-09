using System;
using System.Collections.Generic;

namespace IGraficaIES
{
    // Enum que define los posibles estados civiles de un profesor
    public enum EstadCivil : uint
    {
        Soltero = 1,
        Casado = 2,
        Divorciado = 3,
        Viudo = 4
    }

    // Clase ProfesorExtendido que contiene información adicional sobre los profesores
    public class ProfesorExtendido
    {
        // Campos privados
        private EstadCivil ecivil; // Estado civil
        private string email;       // Email o identificador del profesor
        private int peso;           // Peso del profesor
        private int estatura;       // Estatura del profesor
        private int id;             // Id único

        // Constructores
        public ProfesorExtendido() { }

        // Constructor sin Id
        public ProfesorExtendido(int estatura, int peso, string email, EstadCivil eCivil)
        {
            Estatura = estatura;
            Peso = peso;
            ProfesorFuncionarioId = email;
            ECivil = eCivil;
        }

        // Constructor con Id
        public ProfesorExtendido(int estatura, int peso, string email, int id, EstadCivil eCivil)
        {
            Estatura = estatura;
            Peso = peso;
            Id = id;
            ProfesorFuncionarioId = email;
            ECivil = eCivil;
        }

        // Propiedades públicas
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Estatura
        {
            get { return estatura; }
            set { estatura = value; }
        }

        public int Peso
        {
            get { return peso; }
            set { peso = value; }
        }

        // Email se usa también como Id de ProfesorFuncionario
        public string ProfesorFuncionarioId
        {
            get { return email; }
            set { email = value; }
        }

        public EstadCivil ECivil
        {
            get { return ecivil; }
            set { ecivil = value; }
        }

        // Método estático que genera una lista de ejemplo de profesores extendidos
        public static List<ProfesorExtendido> CrearListaProfesores()
        {
            List<ProfesorExtendido> listaProfesores = new List<ProfesorExtendido>();

            // Agregamos varios profesores con distintos valores de estatura, peso y estado civil
            listaProfesores.Add(new ProfesorExtendido(150, 75, "gamaa@trass.com", EstadCivil.Casado));
            listaProfesores.Add(new ProfesorExtendido(165, 82, "felol@trass.com", EstadCivil.Casado));
            listaProfesores.Add(new ProfesorExtendido(160, 68, "sadic@trass.com", EstadCivil.Divorciado));
            listaProfesores.Add(new ProfesorExtendido(172, 70, "roruj@trass.com", EstadCivil.Viudo));
            listaProfesores.Add(new ProfesorExtendido(185, 91, "pegol@trass.com", EstadCivil.Soltero));
            listaProfesores.Add(new ProfesorExtendido(155, 65, "mahem@trass.com", EstadCivil.Casado));
            listaProfesores.Add(new ProfesorExtendido(178, 88, "jimos@trass.com", EstadCivil.Soltero));
            listaProfesores.Add(new ProfesorExtendido(157, 79, "mogid@trass.com", EstadCivil.Viudo));
            listaProfesores.Add(new ProfesorExtendido(185, 85, "varoe@trass.com", EstadCivil.Casado));
            listaProfesores.Add(new ProfesorExtendido(153, 73, "natop@trass.com", EstadCivil.Divorciado));

            return listaProfesores;
        }
    }
}
