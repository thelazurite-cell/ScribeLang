using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;
using Scribe.Interpreter.BluePrints.Plugins;

namespace Scribe.Interpreter.BluePrints
{
    public static class ScribeEnvironment
    {
        public const string DecimalFormat = "#,0.##############################";
        public const string FloatFormat = "#,0.#########";
        public const string DoubleFormat = "#,0.#################";
        public const string GlobalScopeValue = "Global";
        public static Dictionary<string[], Type> Keywords { get; set; } = new Dictionary<string[], Type>();
        public static string CurrentScope { get; set; } = string.Empty;
        public static List<Plugin> Plugins { get; set; } = new List<Plugin>();

        public static readonly Dictionary<string, SingleOperator> SingleOperators =
            new Dictionary<string, SingleOperator>
            {
                {"!", SingleOperator.LogicalNegate},
                {"$", SingleOperator.VariableAccess},
                {"#", SingleOperator.AttributeHeader}
            };

        public static readonly Dictionary<string, MultiOperator> MultiOperators = new Dictionary<string, MultiOperator>
        {
            {"+", MultiOperator.ArithmeticalAddition},
            {"-", MultiOperator.ArithmeticalSubtraction},
            {"*", MultiOperator.ArithmeticalMultiplication},
            {"/", MultiOperator.ArithmeticalDivision},
            {"%", MultiOperator.ArithmeticalModulo},
            {"&", MultiOperator.BitwiseAnd},
            {"|", MultiOperator.BitwiseOr},
            {"^", MultiOperator.BitwiseXor},
            {">>", MultiOperator.BitwiseRightShift},
            {"<<", MultiOperator.BitwiseLeftShift},
            {"||", MultiOperator.Or},
            {"&&", MultiOperator.And},
            {"<", MultiOperator.LessThan},
            {">", MultiOperator.GreaterThan},
            {"==", MultiOperator.EqualTo},
            {"<=", MultiOperator.LessThanOrEqualTo},
            {">=", MultiOperator.GreaterThanOrEqualTo},
            {"%%", MultiOperator.Contains},
            {"*%", MultiOperator.StartsWith},
            {"%*", MultiOperator.EndsWith},
            {"!!", MultiOperator.CatchError},
            {"->", MultiOperator.ClassAccessor},
            {"[[", MultiOperator.EvaluateStart},
            {"]]", MultiOperator.EvaluateEnd},
            {"(", MultiOperator.GroupStart},
            {")", MultiOperator.GroupEnd},
        };

        public static Dictionary<IKeyword, ItemScope> Variables { get; set; } = new Dictionary<IKeyword, ItemScope>();

        public static bool ValidNewInstanceName(this string name)
        {
            if (IsInstance(name))
                throw new RuntimeException($"ERROR: There is already an instance with the name '{name}'");
            if (name.Contains(' '))
                throw new RuntimeException("ERROR: Instance name cannot contain a space");
            if (SingleOperators.Keys.Any(key => name.Contains(key)))
            {
                return false;
            }

            if (MultiOperators.Keys.Any(name.Contains))
            {
                return false;
            }

            foreach (var keywordName in Keywords.Keys)
            {
                foreach (var keyword in keywordName)
                {
                    if (name.Equals(keyword))
                        throw new RuntimeException($"ERROR: {keyword} is a reserved keyword.");
                }
            }

            return true;
        }

        public static bool IsInstance(string name)
        {
            return Variables.Any(itm => itm.Key.InstanceName == name);
        }

        public static bool GetInstance(string instanceName, out IValueType type)
        {
            foreach (var instance
                in Variables.Where(instance => IsInBounds(instanceName, instance)))
            {
                if (!(instance.Key is IValueType iv)) continue;
                type = iv;
                return true;
            }

            type = null;
            return false;
        }

        private static bool IsInBounds(string instanceName, KeyValuePair<IKeyword, ItemScope> instance)
        {
            return instanceName.Equals(instance.Key.InstanceName) && (instance.Value.ScopeName.Equals(CurrentScope) ||
                                                                      instance.Value.ScopeName.Equals(GlobalScopeValue)
                );
        }

