using ALARm.Core;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AlarmPP.Web.Components.Diagram
{
    public partial class EscortList : ComponentBase
    {
        [Parameter] public EventCallback<bool> UpdateState { get; set; }
        [Parameter]
        public string Style { get; set; }
        public void AddEscort()
        {
            if (newEscort.Distance_Id < 0)
            {
                Toaster.Add("Выберите ПЧ", MatToastType.Danger, "Ошибка добавления сопроваюдающего!!!", "");
                return;
            }
            if ((newEscort.FullName == null) || (newEscort.FullName.Equals("")))
            {
                Toaster.Add("Введите ФИО сопроваждающего", MatToastType.Danger, "Ошибка добавления сопроваюдающего!!!", "");
                return;
            }
            if (AppData.Trip.Escort == null)
                AppData.Trip.Escort = new List<Escort>();
            AppData.Trip.Escort.Add(newEscort);
            if (AppData.Trip.Id>0)
                AppData.RdStructureRepository.InsertEscort(newEscort, AppData.Trip.Id);
            newEscort = new Escort();

            dialogIsOpen = false;

        }
        
        private bool dialogIsOpen = false;
        private Escort newEscort = new Escort();
        
    }
}
