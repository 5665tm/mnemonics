// Last Change: 2014 11 21 20:37

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ExlainSoftware
{
	internal static class Generator
	{
		private static void Render()
		{
			var sb = new StringBuilder();
			sb.Append(@"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> "
				+ "\n");
			sb.Append(SimpleVariable.GenerateTemplates("i", "int", "0", false, "counter", "counter"));
			sb.Append(SimpleVariable.GenerateTemplates("f", "float", "0f", false, "distance", "distance"));
			sb.Append(SimpleVariable.GenerateTemplates("d", "double", "0d", false, "distance", "distance"));
			sb.Append(SimpleVariable.GenerateTemplates("b", "bool", "false", false, "flag", "flag"));
			sb.Append(SimpleVariable.GenerateTemplates("t", "Transform", "GetComponent<Transform>()", false, "transform", "transform"));
			sb.Append(@"</wpf:ResourceDictionary>");
			var fs = new FileStream("export.DotSettings", FileMode.Create);
			var sw = new StreamWriter(fs);
			sw.Write(sb.ToString());
			sw.Flush();
			fs.Flush();
			Console.WriteLine(sb.ToString());
			Console.WriteLine(BaseTemplate.Counter);
			Process.Start(Environment.CurrentDirectory);
		}

		private static void Main()
		{
			Render();
			Console.ReadLine();
		}
	}

	/// <summary>
	///     Базовый класс для всех шаблонов
	/// </summary>
	public abstract class BaseTemplate
	{
		public static int Counter;
		
		/// <summary>
		///     Идентификатор шаблона
		/// </summary>
		public string Uid { get; private set; }

		/// <summary>
		///     Выражение для вычисления имени по умолчанию
		/// </summary>
		public string DefaultName { get; protected set; }

		/// <summary>
		///     Сочетание клавиш для шаблона
		/// </summary>
		public string Shortcut { get; protected set; }

		/// <summary>
		///     Текст шаблона
		/// </summary>
		public string Text { get; protected set; }

		/// <summary>
		///     Сгенерировать XML-комментарии?
		/// </summary>
		public bool GenerateXmlComments { get; private set; }

		/// <summary>
		///     Конструктор для базового класса шаблонов
		/// </summary>
		/// <param name="defaultName">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		protected BaseTemplate(string defaultName, string shortcut,
			bool generateXmlComments)
		{
			Uid = GetGuid();
			DefaultName = defaultName;
			Shortcut = shortcut;
			GenerateXmlComments = generateXmlComments;
		}

		/// <summary>
		///     Создает новый уникальный ключ
		/// </summary>
		/// <returns>Сгенерированный ключ</returns>
		protected string GetGuid()
		{
			return Guid.NewGuid().ToString().Replace("-", "").ToUpperInvariant();
		}

		/// <summary>
		///     Возвращает линию с указанными параметрами
		/// </summary>
		/// <param name="typeValue">Тип параметра</param>
		/// <param name="end">Концовка адреса</param>
		/// <param name="typeKey">Тип ключа</param>
		/// <param name="value">Значение параметра</param>
		/// <returns>Строка</returns>
		public string GetLine(TypeValue typeValue, string end, TypeKey typeKey, string value)
		{
			return "\t<s:" + typeValue +
				" x:Key=\"/Default/PatternsAndTemplates/LiveTemplates/Template/=" + Uid + "/" + end
				+ (end == null ? "@" : "/@") + typeKey + "\">" + value + "</s:" + typeValue + ">\n";
		}

		/// <summary>
		///     Тип ключа
		/// </summary>
		public enum TypeKey
		{
			KeyIndexDefined,
			EntryValue,
			EntryIndexedValue
		}

		public enum TypeValue
		{
			Int64,
			Boolean,
			String
		}

		public enum TypeScope
		{
			InCSharpTypeMember,
			InCSharpTypeAndNamespace,
			InCSharpStatement
		}

		/// <summary>
		///     Возвращает констатное выражение поля по умолчанию
		/// </summary>
		/// <param name="parameter">Значение константы</param>
		/// <returns>Сгенерированное выражение</returns>
		protected string GetExpressionConstant(string parameter)
		{
			return (@"constant(&quot;" + parameter + "&quot;)").Replace("<", "&lt;").Replace(">", "&gt;");
		}

		public class Field
		{
			public readonly string Name;
			public readonly string Expression;

			public Field(string name, string expression)
			{
				Name = name;
				Expression = expression;
			}
		}

		/// <summary>
		///     Производит окончательную сборку шаблона
		/// </summary>
		/// <returns>Возвращает один шаблон</returns>
		protected string AssembleTemplate(IEnumerable<TypeScope> scopes, List<Field> fields)
		{
			Counter++;
			var sb = new StringBuilder();

			// заполняем основную информацию
			sb.Append(GetLine(TypeValue.Boolean, null, TypeKey.KeyIndexDefined, "True") +
				GetLine(TypeValue.String, "Shortcut", TypeKey.EntryValue, Shortcut) +
				GetLine(TypeValue.String, "Description", TypeKey.EntryValue, "") +
				GetLine(TypeValue.String, "Text", TypeKey.EntryValue, Text
					.Replace("\r", "").Replace("\n", "&#xD;\n").Replace("<", "&lt;").Replace(">", "&gt;")) +
				GetLine(TypeValue.Boolean, "Reformat", TypeKey.EntryValue, "True") +
				GetLine(TypeValue.Boolean, "ShortenQualifiedReferences", TypeKey.EntryValue, "True") +
				GetLine(TypeValue.String, "Categories/=5665tm_0020Snippets", TypeKey.EntryIndexedValue, "5665tm Snippets") +
				GetLine(TypeValue.Boolean, "Applicability/=Live", TypeKey.EntryIndexedValue, "True")
				);

			// добавляем области где работает шаблон
			var addScope = new Action<TypeScope, string>((g, codeGuid) =>
				sb.Append(GetLine(TypeValue.Boolean, "Scope/=" + codeGuid,
					TypeKey.KeyIndexDefined, "True") +
					GetLine(TypeValue.String, "Scope/=" + codeGuid + "/Type",
						TypeKey.EntryValue, g.ToString()) +
					GetLine(TypeValue.String, "Scope/=" + codeGuid + "/CustomProperties/=minimumLanguageVersion",
						TypeKey.EntryIndexedValue, "2.0"))
				);
			foreach (var scope in scopes)
			{
				switch (scope)
				{
					case TypeScope.InCSharpTypeAndNamespace:
						addScope(scope, "558F05AA0DE96347816FF785232CFB2A");
						break;
					case TypeScope.InCSharpTypeMember:
						addScope(scope, "B68999B9D6B43E47A02B22C12A54C3CC");
						break;
					case TypeScope.InCSharpStatement:
						addScope(scope, "2C285F182AC98D44B0B4F29D4D2149EC");
						break;
				}
			}

			// добавляем информацию о всех переменных
			for (int i = 0; i < fields.Count(); i++)
			{
				sb.Append(GetLine(
					TypeValue.Boolean, "Field/=" + fields[i].Name, TypeKey.KeyIndexDefined, "True"));
				sb.Append(GetLine(
					TypeValue.String, "Field/=" + fields[i].Name + "/Expression", TypeKey.EntryValue, fields[i].Expression));
				sb.Append(GetLine(
					TypeValue.Int64, "Field/=" + fields[i].Name + "/Order", TypeKey.EntryValue, Convert.ToString(i)));
			}

			return sb + "\n";
		}
	}

	/// <summary>
	///     Базовый класс для всех переменных и методов
	/// </summary>
	public abstract class Member : BaseTemplate
	{
		/// <summary>
		///     Область члена
		/// </summary>
		public enum Visibility : byte
		{
			Private,
			Public,
		}

		/// <summary>
		///     Член локальный?
		/// </summary>
		public bool IsLocal { get; private set; }

		/// <summary>
		///     Член статический?
		/// </summary>
		public bool IsStatic { get; private set; }

		/// <summary>
		///     Область видимости члена
		/// </summary>
		public Visibility Visible { get; private set; }

		/// <summary>
		///     Конструктор для шаблона члена
		/// </summary>
		/// <param name="defaultName">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		/// <param name="isLocal">Член локальный?</param>
		/// <param name="isStatic">Член статический?</param>
		/// <param name="visible">Область видимости члена</param>
		protected Member(string defaultName, string shortcut,
			bool generateXmlComments, bool isLocal, bool isStatic, Visibility visible)
			: base(defaultName, shortcut, generateXmlComments)
		{
			IsLocal = isLocal;
			IsStatic = isStatic;
			Visible = visible;
		}
	}

	/// <summary>
	///     Базовый класс для переменных
	/// </summary>
	public abstract class Variable : Member
	{
		/// <summary>
		///     Инициализировать?
		/// </summary>
		public bool IsVariabled { get; private set; }

		/// <summary>
		///     Использовать ключевое слово var при объявляении?
		/// </summary>
		public bool UseVar { get; private set; }

		/// <summary>
		///     значение по умолчанию при инициализации
		/// </summary>
		public string DefaultValue { get; private set; }

		/// <summary>
		///     Конструктор для шаблона переменной
		/// </summary>
		/// <param name="defaultName">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		/// <param name="isLocal">Член локальный?</param>
		/// <param name="isStatic">Член статический?</param>
		/// <param name="visible">Область видимости члена</param>
		/// <param name="isVariabled">Инициализировать переменную?</param>
		/// <param name="useVar">Использовать var при инициализации?</param>
		/// <param name="defaultValue">Значение по умолчанию при инициализации</param>
		protected Variable(string defaultName, string shortcut,
			bool generateXmlComments, bool isLocal, bool isStatic, Visibility visible,
			bool isVariabled, bool useVar, string defaultValue)
			: base(defaultName, shortcut, generateXmlComments, isLocal, isStatic, visible)
		{
			IsVariabled = isVariabled;
			UseVar = useVar;
			DefaultValue = defaultValue;
		}
	}

	/// <summary>
	///     Шаблон для простой переменной
	/// </summary>
	public class SimpleVariable : Variable
	{
		/// <summary>
		///     Тип переменной
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		/// </summary>
		/// <param name="defaultName">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		/// <param name="isLocal">Член локальный?</param>
		/// <param name="isStatic">Член статический?</param>
		/// <param name="visible">Область видимости члена</param>
		/// <param name="isVariabled">Инициализировать переменную?</param>
		/// <param name="useVar">Использовать var при инициализации?</param>
		/// <param name="defaultValue">Значение по умолчанию при инициализации</param>
		/// <param name="type">Тип переменной</param>
		private SimpleVariable(string defaultName, string shortcut,
			bool generateXmlComments, bool isLocal, bool isStatic, Visibility visible,
			bool isVariabled, bool useVar, string defaultValue, string type)
			: base(defaultName, shortcut, generateXmlComments,
				isLocal, isStatic, visible, isVariabled, useVar, defaultValue)
		{
			Type = type;
		}

		private string GetTemplate()
		{
			var scopes = new List<TypeScope>();
			var fields = new List<Field>();
			string template = Type + " $DefaultName$";

			if (!IsVariabled)
			{
				Shortcut += "n";
			}
			else
			{
				template += " = $DefaultValue$";
			}
			if (IsLocal)
			{
				scopes.Add(TypeScope.InCSharpStatement);
				if (UseVar && IsVariabled)
				{
					template = template.Replace(Type, "var");
				}
			}
			else
			{
				scopes.Add(TypeScope.InCSharpTypeMember);
				if (Visible == Visibility.Public)
				{
					Shortcut = "p" + Shortcut;
					template = "public " + template;
					DefaultName = DefaultName.Substring(0, 1).ToUpperInvariant() + DefaultName.Substring(1, DefaultName.Length - 1);
				}
				else if (Visible == Visibility.Private)
				{
					template = "private " + template;
					template = template.Replace("$DefaultName$", "_$DefaultName$");
				}
				if (IsStatic)
				{
					Shortcut = "s" + Shortcut;
					template = "static " + template;
				}
				if (GenerateXmlComments)
				{
					template = @"/// <summary>
		///     $XMLCOM$
		/// </summary>" + "\n" + template;
					fields.Add(new Field("XMLCOM", GetExpressionConstant("some comment")));
					Shortcut = Shortcut + "x";
				}
			}
			fields.Add(new Field("DefaultName", GetExpressionConstant(DefaultName)));
			fields.Add(new Field("DefaultValue", GetExpressionConstant(DefaultValue)));
			Text = template + ";\n$END$";
			return AssembleTemplate(scopes, fields);
		}
		
		/// <summary>
		///     Сгенерировать новый набор шаблонов для этой переменной
		/// </summary>
		/// <returns>Набор шаблонов</returns>
		public static string GenerateTemplates(string shortcutSpec, string type,
			string defaultValue, bool useVar = false, string defaultNameLocal = "field",
			string defaultNameGlobal = "field")
		{
			var templates = new StringBuilder();
			string defaultName = null;
			bool isLocal = false;
			bool isStatic = false;
			bool generateXmlComments = false;
			bool isVariabled = false;
			var visibility = Visibility.Private;
			// ReSharper disable AccessToModifiedClosure
			var construct = new Action(() => templates.Append(new SimpleVariable
				(
				defaultName,
				shortcutSpec,
				generateXmlComments,
				isLocal,
				isStatic,
				visibility,
				isVariabled,
				useVar,
				defaultValue,
				type
				).GetTemplate()));
			// ReSharper restore AccessToModifiedClosure
			for (int variabledFlag = 0; variabledFlag < 2; variabledFlag++)
			{
				isVariabled = variabledFlag == 1;

				for (int local = 0; local < 2; local++)
				{
					if (local == 0)
					{
						isLocal = false;
						defaultName = defaultNameGlobal;

						for (int staticflag = 0; staticflag < 2; staticflag++)
						{
							isStatic = staticflag == 1;

							for (int generateXmlFlag = 0; generateXmlFlag < 2; generateXmlFlag++)
							{
								generateXmlComments = generateXmlFlag == 1;

								for (int visibleFlag = 0; visibleFlag < 2; visibleFlag++)
								{
									visibility = visibleFlag == 1 ? Visibility.Public : Visibility.Private;
									construct();
								}
							}
						}
					}
					else
					{
						defaultName = defaultNameGlobal;
						isLocal = true;
						construct();
					}
				}
			}
			return templates.ToString();
		}
	}
}