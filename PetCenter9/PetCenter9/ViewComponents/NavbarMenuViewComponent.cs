using PetCenter9.Data;
using Microsoft.AspNetCore.Mvc;
namespace PetCenter9.ViewComponents
{
    public class NavbarMenuViewComponent : ViewComponent
    {
        private PetCenter9Context _context;

        public NavbarMenuViewComponent(PetCenter9Context context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var owners = _context.Owners.OrderBy(c => c.OwnersId).ToList();
            return View("Index", owners);
        }
    }
}
