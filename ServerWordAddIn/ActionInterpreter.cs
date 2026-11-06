using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json.Linq;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;

namespace ServerWordAddIn
{
    public static class ActionInterpreter
    {
        private static readonly Dictionary<string, Word.WdBuiltinStyle> Styles =
            new Dictionary<string, Word.WdBuiltinStyle>
            {
                { "Normal",   Word.WdBuiltinStyle.wdStyleNormal },
                { "Title",    Word.WdBuiltinStyle.wdStyleTitle },
                { "Heading1", Word.WdBuiltinStyle.wdStyleHeading1 },
            };

        public static void Run(Word.Application app, JArray actions)
        {
            var doc = app.Documents.Add();
            app.ActiveWindow.View.Type = Word.WdViewType.wdPrintView; // page color shows here
            var selection = app.Selection;

            foreach (JObject action in actions)
            {
                var type = (string)action["type"];
                switch (type)
                {
                    case "setPageColor":
                        doc.Background.Fill.Visible = Office.MsoTriState.msoTrue;
                        doc.Background.Fill.Solid();
                        doc.Background.Fill.ForeColor.RGB =
                            ColorTranslator.ToOle(ColorTranslator.FromHtml((string)action["color"]));
                        break;

                    case "applyStyle":
                        {
                            var name = (string)action["style"];
                            if (!Styles.ContainsKey(name))
                                throw new NotSupportedException("Unknown style: " + name);
                            object style = Styles[name];
                            selection.set_Style(ref style);
                            break;
                        }

                    case "insertText":
                        selection.TypeText((string)action["text"]);
                        selection.TypeParagraph();
                        break;

                    case "insertPageBreak":
                        {
                            object breakType = Word.WdBreakType.wdPageBreak;
                            selection.InsertBreak(ref breakType);
                            break;
                        }

                    default:
                        throw new NotSupportedException("Unknown action type: " + type);
                }
            }
        }
    }
}