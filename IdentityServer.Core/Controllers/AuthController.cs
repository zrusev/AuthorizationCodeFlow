namespace IdentityServer.Core.Controllers
{
    using Core.Models;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class AuthController: Controller
    {
        private readonly SignInManager<IdentityServerUser> signInManager;
        private readonly UserManager<IdentityServerUser> userManager;

        public AuthController(
            UserManager<IdentityServerUser> userManager,
            SignInManager<IdentityServerUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
            => View(new RegisterViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityServerUser
            {
                UserName = model.Username,
                Nickname = model.Nickname
            };

            var result = await this.userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await this.signInManager.SignInAsync(user, false);

                return Redirect(model.ReturnUrl);
            }

            return View();
        }
    }
}
