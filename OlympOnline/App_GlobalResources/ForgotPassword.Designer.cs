//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option or rebuild the Visual Studio project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Web.Application.StronglyTypedResourceProxyBuilder", "11.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ForgotPassword {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ForgotPassword() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.ForgotPassword", global::System.Reflection.Assembly.Load("App_GlobalResources"));
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to Дата рождения.
        /// </summary>
        internal static string BirthDate {
            get {
                return ResourceManager.GetString("BirthDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Отправить запрос.
        /// </summary>
        internal static string btnSubmit {
            get {
                return ResourceManager.GetString("btnSubmit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to На ваш адрес электронной почты было отослано сообщение с новым паролем.
        /// </summary>
        internal static string EmailSent {
            get {
                return ResourceManager.GetString("EmailSent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Восстановление пароля.
        /// </summary>
        internal static string Header {
            get {
                return ResourceManager.GetString("Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Если вы не можете вспомнить, какой пароль у вашей учётной записи, воспользуйтесь формой ниже.
        /// </summary>
        internal static string Info {
            get {
                return ResourceManager.GetString("Info", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Пользователь с указанным адресом электронной почты не найден.
        /// </summary>
        internal static string MessageNoEmail {
            get {
                return ResourceManager.GetString("MessageNoEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не удалось отправить письмо на ваш email. Попробуйте повторить попытку..
        /// </summary>
        internal static string MessageNoSent {
            get {
                return ResourceManager.GetString("MessageNoSent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to У данного пользователя имеется заполненная анкета. Для изменения пароля укажите следующие данные из анкеты:.
        /// </summary>
        internal static string msgNeedConfirm {
            get {
                return ResourceManager.GetString("msgNeedConfirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неверные данные анкеты.
        /// </summary>
        internal static string msgWrongData {
            get {
                return ResourceManager.GetString("msgWrongData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Фамилия.
        /// </summary>
        internal static string Surname {
            get {
                return ResourceManager.GetString("Surname", resourceCulture);
            }
        }
    }
}