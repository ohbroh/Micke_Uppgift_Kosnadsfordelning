using System;
using System.Reflection;
using System.Text;

namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>
        /// Class that encapsulates the titles mechansim in Agresso
        /// <example>
        /// To use this in your project, create a class with a suitable name, e.g. Titles Se the example here below
        /// 
        /// internal class Titles
        /// {
        ///     // Internals
        ///     using ACT.SE.Common;
        ///     
        ///     [CurrentContext.TitlesSE.Title(891138, "Skapad av")] // This is an existing title and is there fore translated.
        ///     internal static CurrentContext.TitlesSE.Title CreatedBy { get { return CurrentContext.TitlesSE.Title.Read(MethodBase.GetCurrentMethod()); } private set { /* Must exist for the attribute to work */ } }
        ///     
        ///     [CurrentContext.TitlesSE.Title(-1, "Lev.nr.")] // This one is a local one and will not be translated, since there is no negative 
        ///     internal static CurrentContext.TitlesSE.Title SuppNo { get { return CurrentContext.TitlesSE.Title.Read(MethodBase.GetCurrentMethod()); } private set { /* Must exist for the attribute to work */ } }
        ///     
        ///     [CurrentContext.TitlesSE.Title(-1, "Ogiltigt användarnamn {0}")] // This one is a local one and will not be translated, since there is no negative 
        ///     internal static CurrentContext.TitlesSE.Title InvalidUserName { get { return CurrentContext.TitlesSE.Title.Read(MethodBase.GetCurrentMethod()); } private set { /* Must exist for the attribute to work */ } }
        ///     
        ///     [CurrentContext.TitlesSE.Title(-1, "Säsongsavvikelse", "Seasonal variations")] // This one is a local one and is translated to english if current language is not SE, since there is no negative 
        ///     internal static CurrentContext.TitlesSE.Title SeasonalVariations { get { return CurrentContext.TitlesSE.Title.Read(MethodBase.GetCurrentMethod()); } private set { /* Must exist for the attribute to work */ } }
        ///     
        ///     [CurrentContext.TitlesSE.Title(40114, "Periodiseringsnyckel", "Accruals key", 23)] // This one is an existing title but used in a scenario where the string may not be longer than 23 characters, any language that produces such a string will be cut 
        ///     internal static CurrentContext.TitlesSE.Title AccrualsKey { get { return CurrentContext.TitlesSE.Title.Read(MethodBase.GetCurrentMethod()); } private set { /* Must exist for the attribute to work */ } }
        /// }
        /// 
        ///  Note, if you need to add a 3'rd language you do it like this (e.g. Finish):
        ///     [CurrentContext.TitlesSE.Title(-1, "Säsongsavvikelse", "Seasonal variations", "Ei saa peittää", "FI")]  
        ///     internal static CurrentContext.TitlesSE.Title SeasonalVariations { get { return CurrentContext.TitlesSE.Title.Read(MethodBase.GetCurrentMethod()); } private set {  } }
        /// 
        /// // Then to use a title simply;
        /// string s = Titles.SuppNo.Get();
        /// // Or if the title contains place holders. E.g. Invalid username {0}
        /// string user_id = "DONALD";
        /// 
        /// string s = Titles.InvalidUserName.Get(user_id);
        /// </example>
        /// </summary>
        internal class TitlesSE
        {
            #region internal class TitleAttribute : Attribute
            internal class TitleAttribute : Attribute
            {
                /// <summary>
                /// Construcor
                /// </summary>
                /// <param name="title_no">The title_no value, if it is to be local use -1</param>
                /// <param name="titleSE">The title in swedish</param>
                /// <param name="titleEN">The title in english</param>
                /// <param name="titleThirdLanguage">The title in the 3'rd language</param>
                /// <param name="thirdLanguageCode">3'rd language code (e.g. FI)</param>
                /// <param name="maxLength">The maximum length if the returned string, use 0 if unlimited is ok</param>
                internal TitleAttribute(int title_no, string titleSE, string titleEN, string titleThirdLanguage, string thirdLanguageCode, int maxLength = 0)
                {
                    this.TitleNo = title_no;
                    this.Default = titleSE;
                    this.Title1 = titleSE;
                    this.Title2 = titleEN;
                    this.Title3 = titleThirdLanguage;
                    this.thirdLanguageCode = thirdLanguageCode;

                    if (CurrentContext.Session.Language == thirdLanguageCode)
                    {
                        if (titleEN != null && titleEN.Length > 0)
                            this.Default = titleThirdLanguage;
                    }
                    else if (CurrentContext.Session.Language != "SE")
                    {
                        if (titleEN != null && titleEN.Length > 0)
                            this.Default = titleEN;
                    }

                    if (maxLength > 0)
                    {
                        if (this.Default.Length > maxLength)
                            this.Default = this.Default.Substring(0, maxLength);
                    }
                }

                internal TitleAttribute(int title_no, string titleSE, string titleEN = "", int maxLength = 0)
                {
                    this.TitleNo = title_no;
                    this.Default = titleSE;
                    this.Title1 = titleSE;
                    this.Title2 = titleEN;

                    if (CurrentContext.Session.Language != "SE")
                    {
                        if (titleEN != null && titleEN.Length > 0)
                            this.Default = titleEN;
                    }

                    if (maxLength > 0)
                    {
                        if (this.Default.Length > maxLength)
                            this.Default = this.Default.Substring(0, maxLength);
                    }
                }

                internal int TitleNo { get; private set; }
                internal string Default { get; private set; }
                internal string Title1 { get; private set; }
                internal string Title2 { get; private set; }
                internal string Title3 { get; private set; }
                internal string thirdLanguageCode { get; private set; }
                
            }
            #endregion

            #region internal class Title
            internal class Title
            {
                private string m_Value;

                private Title(string value, int title_id, string title1, string title2, string title3, string thirdLanguageCode)
                {
                    m_Value = value;

                    // Fix the value if it has been made incorrectly
                    m_Value = m_Value.Replace("{T", "{0");
                    m_Value = m_Value.Replace("{L", "{0");

                    title1 = title1.Replace("{T", "{0");
                    title1 = title1.Replace("{L", "{0");

                    title2 = title2.Replace("{T", "{0");
                    title2 = title2.Replace("{L", "{0");

                    this.TitleId = title_id;
                    this.Title1 = title1;
                    this.Title2 = title2;
                    this.Title3 = title3;
                    this.thirdLanguageCode = thirdLanguageCode;
                }

                internal static Title Read(MemberInfo member)
                {
                    string name = member.Name;

                    if (name.ToUpper().StartsWith("GET_"))
                        name = name.Substring(4);

                    Type t = member.DeclaringType;

                    foreach (MemberInfo mi in t.GetMember(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty))
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            if (mi.Name == name) // Probably redundant...
                            {
                                TitleAttribute[] attr = (TitleAttribute[])mi.GetCustomAttributes(typeof(TitleAttribute), false);

                                if (attr.Length > 0)
                                {
                                    TitleAttribute ta = attr[0];

                                    // Get the title from CurrentContext.Titles.GetTitle(ta.TitleNo, ta.Default);
                                    string title = CurrentContext.Titles.GetTitle(ta.TitleNo, ta.Default);

                                    if (ta.TitleNo < 0 || title.StartsWith("** Missing title")) // Apparently 553 doesn't handle the default value correctly
                                        return new Title(ta.Default, -1, ta.Title1, ta.Title2, ta.Title3, ta.thirdLanguageCode);
                                    else
                                        return new Title(title, ta.TitleNo, ta.Title1, ta.Title2, ta.Title3, ta.thirdLanguageCode);
                                }
                            }
                        }
                    }

                    return new Title("[Does not exist]", -1, string.Empty, string.Empty, string.Empty, string.Empty);
                }

                internal int TitleId { get; private set; }
                internal string Title1 { get; private set; }
                internal string Title2 { get; private set; }
                internal string Title3 { get; private set; }
                internal string thirdLanguageCode { get; private set; }
                

                [Obsolete("The Get method is replaced with the ToString method, better semantics.", true)]
                internal string Get(params object[] args)
                {
                    return string.Format(m_Value, args);
                }

                public override string ToString()
                {
                    return m_Value;
                }

                public string ToString(params object[] args)
                {
                    // This blows up if the {} contruction is made wrong.
                    if (args != null && args.Length > 0)
                        return string.Format(m_Value, args);
                    else
                        return m_Value;
                }

                public string ToStringByLanguage(string language)
                {
                    return this.ToStringByLanguage(language, null);
                }

                public string ToStringByLanguage(string language, params object[] args)
                {
                    string ret = null;

                    if (this.TitleId == -1)
                    {
                        if (language == "SE")
                            ret = this.Title1;
                        else if (language == this.thirdLanguageCode)
                            ret = this.Title3;
                        else if (language != "SE" && !string.IsNullOrEmpty(this.Title2))
                            ret = this.Title2;
                    }
                    else
                    {
                        string table = "asystitle" + language.ToLower();

                        if (CurrentContext.Database.IsTable(table))
                        {
                            StringBuilder sql = new StringBuilder();
                            sql.Append("SELECT ");
                            sql.Append("    title ");
                            sql.Append("FROM " + table + " ");
                            sql.Append("WHERE title_no = " + this.TitleId.ToString() + " ");

                            string s = string.Empty;

                            CurrentContext.Database.ReadValue(sql.ToString(), ref s);

                            if (!string.IsNullOrEmpty(s))
                                ret = s;
                        }
                    }

                    if (ret == null) // Fall back to the normal handling
                    {
                        if (args != null)
                            ret = this.ToString(args);
                        else
                            ret = this.ToString();
                    }
                    else
                    {
                        if (args != null)
                            ret = string.Format(ret, args);
                    }

                    return ret;
                }
            }
            #endregion
        }
    }
}
