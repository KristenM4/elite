using EliteSTO.umbraco.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EliteSTO.Components;

public class ContactViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContactViewModel model)
    {
        model ??= new ContactViewModel();

        return View(model);
    }
}
