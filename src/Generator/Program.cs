// Last Change: 2014 11 21 16:18

using System;
using System.Text;

namespace ExlainSoftware
{
	internal static class Generator
	{
		private static void Render()
		{
			var sb = new StringBuilder();
			sb.Append(@"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> ");
			sb.Append(SimpleVariable.GenerateTemplates("i", "int", "0", false, "counter", "counter"));
			sb.Append(@"</wpf:ResourceDictionary>");
			Console.WriteLine(sb.ToString());
			//			var fs = new FileStream("export.DotSetting", FileMode.Create);
			//			var sw = new StreamWriter(fs);
			//			sw.Write(sb.ToString());
			//			sw.Close();
			//			fs.Close();
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
		/// <summary>
		///     Идентификатор шаблона
		/// </summary>
		public string Uid { get; private set; }

		/// <summary>
		///     Выражение для вычисления имени по умолчанию
		/// </summary>
		public string DefaultNameExpression { get; protected set; }

		/// <summary>
		///     Сочетание клавиш для шаблона
		/// </summary>
		public string Shortcut { get; protected set; }

		/// <summary>
		///     Текст шаблона
		/// </summary>
		public string Text { get; protected set; }

		/// <summary>
		///     Область действия шаблона
		/// </summary>
		public string Context { get; protected set; }

		/// <summary>
		///     Сгенерировать XML-комментарии?
		/// </summary>
		public bool GenerateXmlComments { get; private set; }

		/// <summary>
		///     Конструктор для базового класса шаблонов
		/// </summary>
		/// <param name="defaultNameExpression">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		protected BaseTemplate(string defaultNameExpression, string shortcut,
			bool generateXmlComments)
		{
			Uid = GetGuid();
			DefaultNameExpression = defaultNameExpression;
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
		/// <param name="uid">Уникальный номер</param>
		/// <param name="end">Концовка адреса</param>
		/// <param name="typeKey">Тип ключа</param>
		/// <param name="value">Значение параметра</param>
		/// <returns>Строка</returns>
		public string GetLine(TypeValue typeValue, string end, TypeKey typeKey, string value, string uid = null)
		{
			if (uid == null)
			{
				uid = Uid;
			}
			return "\t<s:" + typeValue + " x:Key=\"/Default/PatternsAndTemplates/LiveTemplates/Template/="
				+ uid + "/" + end + "/@" + typeKey + "\">" + value + "</s:" + typeValue + ">\n";
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
			Boollean,
			String
		}

		/// <summary>
		///     Производит окончательную сборку шаблона
		/// </summary>
		/// <param name="extrasVariables">Дополнительные переменные</param>
		/// <returns>Возвращает один шаблон</returns>
		protected string AssembleTemplate(string extrasVariables)
		{
			var sb = new StringBuilder();
			sb.Append(GetLine(TypeValue.Boollean, null, TypeKey.KeyIndexDefined, "True") +
				GetLine(TypeValue.String, "Shortcut", TypeKey.EntryValue, Shortcut) +
				GetLine(TypeValue.String, "Description", TypeKey.EntryValue, "") +
				GetLine(TypeValue.String, "Text", TypeKey.EntryValue, Text.Replace("\n", "&#xD;")) +
				GetLine(TypeValue.Boollean, "Reformat", TypeKey.EntryValue, "True") +
				GetLine(TypeValue.Boollean, "ShortenQualifiedReferences", TypeKey.EntryValue, "True") +
				GetLine(TypeValue.String, "Categories/=5665tm_0020Snippets", TypeKey.EntryIndexedValue, "5665tm Snippets") +
				GetLine(TypeValue.Boollean, "Applicability/=Live", TypeKey.EntryIndexedValue, "True")
				);
			return sb.ToString();
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
		/// <param name="defaultNameExpression">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		/// <param name="isLocal">Член локальный?</param>
		/// <param name="isStatic">Член статический?</param>
		/// <param name="visible">Область видимости члена</param>
		protected Member(string defaultNameExpression, string shortcut,
			bool generateXmlComments, bool isLocal, bool isStatic, Visibility visible)
			: base(defaultNameExpression, shortcut, generateXmlComments)
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
		/// <param name="defaultNameExpression">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		/// <param name="isLocal">Член локальный?</param>
		/// <param name="isStatic">Член статический?</param>
		/// <param name="visible">Область видимости члена</param>
		/// <param name="isVariabled">Инициализировать переменную?</param>
		/// <param name="useVar">Использовать var при инициализации?</param>
		/// <param name="defaultValue">Значение по умолчанию при инициализации</param>
		protected Variable(string defaultNameExpression, string shortcut,
			bool generateXmlComments, bool isLocal, bool isStatic, Visibility visible,
			bool isVariabled, bool useVar, string defaultValue)
			: base(defaultNameExpression, shortcut, generateXmlComments, isLocal, isStatic, visible)
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
		/// <param name="defaultNameExpression">Имя по умолчанию</param>
		/// <param name="shortcut">Сочетание клавиши для шаблона</param>
		/// <param name="generateXmlComments">Сгенерировать XML комментарий?</param>
		/// <param name="isLocal">Член локальный?</param>
		/// <param name="isStatic">Член статический?</param>
		/// <param name="visible">Область видимости члена</param>
		/// <param name="isVariabled">Инициализировать переменную?</param>
		/// <param name="useVar">Использовать var при инициализации?</param>
		/// <param name="defaultValue">Значение по умолчанию при инициализации</param>
		/// <param name="type">Тип переменной</param>
		private SimpleVariable(string defaultNameExpression, string shortcut,
			bool generateXmlComments, bool isLocal, bool isStatic, Visibility visible,
			bool isVariabled, bool useVar, string defaultValue, string type)
			: base(defaultNameExpression, shortcut, generateXmlComments,
				isLocal, isStatic, visible, isVariabled, useVar, defaultValue)
		{
			Type = type;
		}

		private string GetTemplate()
		{
			string extrasVariables = null;

			string template = Type + " $DefaultName$";
			DefaultNameExpression = @"constant(&quot;" + DefaultNameExpression + "&quot;)";

			if (!IsVariabled)
			{
				Shortcut += "n";
			}
			else
			{
				template += " = $DefaultValue$";
				extrasVariables = @"			<Variable name=""DefaultValue"" expression=""constant(&quot;"
					+ DefaultValue + @"&quot;)"" initialRange=""0"" />" + "\n";
			}
			if (IsLocal)
			{
				Context = "Expression";
				if (UseVar && IsVariabled)
				{
					template = template.Replace(Type, "var");
				}
			}
			else
			{
				Context = "TypeMember";
				if (Visible == Visibility.Public)
				{
					Shortcut = "p" + Shortcut;
					template = "public " + template;
				}
				else
				{
					template = "private " + template;
				}
				if (IsStatic)
				{
					Shortcut = "s" + Shortcut;
					template = "static " + template;
				}
				if (GenerateXmlComments)
				{
					template = @"
		/// <summary>
		///     $END$
		/// </summary>" + "\n" + template;
					Shortcut = Shortcut + "x";
				}
			}
			Text = template + ";";
			return AssembleTemplate(extrasVariables);
		}

		/// <summary>
		///     Сгенерировать новый набор шаблонов для этой переменной
		/// </summary>
		/// <returns>Набор шаблонов</returns>
		public static string GenerateTemplates(string shortcutSpec, string type,
			string defaultValue, bool useVar = false, string defaultNameLocal = "field", string defaultNameGlobal = "field")
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