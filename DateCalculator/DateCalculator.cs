using System;
using System.Text.RegularExpressions;

namespace BrenoBatistaMachado
{
    /// <summary>
    /// Enum para representar a operação
    /// Caso não seja Adição ou Subtração ele será representado por Invalid.
    /// </summary>
    public enum Operator
    {
        Plus,
        Minus,
        Invalid
    }

    //Classe para encapsular a aritmética das datas.
    //Optei por utilizar uma classe com estados para evitar o uso de muitos parâmetros
    //nas classes
    /// <summary>
    /// Classe para encapsular a aritmética das datas.
    /// Optei por utilizar uma classe com estados para evitar o uso de muitos parâmetros
    /// nas classes
    /// </summary>
    public class DateCalculator
    {
        private int day;
        private int month;
        private int year;
        private long hour;
        private long minute;

		public static void Main()
		{
		}
		
        /// <summary>
        /// Realiza o calculo Soma/Subtração em uma data no formato: “dd/MM/yyyy HH24:mi”
        /// </summary>
        /// <param name="date">Data no formato dd/MM/yyyy HH24:mi</param>
        /// <param name="op">Operador: + ou -</param>
        /// <param name="value">Quantidade de minutos a ser calculada em cima da data, 
        /// de acordo com o parâmetro</param>
        /// <returns>Data no formato “dd/MM/yyyy HH24:mi” com o resultado da operação</returns>
        public string ChangeDate(string date, char op, long value)
        {
            if (!ParseDate(date))
            {
                throw new ArgumentException("Date in invalid.");
            }

            Operator opValue = ParseOperator(op);

            if (opValue == Operator.Invalid)
            {
                throw new ArgumentException("Operator invalid. The operator must be '+' or '-'.");
            }

            value = Math.Abs(value);

            switch(opValue)
            {
                case Operator.Plus:
                    SumMinutesInDate(value);
                    break;
                case Operator.Minus:
                    SubMinutesInDate(value);
                    break;
            }            
            
            return GetFormatedDate();
        }

        /// <summary>
        /// Realiza o parse da data nos atributos da classe
        /// </summary>
        /// <param name="date">String contendo a data</param>
        /// <returns>Retorna se houve sucesso ou não no Parse da data</returns>
        private bool ParseDate(string date)
        {
            if (!ValidateDateFormat(date))
            {
                throw new ArgumentException("Date in invalid format. The date must be in format: \"dd/MM/yyyy HH24:mi\".");
            }

            string[] datePieces = Regex.Split(date, @"\D+");

            try
            {
                this.day = int.Parse(datePieces[0]);
                this.month = int.Parse(datePieces[1]);
                this.year = int.Parse(datePieces[2]);
                this.hour = int.Parse(datePieces[3]);
                this.minute = int.Parse(datePieces[4]);
            }
            catch (InvalidCastException)
            {
                return false;
            }

            if (!ValidateDate())
                return false;

            if (!ValidadeTime())
                return false;

            return true;
        }

        //Valida se a string está no formato de data válida "dd/MM/yyyy HH24:mi"
        private bool ValidateDateFormat(string date)
        {
            Regex regex = new Regex(@"[0-3][0-9]\/[0-1][0-9]\/[0-9]{4} [0-2][0-9]:[0-5][0-9]");
            Match match = regex.Match(date);
            return match.Success;
        }

        /// <summary>
        /// Valida se a data (através dos atributos da classe) é válida
        /// </summary>
        /// <returns>Se a data é válida ou não</returns>
        private bool ValidateDate()
        {
            if (this.month > 12)
                return false;

            var maxDaysInMonth = GetDaysInMonth(this.month);

            if (this.day > maxDaysInMonth)
                return false;

            return true;
        }

        /// <summary>
        /// Valida se a hora (através dos atributos da classe) é válida
        /// </summary>
        /// <returns>Se a hora é válida ou não</returns>
        private bool ValidadeTime()
        {
            if (this.hour > 23)
                return false;

            if (this.minute > 59)
                return false;

            return true;
        }

