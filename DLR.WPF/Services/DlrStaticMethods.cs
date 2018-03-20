using DLR.WPF.DlrServer;
using DLR.WPF.Models;

namespace DLR.WPF.Services
{
    public static class DlrStaticMethods
    {
        public static string GetFullActName(ActType actType)
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

        public static ActType GetActType(ActBase act)
        {
            return act.ActType;
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

        public static string GetRegionName(Region region)
        {
            switch (region)
            {
                case Region.All: return "Центральный";
                case Region.Dzerzhinsky: return "Дзержинский";
                case Region.Industrial: return "Индустриальный";
                case Region.Kirov: return "Кировский";
                case Region.Leninsky: return "Ленинский";
                case Region.Motovilikhinsky: return "Мотовилихинский";
                case Region.NewLyads: return "Новые ляды";
                case Region.Ordzhonikidzevsky: return "Орджоникидзевский";
                case Region.Sverdlovsky: return "Свердловский";
                default: return "";
            }
        }
    }
}