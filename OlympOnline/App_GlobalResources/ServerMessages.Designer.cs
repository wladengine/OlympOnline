//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // класс с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить элемент, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте проект Visual Studio.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Web.Application.StronglyTypedResourceProxyBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ServerMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ServerMessages() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, используемый этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.ServerMessages", global::System.Reflection.Assembly.Load("App_GlobalResources"));
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Переопределяет свойство CurrentUICulture текущего потока для всех
        ///   подстановки ресурсов с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Требуется авторизация.
        /// </summary>
        internal static string AuthorizationRequired {
            get {
                return ResourceManager.GetString("AuthorizationRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Ошибка при удалении файла.
        /// </summary>
        internal static string ErrorWhileDeleting {
            get {
                return ResourceManager.GetString("ErrorWhileDeleting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Файл не найден.
        /// </summary>
        internal static string FileNotFound {
            get {
                return ResourceManager.GetString("FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Некорректный идентификатор.
        /// </summary>
        internal static string IncorrectGUID {
            get {
                return ResourceManager.GetString("IncorrectGUID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Файл защищён от записи.
        /// </summary>
        internal static string ReadOnlyFile {
            get {
                return ResourceManager.GetString("ReadOnlyFile", resourceCulture);
            }
        }
    }
}
