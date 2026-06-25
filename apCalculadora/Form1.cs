using System;
using System.Drawing;
using System.Text;
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

        // Evento executado quando um botão de caractere é clicado
        private void BtnCaractere_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string texto = btn.Text;

            // Se o resultado já foi exibido e o usuário digitar um número, ponto ou parêntese, limpa os campos
            if (exibiuResultado && (char.IsDigit(texto[0]) || texto == "(" || texto == "."))
            {
                txtVisor.Clear();
                txtResultado.Clear();
                lbSequencias.Text = "Pósfixa:";
                exibiuResultado = false;
            }

            // Se o usuário pressionar um operador após o resultado, permite continuar os cálculos usando o resultado obtido
            else if (exibiuResultado && EhOperadorSimples(texto[0]))
                exibiuResultado = false;

            txtVisor.Text += texto;
        }


        // Evento executado quando o botão "=" é clicado
        private void btnIgual_Click(object sender, EventArgs e)
        {
            string expressaoDigitada = txtVisor.Text.Trim();

            // Não caulcula se a expressão estiver vazia
            if (string.IsNullOrEmpty(expressaoDigitada))
                return;

            try
            {
                // Calcula o resultado da expressão digitada
                var resultado = CalcularResultado.Calcular(expressaoDigitada);

                // Exibe no label a expressão em infixa, pósfixa e os valores das variáveis
                var texto = "";
                texto += "Infixa : " + resultado.Infixa + "\n";
                texto += "Pósfixa: " + resultado.Posfixa + "\n";

                var textoValores = "Valores: ";
                for (int i = 0; i < resultado.Infixa.Length; i++)
                {
                    char caractere = resultado.Infixa[i];
                    // Se o caractere for uma letra entre A e Z, adiciona o valor correspondente ao textoValores
                    if (caractere >= 'A' && caractere <= 'Z')
                    {
                        var formatado = resultado.Valores[caractere - 'A'].ToString();
                        textoValores += $"{caractere}={formatado}  ";
                    }
                }
                texto += textoValores.ToString().TrimEnd();

                // Exibe o resultado no label e no textbox
                lbSequencias.Text = texto;
                var resultadoString = resultado.Resultado.ToString();
                txtResultado.Text = resultadoString;
                txtVisor.Text = resultadoString;
                exibiuResultado = true;
            }
            catch (Exception ex)
            {
                // Exibe uma mensagem de erro para expressão inválida 
                MostrarErro("Erro: " + ex.Message);
            }
        }

        // Evento executado quando o botão "C" é clicado
        private void btnClear_Click(object sender, EventArgs e)
        {
            // Limpa os campos e reseta o estado da calculadora
            txtVisor.Clear();
            txtResultado.Clear();
            lbSequencias.Text = "";
            exibiuResultado = false;
            txtVisor.Focus();
        }

        // Valida os caracteres digitados
        private void txtVisor_KeyPress(object sender, KeyPressEventArgs e)
        {
            char caractere = e.KeyChar;

            bool valido =
                char.IsDigit(caractere) ||      // Se for um dígito
                caractere == '.' ||             // Decimal
                caractere == '+' ||
                caractere == '-' ||
                caractere == '*' ||
                caractere == '/' ||
                caractere == '^' ||
                caractere == '(' ||
                caractere == ')' ||
                caractere == (char)8 ||   // Espaço
                caractere == (char)13;    // Enter

            // Impede a digitação de caracteres inválidos
            if (!valido)
            {
                e.Handled = true; 
                return;
            }

            // Realiza o cálculo quando o Enter é pressionado
            if (caractere == (char)13)
            {
                e.Handled = true;
                btnIgual_Click(sender, EventArgs.Empty);
            }
        }


        // Verifica se o caractere é um operador simples
        private bool EhOperadorSimples(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
        }

        // Exibe uma mensagem de erro, quando necessário
        private void MostrarErro(string msg)
        {
            txtResultado.Text = msg;
            txtResultado.ForeColor = Color.Red;
            lbSequencias.Text = "";
            exibiuResultado = false;
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
