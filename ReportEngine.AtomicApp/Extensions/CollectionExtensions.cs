using System.Collections.ObjectModel;
using Mapster;

namespace ReportEngine.AtomicApp.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Сервис вернул IEnumerable<TModel> и нужна ObservableCollection для UI.
        /// Создаётся новая коллекция с нуля и нужно привязать её к DataGrid / ListView.
        /// </summary>
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> items)
        {
            return new ObservableCollection<T>(items);
        }

        /// <summary>
        /// Нужно отправить данные на сервер / в БД
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(this ObservableCollection<T> collection)
        {
            return collection;
        }

        /// <summary>
        /// Нужно заменить все элементы, но не менять объект коллекции, т.е. обновить UI коллкцию
        /// </summary>
        public static void ReplaceWith<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            foreach (var item in items)
                collection.Add(item);
        }
    }
}
