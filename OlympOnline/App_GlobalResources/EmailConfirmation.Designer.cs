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
    internal class EmailConfirmation {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailConfirmation() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, используемый этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.EmailConfirmation", global::System.Reflection.Assembly.Load("App_GlobalResources"));
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
        ///   Ищет локализованную строку, такую же, как На ваш адрес электронной почты.
        /// </summary>
        internal static string FirstMail1 {
            get {
                return ResourceManager.GetString("FirstMail1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как было выслано письмо с инструкциями для окончания регистрации..
        /// </summary>
        internal static string FirstMail2 {
            get {
                return ResourceManager.GetString("FirstMail2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Подтверждение EMail адреса.
        /// </summary>
        internal static string Header {
            get {
                return ResourceManager.GetString("Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Вы успешно прошли регистрацию на сайте..
        /// </summary>
        internal static string Success {
            get {
                return ResourceManager.GetString("Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как &lt;a href=&quot;../../Applicant/Main&quot;&gt;Войдите на сайт&lt;/a&gt; для начала работы.
        /// </summary>
        internal static string SuccessLink {
            get {
                return ResourceManager.GetString("SuccessLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как Неверный EMail адрес.
        /// </summary>
        internal static string WrongEmail {
            get {
                return ResourceManager.GetString("WrongEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, такую же, как При подтверждении EMail адреса произошла ошибка. Возможно, вы не до конца скопировали ссылку из письма.
        /// </summary>
        internal static string WrongTicket {
            get {
                return ResourceManager.GetString("WrongTicket", resourceCulture);
            }
        }
    }
}
