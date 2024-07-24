using System.ComponentModel.DataAnnotations;

namespace LanchesMac.ViewModels;

public class LoginViewModel{
    [Required(ErrorMessage = "Informe o usuário")]
    [Display(Name = "Usuário")]
    public string UserName {get; set;}

    [Required(ErrorMessage = "Informe uma senha")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password {get; set;}
    public string ReturnUrl {get; set;} //serve para retornar o usuário para a página onde estava
}