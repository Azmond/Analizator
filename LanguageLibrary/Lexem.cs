using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageLibrary
{
        public enum LexemKind
        {
            LangBegin,          
            LangEnd,            
            AnalysisStart,      
            AnalysisEnd,        
            SynthesisStart,     
            SynthesisEnd,       
            Var,                
            And,                
            Or,                 
            Sin,                
            Cos,                
            Abs,                
            Plus,               
            Minus,              
            Multiply,           
            Divide,             
            Assign,             
            Semicolon,          
            Dot,                
            Comma,              
            Any,
            Number,             
            Unknown             
        }

        public class LocationEntity
        {
            public int Offset;
            public int Length;
            public int StartLocation;
            public int EndLocation;
        }

        public class Lexem : LocationEntity
        {
            public LexemKind Kind;
            public string Value;
            public int Row;
        }

        public class LexemDefinition<T>
        {
            public LexemKind Kind { get; protected set; }
            public T Representation { get; protected set; }
        }

        public class StaticLexemDefinition : LexemDefinition<string>
        {
            public bool IsKeyword;

            public StaticLexemDefinition(string rep, LexemKind kind, bool isKeyword = false)
            {
                Representation = rep;
                Kind = kind;
                IsKeyword = isKeyword;
            }
        }

        public class DynamicLexemDefinition : LexemDefinition<Regex>
        {
            public DynamicLexemDefinition(string rep, LexemKind kind)
            {
                Representation = new Regex(@"\G" + rep, RegexOptions.Compiled);
                Kind = kind;
            }
        }

        public static class LexemDefinitions
        {
            public static StaticLexemDefinition[] Statics = new[]
    {
        new StaticLexemDefinition("Начало", LexemKind.LangBegin, true),
        new StaticLexemDefinition("Анализ", LexemKind.AnalysisStart, true),
        new StaticLexemDefinition("Синтез", LexemKind.SynthesisStart, true),
        new StaticLexemDefinition("Конец анализа", LexemKind.AnalysisEnd, true),
        new StaticLexemDefinition("Конец синтеза", LexemKind.SynthesisEnd, true),
        new StaticLexemDefinition("and", LexemKind.And, true),
        new StaticLexemDefinition("or", LexemKind.Or, true),
        new StaticLexemDefinition("sin", LexemKind.Sin, true),
        new StaticLexemDefinition("cos", LexemKind.Cos, true),
        new StaticLexemDefinition("abs", LexemKind.Abs, true),
        new StaticLexemDefinition("=:", LexemKind.Assign),
        new StaticLexemDefinition("+", LexemKind.Plus),
        new StaticLexemDefinition("-", LexemKind.Minus),
        new StaticLexemDefinition("*", LexemKind.Multiply),
        new StaticLexemDefinition("/", LexemKind.Divide),
        new StaticLexemDefinition(".", LexemKind.Dot),
        new StaticLexemDefinition(",", LexemKind.Comma),
        new StaticLexemDefinition(";", LexemKind.Semicolon),
        new StaticLexemDefinition("Конец", LexemKind.LangEnd, true)
    };

            public static DynamicLexemDefinition[] Dynamics = new[]
    {
        new DynamicLexemDefinition("([а-яА-Я][а-яА-Я]\\d+|[а-яА-Я][а-яА-Я][^\\S]+)", LexemKind.Var),
        new DynamicLexemDefinition("\\d+", LexemKind.Number),
        new DynamicLexemDefinition("\\S+", LexemKind.Unknown),

    };
        }

        public class Lexer
        {
            private char[] SpaceChars = new[] { ' ', '\n', '\r', '\t' };
            private string Source;
            private int Offset;
            private int Str;

            public IEnumerable<Lexem> Lexems { get; private set; }
            public List<Lexem> ErrorLexems { get; private set; }

            public Lexer(string src)
            {
                Source = src;
                Parse();
                FindErrors();
            }

            private void Parse()
            {
                var lexems = new List<Lexem>();
                Str = 0;

                while (InBounds())
                {
                    SkipSpaces();
                    if (!InBounds()) break;

                    var lex = ProcessStatic() ?? ProcessDynamic();
                    if (lex == null)
                        throw new Exception(string.Format("Unknown lexem at {0}", Offset));

                    lexems.Add(lex);
                }

                Lexems = lexems;
            }

            private void SkipSpaces()
            {
                while (InBounds() && Source[Offset].IsAnyOf(SpaceChars))
                {
                    if (Source[Offset] == '\n') Str++;
                    Offset++;
                }
            }


            private Lexem ProcessStatic()
            {
                foreach (var def in LexemDefinitions.Statics)
                {
                    var rep = def.Representation;
                    var len = rep.Length;

                    if (Offset + len > Source.Length || Source.Substring(Offset, len) != rep)
                        continue;

                    if (Offset + len < Source.Length && def.IsKeyword)
                    {
                        var nextChar = Source[Offset + len];
                        if (nextChar == '_' || char.IsLetterOrDigit(nextChar))
                            continue;
                    }

                    Offset += len;
                    return new Lexem { Kind = def.Kind, Offset = Offset, Length = len, Row = Str };
                }

                return null;
            }

            private Lexem ProcessDynamic()
            {
                foreach (var def in LexemDefinitions.Dynamics)
                {
                    var match = def.Representation.Match(Source, Offset);
                    if (!match.Success)
                        continue;

                    Offset += match.Length;
                    return new Lexem { Kind = def.Kind, Offset = Offset, Length = match.Length, Value = match.Value, Row = Str };
                }

                return null;
            }

            private bool InBounds()
            {
                return Offset < Source.Length;
            }

            private void FindErrors()
            {
                ErrorLexems = new List<Lexem>(); 
                foreach(Lexem lex in Lexems)
                {
                    if (lex.Kind == LexemKind.Unknown)
                        ErrorLexems.Add(lex);
                }
            }

        }



}


