using System.Reflection;
using System.Reflection.Emit;

namespace IcyRain.Internal
{
    /// <summary>Генератор динамической сборки</summary>
    public static class DynamicAssembly
    {
#if DEBUG && NETFRAMEWORK
        private static readonly string _assemblyFileName = $"{ModuleName}.{System.DateTime.Now.Ticks}.dll";
#endif
        static DynamicAssembly()
        {
            Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ModuleName),
#if NETFRAMEWORK
                AssemblyBuilderAccess.RunAndSave);
#else
                AssemblyBuilderAccess.Run);
#endif
            Module = Assembly.DefineDynamicModule(Assembly.GetName().Name
#if DEBUG && NETFRAMEWORK
                , _assemblyFileName
#endif
                );
        }

        /// <summary>Имя модуля</summary>
        public static string ModuleName => "IcyRain.Generated";

        /// <summary>Генерируемая сборка</summary>
        public static AssemblyBuilder Assembly { get; }

        /// <summary>Генерируемый</summary>
        internal static ModuleBuilder Module { get; }
#if DEBUG
        /// <summary>Сохранить сборку в папку с запускаемой библиотекой</summary>
        public static void Save()
        {
#if NETFRAMEWORK
            System.Diagnostics.Debug.WriteLine(System.IO.Path.Combine(System.Environment.CurrentDirectory, _assemblyFileName));
            Assembly.Save(_assemblyFileName);
#endif
        }
#endif
    }
}