        public static string[] ParseValueInstance(IValueType ivt, IEnumerable<string> strings)
        {
            var stringsList = strings.ToList();
            if (!stringsList.Any())
                throw new RuntimeException("Error: Internal failure.");
            var returns = new string[2];
            var str = stringsList.FirstOrDefault();

            var words = ivt.GetType().GetAttributeValue((KeywordAttribute itm) => itm.Words);
            var word = words.FirstOrDefault();
            var reg = str.IndexOf(word, StringComparison.InvariantCulture);
            str = str.Remove(reg, word.Length);
            if (string.IsNullOrWhiteSpace(str))
                throw new RuntimeException("ERROR: Expected an instance name");
            if (str.Contains('='))
            {
                try
                {
                    var split = str.Split('=', 2);
                    var inst = split[0].Trim();
                    CheckValidName(inst, returns);
                    var val = split[1].Trim();
                    val = CheckValForOtherInst(val, word);

                    if (string.IsNullOrWhiteSpace(val))
                        throw new RuntimeException("ERROR: Expected Value");
                    returns[1] = val;
                }
                catch (RuntimeException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw new RuntimeException("ERROR: Expected Value");
                }
            }
            else
            {
                var instanceName = str.Trim();
                if (!instanceName.ValidNewInstanceName())
                    throw new RuntimeException($"ERROR: {instanceName} is an invalid instance name");
                returns[0] = instanceName;
            }

            return returns;
        }

        private static string CheckValForOtherInst(string val, string word)
        {
            //if (!val.Contains('=')) return val;
            var access = SingleOperators.SingleOrDefault(itm => itm.Value == SingleOperator.VariableAccess).Key;
            var negate = SingleOperators.SingleOrDefault(itm => itm.Value == SingleOperator.LogicalNegate).Key;
            var inner = val.Split('=', 2);
            var innerInst = inner[0].Trim();

            if ((innerInst.StartsWith(access) || innerInst.StartsWith(negate)) && Variables.Any(itm =>
                itm.Key.InstanceName == innerInst.Replace(access, "").Replace(negate, "")))
            {
                var remInsAcc = innerInst.Replace(access, "").Trim();
                remInsAcc = remInsAcc.Replace(negate, "").Trim();
                var isNegate = innerInst.Contains(negate);
                val = GetInstanceValueStr(remInsAcc, isNegate, word);
            }
            else if (innerInst.StartsWith(word))
            {
                var newInstance = Activator.CreateInstance(GetScribeInstanceType(word));
                if (!(newInstance is IValueType ivfNew)) return val;
                ivfNew.Validate(new[] {val});
                val = ivfNew.Value.ToString();
            }

            return val;
        }

        private static Type GetScribeInstanceType(string word)
        {
            Type kw = null;
            foreach (var keywordsKey in Keywords.Keys)
            {
                if (keywordsKey.Any(s => s == word))
                {
                    kw = Keywords[keywordsKey];
                }

                if (kw != null)
                    break;
            }

            if (kw == null)
            {
                throw new RuntimeException("ERROR: Couldn't determine the type of");
            }

            return kw;
        }

        private static string GetInstanceValueStr(string innerInst, bool isNegate = false, string word = null)
        {
            var reference = Variables.FirstOrDefault(itm => itm.Key.InstanceName == innerInst);
            if (!(reference.Key is IValueType referenceType)) return null;

            if (word == null) return ProcessValueOperators(isNegate, referenceType);

            var words = referenceType.GetType().GetAttributeValue((KeywordAttribute itm) => itm.Words);
            if (!words.Contains(word))
                throw new RuntimeException($"ERROR: Couldn't convert type {word} to {words.FirstOrDefault()}");

            return ProcessValueOperators(isNegate, referenceType);
        }

        public static Object GetInstanceValue(string innerInst, bool isNegate = false)
        {
            var reference = Variables.FirstOrDefault(itm => itm.Key.InstanceName == innerInst);
            if (!(reference.Key is IValueType referenceType)) return null;
            return referenceType.Value;
        }

        private static string ProcessValueOperators(bool isNegate, IValueType refType)
        {
            string val;
            if (refType.Value is bool value && isNegate)
                val = (!value).ToString();
            else
                switch (refType.Value)
                {
                    case decimal typeValue when isNegate:
                        val = (typeValue * -1).ToString(DecimalFormat);
                        break;
                    case float typeValue when isNegate:
                        val = (typeValue * -1).ToString(FloatFormat);
                        break;
                    case double typeValue when isNegate:
                        val = (typeValue * -1).ToString(DoubleFormat);
                        break;
                    default:
                    {
                        if ((refType.Value is short || refType.Value is int || refType.Value is long) && isNegate)
                            val = ((int) refType.Value * -1).ToString();
                        else
                            val = refType.Value.ToString();
                        break;
                    }
                }

            return val;
        }

        private static void CheckValidName(string instance, IList<string> returnStrings)
        {
            if (!instance.ValidNewInstanceName())
                throw new RuntimeException("ERROR: Invalid Instance Name");
            returnStrings[0] = instance;
        }
    }
}