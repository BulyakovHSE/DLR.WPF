using System;
using System.Windows;
using System.Windows.Controls;
using DLR.WPF.DlrServer;

namespace DLR.WPF.Services
{
    public class ActTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ActInspectionTemplate { get; set; }

        public DataTemplate ActInspectionFlTemplate { get; set; }

        public DataTemplate ActInspectionUlipTemplate { get; set; }

        public DataTemplate AgreementStatementTemplate { get; set; }

        public DataTemplate AreaMesurementTemplate { get; set; }

        public DataTemplate CheckingJournalTemplate { get; set; }

        public DataTemplate CitizensCheckPlanTemplate { get; set; }

        public DataTemplate OrderInspectionUlipTemplate { get; set; }

        public DataTemplate PhotoTableTemplate { get; set; }

        public DataTemplate ProtocolTemplate { get; set; }

        public DataTemplate RegulationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var type = item.GetType();
            if (type == typeof(ActInspection)) return ActInspectionTemplate;
            if (type == typeof(ActInpectationFl)) return ActInspectionFlTemplate;
            if (type == typeof(ActInspectationUlIp)) return ActInspectionUlipTemplate;
            if (type == typeof(AreaMeasurement)) return AreaMesurementTemplate;
            if (type == typeof(AgreementStatement)) return AgreementStatementTemplate;
            if (type == typeof(CheckingJournal)) return CheckingJournalTemplate;
            if (type == typeof(CitizensCheckPlan)) return CitizensCheckPlanTemplate;
            if (type == typeof(OrderInspectionUlIp)) return OrderInspectionUlipTemplate;
            if (type == typeof(Protocol)) return ProtocolTemplate;
            if (type == typeof(Regulation)) return RegulationTemplate;
            if (type == typeof(PhotoTable)) return PhotoTableTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}