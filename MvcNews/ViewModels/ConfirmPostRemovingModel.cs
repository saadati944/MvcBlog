using Microsoft.AspNetCore.Mvc;

namespace MvcNews.ViewModels
{
    public class ConfirmPostRemovingModel
    {
        [HiddenInput] public bool Sure { get; set; } = true;
        [HiddenInput] public int Id { get; set; }
    }
}