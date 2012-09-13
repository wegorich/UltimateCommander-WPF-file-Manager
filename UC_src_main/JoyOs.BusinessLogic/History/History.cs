using System.Collections.Generic;

namespace JoyOs.BusinessLogic.History
{
    public class History<T> : IHistory<T>
    {
        private readonly List<T> _collection = new List<T>();
        private int _maxSize;

        #region IHistory
        #region Свойства
        /// <summary>
        /// Определяет максимальный размер истории
        /// </summary>
        public int MaxSize
        {
            get { return _maxSize; }
            set
            {
                value = (value < 0) ? 0 : value;
                _maxSize = value;
            }
        }

        /// <summary>
        /// Показывает текущий размер истории
        /// </summary>
        public int Size
        {
            get { return _collection.Count; }
        }

        /// <summary>
        /// Текущее положение элемента
        /// </summary>
        public int CurrentItemIndex { get; private set; }

        /// <summary>
        /// Доступ к конкретному элементу в истории
        /// </summary>
        /// <param name="index">позиция элемента</param>
        /// <returns>T объект</returns>
        public T this[int index]
        {
            get { return _collection[index]; }
            set { _collection[index] = value; }
        }

        /// <summary>
        /// Текущий объект в истории
        /// </summary>
        public T Current
        {
            get { return _collection[CurrentItemIndex]; }
            set { CurrentItemIndex = _collection.IndexOf(value); }
        }
        #endregion

        #region Функции
        /// <summary>
        /// Двигаемся вперед
        /// </summary>
        /// <returns>Следующий элемент || default</returns>
        public T GoAhead()
        {
            return _collection[++CurrentItemIndex];
        }

        /// <summary>
        /// Двигаемся назад
        /// </summary>
        /// <returns>Предыдущий элемент || default</returns>
        public T GoBack()
        {
            return _collection[--CurrentItemIndex];
        }

        /// <summary>
        /// Получить все элементы коллекции
        /// </summary>
        /// <returns>IEnumerable коллекция</returns>
        public IEnumerable<T> GetAll()
        {
            return _collection;
        }

        /// <summary>
        /// Добовляет новый элемент в историю
        /// </summary>
        /// <param name="value">ваш объект</param>
        public void Add(T value)
        {
            RemoveUnused();

            CurrentItemIndex = _collection.Count;
            _collection.Add(value);
        }

        /// <summary>
        /// Добавляем коллекцию элементов в историю
        /// </summary>
        /// <remarks>
        /// Медленная операция
        /// </remarks>
        /// <param name="collection">Коллекция элементов</param>
        public void AddRange(IEnumerable<T> collection)
        {
            _collection.AddRange(collection);
            RemoveUnused();
            CurrentItemIndex = _collection.Count - 1;
        }

        /// <summary>
        /// Очищаем историю
        /// </summary>
        public void Clear()
        {
            _collection.Clear();
        }
        #endregion
        #endregion

        private void RemoveUnused()
        {
            if (CurrentItemIndex < _collection.Count - 1)
            {
                //UNDONE: возможно не очень быстрое решение
                _collection.RemoveRange(CurrentItemIndex+1, _collection.Count - 1 - CurrentItemIndex);
            }

            if (_collection.Count > MaxSize)
            {
                _collection.RemoveRange(0, _collection.Count - MaxSize);
            }
        }
    }
}
