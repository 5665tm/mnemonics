namespace ExlainSoftware
{
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
}