using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using AlarmPP.Web.Services;
using BlazorContextMenu;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace AlarmPP.Web.Components.Diagram
{
    public partial class AdditionalParams : ComponentBase
    {

        [Parameter]
        public List<Kilometer> Kilometers { get; set; }
        
        public bool AdditionalDeleteDialog { get; set; } = false;
        public bool AdditionalEditDialog { get; set; } = false;
        public int CurrentRow { get; set; } = 0;

        public string Editor { get; set; }
        public string EditReason { get; set; }
        [Parameter]
        public Digression addDig { get; set; } = new Digression();

        void DeleteAddClick(Digression add)
        {
            addDig = add;
            AdditionalDeleteDialog = true;
        }
        void ModifyAddClick(Digression add)
        {
            addDig = add;
            AdditionalEditDialog = true;
        }

        public async Task GoToMark(int yposition, int rowIndex)
        {
            CurrentRow = rowIndex;
            AppData.SliderYPosition = yposition;
            AppData.SliderXPosition = Math.Round(AppData.SliderYPosition / 10);
            AppData.SliderCenterXPosition = AppData.SliderXPosition + 25;
            object[] paramss = new object[] { AppData.SliderYPosition - 200 };
            await JSRuntime.InvokeVoidAsync("ScrollMainSvg", paramss);


        }

        void EditAdditional(RdAction action, bool dialog)
        {
            if (Kilometers != null)
            {
                if (Editor == null || EditReason == null || Editor.Equals("") || EditReason.Equals("") || Editor.Equals(string.Empty) || EditReason.Equals(string.Empty))
                {
                    Toaster.Add($"Заполните все поля диалогового окна", MatBlazor.MatToastType.Warning, "Редактирование доп параметров");
                    return;
                }
                addDig.EditReason = EditReason;
                addDig.Editor = Editor;
                try
                {
                    var kilometer = (from km in Kilometers where km.Number == addDig.Km select km).First();
                    if (AppData.RdStructureRepository.UpdateAdditionalBase(addDig, kilometer, action) > 0)
                    {
                        Toaster.Add($"Редактирование успешно завершено", MatBlazor.MatToastType.Success, "Редактирование доп параметров");
                        dialog = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Не уадлость завершить редактирование из за ошибки: " + e.Message);
                }
            }
        }
    }

}
