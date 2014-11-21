using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExlainSoftware
{
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
}