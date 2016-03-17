using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageLibrary
{
    public class LogError
    {
        public Dictionary<int, string> ErrorsMessage = new Dictionary<int, string>()
        {
            {0, "Неизвестная ошибка"},
            {1, "Неизвестная последовательность символов"},
            {2, "Язык должен начинаться со слова 'Начало'"},
            {3, "Язык должен заканчиваться словом 'Конец'"},
            {4, "Вещественное число в составе комплексного должно состоять из целых чисел соеденненных между собой точкой '.'"},
            {5, "Комплексное число должно состоять из вещественных чисел соеденненных между собой запятой ','"},
            {6, "Не правильно задано комплексное число"},
            {7, "После ';' ожидалось новое 'Определение' или комплексное число"},
            {8, "В функции должно быть укказано целое число или переменная"},
            {9, "Правым операндом логической операции может быть могут быть функции, переменные или целые числа"},
            {10, "Правым операндом мультипликативной операции могут быть логические операции, функции, переменные или целые числа"},
            {11, "В качестве правого операнда аддитивной операции могут быть мультипликативные и логические операции, функции, переменные или целые числа"},
            {12, "Правой частью операции присваения могут быть аддитивные, мультипликативные, логические операции, функции, переменные или целые числа"},
            {13, "После переменной должен идти оператор приваевания '=:'"},
            {14, "После определений должна идти переменная состоящая из двух букв и по желанию цифр"},
            {15, "Язык слова 'Конец' не должно быть других символов"},
        };

        public Dictionary<int, string> SourceType = new Dictionary<int, string>()
        {
            {1, "Лексический"},
            {2, "Синтаксический"}
        };

        public List<Error> ListError;

        public LogError()
        {
            ListError = new List<Error>();
        }

        public bool IsError(int code)
        {
            if (ListError.Where(p => p.ErrorCode == code).Count() > 0)
                return true;
            else
                return false;
        }


        public void Add(Error err)
        {
            ListError.Add(err);
        }

        public void Clear()
        {
            ListError.Clear();
        }

        public void ParseLexemError(List<Lexem> LexErrList)
        {
            if (LexErrList!=null)
            foreach(Lexem lex in LexErrList)
            {
                ListError.Add(new Error { ErrorSource = 1, ErrorCode = 1, ErrorLexem = lex });
            }
        }


    }


    public class Error
    {
        public int ErrorSource;
        public int ErrorCode;
        public Lexem ErrorLexem;
    }


}
