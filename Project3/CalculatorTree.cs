using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project3
{
    public class CalculatorTree
    {
        // defining stuff
        static char[] operators = ['+', '-', '*', '/','%', '^'];
        static string[] numbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
        // PEMDAS priority list
        static char[] class3ops = ['+', '-'];
        static char[] class2ops = ['*', '/', '%'];
        static char[] class1ops = ['^']; // this one's an array for consistency
        Dictionary<string, double> variables = new Dictionary<string, double>();

        public static void SaveVars(Dictionary<string, double> vars, string filepath)
        {
            using StreamReader sr = new StreamReader(filepath);
            for (int i = 0; i < vars.Count; i++)
            {
                

            }
        }
        // check to see if a string has specific characters
        public static bool CheckForOperators (string input)
        {
            bool hasChars = false;
            for (int i = 0; i < operators.Length; i++)
            {
                if (input.Contains(operators[i]))
                {
                    hasChars = true;
                    break;
                }
            }
            return hasChars;
        }
        
        // check for characters outside of the ones the calc will use
        public static bool CheckForUnusualStuff(string input)
        {
            string unusRegex = @"[0-9.]";
            if (Regex.IsMatch(input, unusRegex) == false)
            {
                return true;
            }
            return false;
        }

        // tree node class & constructor
        public class TreeNode<T>
        {
            public string value;
            public List<TreeNode<string>> children;
            public TreeNode(string value)
            {
                this.value = value;
                children = new List<TreeNode<string>>();
            }
        }

        // Search for a operator and create a new node if there is one. If not return the original node.
        public static TreeNode<string> CreateNode(string input, TreeNode<string> ogNode)
        {
            for (int i = input.Length; i > 0; i--)
            {
                if (class3ops.Contains(input[i-1]))
                {
                    TreeNode<string> parent = new TreeNode<string>(Convert.ToString(input[i-1]));
                    return parent;
                }
            }
            for (int i = input.Length; i > 0; i--)
            {
                if (class2ops.Contains(input[i-1]))
                {
                    TreeNode<string> parent = new TreeNode<string>(Convert.ToString(input[i-1]));
                    return parent;
                }
            }
            for (int i = input.Length; i > 0; i--)
            {
                if (class1ops.Contains(input[i-1]))
                {
                    TreeNode<string> parent = new TreeNode<string>(Convert.ToString(input[i-1]));
                    return parent;
                }
            }
            // return the inputted node if nothing was found
            return ogNode;
        }

        public static bool NodeIsAnOperator(TreeNode<string> node)
        {
            bool isAnOperator = false;
            for (int i = 0; i < operators.Length; i++)
            {
                if (node.value.Equals(Convert.ToString(operators[i])))
                {
                    isAnOperator = true;
                    break;
                }
            }
            return isAnOperator;
        }
        // split the string at the last delimiter instead of the first
        // courtesy of Phil K at https://stackoverflow.com/questions/21733756/best-way-to-split-string-by-last-occurrence-of-character
        public static string[] SplitAtLastDelimiter(string input, string delimiter)
        {
            string[] output = new string[2];
            int index = input.LastIndexOf(delimiter);
            if (index != -1)
            {
                output[0] = input.Substring(0, index);
                output[1] = input.Substring(index + 1);
            }
            return output;
        }
        
        // create a tree that represents the equation
        public static TreeNode<string> CreateTree(TreeNode<string> parent)
        {
            TreeNode<string> newNode = new TreeNode<string>(parent.value);
            if (parent.value.Contains('+') || parent.value.Contains('-') || parent.value.Contains('*') || parent.value.Contains('/') || parent.value.Contains('^') || parent.value.Contains('%'))
            {
                    // defining stuff
                    string[] splitEquation = new string[2];
                    // this is so I can create the root node using the function
                    newNode = CreateNode(parent.value, parent);
                    splitEquation = SplitAtLastDelimiter(parent.value, newNode.value);
                    newNode.children.Add(new TreeNode<string>(splitEquation[0]));
                    newNode.children.Add(new TreeNode<string>(splitEquation[1]));
                    newNode.children[0] = CreateTree(newNode.children[0]);
                    newNode.children[1] = CreateTree(newNode.children[1]);
            }
            return newNode;
        }

        public static TreeNode<string> TravelDownward(TreeNode<string> current)
        {
            for (int i = 0; i < current.children.Count; i++)
            {
                if (current.children[i] == null)
                {
                    continue;
                }
                else if (NodeIsAnOperator(current.children[i]) == true)
                {
                    current = TravelDownward(current.children[i]);
                }
            }
            return current;
        }
        // calculate the tree from the bottom level up

        public static double CalculateTree(TreeNode<string> tree, Dictionary<char, Func<double, double, double>> dispatch)
        {
            TreeNode<string> current = tree;
            if (current.children.Count == 0)
            {
                return Double.Parse(tree.value);
            }
            while (current.children[0] != null && current.children[1] != null)
            {
                // import dispatch table
                current = TravelDownward(current);
                double val1 = Double.Parse(current.children[0].value);
                double val2 = Double.Parse(current.children[1].value);
                char op = Convert.ToChar(current.value);
                current.value = Convert.ToString(dispatch[op](val1, val2));
                current.children[0] = null;
                current.children[1] = null;
                current = TravelDownward(tree);
                //CalculateTree(tree, dispatch);
            }
            return Double.Parse(tree.value);
        }
        // the main menu for the calculator
        public static void CalculatorMode()
        {
            Dictionary<char, Func<double, double, double>> dispatch = new Dictionary<char, Func<double, double, double>>();
            Dictionary<string, double> variables = new Dictionary<string, double>();
            dispatch['+'] = CalcFunctions.Add;
            dispatch['-'] = CalcFunctions.Subtract;
            dispatch['*'] = CalcFunctions.Multiply;
            dispatch['/'] = CalcFunctions.Divide;
            dispatch['%'] = CalcFunctions.Modulate;
            dispatch['^'] = CalcFunctions.Exponentiate;
            string input;
            variables["ans"] = 0;
            Console.WriteLine("All equations are done using standard notation.");
            Console.WriteLine("Enter 'C' to clear the current state or 'var' to create a custom variable.");
            Console.WriteLine("To use variables, enclose them in curly braces, such as {var}.");
            Console.WriteLine("To save your variables to a file, type 'save'");
            while (true)
            {
                Console.Write("Enter your equation: ");
                input = Console.ReadLine();

                // checking for extra commands
                if (input == "save")
                {

                }
                if (input == "quit" || input == "QUIT")
                {
                    break;
                }
                else if (CheckForUnusualStuff(input))
                {
                    if (input == "c" || input == "C")
                    {
                        variables["ans"] = 0;
                    }
                    if (input == "var" || input == "VAR")
                    { 
                        string nameRegex = @"[a-z]{1,}";
                        string valRegex = @"[0-9.]{1,}";
                        while (true)
                        {
                            Console.Write("Enter your variable name, or 'exit' to return: ");
                            string varName = Console.ReadLine();
                            if (varName == "exit")
                            {
                                break;
                            }
                            if (Regex.IsMatch(varName, nameRegex) == true)
                            {
                                Console.Write("Enter the number you'd like to save: ");
                                string val = Console.ReadLine();
                                if (Regex.IsMatch(val, valRegex))
                                {
                                    variables[varName] = Double.Parse(val);
                                    Console.WriteLine("Your variable has been saved.");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid number, please try again.\n");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid name, please try again.\n");
                            }
                        
                        }
                    }
                    // check for vars
                    else if (input.Contains('{') && input.Contains('}'))
                    {
                        while (input.Contains('{') && input.Contains('}'))
                        {
                            string possibleVar;
                            int index1 = input.IndexOf('{');
                            int index2 = input.IndexOf('}');
                            possibleVar = input.Substring(index1+1, index2-1);
                            if (variables.ContainsKey(possibleVar))
                            {
                                input = input.Insert(index1, Convert.ToString(variables[possibleVar]));
                                index1 = input.IndexOf('{');
                                index2 = input.IndexOf('}');
                                input = input.Remove(index1, possibleVar.Length+2);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Unrecognized input. Please try again.");
                        continue;
                    }
                }
                // check for null
                if (input == null) { Console.WriteLine("Error: No input was detected."); }
                // checks if only a number was entered
                else if (CheckForOperators(input) == false && CheckForUnusualStuff(input) == false)
                {
                    variables["ans"] = Double.Parse(input);
                    Console.WriteLine(variables["ans"]);
                }
                else if (input == "var")
                {
                    continue;
                }
                // if no errors, then create the tree
                else
                {
                    input.Trim();
                    TreeNode<string> node = new TreeNode<string>(input);
                    node = CreateTree(node);
                    variables["ans"] = CalculateTree(node, dispatch);
                    Console.WriteLine(variables["ans"]);
                }
            }
        }
    }
}

