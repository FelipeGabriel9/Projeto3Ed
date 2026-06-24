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
        private bool exibiuResultado = false;
        public apCalculadora()
        {
            InitializeComponent();
        }

        private void BtnCaractere_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string texto = btn.Text;

           
            if (exibiuResultado && (char.IsDigit(texto[0]) || texto == "(" || texto == "."))
            {
                txtVisor.Clear();
                txtResultado.Clear();
                lbSequencias.Items.Add("Pósfixa:");
                exibiuResultado = false;
            }
            else if (exibiuResultado && EhOperadorSimples(texto[0]))
            {
            
                exibiuResultado = false;
            }

            
            switch (texto)
            {
                case "*": txtVisor.Text += "*"  ;  break;
                case "−": txtVisor.Text += "-"  ;  break;
                default : txtVisor.Text += texto;  break;
            }
        }

        private void btnIgual_Click(object sender, EventArgs e)
        {
            string expressaoDigitada = txtVisor.Text.Trim();

            if (string.IsNullOrEmpty(expressaoDigitada))
                return;

            try
            {
                ResultadoCalculo resultado = ResultadoCalculo.Calcular(expressaoDigitada);

                var sb = new StringBuilder();
                sb.AppendLine("Infixa  (letras): " + resultado.Infixa);
                sb.AppendLine("Pósfixa (letras): " + resultado.Posfixa);

                
                var sbValores = new StringBuilder("Valores: ");
                for (int i = 0; i < resultado.Infixa.Length; i++)
                {
                    char c = resultado.Infixa[i];
                    if (c >= 'A' && c <= 'Z')
                        sbValores.AppendFormat("{0}={1}  ", c, FormatarDouble(resultado.Valores[c - 'A']));
                }
                sb.Append(sbValores.ToString().TrimEnd());

                lbSequencias.Text = sb.ToString();

                string resultadoString = FormatarDouble(resultado.Resultado);
                txtResultado.Text = resultadoString;
                txtVisor.Text = resultadoString;
                exibiuResultado = true;
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
            exibiuResultado = false;
            txtVisor.Focus();
        }
        private void txtVisor_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            bool valido =
                char.IsDigit(c) ||   //Di
                c == '.' ||   // Decimal
                c == '+' ||
                c == '-' ||
                c == '*' ||
                c == '/' ||
                c == '^' ||
                c == '(' ||
                c == ')' ||
                c == (char)8 ||   // Espaço
                c == (char)13;    // Calcula

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



        private bool EhOperadorSimples(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
        }

        private void MostrarErro(string msg)
        {
            txtResultado.Text = msg;
            txtResultado.ForeColor = Color.Red;
            lbSequencias.Text = "";
            exibiuResultado = false;
        }

        private string FormatarDouble(double valorResultado)
        {
            return valorResultado.ToString();
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
