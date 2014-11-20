// Last Change: 2014 11 21 12:55 AM

using System;
using System.Text;

namespace ExlainSoftware
{
	internal static class Generator
	{
		private static void Render()
		{
			var sb = new StringBuilder();
			sb.Append(SimpleVariable.GenerateTemplates("i", "int", "0"));
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
		/// Выражение для вычисления имени по умолчанию
		/// </summary>
		public string DefaultNameExpression { get; private set; }

		/// <summary>
		///     Сочетание клавиш для шаблона
		/// </summary>
		public string Shortcut { get; private set; }

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
			Uid = Guid.NewGuid().ToString().ToLower();
			DefaultNameExpression = defaultNameExpression;
			Shortcut = shortcut;
			GenerateXmlComments = generateXmlComments;
		}

		/// <summary>
		/// Производит окончательную сборку шаблона
		/// </summary>
		/// <param name="extrasVariables">Дополнительные переменные</param>
		/// <returns>Возвращает один шаблон</returns>
		protected string AssembleTemplate(string extrasVariables)
		{
			var sb = new StringBuilder();
			sb.Append("\n");
			sb.Append(@"	<Template uid=""" + Uid + @""" shortcut=""" + Shortcut +
				@""" description="""" text=""" + Text +
				@""" reformat=""True"" shortenQualifiedReferences=""True"">");
			sb.Append("\n");
			sb.Append(@"		<Context>" + "\n");
			sb.Append(@"			<CSharpContext context=""" + Context +
						@""" minimumLanguageVersion=""2.0"" />" + "\n");
			sb.Append(@"		</Context>" + "\n");
			sb.Append(@"		<Variables>" + "\n");
			sb.Append(@"			<Variable name=""DefaultName"" expression="""
				+ DefaultNameExpression + @""" initialRange=""0"" />" + "\n");
			sb.Append(extrasVariables);
			sb.Append(@"		</Variables>" + "\n");
			sb.Append(@"	</Template>" + "\n");
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
		/// 
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

		/// <summary>
		///     Сгенерировать новый набор шаблонов для этой переменной
		/// </summary>
		/// <returns>Набор шаблонов</returns>
		public static string GenerateTemplates(string shortcutSpec, string type,
			string defaultValue,  bool useVar = false)
		{
			var templates = new StringBuilder();
			return templates.ToString();
		}
	}
}