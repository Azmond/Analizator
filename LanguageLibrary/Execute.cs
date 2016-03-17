using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace LanguageLibrary
{
    public class Execute
    {
        ListView error;

        string sourceTemplate =
                                @"using System;

                                namespace test
                                {
                                    class Program
                                    {
                                        static void Main(string[] args)
                                        {
                                            @init

                                            Console.WriteLine( @Head );
                                            Console.Write(@name);
                                            Console.WriteLine(test);
                                            Console.ReadLine();
                                        }
                                    }
                                }
";

        public Execute(ListView err)
        {
            error = err;
        }

        public void execute(Node lng,string varname)
        {
            string sourceCode = sourceTemplate.Replace("@Head", '"'+lng.head+'"');
            sourceCode = sourceCode.Replace("@init", lng.result);
            sourceCode = sourceCode.Replace("@name", "\""+varname+"=\"");
            CodeSnippetCompileUnit snippetCompileUnit = new CodeSnippetCompileUnit(sourceCode);

            using (CSharpCodeProvider provider =
            new CSharpCodeProvider(new Dictionary<String, String> { { "CompilerVersion", "v3.5" } }))
            {
                CompilerParameters parameters = new CompilerParameters();
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.GenerateExecutable = true;
                parameters.GenerateInMemory = false;
                parameters.IncludeDebugInformation = false;
                parameters.OutputAssembly = "test.exe";
                
                
                CompilerResults results = provider.CompileAssemblyFromDom(parameters, snippetCompileUnit);
                if (!results.Errors.HasErrors)
                {
                    
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    int i = 0;
                    foreach (CompilerError compilerError in results.Errors)
                    {
                        error.Items.Add(string.Format("{0}", compilerError.Line));
                        error.Items[i].SubItems.Add(string.Format("{0}",compilerError.Column));
                        error.Items[i].SubItems.Add("Компилятор");
                        error.Items[i].SubItems.Add(compilerError.ErrorText);
                        error.Items[i].SubItems.Add("");
                        i++;
                        
                    }
                    
                        
                }
            }
        }

    }
}