        /// <summary>
        /// Retorna a quantidade máxima de dias que o mês pode ter
        /// </summary>
        /// <param name="month">Mês a verificar</param>
        /// <returns>Quantidade máxima de dias que o mês pode ter</returns>
        private int GetDaysInMonth(int month)
        {
            switch (this.month)
            {
                case 1:
                    goto case 3;
                case 3:
                    goto case 5;
                case 5:
                    goto case 7;
                case 7:
                    goto case 8;
                case 8:
                    goto case 10;
                case 10:
                    goto case 12;
                case 12:
                    return 31;

                case 4:
                    goto case 6;
                case 6:
                    goto case 9;
                case 9:
                    goto case 11;
                case 11:
                    return 30;

                case 2:
                    return 28;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Validar se o operador é válido e retorna o ENUM que representa o operador
        /// </summary>
        /// <param name="op">Operador: + ou -<//param>
        /// <returns>Retorna um enum Operator</returns>
        private Operator ParseOperator(char op)
        {
            switch(op)
            {
                case '+':
                    return Operator.Plus;
                case '-':
                    return Operator.Minus;
                default:
                    return Operator.Invalid;
            }
        }

        #region Métodos para Adição da data

        /// <summary>
        /// Adiciona os minutos da data
        /// </summary>
        /// <param name="minutes">Minutos a serem adicionados</param>
        private void SumMinutesInDate(long minutes)
        {
            long hours = SumMinutes(minutes);
            long days = SumHours(hours);

            while (days != 0)
            {
                days = SumDaysInDate(days);
            }
        }

        /// <summary>
        /// Adicona minutos. Caso os minutos a serem adicionados ultrapasse uma hora,
        /// é retornado a quantidade de horas que deverá ser incrementada
        /// </summary>
        /// <param name="minutes">Minutos a serem adicionados</param>
        /// <returns>Quantidade de horas que deverá ser incrementada</returns>
        private long SumMinutes(long minutes)
        {
            long min = this.minute + minutes;

            if (min > 59)
            {
                this.minute = min % 60;
                min = min / 60;
            }
            else
            {
                this.minute = min;
                min = 0;
            }

            return min;
        }

        /// <summary>
        /// Adiciona horas. Caso as horas ultrapasse 24 horas, é retornado a 
        /// quantidade de dias que derá ser adicionado.
        /// </summary>
        /// <param name="hours">Horas a serem adicionadas</param>
        /// <returns>Quantidade de dias que derá ser adicionado</returns>
        private long SumHours(long hours)
        {
            long h = this.hour + hours;

            if (h > 23)
            {
                this.hour = h % 24;
                h = h / 24;
            }
            else
            {
                this.hour = h;
                h = 0;
            }

            return h;
        }

        /// <summary>
        /// Adiciona dias na data. Caso tenha mais dias que o mês atual,
        /// ele irá adiconar os dias no mês e retornar a quantidade de dias
        /// que não foram adicionados ainda
        /// </summary>
        /// <param name="days">Dias a serem adicionados</param>
        /// <returns>Dias Faltantes a serem adicionados</returns>
        private long SumDaysInDate(long days)
        {
            var maxDaysInMonth = GetDaysInMonth(this.month);

            long d = days + this.day;

            if (d <= maxDaysInMonth)
            {
                this.day = (int)d;
                return 0;
            }

            SumMonth();

            d -= maxDaysInMonth;

            maxDaysInMonth = GetDaysInMonth(this.month);

            if (d <= maxDaysInMonth)
            {
                this.day = (int)d;
                return 0;
            }

            this.day = maxDaysInMonth;

            return d - maxDaysInMonth;
        }

        /// <summary>
        /// Adiciona um mês
        /// </summary>
        private void SumMonth()
        {
            if (this.month > 12)
            {
                this.month = 1;
                this.year++;
            }
            else
            {
                this.month++;
            }
        }
        
        #endregion

        #region Métodos para Subtação da data

        /// <summary>
        /// Subtrai os minutos da data
        /// </summary>
        /// <param name="minutes">Minutos a serem subtraidos</param>
        private void SubMinutesInDate(long minutes)
        {
            long hours = SubMinutes(minutes);
            long days = SubHours(hours);

            while (days != 0)
            {
                days = SubDaysInDate(days);
            }
        }

        /// <summary>
        /// Subtrai minutos. Caso os minutos a serem subtraídos seja negativo (deve subtrair horas),
        /// é retornado a quantidade de horas que deverá ser subtraida
        /// </summary>
        /// <param name="minutes">Minutos a serem removidos</param>
        /// <returns>Quantidade de horas que deverá ser subtraida</returns>
        private long SubMinutes(long minutes)
        {
            long min = this.minute - minutes;

            if (min < 0)
            {
                min = Math.Abs(min);
                this.minute = 60 - (min % 60);
                min = min / 60;
                min++;
            }
            else
            {
                this.minute = min;
                min = 0;
            }

            return min;
        }

        /// <summary>
        /// Remove horas. Caso as horas seja negativo (deve remover dias), é retornado a 
        /// quantidade de dias que derá ser subtraído.
        /// </summary>
        /// <param name="hours">Horas a serem subtraídas</param>
        /// <returns>Quantidade de dias que derá ser subtraído</returns>
        private long SubHours(long hours)
        {
            long h = this.hour - hours;

            if (h < 0)
            {
                h = Math.Abs(h);
                this.hour = 24 - (h % 24);

                h = h / 24;

                if (this.hour == 24)
                    this.hour = 0;
                else
                    h++;
            }
            else
            {
                this.hour = h;
                h = 0;
            }

            return h;
        }

        /// <summary>
        /// Subtrai dias na data. Caso tenha menos dias que o mês atual,
        /// ele irá remover os dias no mês e retornar a quantidade de dias
        /// que não foram removidas ainda
        /// </summary>
        /// <param name="days">Dias a serem removidos</param>
        /// <returns>Dias Faltantes a serem removidos</returns>
        private long SubDaysInDate(long days)
        {
            var maxDaysInMonth = GetDaysInMonth(this.month);

            long d = this.day - days;

            if (d > 0)
            {
                this.day = (int)d;
                return 0;
            }
           

            d = Math.Abs(d);

            SubMonth();
            
            maxDaysInMonth = GetDaysInMonth(this.month);
                        
            if (d <= maxDaysInMonth)
            {
                this.day = maxDaysInMonth - (int)d;

                if (this.day == 0)
                {
                    SubMonth();
                    this.day = GetDaysInMonth(this.month);
                }

                return 0;
            }

            this.day = maxDaysInMonth;

            return d;
        }

        /// <summary>
        /// Subtrai um mês
        /// </summary>
        private void SubMonth()
        {
            if (this.month == 1)
            {
                this.month = 12;
                this.year--;
            }
            else
            {
                this.month--;
            }
        }

        #endregion

        /// <summary>
        /// A partir dos atributos da classe, retorna a string que representa a data no formato "dd/MM/yyyy HH24:mi"
        /// </summary>
        /// <returns>Data no formato "dd/MM/yyyy HH24:mi"</returns>
        private string GetFormatedDate()
        {
            return string.Format("{0:D2}/{1:D2}/{2:D4} {3:D2}:{4:D2}", this.day, this.month, this.year, this.hour, this.minute);
        }        
    }
}
