using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TruthTableGenerator
{
    class TruthTableGenerator
    {
        private static Dictionary<char, uint> mOperatorPrecedence = new Dictionary<char, uint>();

        static TruthTableGenerator()
        {
            mOperatorPrecedence.Add('~', 3);
            mOperatorPrecedence.Add('*', 2);
            mOperatorPrecedence.Add('+', 1);
            mOperatorPrecedence.Add('(', 0);
            mOperatorPrecedence.Add(')', 0);
        }

        /// <summary>
        /// BooleanAlgebra write rule
        /// 1.input variable is uppercase 
        /// 2.not operator is '~'
        /// 3.and operator is '*'
        /// 4.or operator is '+'
        /// </summary>
        /// <param name="booleanAlgebra"></param>
        /// <returns>
        /// Truth Table
        /// </returns>
        public static string GetTruthTable(string booleanAlgebra)
        {
            List<char> postfixNotation = getPostfixNotation(booleanAlgebra);
            List<char> operands = GetVariables(booleanAlgebra);

            //진리표 만들기
            int x = (int)Math.Pow(2, operands.Count);
            int y = operands.Count + 1;
            char[,] truthTable = new char[x, y];

            for (int j = 0; j < truthTable.GetLength(1); j++)
            {
                truthTable[0, j] = '0';
            }
            
            //경우의 수 입력
            for (int i = 1; i < truthTable.GetLength(0); i++)
            {
                char carry = '1';
                for (int j = 0; j < truthTable.GetLength(1)-1; j++)
                {
                    switch (truthTable[i - 1, j])
                    {
                        case '0':
                            if (carry == '1')
                            {
                                truthTable[i, j] = '1';
                                carry = '0';
                            }
                            else
                            {
                                truthTable[i, j] = '0';
                            }
                            break;
                        case '1':
                            if (carry == '1')
                            {
                                truthTable[i, j] = '0';
                                carry = '1';
                            }
                            else
                            {
                                truthTable[i, j] = '1';
                            }
                            break;
                        default:
                            Debug.Assert(false);
                            throw new Exception();
                    }
                }
            }

            for (int i = 0; i < truthTable.GetLength(0); i++)
            {
                Dictionary<char, char> variables = new Dictionary<char, char>();
                for (int j = 0; j < operands.Count; j++)
                {
                    variables.Add(operands[j], truthTable[i, j]);
                }

                truthTable[i, operands.Count] = calculatePostfixNotation(postfixNotation, variables);
            }

            StringBuilder sbReturn = new StringBuilder(truthTable.Length * 2);

            foreach (var item in operands)
            {
                sbReturn.Append(item + "\t");
            }

            sbReturn.Append("out\n");

            for (int i = 0; i < truthTable.GetLength(0); i++)
            {
                for (int j = 0; j < truthTable.GetLength(1); j++)
                {
                    sbReturn.Append(truthTable[i, j] + "\t");
                }
                sbReturn.Append("\n");
            }

            return sbReturn.ToString();
        }

        public static List<char> GetVariables(string booleanAlgebra)
        {            
            HashSet<char> variables = new HashSet<char>();

            foreach (var element in booleanAlgebra.ToCharArray())
            {
                //ASCII CODE Upper case
                if (bIsVariable(element))
                {
                    variables.Add(element);
                }
            }

            List<char> returnList = new List<char>(variables.Count);

            foreach (var item in variables)
            {
                returnList.Add(item);
            }

            return returnList;
        }

        public static void TestPrivate()
        {
            string returnValue;
            returnValue = getString(getPostfixNotation("A"));
            Debug.Assert(returnValue == "A");

            returnValue = getString(getPostfixNotation("A+B"));
            Debug.Assert(returnValue == "AB+");

            returnValue = getString(getPostfixNotation(" A * B + C"));
            Debug.Assert(returnValue == "AB*C+");

            returnValue = getString(getPostfixNotation("A * ( B + C )"));
            Debug.Assert(returnValue == "ABC+*");

            returnValue = getString(getPostfixNotation("( A * ( ~B + C ))"));
            Debug.Assert(returnValue == "AB~C+*");

            List<char> returnList;
            Dictionary<char, char> variables = new Dictionary<char, char>();
            variables.Add('A', '1');
            variables.Add('B', '1');
            variables.Add('C', '1');

            returnList = getPostfixNotation("A");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '1');

            returnList = getPostfixNotation("~A");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '0');

            returnList = getPostfixNotation("A * B");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '1');

            returnList = getPostfixNotation("A * ~B");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '0');

            returnList = getPostfixNotation("A + B");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '1');

            returnList = getPostfixNotation("~A * B");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '0');

            returnList = getPostfixNotation("~A * ~B");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '0');

            returnList = getPostfixNotation("( A * ( ~B + C ))");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '1');

            returnList = getPostfixNotation("( A * ( ~B + ~C ))");
            Debug.Assert(calculatePostfixNotation(returnList, variables) == '0');
        }

        private static string getString(List<char> list)
        {
            StringBuilder sb = new StringBuilder(list.Count);
            foreach (var item in list)
            {
                sb.Append(item);
            }

            return sb.ToString();
        }

        private static List<char> getPostfixNotation(string BooleanAlgebra)
        {
            List<char> listExp = new List<char>(BooleanAlgebra.Length);
            Stack<char> operatorStack = new Stack<char>();

            foreach (var element in BooleanAlgebra.ToCharArray())
            {
                if (element == ' ')
                {
                    continue;
                }

                //ASCII CODE Upper case
                if (element >= 65 && element <= 90)
                {
                    listExp.Add(element);
                    continue;
                }

                switch (element)
                {
                    case '(':
                        operatorStack.Push(element);
                        break;
                    case ')':
                        while (operatorStack.Count != 0)
                        {
                            if (operatorStack.Peek() != '(')
                            {
                                listExp.Add(operatorStack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }

                        operatorStack.Pop();
                        break;
                    case '+':
                    case '*':
                    case '~':
                        while (operatorStack.Count != 0)
                        {
                            if (mOperatorPrecedence[operatorStack.Peek()] >= mOperatorPrecedence[element])
                            {
                                // 스택에 있는 연산자의 우선 순위가 자신보다 높거나 같다면 출력 리스트에 이어 붙여준다.
                                listExp.Add(operatorStack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }

                        operatorStack.Push(element);
                        break;
                    default:
                        Debug.Assert(false, "여기로 오면 안됨");
                        throw new Exception();
                }
            }

            while (operatorStack.Count != 0)
            {
                listExp.Add(operatorStack.Pop());
            }

            return listExp;
        }
        
        private static char calculatePostfixNotation(List<char> postfixNotation, Dictionary<char, char> variables)
        {
            Stack<char> operandStack = new Stack<char>();
            foreach (var element in postfixNotation)
            {
                if (bIsVariable(element))
                {
                    operandStack.Push(variables[element]);
                    continue;
                }

                switch (element)
                {
                    case '~':
                        {
                            char operand = operandStack.Pop();
                            operandStack.Push(operateNot(operand));
                        }
                        break;
                    case '*':
                        {
                            char operand1 = operandStack.Pop();
                            char operand2 = operandStack.Pop();
                            operandStack.Push(operateAnd(operand1, operand2));
                        }
                        break;
                    case '+':
                        {
                            char operand1 = operandStack.Pop();
                            char operand2 = operandStack.Pop();
                            operandStack.Push(operateOr(operand1, operand2));
                        }
                        break;
                    default:
                        break;
                }
            }

            return operandStack.Pop();
        }

        private static bool bIsVariable(char element)
        {
            return (element >= 65 && element <= 90);
        }

        private static char operateNot(char operand)
        {
            switch (operand)
            {
                case '0':
                    return '1';
                case '1':
                    return '0';
                default:
                    Debug.Assert(false);
                    throw new Exception();
            }
        }

        private static char operateAnd(char operand1, char operand2)
        {
            if (operand1 == '1' && operand2 == '1')
            {
                return '1';
            }
            else
            {
                return '0';
            }
        }
        private static char operateOr(char operand1, char operand2)
        {
            if (operand1 == '1' || operand2 == '1')
            {
                return '1';
            }
            else
            {
                return '0';
            }
        }
    }
}
