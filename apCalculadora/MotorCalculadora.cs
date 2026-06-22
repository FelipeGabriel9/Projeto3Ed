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

        public ResultadoCalculo Calcular(string expressao)
        {
            if (string.IsNullOrWhiteSpace(expressao))
                throw new ArgumentException("A expressão não pode ser vazia.");

            List<string> tokens = Tokenizar(expressao);
            (double[] valores, string infixaLetras) = MapearOperandos(tokens);

            string posfixa = ConverterInfixaParaPosfixa(infixaLetras);
            double resultado = AvaliarPosfixa(posfixa, valores);

            return new ResultadoCalculo(infixaLetras, posfixa, valores, resultado);
        }

        private static List<string> Tokenizar(string expressao)
        {
            var tokens = new List<string>();
            int i = 0;

            while (i < expressao.Length)
            {
                char c = expressao[i];

                if (char.IsWhiteSpace(c)) { i++; continue; }

                bool ehSinalUnario = (c == '-' || c == '+')
                    && (tokens.Count == 0 || tokens[tokens.Count - 1] == "(");

                if (char.IsDigit(c) || c == '.' || ehSinalUnario)
                {
                    var sb = new StringBuilder();
                    if (ehSinalUnario) { sb.Append(c); i++; }

                    while (i < expressao.Length
                           && (char.IsDigit(expressao[i]) || expressao[i] == '.'))
                    {
                        sb.Append(expressao[i++]);
                    }

                    string numStr = sb.ToString();
                    if (!double.TryParse(numStr, NumberStyles.Any,
                                         CultureInfo.InvariantCulture, out _))
                        throw new FormatException(
                            $"Token numérico inválido: '{numStr}'.");

                    tokens.Add(numStr);
                    continue;
                }

                if (CaracteresOperadores.Contains(c))
                {
                    tokens.Add(c.ToString());
                    i++;
                    continue;
                }

                throw new FormatException(
                    $"Caractere inválido '{c}' na posição {i} da expressão.");
            }

            return tokens;
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

        /// <summary>
        /// Prioridade numérica: menor valor = maior prioridade aritmética.
        /// </summary>
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

        private static bool EhOperador(char c)
            => CaracteresOperadores.IndexOf(c) >= 0;

        private static bool EhOperadorOuParentese(string token)
            => token.Length == 1 && EhOperador(token[0]);
    }
}
