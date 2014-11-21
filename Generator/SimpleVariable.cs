using System;
using System.Collections.Generic;
using System.Text;

namespace ExlainSoftware
{
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