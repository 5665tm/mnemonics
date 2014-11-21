namespace ExlainSoftware
{
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
}