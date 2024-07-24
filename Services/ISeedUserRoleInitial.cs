namespace LanchesMac.Services;

public interface ISeedUserRolerInitial
{
    void SeedRoles();
    void SeedUsers(); //Cria os usuários e os atrbui aos perfis.
}