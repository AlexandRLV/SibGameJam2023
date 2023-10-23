using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace NetFrame.Utils
{
    public class DatagramsGenerator
    {
        private string _path;

        public DatagramsGenerator(string path)
        {
            _path = path;
        }

        public void Run()
        {
            var dictionaryType = "Dictionary<string, INetFrameDatagram>";
            var dictionaryName = "_datagrams";
            var className = "NetFrameDatagramCollection";

            // Find all types in the assembly that implement INetFrameDatagram
            
            var assembly = Assembly.GetExecutingAssembly();
            var implementingTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(INetFrameDatagram)));

            // Generate usings for the namespaces
            var usings = implementingTypes.Select(t => t.Namespace).Distinct().ToList();

            var stringBuilder = new StringBuilder();

            // Generate code
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine();

            foreach (var ns in usings)
            {
                stringBuilder.AppendLine($"using {ns};");
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"namespace {typeof(DatagramsGenerator).Namespace}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"\tpublic class {className}");
            stringBuilder.AppendLine("\t{");
            stringBuilder.AppendLine($"\t\tpublic {dictionaryType} {dictionaryName};");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"\t\tpublic {className}()");
            stringBuilder.AppendLine("\t\t{");
            stringBuilder.AppendLine($"\t\t\t{dictionaryName} = new {dictionaryType}();");
            stringBuilder.AppendLine();

            foreach (var type in implementingTypes)
            {
                stringBuilder.AppendLine($"\t\t\t{dictionaryName}.Add(\"{type.Name}\", new {type.Name}());");
            }

            stringBuilder.AppendLine("\t\t}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"\t\tpublic INetFrameDatagram GetDatagramByKey(string key)");
            stringBuilder.AppendLine("\t\t{");
            stringBuilder.AppendLine($"\t\t\treturn {dictionaryName}[key];");
            stringBuilder.AppendLine("\t\t}");
            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");
            
            var outputPath = Path.Combine(_path, "NetFrame", "Utils", className + ".cs");
            File.WriteAllText(outputPath, stringBuilder.ToString());
        }
    }
}
