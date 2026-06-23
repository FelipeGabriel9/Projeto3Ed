using System;
using System.Collections.Generic;
using System.Text;

public class ResultadoCalculo
{
    // Propriedades da classe 
    public string Infixa { get; }
    public string Posfixa { get; }
    public double[] Valores { get; }
    public double Resultado { get; }

    // Construtor da classe
    public ResultadoCalculo(string infixa, string posfixa, double[] valores, double resultado)
    {
        Infixa = infixa;
        Posfixa = posfixa;
        Valores = valores;
        Resultado = resultado;
    }

    // Método que percorre toda a expressão recebida
    public List<string> LerCadeia(string cadeia)
    {
        var dados = new List<string>();
        int indice = 0;

        while (indice < cadeia.Length)   // Enquanto não percorreu a expressão inteira
        {
            char caractere = cadeia[indice];

            if (caractere == ' ')   // Se houver espaço, anda para o próximo caractere
                indice++;

            if (char.IsDigit(caractere) || caractere == '.')    // Verifica se o caractere é um decimal (está entre 0 e 9) e se é um ponto (.)
            {
                var numero = "";
                while (indice < cadeia.Length && (char.IsDigit(cadeia[indice]) || cadeia[indice] == '.'))  // Enquanto a expressão não chegou ao fim e é um caratere válido
                {
                    numero += cadeia[indice];   // Adiciona o dado lido à variável numero
                    indice++;
                }
                dados.Add(numero.ToString());   // Adiciona o número à lista de dados
                continue;
            }

            if (caractere == '-' && (dados.Count == 0 || dados[dados.Count - 1] == "("))    // Verifica se o caractere é '-' e se está antes de um número
            {
                var numero = "";
                numero += caractere;
                indice++;

                while (indice < cadeia.Length && (char.IsDigit(cadeia[indice]) || cadeia[indice] == '.'))  // Enquanto a expressão não chegou ao fim e é um caratere válido
                {
                    numero += cadeia[indice];   // Adiciona o dado lido à variável numero
                    indice++;
                }

                if (numero.Length > 1)  // Se há valores inseridos na variável
                {
                    dados.Add(numero.ToString());   // Adicionamos na lista de dados
                    continue;
                }

                else
                {
                    dados.Add("-");     // Caso contrário, adicionamos apenas o sinal de menos
                    continue;
                }
            }

            dados.Add(caractere.ToString());    // Adiciona o caractere à lista de dados
            indice++;
        }
        return dados;   // Retorna a lista de dados
    }

    public static void CriarVetorEInfixa(List<string> tokens, out double[] valores, out string infixaLetras)
    {
        valores = new double[26];
        var sb = new StringBuilder();
        int letraIdx = 0;

        foreach (string tok in tokens)
        {
            double num;
            if (double.TryParse(tok, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out num))
            {
                if (letraIdx >= 26)
                    throw new Exception("Expressão possui mais de 26 operandos distintos.");
                valores[letraIdx] = num;
                sb.Append((char)('A' + letraIdx));
                letraIdx++;
            }
            else
            {
                sb.Append(tok);
            }
        }
        infixaLetras = sb.ToString();
    }

    private bool EhOperador(char caractere)      // Método que verifica se o caractere é um operador
    {
        return caractere == '+' || caractere == '-' || caractere == '*' || caractere == '/' || caractere == '^' ||
                caractere == '(' || caractere == ')';
    }

    private bool TemPrecedencia(char topo, char lido)
    {
        if (lido == '(') 
            return false;

        if (topo == '(') 
            return false;

        int prioTopo = Prioridade(topo);
        int prioLido = Prioridade(lido);

        if (lido == '^')
            return prioTopo > prioLido;

        return prioTopo >= prioLido;
    }

    // Método que devolve a prioridade de um operando
    private static int Prioridade(char operador)
    {
        switch (operador)
        {
            case '^': return 3;
            case '*': return 2;
            case '/': return 2;
            case '+': return 1;
            case '-': return 1;
            default: return 0;
        }
    }

