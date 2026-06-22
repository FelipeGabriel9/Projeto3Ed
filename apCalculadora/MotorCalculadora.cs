using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Calculadora.Core
{
    public sealed class MotorCalculadora
    {
        private const string CaracteresOperadores = "^*/+-()";

        public ResultadoCalculo Calcular(string cadeia)
        {
            if (string.IsNullOrWhiteSpace(cadeia))
                throw new ArgumentException("A expressão não pode ser vazia.");

            var dados = LerCadeia(cadeia);
            (double[] valores, string infixaLetras) = MapearOperandos(dados);

            string posfixa = ConverterInfixaParaPosfixa(infixaLetras);
            double resultado = AvaliarPosfixa(posfixa, valores);

            return new ResultadoCalculo(infixaLetras, posfixa, valores, resultado);
        }

        private  List<string> LerCadeia(string cadeia)
        {
            List<string> dados = new List<string>();
            int i = 0;

            while (i < cadeia.Length)
            {
                    char c = cadeia[i];

                    if (c == ' ')
                        i++;
           
                    else if ((c >= '0' && c <= '9') || c == '.' || ((c == '+' || c == '-') && (dados.Count == 0 || dados[dados.Count - 1] == "(")))
                    {
                        string numero = LerNumero(cadeia, ref i, dados);
                        dados.Add(numero);
                    } 

                    else if (EhOperador(c))
                    {
                        dados.Add(c.ToString());
                        i++;
                    }
                    else
                    {
                        throw new FormatException(
                            $"Caractere inválido '{c}' na posição {i}.");
                    }
            }
            return dados;
        }


        private static bool EhOperador(char c)
        {
            return c == '+' ||
                   c == '-' ||
                   c == '*' ||
                   c == '/' ||
                   c == '^' ||
                   c == '(' ||
                   c == ')';
        }
        private static string LerNumero(string cadeia, ref int i, List<string> dados)
        {
            string numero = "";
            if ((cadeia[i] == '+' || cadeia[i] == '-') &&
                (dados.Count == 0 || dados[dados.Count - 1] == "("))
            {
                numero += cadeia[i];
                i++;
            }
            while (i < cadeia.Length)
            {
                char c = cadeia[i];

                if ((c >= '0' && c <= '9') || c == '.')
                {
                    numero += c;
                    i++;
                }
                else
                    break;
            }
            double.Parse(numero);
            return numero;
        }


        private static (double[] valores, string infixaLetras)
            MapearOperandos(List<string> tokens)
        {
            const int maxOperandos = 26; // letras A–Z
            var valores = new List<double>();
            var sb = new StringBuilder();
            int proximaLetra = 0;

            foreach (string token in tokens)
            {
                if (EhOperadorOuParentese(token))
                {
                    sb.Append(token[0]);
                }
                else
                {
                    if (proximaLetra >= maxOperandos)
                        throw new OverflowException(
                            "A expressão possui mais de 26 operandos distintos.");

                    double valor = double.Parse(token, CultureInfo.InvariantCulture);
                    valores.Add(valor);

                    sb.Append((char)('A' + proximaLetra));
                    proximaLetra++;
                }
            }

            return (valores.ToArray(), sb.ToString());
        }

        private static string ConverterInfixaParaPosfixa(string infixaLetras)
        {
            var resultado = new StringBuilder();
            var pilha = new Pilha<char>();

            foreach (char simboloLido in infixaLetras)
            {
                if (!EhOperador(simboloLido))
                {
                    // Operando: vai direto para a saída
                    resultado.Append(simboloLido);
                }
                else
                {
                    // Operador: desempilha enquanto o topo tiver precedência
                    bool parar = false;

                    while (!parar && !pilha.EstaVazia
                           && TemPrecedencia(pilha.OTopo(), simboloLido))
                    {
                        char topo = pilha.Desempilhar();

                        if (topo != '(')
                            resultado.Append(topo);
                        else
                            parar = true; // encontrou '(' ao tratar um ')'
                    }

                    // ')' nunca vai para a pilha; remove o '(' correspondente
                    if (simboloLido == ')')
                    {
                        if (!pilha.EstaVazia && pilha.OTopo() == '(')
                            pilha.Desempilhar();
                    }
                    else
                    {
                        pilha.Empilhar(simboloLido);
                    }
                }
            }

            // Descarrega o restante da pilha
            while (!pilha.EstaVazia)
            {
                char op = pilha.Desempilhar();
                if (op != '(') resultado.Append(op);
            }

            return resultado.ToString();
        }

        private static double AvaliarPosfixa(string posfixa, double[] valoresOperandos)
        {
            var pilha = new Pilha<double>();

            foreach (char simbolo in posfixa)
            {
                if (!EhOperador(simbolo))
                {
                    // Letra → busca valor pelo índice (A=0, B=1, …)
                    int indice = simbolo - 'A';
                    pilha.Empilhar(valoresOperandos[indice]);
                }
                else
                {
                    double operando2 = pilha.Desempilhar();
                    double operando1 = pilha.Desempilhar();
                    double resultado = ValorDaSubExpressao(operando1, simbolo, operando2);
                    pilha.Empilhar(resultado);
                }
            }

            return pilha.Desempilhar(); // único elemento restante = resultado final
        }

        private static double ValorDaSubExpressao(double operando1, char op, double operando2)
        {
            switch (op)
            {
                case '+':
                    return operando1 + operando2;

                case '-':
                    return operando1 - operando2;

                case '*':
                    return operando1 * operando2;

                case '/':
                    if (operando2 == 0)
                        throw new DivideByZeroException("Divisão por zero detectada.");
                    return operando1 / operando2;

                case '^':
                    return Math.Pow(operando1, operando2);

                default:
                    throw new InvalidOperationException(
                        $"Operador desconhecido: '{op}'.");
            }
        }

        private static bool TemPrecedencia(char topo, char lido)
        {
            // '(' na pilha nunca tem precedência sobre nada
            if (topo == '(') return false;

            // '(' lido nunca é precedido por nada (sempre empilha)
            if (lido == '(') return false;

            // ')' lido: desempilha tudo até '('
            if (lido == ')') return true;

            // ^ é direito-associativo: ao ler '^' com '^' no topo, empilha (não desempilha)
            if (topo == '^' && lido == '^') return false;

            return PrioridadeNumerica(topo) <= PrioridadeNumerica(lido);
        }
        
        private static int PrioridadeNumerica(char op)
        {
            switch (op)
            {
                case '^':
                    return 1;

                case '*':
                case '/':
                    return 2;

                case '+':
                case '-':
                    return 3;

                default:
                    return int.MaxValue;
            }
        }
       

        private static bool EhOperadorOuParentese(string token)
            => token.Length == 1 && EhOperador(token[0]);
    }
}
