using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Catel.Linq;
using DLR.WPF.DlrServer;
using Word = Microsoft.Office.Interop.Word;

namespace DLR.WPF.Services
{
    public class WordTemplateFillingService
    {
        private static string _templatesPath = System.AppDomain.CurrentDomain.BaseDirectory + @"ActTemplates\";
        private static string _resultsPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Acts\";

        public static void FillWordBookmarks(ref ActBase act)
        {
            var type = act.GetType();

            var application = new Word.Application();
            var originalDocument = GetTemplateDoc(act, application);
            originalDocument.ActiveWindow.Selection.WholeStory();
            var newDocument = new Word.Document();
            newDocument.Range().FormattedText = originalDocument.Range().FormattedText;
            originalDocument.Close();

            var bookmarks = newDocument.Bookmarks;
            List<Word.Bookmark> bookMarks = new List<Word.Bookmark>();
            foreach (Word.Bookmark bookmark in bookmarks)
            {
                bookMarks.Add(bookmark);
            }

            var properties = type.GetProperties();
            var ok = false;
            foreach (var propertyInfo in properties)
            {
                bool noUnderline = propertyInfo.Name.Contains("NoUnderline");
                var propName = propertyInfo.Name.Replace("NoUnderline", "");
                if (bookMarks.Exists(x => x.Name == propName))
                {
                    var m = bookMarks.First(x => x.Name == propName);
                    var value = propertyInfo.GetValue(act);
                    Word.Range r;
                    if (value is string val)
                    {
                        if (noUnderline)
                        {
                            r = m.Range;
                            r.InsertAfter(val);
                        }
                        else
                        {
                            r = m.Range;
                            r.Delete(Word.WdUnits.wdCharacter, val.Length);
                            r.InsertAfter(val);
                            r.Underline = Word.WdUnderline.wdUnderlineSingle;
                        }
                    }
                    else if (value is DateTime date)
                    {
                        r = m.Range;
                        string dateStr = "";
                        if (propName.Contains("DateTime"))
                            dateStr = "\"" + date.ToString("dd") + "\""+ date.ToString(" MMMM yyyy HH:mm");
                        else if (propName.Contains("Time"))
                            dateStr = "\"" + date.ToString("HH") + "\" час. \"" + date.ToString("mm") + "\" мин.";
                        else if (propName.Contains("Date"))
                            dateStr = "\"" + date.ToString("dd") + "\""+ date.ToString(" MMMM yyyy");
                        r.Delete(Word.WdUnits.wdCharacter, dateStr.Length);
                        r.InsertAfter(dateStr);
                        r.Underline = Word.WdUnderline.wdUnderlineSingle;
                    }

                    ok = true;
                }
            }

            if (properties.Length > 0 && ok)
            {
                SaveAndCloseNewDocument(newDocument, ref act);
            }
            else
                newDocument.Close(Word.WdSaveOptions.wdDoNotSaveChanges);

            application.Quit(Word.WdSaveOptions.wdDoNotSaveChanges);
        }

        private static Word.Document GetTemplateDoc(ActBase act, Word.Application app)
        {
            return app.Documents.Open(_templatesPath + GetTemplateFileName(act));
        }

        private static string GetTemplateFileName(ActBase act)
        {
            var type = act.GetType();
            if (type == typeof(ActInspection)) return "ActInspection.docx";
            if (type == typeof(ActInpectationFl)) return "ActInspectionFl.docx";
            if (type == typeof(ActInspectationUlIp)) return "ActInspectionUlIp.rtf";
            if (type == typeof(AreaMeasurement)) return "AreaMeasurement.docx";
            if (type == typeof(AgreementStatement)) return "AgreementStatement.RTF";
            if (type == typeof(CheckingJournal)) return "CheckingJournal.RTF";
            if (type == typeof(CitizensCheckPlan)) return "CitizensCkeckingPlan.docx";
            if (type == typeof(OrderInspectionUlIp)) return "OrderInspectionUlIp.rtf";
            if (type == typeof(Protocol)) return "Protocol.docx";
            if (type == typeof(Regulation)) return "Regulation.docx";
            if (type == typeof(PhotoTable)) return "PhotoTable.docx";
            throw new NotImplementedException("Этот тип акта еще не поддерживается!");
        }

        private static void SaveAndCloseNewDocument(Word.Document document, ref ActBase act)
        {
            var tempFile = GetTemplateFileName(act);
            var tempPath = _resultsPath + tempFile;
            var newPath = Path.GetDirectoryName(tempPath) + Path.GetFileName(tempPath).Replace(Path.GetExtension(tempPath), "") + act.Id + Path.GetExtension(tempPath);
            document.SaveAs(newPath);
            document.Close();
            act.DocumentBytes = File.ReadAllBytes(newPath);
        }
    }
}