    string ConverterInfixaParaPosfixa(string cadeiaLida)
    {
        string resultado = "";
        var umaPilha = new PilhaLista<char>(); // Instancia a Pilha

        for (int indice = 0; indice < cadeiaLida.Length; indice++)
        {
            char simboloLido = cadeiaLida[indice];

            if (!EhOperador(simboloLido)) // Se for operando (letras A-Z)
            {
                resultado += simboloLido; // adiciona no resultado
            }
            else // símbolo é operador ou parêntese
            {
                bool parar = false;

                while (!parar && !umaPilha.EstaVazia && TemPrecedencia(umaPilha.oTopo(), simboloLido))
                {
                    char operadorComMaiorPrecedencia = umaPilha.Desempilhar();
                    if (operadorComMaiorPrecedencia != '(')
                    {
                        resultado += operadorComMaiorPrecedencia;
                    }
                    else
                    {
                        parar = true;
                    }
                }

                if (simboloLido != ')')
                {
                    umaPilha.Empilhar(simboloLido);
                }
                else
                {
                    // Se o símbolo lido for ')', remove o '(' correspondente que sobrou no topo da pilha
                    if (!umaPilha.EstaVazia && umaPilha.oTopo() == '(')
                    {
                        umaPilha.Desempilhar();
                    }
                }
            }
        } // fim do for

        // Descarrega o restante da Pilha para a Saída
        while (!umaPilha.EstaVazia)
        {
            char operadorRestante = umaPilha.Desempilhar();
            if (operadorRestante != '(')
            {
                resultado += operadorRestante;
            }
        }

        return resultado;
    }

    private static double ValorDaSubExpressao(double op1, char operador, double op2)
    {
        switch (operador)
        {
            case '+': return op1 + op2;
            case '-': return op1 - op2;
            case '*': return op1 * op2;
            case '/':
                if (op2 == 0) 
                    throw new Exception("Não é possível dividir por zero.");
                return op1 / op2;
            case '^': return Math.Pow(op1, op2);
            default: throw new Exception("Operador desconhecido: " + operador);
        }
    }

    double ValorDaExpressaoPosfixa(string cadeiaPosfixa, double[] valoresOperandos)
    {
        var umaPilha = new PilhaLista<double>();

        for (int atual = 0; atual < cadeiaPosfixa.Length; atual++)
        {
            char simbolo = cadeiaPosfixa[atual];

            if (!EhOperador(simbolo)) // É Operando (Letra de 'A' a 'Z')
            {
                // Subtrai o caractere 'A' para descobrir o índice correto no vetor.
                // Exemplo: 'A' - 'A' = 0; 'B' - 'A' = 1; 'C' - 'A' = 2...
                int indice = simbolo - 'A';
                umaPilha.Empilhar(valoresOperandos[indice]);
            }
            else // É Operador (+, -, *, /, ^)
            {
                // ATENÇÃO: O primeiro elemento desempilhado é o SEGUNDO operando da conta
                double operando2 = umaPilha.Desempilhar();
                double operando1 = umaPilha.Desempilhar();

                // Calcula o resultado da subexpressão (ex: operando1 + operando2)
                double valorParcial = ValorDaSubExpressao(operando1, simbolo, operando2);

                // Empilha o resultado parcial para ser usado nos próximos operadores
                umaPilha.Empilhar(valorParcial);
            }
        }

        // O último elemento que restar na pilha é o resultado final da expressão inteira
        return umaPilha.Desempilhar();
    }

    public static ResultadoCalculo Calcular(string expressaoDigitada)
    {
        // Criamos uma instância temporária da classe para poder acessar os métodos que não são estáticos
        var motor = new ResultadoCalculo("", "", new double[0], 0);

        // 1. Tokeniza a expressão usando o método da instância
        var tokens = motor.LerCadeia(expressaoDigitada);

        // 2. Cria o vetor de valores reais e substitui por letras (Método estático)
        double[] valores;
        string infixaLetras;
        CriarVetorEInfixa(tokens, out valores, out infixaLetras);

        // 3. Converte a cadeia de letras infixa para pós-fixa usando a instância
        string posfixa = motor.ConverterInfixaParaPosfixa(infixaLetras);

        // 4. Avalia o resultado final da expressão pós-fixa usando a instância
        double resultado = motor.ValorDaExpressaoPosfixa(posfixa, valores);

        // 5. Retorna o objeto final com todos os dados calculados com sucesso
        return new ResultadoCalculo(infixaLetras, posfixa, valores, resultado);
    }
}