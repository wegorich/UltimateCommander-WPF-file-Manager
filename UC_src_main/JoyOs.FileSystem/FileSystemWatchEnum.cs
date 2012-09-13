using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace JoyOs.FileSystem
{
    class FileSystemWatchEnum : IFileSystemWatchEnum
    {
        /// <summary>
        /// Емкость коллекции по умолчанию
        /// </summary>
        private const int DefaultCapacity = 2;

        /// <summary>
        /// Элементы коллекции
        /// </summary>
        private FileSystemWatcher[ ] _items = new FileSystemWatcher[0]; 

        /// <summary>
        /// Число элементов в коллекции
        /// </summary>
        private long _count;

        /// <summary>
        /// Версия
        /// </summary>
        private int _version;

        private bool _globalWatchingState;

        /// <summary>
        /// Разрушен ли объект
        /// </summary>
        private bool _isDisposed;

        public FileSystemWatchEnum(int capacity = DefaultCapacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Емкость не может быть отрицательной");

            _globalWatchingState = true;

            _items = new FileSystemWatcher[capacity];
        }

        ~FileSystemWatchEnum()
        {
            Dispose( false );
        }

        #region Члены IFileSystemWatchEnum

        public int Count
        {
            get { return ( int ) _count; }
        }

        public long LongCount
        {
            get { return _count; }
        }

        public long Capacity
        {
            get 
            {
                return _items.Length;
            }

            set
            {
                if (value < _count) value = _count;

                if (value == _items.LongLength) return;
                
                var newItems = new FileSystemWatcher[value];
                Array.Copy( _items, 0L, newItems, 0L, _count );
                _items = newItems;
            }
        }

        public FileSystemWatcher this[string path]
        {
            get 
            { 
                if (string.IsNullOrEmpty( path ))
                    return null;

                for (var i = 0L; i < _count; i++ )
                {
                    if (_items[i].Path == path)
                        return _items[i];
                }

                return null;
            }
        }

        public FileSystemWatcher this[long index]
        {
            get 
            {
                if ( index < 0L || index >= _count)
                    throw new ArgumentOutOfRangeException( "index", "Индекс вне пределов коллекции" );

                return _items[index];
            }
        }

        public IEnumerable<string> WatchingPaths
        {
            get 
            {
                for (var i = 0L; i < _count; i++)
                {
                    if (_items[i] != null)
                        yield return _items[i].Path;
                }

                yield break;
            }
        }

        public bool Contains(string path)
        {
            return ( this[path] != null );
        }

        public void Clear()
        {
            if (_count > 0)
            {
                for (var i = 0L; i < _count; i++)
                {
                    if (_items[i] == null) continue;

                    _items[i].Dispose();
                    _items[i] = null;
                }

                _count = 0;
            }

            ++_version;
        }

        public FileSystemWatcher Add(string path)
        {
            if (string.IsNullOrEmpty( path ))
                throw new ArgumentException ( "path", "Путь пуст или не существует");

            var currentWatcher = this[path];

            // Если такого элемента нет    
            if (currentWatcher == null)
            {
                // Увеличиваем емкость в два раза, если предел исчерпан
                if (_count == Capacity) Capacity = _count << 1;

                _items[_count] = new FileSystemWatcher( path )
                {
                    NotifyFilter = NotifyFilters.Size | NotifyFilters.LastAccess
                                         | NotifyFilters.LastWrite | NotifyFilters.FileName
                                         | NotifyFilters.DirectoryName | NotifyFilters.Attributes

                    //Filter = "*.htm"; // Устанавливаем фильтр для отслеживаемых файлов
                };

                _items[_count].BeginInit();

                // Вызываемые события
                _items[_count].Changed += DefaultChanged; // Изменен
                _items[_count].Created += DefaultCreated; // Создан
                _items[_count].Deleted += DefaultDeleted; // Удален
                _items[_count].Renamed += DefaultRenamed; // Переименован
                _items[_count].Error += DefaultError; // Ошибка

                // Включаем наблюдение
                _items[_count].EnableRaisingEvents = true;
                ++_version;

                // Коллекция изменена, оповещаем слушателей
                OnChanged();

                _items[_count].EndInit();

                // Возвращаем текущий элемент, увеличиваем # элементов
                return _items[_count++];
            }

            // UNDONE: Такой элемент уже есть -> генерировать исключение ?
            return currentWatcher;
        }

        public bool Remove(string path)
        {
            if (string.IsNullOrEmpty( path ))
                throw new ArgumentException( "path", "Путь пуст или не существует" );

            // Есть ли искомый элемент в коллекции ?
            var br = false;

            for ( var i = 0L; i < _count; i++)
            {
                // Нашли нужный
                if (_items[i] != null && _items[i].Path == path)
                {
                    _items[i].Dispose();                    

                    // Уменьшаем # элементов
                    if (i < --_count)
                    {
                        // Смещаемся на удаленный элемент влево
                        Array.Copy( _items, i + 1L, _items, i, _count - i );
                    }

                    // Очищаем последний элемент
                    // TODO: _count < LongLength 
                    _items[_count] = null;

                    ++_version;

                    // Искомый элемент таки найден !
                    br = true;
                    break;
                }
            }

            return br;
        }

        public bool GlobalEnable
        {
            get 
            {
                return _globalWatchingState;
            }
            set
            {
                // Если не совпадает, то изменяем
                if (_globalWatchingState == value) return;
               
                for (var i = 0L; i < _count; i++)
                {
                    if (_items[i] != null)
                        _items[i].EnableRaisingEvents = value;
                }

                _globalWatchingState = value;
            }
        }

        public event FileSystemEventHandler DefaultChanged;

        public event FileSystemEventHandler DefaultCreated;

        public event FileSystemEventHandler DefaultDeleted;

        public event ErrorEventHandler DefaultError;

        public event RenamedEventHandler DefaultRenamed;

        public event EventHandler Changed;

        #endregion

        #region Члены IEnumerable<FileSystemWatcher>

        public IEnumerator<FileSystemWatcher> GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Члены IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator( this );
        }

        #endregion

        #region Члены IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            Dispose( true );
        }

        #endregion

        #region Nested Type: Enumerator

        [Serializable, StructLayout( LayoutKind.Sequential )]
        public struct Enumerator : IEnumerator<FileSystemWatcher>, IDisposable, IEnumerator
        {
            private readonly FileSystemWatchEnum _enum;

            private long _index;
            private FileSystemWatcher _current;
            private readonly int version;

            internal Enumerator(FileSystemWatchEnum fsenum)
            {
                _enum = fsenum;
                _index = 0;
                _current = default( FileSystemWatcher );
                version = fsenum._version;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                FileSystemWatchEnum fsenum = _enum;

                if ((version == fsenum._version) && (_index < fsenum._count))
                {
                    _current = fsenum._items[_index];
                    _index++;
                    return true;
                }

                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (version != _enum._version)
                {
                    throw new InvalidOperationException( "Коллекция была изменена после создания перечислителя." );
                }

                _index = _enum._count + 1;
                _current = default( FileSystemWatcher );

                return false;
            }

            public FileSystemWatcher Current
            {
                get { return _current;  }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((_index == 0) || (_index == (_enum._count + 1)))
                    {
                        throw new InvalidOperationException(
                            "Перечислитель помещается перед первым элементом коллекции или после последнего элемента." );                        
                    }

                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if (version != _enum._version)
                {
                    throw new InvalidOperationException( "Коллекция была изменена после создания перечислителя." );
                }

                _index = 0;
                _current = default( FileSystemWatcher );
            }
        }

        #endregion

        protected void Dispose(bool disposing)
        {            
            lock (this)
            {
                // Если еще не освобождали ресурсы
                if (_isDisposed) return;
                
                    try
                    {
                        // Нужно ли освобождать управляемые ресурсы ?
                        if (disposing)
                        {
                            Changed = null;
                        }

                        // Всегда освобождаем неуправляемые ресурсы
                        Clear();
                    }
                    finally
                    {
                        _isDisposed = true;
                    }
                }        
            
        }

        protected virtual void OnChanged()
        {
            if (Changed != null)
                Changed( this, new EventArgs() );
        }
    }
}
