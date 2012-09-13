using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JoyOs.BusinessLogic;
using System.ComponentModel.Design;

namespace UltimaCmd.Help
{
    /// <summary>
    /// Тип справочного соединения
    /// </summary>
    enum SessionFlags
    {
        /// <summary>
        /// Используем локальное хранилище справки
        /// </summary>
        LocalHelpSession = 0,
        /// <summary>
        /// Выходим за справкой в Internet
        /// </summary>
        InternetHelpSession
    };

    /// <summary>
    /// Поддержка работы со справочной системой
    /// </summary>
    interface IHelpSystem : IHelpService, IBaseInterface
    {
        /// <summary>
        /// Просто показать общий content
        /// </summary>
        void ShowGeneralHelpContent();

        /// <summary>
        /// Есть ли локальная справка
        /// </summary>
        /// <returns>
        /// Boolean value 
        /// True - есть
        /// False - нет
        /// </returns>
        bool LookUpForHelpContent { get; }

        /// <summary>
        /// Проверка есть ли заданный индекс
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsIndexFound(long index);

        /// <summary>
        /// Есть ли такая страница
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        bool IsPageFound(long page);

        /// <summary>
        /// Нужен ли выход в интернет ?
        /// </summary>
        bool IsInternetNeeded { get; }        

        /// <summary>
        /// Диалог "О программе"
        /// </summary>
        void ShowAboutBox();
    }
}
