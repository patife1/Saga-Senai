using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.ViewModels;

namespace SistemaEstoque.Controllers
{
    public class ContaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContaController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Conta/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Conta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    TempData["Sucesso"] = "Login realizado com sucesso!";
                    return RedirectToLocal(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Conta bloqueada temporariamente.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                }
            }

            return View(model);
        }

        // GET: /Conta/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Conta/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email 
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Criar roles se não existirem
                    await CriarRolesSeNecessario();

                    // Adicionar role padrão
                    await _userManager.AddToRoleAsync(user, "Funcionario");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    TempData["Sucesso"] = "Conta criada com sucesso!";
                    return RedirectToAction("Index", "Dashboard");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // POST: /Conta/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Sucesso"] = "Logout realizado com sucesso!";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Conta/AcessoNegado
        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }

        // Método auxiliar para redirecionamento
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        // Método para criar roles padrão
        private async Task CriarRolesSeNecessario()
        {
            string[] roleNames = { "Admin", "Gerente", "Funcionario" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
