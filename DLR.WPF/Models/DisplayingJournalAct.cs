using System;
using System.ComponentModel;
using System.Windows.Controls;
using DLR.WPF.DlrServer;

namespace DLR.WPF.Models
{
    public class DisplayingJournalAct
    {
        [DisplayName("Идентификатор")]
        public int Id => _act.Id;

        [DisplayName("Тип")]
        public string ActTypeStr => GetFullActName(GetActType(_act));

        public DisplayingJournalAct(ActBase act)
        {
            _act = act;
        }

        private ActBase _act;

        private static ActType GetActType(ActBase act)
        {
            var type = act.GetType();
            if (type == typeof(ActInspection)) return ActType.АктОбследования;
            if (type == typeof(ActInpectationFl)) return ActType.АктПроверкиФизЛица;
            if (type == typeof(ActInspectationUlIp)) return ActType.АктПроверкиЮл;
            if (type == typeof(AreaMeasurement)) return ActType.ОбмерПлощадиЗу;
            if (type == typeof(AgreementStatement)) return ActType.ЗаявлениеСоглВнеплВыездПроверки;
            if (type == typeof(CheckingJournal)) return ActType.ЖурналУчетаПроверокЮл;
            if (type == typeof(CitizensCheckPlan)) return ActType.ПланПроверокГраждан;
            if (type == typeof(OrderInspectionUlIp)) return ActType.РаспоряжениеПроверкиЮл;
            if (type == typeof(Protocol)) return ActType.ПротоколАдмПравонарушения;
            if (type == typeof(Regulation)) return ActType.ПредписаниеУтсрНарушЗемЗакона;
            if (type == typeof(PhotoTable)) return ActType.ФотоТаблица;
            return 0;
        }

        private string GetFullActName(ActType actType)
        {
            switch (actType)
            {
                case ActType.АктОбследования: return "Акт обследования земельного участка";
                case ActType.АктПроверкиФизЛица: return "Акт проверки соблюдения земельного законодательства физическим лицом";
                case ActType.ПланПроверокГраждан: return "План проверок граждан";
                case ActType.АктПроверкиЮл: return "Акт проверки юридического лица, индивидуального предпринимателя";
                case ActType.ЖурналУчетаПроверокЮл: return "Журнал учета проверок юридического лица, индивидуального предпренимателя";
                case ActType.ЗаявлениеСоглВнеплВыездПроверки: return "Заявление о согласовании проведения внеплановой выездной проверки юридического лица, индивидуального предпринимателя";
                case ActType.ОбмерПлощадиЗу: return "Обмер площади земельного участка";
                case ActType.ПредписаниеУтсрНарушЗемЗакона: return "Предписание об устранении нарушения земельного законодательства";
                case ActType.ПротоколАдмПравонарушения: return "Протокол об административном правонарушении";
                case ActType.ФотоТаблица: return "Фототаблица";
                case ActType.РаспоряжениеПроверкиЮл: return "Распоряжение о проведении проверки юридического лица, индивидуального предпринимателя";
                default: return "";
            }
        }

        public static explicit operator ActBase(DisplayingJournalAct source)
        {
            return source._act;
        }

        public static explicit operator DisplayingJournalAct(ActBase source)
        {
            return new DisplayingJournalAct(source);
        }
    }
}