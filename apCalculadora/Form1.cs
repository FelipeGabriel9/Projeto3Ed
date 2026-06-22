using Calculadora.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto3Ed
{
    public partial class apCalculadora : Form
    {
        private bool _resultadoExibido = false;
        public apCalculadora()
        {
            InitializeComponent();
        }

        private void BtnCaractere_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string texto = btn.Text;

            // Se resultado foi exibido e o usuário pressionar um dígito, reinicia
            if (_resultadoExibido && (char.IsDigit(texto[0]) || texto == "(" || texto == "."))
            {
                txtVisor.Clear();
                txtResultado.Clear();
                lbSequencias.Text = "Pósfixa:";
                _resultadoExibido = false;
            }
            else if (_resultadoExibido && EhOperadorSimples(texto[0]))
            {
                // Continua operando com o resultado
                _resultadoExibido = false;
            }

            // Traduz os símbolos visuais para os caracteres internos
            switch (texto)
            {
                case "*": AppendVisor("*"); break;
                case "−": AppendVisor("-"); break;
                default: AppendVisor(texto); break;
            }
        }

        private void btnIgual_Click(object sender, EventArgs e)
        {
            string expressaoDigitada = txtVisor.Text.Trim();
            if (string.IsNullOrEmpty(expressaoDigitada)) return;

            try
            {
                ResultadoCalculo res = ResultadoCalculo.Calcular(expressaoDigitada);

                var sb = new StringBuilder();
                sb.AppendLine("Infixa  (letras): " + res.Infixa);
                sb.AppendLine("Pósfixa (letras): " + res.Posfixa);

                // Mostra os valores das letras usadas
                var sbValores = new StringBuilder("Valores: ");
                for (int i = 0; i < res.Infixa.Length; i++)
                {
                    char c = res.Infixa[i];
                    if (c >= 'A' && c <= 'Z')
                        sbValores.AppendFormat("{0}={1}  ", c, FormatarDouble(res.Valores[c - 'A']));
                }
                sb.Append(sbValores.ToString().TrimEnd());

                lbSequencias.Text = sb.ToString();

                string resultStr = FormatarDouble(res.Resultado);
                txtResultado.Text = resultStr;
                txtVisor.Text = resultStr;
                _resultadoExibido = true;
            }
            catch (DivideByZeroException)
            {
                MostrarErro("Erro: Divisão por zero!");
            }
            catch (Exception ex)
            {
                MostrarErro("Erro: " + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtVisor.Clear();
            txtResultado.Clear();
            lbSequencias.Text = "Pósfixa:";
            _resultadoExibido = false;
            txtVisor.Focus();
        }
        private void txtVisor_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            bool valido =
                char.IsDigit(c) ||   // 0-9
                c == '.' ||   // ponto decimal
                c == '+' ||
                c == '-' ||
                c == '*' ||
                c == '/' ||
                c == '^' ||
                c == '(' ||
                c == ')' ||
                c == (char)8 ||   // Backspace
                c == (char)13;             // Enter → calcula

            if (!valido)
            {
                e.Handled = true;  // Bloqueia o caractere inválido
                return;
            }

            if (c == (char)13)
            {
                e.Handled = true;
                btnIgual_Click(sender, EventArgs.Empty);
            }
        }

        private void txtVisor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                btnClear_Click(sender, EventArgs.Empty);
                e.Handled = true;
            }
        }

        private void AppendVisor(string s)
        {
            txtVisor.Text += s;
            txtVisor.SelectionStart = txtVisor.Text.Length;
            txtVisor.Focus();
        }

        private bool EhOperadorSimples(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
        }

        private void MostrarErro(string msg)
        {
            txtResultado.Text = msg;
            txtResultado.ForeColor = Color.OrangeRed;
            lbSequencias.Text = "";
            _resultadoExibido = false;
        }

        private string FormatarDouble(double v)
        {
            // Remove zeros desnecessários
            if (v == Math.Floor(v) && !double.IsInfinity(v))
                return ((long)v).ToString();
            return v.ToString("G10", System.Globalization.CultureInfo.InvariantCulture);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btnIgual_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
