using System.Collections.Generic;

namespace JoyOs.BusinessLogic.History
{
    public interface IHistory<T>:IBaseInterface
    {
        /// <summary>
        /// Определяет максимальный размер истории
        /// </summary>
        int MaxSize { get; set; }

        /// <summary>
        /// Показывает текущий размер истории
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Текущее положение элемента
        /// </summary>
        int CurrentItemIndex { get; }

        /// <summary>
        /// Доступ к конкретному элементу в истории
        /// </summary>
        /// <param name="index">позиция элемента</param>
        /// <returns>T объект</returns>
        T this[int index] { get; set; }
        
        /// <summary>
        /// Текущий объект в истории
        /// </summary>
        T Current { get; set; }

        /// <summary>
        /// Двигаемся вперед
        /// </summary>
        /// <returns>Следующий элемент || default</returns>
        T GoAhead();

        /// <summary>
        /// Двигаемся назад
        /// </summary>
        /// <returns>Предыдущий элемент || default</returns>
        T GoBack();

        /// <summary>
        /// Получить все элементы коллекции
        /// </summary>
        /// <returns>IEnumerable коллекция</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Добовляет новый элемент в историю
        /// </summary>
        /// <param name="value">ваш объект</param>
        void Add(T value);

        /// <summary>
        /// Добавляем коллекцию элементов в историю
        /// </summary>
        /// <remarks>
        /// Медленная операция
        /// </remarks>
        /// <param name="collection">Коллекция элементов</param>
        void AddRange(IEnumerable<T> collection);

        /// <summary>
        /// Очищаем историю
        /// </summary>
        void Clear();
    }
}
