using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic
{
    public class ChicVariables
    {
        public static string FilePath = $"{Program.Root}variables.chicvars";

        public static T GetVariable<T>(string variableName)
        {
            if (GetVariableType(variableName) != typeof(T))
                throw new Exception($"Type {GetVariableType(variableName)} isn't {typeof(T)}");

            string value = GetVariableValue(variableName);

            var type = typeof(T);

            if (type == typeof(string))
                return (T)(object)value;
            if (type == typeof(char))
                return (T)(object)value.ToCharArray()[0];
            if (type == typeof(int))
                return (T)(object)int.Parse(value);
            if (type == typeof(float))
                return (T)(object)float.Parse(value);
            if (type == typeof(double))
                return (T)(object)double.Parse(value);
            if (type == typeof(long))
                return (T)(object)long.Parse(value);

            if (type.GetInterfaces().Contains(typeof(IChicVariable<T>)))
                return ((IChicVariable<T>)Activator.CreateInstance(type)).ChicParse(value);

            return default(T);
        }

        public static string GetVariableValue(string variableName)
            => GetVariableLine(variableName).Split('=')[1];

        public static Type GetVariableType(string variableName)
            => Type.GetType(GetVariableLine(variableName).Split('>')[0]);

        private static string GetVariableLine(string variableName)
            => GetVariables().Split(';').First(var => var.Split('>')[1].Split('=')[0] == variableName).Replace(Environment.NewLine, "");

        private static string GetVariables()
            => File.ReadAllText(FilePath);
    }

    public class TestVariable : IChicVariable<TestVariable>
    {
        public string Name;
        public int kek;

        public TestVariable ChicParse(string s)
        {
            var split = s.Split(',');

            Name = split.First(s => s.Split('-')[0] == "Name").Split('-')[1];
            kek = int.Parse(split.First(s => s.Split('-')[0] == "kek").Split('-')[1]);

            return this;
        }
    }

    public interface IChicVariable<T>
    {
        public T ChicParse(string s);
    }
}