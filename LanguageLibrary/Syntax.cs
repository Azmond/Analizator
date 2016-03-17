using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LanguageLibrary
{
    public class Syntax
    {
        private List<Lexem> Lexems;
        LogError Log;
        private int LexemId;
        public List<string> Variable;
        public string varname;
        public Node resultat;
        


        private Lexem Any()
        {
            if (LexemId != Math.Min(LexemId, Lexems.Count - 1))
            {
                var lex = Lexems[LexemId];
                Skip();
                return lex;
            }
            else
                return null;
        }


        private Lexem Ensure(LexemKind type)
        {
            var lex = Lexems[LexemId];

            if (lex.Kind != type)
                return null;

            Skip();
            return lex;
        }


        private bool Check(LexemKind lexem)
        {
            var lex = Lexems[LexemId];

            if (lex.Kind != lexem)
                return false;

            Skip();
            return true;
        }

        [DebuggerStepThrough]
        private void Skip(int count = 1)
        {
            LexemId = Math.Min(LexemId + count, Lexems.Count - 1);
        }


        private T Attempt<T>(Func<T> getter) where T : LocationEntity
        {
            var backup = LexemId;
            var result = Bind(getter);
            if (result == null)
                LexemId = backup;
            
            
            return result;
        }


        private T Bind<T>(Func<T> getter) where T : LocationEntity
        {
            var startId = LexemId;
            var start = Lexems[LexemId];

            var result = getter();

            if (result != null)
            {
                result.StartLocation = start.StartLocation;

                var endId = LexemId;
                if (endId > startId && endId > 0)
                    result.EndLocation = Lexems[LexemId - 1].EndLocation;
            }
            return result;
        }

  


        
        public Syntax(IEnumerable<Lexem> lex, LogError log)
        {
            Lexems = lex.ToList();
            Log = log;
            Variable = new List<string>();
            resultat = ParseLanguage();
        }

        public Node ParseLanguage()
        {

            ParseLanguageNode temp = new ParseLanguageNode();

            if (!Check(LexemKind.LangBegin))
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode=2, ErrorSource=2 });
                return null;
            }

            var definitions = Attempt(ParseDefinitions);
            if (definitions == null) return null;

            temp.head = definitions.head;

            var ending = Attempt(ParseEnding);
            if (ending == null) return null;

            temp.result = ending.result;

            if (!Check(LexemKind.LangEnd))
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 3, ErrorSource = 2 });
                return null;
            }

            Lexem lex;
            do
            {
            lex = Any();
            if (lex == null) return temp;
            Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 15, ErrorSource = 2 });
            }while(lex!=null);

            return temp;

        }


        public Node ParseDefinitions()
        {
            ParseDefinitionsNode temp = new ParseDefinitionsNode();
            int i = 0;
            do
            {
                i++;

                var definition = Attempt(ParseDefinition);
                if (definition == null)
                {
                    if (i > 1)
                    {
                        Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 7, ErrorSource = 2 });
                    }
                        return null;
                    
                }
                temp.head += definition.head;

            } while (Check(LexemKind.Semicolon));

            return temp;
        }


        public Node ParseDefinition()
        {
            ParseDefinitionNode temp = new ParseDefinitionNode();

             if (!Check(LexemKind.AnalysisStart))
             {
                 if(Check(LexemKind.SynthesisStart))
                     temp.head += "Синтез\\n";
                 
             }
             else
             {
                 temp.head += "Анализ\\n";
             }

             var complex = Attempt(ParseComplex);
             if (complex == null)
             {
                 Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 6, ErrorSource = 2 });
                 return null;
             }

             temp.head += complex.head + "\\n";

             if (!Check(LexemKind.AnalysisEnd))
             {
                 if (Check(LexemKind.SynthesisEnd))
                     temp.head += "Конец синтеза\\n"; ;
             }
             else
             {
                 temp.head += "Конец анализа\\n";
             }


             return temp;
        }


        public Node ParseComplex()
        {
            ParseComplexNode temp = new ParseComplexNode();
            var float1 = Attempt(ParseFloat);
            if (float1 == null)
            {
                return null;
            }


            if (!Check(LexemKind.Comma))
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 5, ErrorSource = 2 });
                return null;
            }

            var float2 = Attempt(ParseFloat);
            if (float2 == null)
            {
                return null;
            }

            temp.head = float1.head + "," + float2.head;

            return temp;
        }

        public Node ParseFloat()
        {
            ParseFloatNode temp = new ParseFloatNode();
            var number1 = Ensure(LexemKind.Number);
            if (number1 == null)
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 4, ErrorSource = 2 });
                return null;
            }
            
            if (!Check(LexemKind.Dot))
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 4, ErrorSource = 2 });
                return null;
            }

            var number2 = Ensure(LexemKind.Number);
            if (number2 == null)
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 4, ErrorSource = 2 });
                return null;
            }

            temp.head = number1.Value + "." + number2.Value;

            return temp;
        }


        public Node ParseEnding()
        {
            ParseEndingNode temp = new ParseEndingNode();
            var vars = Ensure(LexemKind.Var);
            if (vars == null)
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 14, ErrorSource = 2 });
                return null;
            }

            varname=vars.Value;

            if (!Check(LexemKind.Assign))
            {
                Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 13, ErrorSource = 2 });
                return null;
            }

            var rightpart = Attempt(ParseRightPart);
            if (rightpart == null)
            {
                if (!Log.IsError(11))
                    Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 12, ErrorSource = 2 });
                return null;
            }

            temp.result += "int test=" + rightpart.result + ";";

            return temp;
        }

        public Node ParseRightPart()
        {
            ParseRightPartNode temp = new ParseRightPartNode();
            var minus = Check(LexemKind.Minus);
            if (minus == true) temp.result = "-";
            var op = "";
            int i = 0;
            do
            {
                i++;
                var blok = Attempt(ParseBlok);
                if (blok == null)
                {
                    if(i>1)
                    {
                        if (!Log.IsError(10))
                            Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 11, ErrorSource = 2 });
                    }
                    return null;
                }

                temp.result += blok.result;

                if (!Check(LexemKind.Plus))
                {
                   op = (Check(LexemKind.Minus))?("-"):(null);
                }
                else
                {
                   op = "+";
                }

                if (op != null)
                    temp.result += op;

            }while(op!=null);

            return temp;
        }


        public Node ParseBlok()
        {
            ParseBlokNode temp = new ParseBlokNode();
            var op = "";
            int i = 0;

            do
            {
                i++;
                var blok1 = Attempt(ParseBlok1);
                if (blok1 == null)
                {
                    if (i > 1)
                    {
                        if (!Log.IsError(9))
                            Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 10, ErrorSource = 2 });
                    }
                    return null;
                }

                temp.result += blok1.result;

                if (!Check(LexemKind.Multiply))
                {
                    op = (Check(LexemKind.Divide)) ? ("/") : (null);
                }
                else
                {
                    op = "*";
                }

                if (op != null)
                temp.result += op;

            } while (op != null);

            return temp;
        }

        public Node ParseBlok1()
        {
            ParseBlok1Node temp = new ParseBlok1Node();
            var op = "";
            int i = 0;

            do
            {
                i++;
                var blok2 = Attempt(ParseBlok2);
                if (blok2 == null)
                {
                    if (i > 1)
                    {
                        if(!Log.IsError(8))
                            Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 9, ErrorSource = 2 });
                    }
                    return null;
                }

                temp.result += blok2.result;


                if (!Check(LexemKind.And))
                {
                    op = (Check(LexemKind.Or)) ? ("|") : (null);
                }
                else
                {
                    op = "&";
                }

                if (op != null)
                temp.result += op;

            } while (op != null);


            return temp;
        }


        public Node ParseBlok2()
        {
            ParseBlok2Node temp = new ParseBlok2Node();
            var op = ""; var end = ""; int i = 0;

            do
            {
                i++;
                if (Check(LexemKind.Sin))
                {
                    op = "sin";
                    temp.result = string.Format("Math.Sin({0}", temp.result); end += ")";  
                }
                else if (Check(LexemKind.Cos))
                {
                    op = "cos";
                    temp.result = string.Format("Math.Cos({0}", temp.result); end += ")";
                }
                else if (Check(LexemKind.Abs))
                {
                    op = "abs";
                    temp.result = string.Format("Math.Abs({0}", temp.result); end += ")";
                }
                else
                {
                    op = null;
                }
            } while (op != null);

            
            var blok3 = Attempt(ParseBlok3);
            if (blok3 == null)
            {
                if (i > 1)
                    Log.Add(new Error { ErrorLexem = Lexems[LexemId], ErrorCode = 8, ErrorSource = 2 });
                return null;
            }

            temp.result += blok3.result + end;
           
            if (i > 1)
            {
                temp.result = "int.Parse(Math.Round(" + temp.result + ").ToString())";
            }

            return temp;
        }


        public Node ParseBlok3()
        {
            var var = Ensure(LexemKind.Var);
            if (var == null)
            {
                var = Ensure(LexemKind.Number);
                if (var == null) return null;
            }

            ParseBlok3Node temp = new ParseBlok3Node();
            if (var.Kind == LexemKind.Var)
            {
                Variable.Add(var.Value);
            }
            temp.result = var.Value;
            
            return temp;
        }






    }
}
