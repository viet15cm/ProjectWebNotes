using Microsoft.AspNetCore.Mvc.Rendering;


namespace ProjectWebNotes.Areas.Identity.Pages.Models
{
    public static class ManagerNavPages
    {
        //ConfirmedEmail
        //ChangeEmail
        public static string ProFile => "ProFile";
        public static string ChangeEmail => "ChangeEmail";
        public static string OnOff2fa => "OnOff2fa";

        public static string ConfirmedEmail => "ConfirmedEmail";

        public static string ChangePassword => "ChangePassword";
        public static string OnOff2faNavClass(ViewContext viewContext) => PageNavClass(viewContext, OnOff2fa);
        public static string ChangeEmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangeEmail);
        public static string ConfirmedEmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, ConfirmedEmail);
        public static string ProFileNavClass(ViewContext viewContext) => PageNavClass(viewContext, ProFile);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActiveManager"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
